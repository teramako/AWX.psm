using AnsibleTower.Resources;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "API", DefaultParameterSetName = "NonSendData")]
    public class InvokeAPICommand : APICmdletBase
    {

        [Parameter(Mandatory = true, Position = 0)]
        public Method Method { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ArgumentCompleter(typeof(ApiPathCompleter))]
        public string Path { get; set; } = string.Empty;

        [Parameter(Position = 2)]
        public string QueryString { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "SendData", Mandatory = true, ValueFromPipeline = true)]
        public object? SenData { get; set; }

        [Parameter()]
        public SwitchParameter AsJson { get; set; }

        private Uri? uri;

        protected override void BeginProcessing()
        {
            NameValueCollection? queryInPath = null;
            if (Path?.IndexOf('?') > 0)
            {
                var pathAndQuery = Path.Split('?', 2);
                Path = pathAndQuery[0];
                queryInPath = HttpUtility.ParseQueryString(pathAndQuery[1]);
            }
            var uriBuilder = new UriBuilder(ApiConfig.Instance.Origin)
            {
                Path = Path,
                Query = (queryInPath == null ? "" : queryInPath.ToString() + '&') + QueryString,
            };
            uri = uriBuilder.Uri;
        }
        protected override void ProcessRecord()
        {
            if (uri == null) { return; }
            WriteVerboseRequest(uri, Method);

            Task<RestAPIResult<string>>? task;

            switch (Method)
            {
                case Method.GET:
                    task = RestAPI.GetAsync<string>(uri.PathAndQuery);
                    break;
                case Method.POST:
                    if (SenData == null)
                    {
                        throw new ArgumentNullException(nameof(SenData));
                    }
                    task = RestAPI.PostJsonAsync<string>(uri.PathAndQuery, SenData);
                    break;
                case Method.OPTIONS:
                    task = RestAPI.OptionsJsonAsync<string>(uri.PathAndQuery);
                    break;
                default:
                    throw new NotSupportedException();

            }
            task.Wait();
            var result = task.Result;
            WriteVerboseResponse(result.Response);
            if (result.Response.ContentType == "application/json")
            {
                var json = JsonSerializer.Deserialize<JsonElement>(result.Contents, Json.DeserializeOptions);
                try
                {
                    var obj = Json.ObjectToInferredType(json, true);
                    if (AsJson)
                    {
                        WriteObject(JsonSerializer.Serialize(obj, Json.SerializeOptions), false);
                    }
                    else
                    {
                        WriteObject(obj, false);
                    }
                    return;
                }
                catch (Exception ex)
                {
                    WriteWarning($"Could not convert to inferred type. Fallback to string: {ex.Message}");
                }
            }
            WriteObject(result.Contents);
        }
    }

    class ApiPathCompleter : IArgumentCompleter
    {
        private readonly Type resourceType = typeof(ResourceType);
        public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                              string parameterName,
                                                              string wordToComplete,
                                                              CommandAst commandAst,
                                                              IDictionary fakeBoundParameters)
        {
            var paths = wordToComplete.Split('/');
            Method method = Method.GET;
            if (fakeBoundParameters.Contains("Method"))
            {
                var param = fakeBoundParameters["Method"] as string;
                Enum.TryParse<Method>(param, true, out method);
            }
            switch (paths.Length)
            {
                case <= 3:
                    {
                        // /api/v2/
                        foreach (var path in new string[] { "v2", "o" })
                        {
                            yield return new CompletionResult($"/api/{path}/");
                        }

                        break;
                    }
                case 4:
                    {
                        // 0   1  2   3
                        //  /api/v2/___
                        string p3 = paths[3];
                        foreach (var field in resourceType.GetFields())
                        {
                            foreach (var attr in field.GetCustomAttributes<ResourcePathAttribute>(false))
                            {
                                if (method != attr.Method)
                                {
                                    var subPaths = field.GetCustomAttributes<ResourceSubPathAttribute>(false)
                                                        .Where(attr => attr.Method == method)
                                                        .ToArray();
                                    if (subPaths.Length == 0)
                                        continue;
                                }
                                if (attr.PathName.StartsWith(p3))
                                {
                                    var text = $"/api/v2/{attr.PathName}/";
                                    yield return new CompletionResult(text,
                                                                      attr.PathName,
                                                                      CompletionResultType.ParameterValue,
                                                                      attr.Description);
                                    break;
                                }
                            }

                        }
                        break;
                    }
                case 6:
                    {
                        // 0   1  2   3    4   5
                        //  /api/v2/___/{id}/___
                        string p3 = paths[3];
                        string p4 = paths[4];
                        string p5 = paths[5];
                        ResourcePathAttribute? p3Attr = null;
                        FieldInfo? resourceField = null;
                        
                        foreach (var field in resourceType.GetFields())
                        {
                            var attr = field.GetCustomAttribute<ResourcePathAttribute>(false);
                            if (attr == null) continue;
                            if (attr.PathName == p3)
                            {
                                resourceField = field;
                                p3Attr = attr;
                                break;
                            }
                        }

                        if (!ulong.TryParse(p4, out _)) break;
                        if (p3Attr == null) break;
                        if (resourceField == null) break;

                        foreach (var subPathAttr in resourceField.GetCustomAttributes<ResourceSubPathAttribute>(false))
                        {
                            if (subPathAttr.PathName.StartsWith(p5))
                            {
                                if (method != subPathAttr.Method) continue;
                                var text = $"/api/v2/{p3}/{p4}/{subPathAttr.PathName}/";
                                var tooltip = string.IsNullOrEmpty(subPathAttr.Description)
                                              ? $"{method} {resourceField.Name}"
                                              : subPathAttr.Description;
                                yield return new CompletionResult(text,
                                                                  subPathAttr.PathName,
                                                                  CompletionResultType.ParameterValue,
                                                                  tooltip);
                            }

                        }
                        break;
                    }
            }
        }
    }
}
