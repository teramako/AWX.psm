using AnsibleTower.Resources;
using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Management.Automation;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace AnsibleTower.Cmdlets
{
    public abstract class GetCmdletBase<T> : APICmdletBase where T: class
    {
        public GetCmdletBase()
        {
            var attr = GetResourceType<T>();
            CanAggregate = attr.CanAggregate;
            Dump($"GetCmeletBase constructor is called.");
        }
        [Parameter(Mandatory = true, Position = 0, ValueFromRemainingArguments = true, ValueFromPipeline = true)]
        [PSDefaultValue(Value = 1, Help = "The resource ID")]
        public ulong[] Id { get; set; } = [];

        protected bool CanAggregate { get; }
        protected HashSet<ulong> IDs { get; set; } = [];
        protected ResourceTypeAttribute ResourceTypeAttr = GetResourceType<T>();
        protected override void ProcessRecord()
        {
            if (CanAggregate)
            {
                foreach (var id in Id)
                {
                    IDs.Add(id);
                }
                return;
            }
            else
            {
                foreach (var id in Id)
                {
                    if (!IDs.Add(id))
                    {
                        // skip already processed
                        continue;
                    }
                    Uri uri = CreateURI(APIv2RootPath, ResourceTypeAttr.Type, id);
                    var res = GetResource<T>(uri);
                    if (res != null)
                    {
                        WriteObject(res);
                    }
                }
            }
        }
        protected override void EndProcessing()
        {
            if (CanAggregate)
            {
                var query = HttpUtility.ParseQueryString("");
                query.Add("id__in", string.Join(',', IDs));
                var uri = CreateURI(APIv2RootPath, ResourceTypeAttr.Type, query);
                foreach (var resultSet in GetResultSet<T>(uri, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    /// <summary>
    /// Abstract class for <c>Find-*</c> Cmdlet
    /// <br/><br/>
    /// <example>
    /// Inherited this class should be add like following class attribute:
    /// <code>
    ///     [Cmdlet(VerbsCommon.Find, "Description", DefaultParameterSetName = "All")]
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc cref="APICmdletBase"/>
    public abstract class FindCmdletBase<T> : APICmdletBase where T : class
    {
        public abstract ResourceType Type { get; set; }
        public abstract ulong Id { get; set; }

        /// <summary>
        /// <c>"order_by"</c> query parameter for API.
        /// <br/>
        /// To sort in reverse (Descending), add <c>"!"</c> prefix instead of <c>"-"</c>.
        /// <br/>
        /// See: 
        /// <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/sorting.html">
        /// 4. Sorting — Automation Controller API Guide
        /// </a>
        /// </summary>
        public abstract string[] OrderBy { get; set; }

        /// <summary>
        /// <c>"search"</c> query parameter for API.
        /// <br/>
        /// See: 
        /// <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/searching.html">
        /// 5. Searching — Automation Controller API Guide
        /// </a>
        /// </summary>
        [Parameter(Position = 1)]
        public string[]? Search { get; set; }

        /// <summary>
        /// Max size of per page.
        /// This parameter is converted to <c>"page_size"</c> query parameter for API.
        /// <br/>
        /// See: 
        /// <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
        /// 7. Pagination — Automation Controller API Guide
        /// </a>
        /// </summary>
        [Parameter()]
        [ValidateRange(1, 200)]
        public ushort Count { get; set; } = 20;

        /// <summary>
        /// <c>"page"</c> query parameter for API.
        /// <br/>
        /// See: 
        /// <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
        /// 7. Pagination — Automation Controller API Guide
        /// </a>
        /// </summary>
        [Parameter()]
        [ValidateRange(1, ushort.MaxValue)]
        public uint Page { get; set; } = 1;

        /// <summary>
        /// Retrieve resources from <see cref="Page">Page</see> to the last, if this switch is true.<br/>
        /// <b>Caoution</b>: "GetAsync" request will be send every per pages.
        /// </summary>
        [Parameter()]
        public SwitchParameter All { get; set; }

        protected ResourceTypeAttribute ResourceTypeAttr = GetResourceType<T>();
        protected readonly NameValueCollection Query = HttpUtility.ParseQueryString("");

        /// <summary>
        /// Add query parameters to <see cref="Query">Query</see>
        /// <list type="bullet">
        ///     <item><see cref="Search">Search</see> to <c>search</c></item>
        ///     <item><see cref="OrderBy">OrderBy</see> to <c>order_by</c></item>
        ///     <item><see cref="Count">Count</see> to <c>page_size</c></item>
        ///     <item><see cref="Page">Page</see> to <c>page</c></item>
        /// </list>
        /// </summary>
        protected virtual void SetupCommonQuery()
        {
            if (Search != null)
            {
                Query.Add("search", string.Join(',', Search));
            }
            if (OrderBy != null)
            {
                Query.Add("order_by",
                          string.Join(',', OrderBy.Select(item => item.StartsWith('!') ? $"-{item.Substring(1)}" : item)));
            }
            Query.Add("page_size", $"{Count}");
            Query.Add("page", $"{Page}");
        }
        protected virtual Uri SetupUri()
        {
            return Id > 0
                    ? CreateURI(APIv2RootPath, ResourceTypeAttr.Type, Type, Id, Query)
                    : CreateURI(APIv2RootPath, ResourceTypeAttr.Type, Query);
        }

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            Find(SetupUri());
        }
        protected virtual void Find(Uri uri)
        {
            foreach (var resultSet in GetResultSet<T>(uri, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
    public abstract class APICmdletBase : Cmdlet
    {
        [Conditional("DEBUG")]
        protected static void Dump(string msg)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Debug: {msg}");
            Console.ForegroundColor = currentColor;
        }
        protected const string APIv2RootPath = "/api/v2/";
        protected static Uri CreateURI(string basePath, ResourceType type, ulong id)
        {
            var uriBuilder = new UriBuilder(ApiConfig.Instance.Origin)
            {
                Path = CreatePath(basePath, type, null, id)
            };
            return uriBuilder.Uri;
        }
        protected static Uri CreateURI(string basePath, ResourceType type, NameValueCollection? query)
        {
            var uriBuilder = new UriBuilder(ApiConfig.Instance.Origin)
            {
                Path = CreatePath(basePath, type),
                Query = query?.ToString()
            };
            return uriBuilder.Uri;

        }
        protected static Uri CreateURI(string basePath, ResourceType type, ResourceType subType, ulong id, NameValueCollection? query)
        {
            var uriBuilder = new UriBuilder(ApiConfig.Instance.Origin)
            {
                Path = CreatePath(basePath, type, subType, id),
                Query = query?.ToString()
            };
            return uriBuilder.Uri;

        }
        /// <summary>
        /// Create request URI Path for API
        /// <list type="bullet">
        ///     <item><paramref name="basePath"/>/{Path name of <paramref name="type"/>}/</item>
        ///     <item><paramref name="basePath"/>/{Path name of <paramref name="type"/>}/{<paramref name="id"/>}/</item>
        ///     <item><paramref name="basePath"/>/{Path name of <paramref name="subType"/>}/{<paramref name="id"/>}/{Path name of <paramref name="type"/>}/</item>
        /// </list>
        /// </summary>
        private static string CreatePath(string basePath, ResourceType type, ResourceType? subType = null, ulong? id = null)
        {
            Type resourceType = typeof(ResourceType);
            var sb = new StringBuilder(basePath);
            var appendId = false;
            if (subType != null && subType != ResourceType.None)
            {
                var subResourceAttr = resourceType.GetField($"{subType}")?.GetCustomAttribute<ResourcePathAttribute>(false)
                                      ?? throw new ArgumentException($"{nameof(subType)} has not a {nameof(ResourcePathAttribute)}");
                sb.Append(subResourceAttr.PathName);
                sb.Append('/');
                if (id != null)
                {
                    sb.Append($"{id}/");
                    appendId = true;
                }
            }
            var resourceAttr = resourceType.GetField($"{type}")?.GetCustomAttribute<ResourcePathAttribute>(false)
                               ?? throw new ArgumentException($"{nameof(type)} has not a {nameof(ResourcePathAttribute)}");
            sb.Append(resourceAttr.PathName);
            sb.Append('/');
            if (appendId || id == null)
            {
                return sb.ToString();
            }
            sb.Append($"{id}/");
            return sb.ToString();
        }

        /// <summary>
        /// Send a request to retrieve a resource.
        /// (HTTP method: <c>GET</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uri"></param>
        /// <param name="acceptType"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? GetResource<TValue>(Uri uri, AcceptType acceptType = AcceptType.Json)
            where TValue : class
        {
            WriteVerboseRequest(uri, Method.GET);
            try
            {
                using var apiTask = RestAPI.GetAsync<TValue>(uri.PathAndQuery, acceptType);
                apiTask.Wait();
                var result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch (RestAPIException ex)
            {
                WriteApiError(ex);
            }
            return null;
        }
        /// <summary>
        /// Send requests to retrieve resource list.
        /// (HTTP method: <c>GET</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uri"></param>
        /// <param name="getAll"></param>
        /// <returns>Returns successed responses</returns>
        protected virtual IEnumerable<ResultSet<TValue>> GetResultSet<TValue>(Uri uri, bool getAll)
            where TValue : class
        {
            WriteVerbose($"> Host: {uri.Host}:{uri.Port}");
            string nextPathAndQuery = uri.PathAndQuery;
            do
            {
                WriteVerbose($"> {Method.GET} {nextPathAndQuery}");
                RestAPIResult<ResultSet<TValue>>? result;
                try
                {
                    using var apiTask = RestAPI.GetAsync<ResultSet<TValue>>(nextPathAndQuery);
                    apiTask.Wait();
                    result = apiTask.Result;
                    WriteVerboseResponse(result.Response);
                }
                catch (RestAPIException ex)
                {
                    WriteVerboseResponse(ex.Response);
                    WriteApiError(ex);
                    break;
                }
                var resultSet = result.Contents;

                yield return resultSet;

                nextPathAndQuery = string.IsNullOrEmpty(resultSet?.Next) ? string.Empty : resultSet.Next;
            } while (getAll && !string.IsNullOrEmpty(nextPathAndQuery));
        }
        /// <summary>
        /// Send a request to create the resource.
        /// (HTTP method: <c>POST</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uri"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? CreateResource<TValue>(Uri uri, object sendData)
            where TValue : class
        {
            WriteVerboseRequest(uri, Method.POST);
            try
            {
                using var apiTask = RestAPI.PostJsonAsync<TValue>(uri.PathAndQuery, sendData);
                apiTask.Wait();
                RestAPIResult<TValue> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch(RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);

            }
            return null;
        }
        /// <summary>
        /// Send a request to replace the resource.
        /// (HTTP method: <c>PUT</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uri"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? UpdateResource<TValue>(Uri uri, object sendData)
            where TValue : class
        {
            WriteVerboseRequest(uri, Method.PUT);
            try
            {
                using var apiTask = RestAPI.PutJsonAsync<TValue>(uri.PathAndQuery, sendData);
                apiTask.Wait();
                RestAPIResult<TValue> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch (RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);
            }
            return null;
        }
        /// <summary>
        /// Send a request to modify part of the resource.
        /// (HTTP method: <c>PATCH</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="uri"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? PatchResource<TValue>(Uri uri, object sendData)
            where TValue : class
        {
            WriteVerboseRequest(uri, Method.PATCH);
            try
            {
                using var apiTask = RestAPI.PatchJsonAsync<TValue>(uri.PathAndQuery, sendData);
                apiTask.Wait();
                RestAPIResult<TValue> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch (RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);

            }
            return null;
        }
        /// <summary>
        /// Send a request to delete the resource.
        /// (HTTP method <c>DELETE</c>)
        /// </summary>
        /// <param name="uri">a URI for the resource</param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual IRestAPIResponse? DeleteResource(Uri uri)
        {
            WriteVerboseRequest(uri, Method.DELETE);
            try
            {
                using var apiTask = RestAPI.DeleteAsync(uri.PathAndQuery);
                apiTask.Wait();
                RestAPIResult<string> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
            }
            catch (RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);
            }
            return null;
        }
        /// <summary>
        /// Send and get a API Help of the URI.
        /// (HTTP method <c>OPTIONS</c>)
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual ApiHelp? GetApiHelp(Uri uri)
        {
            WriteVerboseRequest(uri, Method.OPTIONS);
            try
            {
                using var apiTask = RestAPI.OptionsJsonAsync<ApiHelp>(uri.PathAndQuery);
                apiTask.Wait();
                RestAPIResult<ApiHelp> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch (RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);
            }
            return null;
        }
        protected virtual void WriteVerboseRequest(Uri uri, Method method)
        {
            WriteVerbose($"> Host: {uri.Host}:{uri.Port}");
            WriteVerbose($"> {method} {uri.PathAndQuery}");
        }
        protected virtual void WriteVerboseResponse(IRestAPIResponse response, bool onlyContentHeaders = false)
        {
            WriteVerbose($"HTTP/{response.HttpVersion} {response.StatusCode:d} {response.ReasonPhrase}");
            WriteVerboseHeaders(response.ContentHeaders, '<');
            if (!onlyContentHeaders)
            {
                WriteVerboseHeaders(response.ResponseHeaders, '<');
            }
        }
        private void WriteVerboseHeaders(FrozenDictionary<string, IEnumerable<string>>? headers, char indicator)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                if (header.Key == "Authorization")
                {
                    WriteVerbose($"{indicator} {header.Key}: Bearer ************");
                    continue;
                }
                WriteVerbose($"{indicator} {header.Key}: {string.Join(", ", header.Value)}");
            }
        }
        protected void WriteApiError(RestAPIException ex)
        {
            WriteError(new ErrorRecord(ex, "APIError", ErrorCategory.InvalidResult, ex.Response));
        }
        protected static ResourceTypeAttribute GetResourceType<TAttr>() where TAttr : class
        {
            Type t = typeof(TAttr);
            if (Cache.TryGetValue(t, out var type)) return type;
            var attr = t.GetCustomAttribute<ResourceTypeAttribute>(false)
                ?? throw new Exception($"{nameof(TAttr)} has no {nameof(ResourceTypeAttribute)}");
            Cache.Add(t, attr);
            return attr;
        }
        private static readonly ConditionalWeakTable<Type, ResourceTypeAttribute> Cache = [];

        private Sleep? _sleep;
        protected void Sleep(int milliseconds)
        {
            using (_sleep = new Sleep())
            {
                _sleep.Do(milliseconds);
            }
        }
        protected override void StopProcessing()
        {
            _sleep?.Stop();
        }
    }

}
