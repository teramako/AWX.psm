using AWX.Resources;
using System.Collections;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;
using System.Text.Json;
using System.Web;

namespace AWX.Cmdlets
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

        private string pathAndQuery = string.Empty;

        protected override void BeginProcessing()
        {
            var query = HttpUtility.ParseQueryString(QueryString);
            NameValueCollection? queryInPath = null;
            if (Path.IndexOf('?') > 0)
            {
                var buf = Path.Split('?', 2);
                Path = buf[0];
                queryInPath = HttpUtility.ParseQueryString(buf[1]);
                queryInPath.Add(query);
                pathAndQuery = $"{Path}?{queryInPath}";
                return;
            }
            if (query.Count > 0)
            {
                pathAndQuery = $"{Path}?{query}";
            }
            else
            {
                pathAndQuery = Path;
            }
        }
        protected override void ProcessRecord()
        {
            if (string.IsNullOrEmpty(pathAndQuery)) { return; }
            WriteVerboseRequest(pathAndQuery, Method);

            Task<RestAPIResult<string>>? task;

            switch (Method)
            {
                case Method.GET:
                    task = RestAPI.GetAsync<string>(pathAndQuery);
                    break;
                case Method.POST:
                    if (SenData == null)
                    {
                        throw new ArgumentNullException(nameof(SenData));
                    }
                    task = RestAPI.PostJsonAsync<string>(pathAndQuery, SenData);
                    break;
                case Method.OPTIONS:
                    task = RestAPI.OptionsJsonAsync<string>(pathAndQuery);
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
        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName,
                                                              string wordToComplete, CommandAst commandAst,
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
                    foreach (var item in Complete()) { yield return item; }
                    break;
                case 4:
                    foreach (var item in Complete(method, paths[3])) { yield return item; }
                    break;
                case 5:
                    foreach (var item in Complete(method, paths[3], paths[4])) { yield return item; }
                    break;
                case 6:
                    foreach (var item in Complete(method, paths[3], paths[4], paths[5])) { yield return item; }
                    break;
            }
        }
        private static IEnumerable<CompletionResult> Complete()
        {
            string[] paths = ["v2", "o"];
            foreach (var path in paths)
            {
                yield return new CompletionResult($"/api/{path}/");
            }
        }
        private static IEnumerable<CompletionResult> Complete(Method method, string p3)
        {
            foreach (var field in typeof(ResourceType).GetFields())
            {
                foreach (var attr in field.GetCustomAttributes<ResourcePathAttribute>(false))
                {
                    if (method != attr.Method)
                    {
                        if (!field.GetCustomAttributes<ResourceSubPathAttribute>(false)
                                  .Where(attr => attr.Method == method)
                                  .Any())
                        {
                            continue;
                        }
                    }
                    if (attr.PathName.StartsWith(p3))
                    {
                        var text = $"/api/v2/{attr.PathName}/";
                        var tooltip = string.IsNullOrEmpty(attr.Description)
                                      ? $"{method} {field.Name}"
                                      : attr.Description;
                        yield return new CompletionResult(text, attr.PathName, CompletionResultType.ParameterValue,
                                                          tooltip);
                        break;
                    }
                }

            }
        }
        private static IEnumerable<CompletionResult> Complete(Method method, string p3, string p4)
        {
            switch (p3)
            {
                case "config":
                case "settings":
                case "dashboard":
                case "analytics":
                case "bulk":
                    yield break;
            }
            ResourcePathAttribute? p3Attr = null;
            FieldInfo? resourceField = null;

            foreach (var field in typeof(ResourceType).GetFields())
            {
                var attr = field.GetCustomAttributes<ResourcePathAttribute>(false).FirstOrDefault();
                if (attr == null) continue;
                if (attr.PathName == p3)
                {
                    resourceField = field;
                    p3Attr = attr;
                    break;
                }
            }
            if (p3Attr == null) yield break;
            if (resourceField == null) yield break;
            if (!ulong.TryParse(p4, out _)) yield break;

            var subPathAttrs = resourceField.GetCustomAttributes<ResourceSubPathAttribute>(false).ToArray();
            if (subPathAttrs.Length == 0) yield break;

            foreach (var subAttr in subPathAttrs.Where(attr => attr.Method == method))
            {
                var tooltip = string.IsNullOrEmpty(subAttr.Description)
                              ? $"{method} {resourceField.Name}"
                              : subAttr.Description;
                if (string.IsNullOrEmpty(subAttr.PathName))
                {
                    yield return new CompletionResult($"/api/v2/{p3}/{p4}/", $"{p3}/{p4}/",
                                                      CompletionResultType.ParameterValue, tooltip);
                }
                else
                {
                    yield return new CompletionResult($"/api/v2/{p3}/{p4}/{subAttr.PathName}/",
                                                      $"{p3}/{p4}/{subAttr.PathName}/",
                                                      CompletionResultType.ParameterValue, tooltip);
                }
            }
        }
        private static IEnumerable<CompletionResult> Complete(Method method, string p3, string p4, string p5)
        {
            ResourcePathAttribute? p3Attr = null;
            FieldInfo? resourceField = null;

            foreach (var field in typeof(ResourceType).GetFields())
            {
                var attr = field.GetCustomAttributes<ResourcePathAttribute>(false).FirstOrDefault();
                if (attr == null) continue;
                if (attr.PathName == p3)
                {
                    resourceField = field;
                    p3Attr = attr;
                    break;
                }
            }

            if (!ulong.TryParse(p4, out _)) yield break;
            if (p3Attr == null) yield break;
            if (resourceField == null) yield break;

            foreach (var subPathAttr in resourceField.GetCustomAttributes<ResourceSubPathAttribute>(false)
                                                     .Where(attr => !string.IsNullOrEmpty(attr.PathName)
                                                                    && attr.Method == method))
            {
                if (subPathAttr.PathName.StartsWith(p5))
                {
                    var text = $"/api/v2/{p3}/{p4}/{subPathAttr.PathName}/";
                    var tooltip = string.IsNullOrEmpty(subPathAttr.Description)
                                  ? $"{method} {resourceField.Name}"
                                  : subPathAttr.Description;
                    yield return new CompletionResult(text, subPathAttr.PathName, CompletionResultType.ParameterValue,
                                                      tooltip);
                }

            }

        }
    }
}
