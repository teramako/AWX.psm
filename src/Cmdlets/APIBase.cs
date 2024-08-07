using AWX.Resources;
using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
{
    public abstract class InvokeJobBase : APICmdletBase
    {
        protected readonly JobProgressManager JobManager = [];
        private Sleep? _sleep;
        protected void Sleep(int milliseconds)
        {
            using (_sleep = new Sleep())
            {
                _sleep.Do(milliseconds);
            }
        }
        protected void WriteJobIndicator(JobProgress jp, bool suppressJobLog)
        {
            WriteHost($"====== [{jp.Id}] {jp.Job?.Name} ======\n",
                      foregroundColor: ConsoleColor.Magenta,
                      tags: ["Ansible", "Indicator", $"job-{jp.Id}"],
                      dontshow: suppressJobLog);
        }
        private ulong lastShownJob = 0;
        protected void WriteJobLog(JobProgress jp, bool suppressJobLog)
        {
            if (string.IsNullOrWhiteSpace(jp.CurrentLog))
            {
                return;
            }
            if (lastShownJob != jp.Id)
            {
                WriteJobIndicator(jp, suppressJobLog);
            }
            WriteHost(jp.CurrentLog,
                      tags: ["Ansible", "JobLog", $"job-{jp.Id}"],
                      dontshow: suppressJobLog);
            lastShownJob = jp.Id;
        }

        protected void WaitJobs(string activityId,
                                int intervalSeconds,
                                bool suppressJobLog)
        {
            if (JobManager.Count == 0)
            {
                return;
            }
            JobManager.Start(activityId, intervalSeconds);
            do
            {
                UpdateAllProgressRecordType(ProgressRecordType.Processing);
                for(var i = 1; i <= intervalSeconds; i++)
                {
                    Sleep(1000);
                    JobManager.UpdateProgress(i);
                    WriteProgress(JobManager.RootProgress);
                }
                JobManager.UpdateJob();
                // Remove Progressbar
                UpdateAllProgressRecordType(ProgressRecordType.Completed);

                ShowJobLog(suppressJobLog);

                WriteObject(JobManager.CleanCompleted(), true);
            } while(JobManager.Count > 0);
        }

        private void UpdateAllProgressRecordType(ProgressRecordType type)
        {
            JobManager.RootProgress.RecordType = type;
            WriteProgress(JobManager.RootProgress);
            foreach (var jp in JobManager.GetAll())
            {
                jp.Progress.RecordType = type;
                WriteProgress(jp.Progress);
            }
        }

        protected void ShowJobLog(bool suppressJobLog)
        {
            var jpList = JobManager.GetJobLog();
            foreach (var jp in jpList)
            {
                if (jp == null) continue;
                WriteJobLog(jp, suppressJobLog);
            }
        }

        protected override void StopProcessing()
        {
            _sleep?.Stop();
        }
    }

    public abstract class GetCmdletBase : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromRemainingArguments = true, ValueFromPipeline = true)]
        [PSDefaultValue(Value = 1, Help = "The resource ID")]
        public ulong[] Id { get; set; } = [];

        protected readonly HashSet<ulong> IdSet  = [];
        protected readonly NameValueCollection Query = HttpUtility.ParseQueryString("");
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
    /// <inheritdoc cref="APICmdletBase"/>
    public abstract class FindCmdletBase : APICmdletBase
    {
        public abstract ResourceType Type { get; set; }
        public abstract ulong Id { get; set; }

        /// <summary>
        /// <c>"search"</c> query parameter for API.
        /// <br/>
        /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/searching.html">
        /// 5. Searching — Automation Controller API Guide
        /// </a>
        /// </summary>
        [Parameter()]
        public string[]? Search { get; set; }

        /// <summary>
        /// <c>"order_by"</c> query parameter for API.
        /// <br/>
        /// To sort in reverse (Descending), add <c>"!"</c> prefix instead of <c>"-"</c>.
        /// <br/>
        /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/sorting.html">
        /// 4. Sorting — Automation Controller API Guide
        /// </a>
        /// </summary>
        public abstract string[] OrderBy { get; set; }

        /// <summary>
        /// Max size of per page.
        /// This parameter is converted to <c>"page_size"</c> query parameter for API.
        /// <br/>
        /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
        /// 7. Pagination — Automation Controller API Guide
        /// </a>
        /// </summary>
        [Parameter()]
        [ValidateRange(1, 200)]
        public ushort Count { get; set; } = 20;

        /// <summary>
        /// <c>"page"</c> query parameter for API.
        /// <br/>
        /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
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

        protected virtual void Find<T>(string path) where T : class
        {
            foreach (var resultSet in GetResultSet<T>($"{path}?{Query}", All))
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
        /// <summary>
        /// Write message to the console as Information
        /// </summary>
        /// <param name="message"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="tags"></param>
        /// <param name="dontshow">Follow the <c>$InformationPreference</c> value, don't output to console forcely.</param>
        protected void WriteHost (string message,
                                  ConsoleColor? foregroundColor = null,
                                  ConsoleColor? backgroundColor = null,
                                  string[]? tags = null,
                                  bool dontshow = false)
        {
            var msg = new HostInformationMessage() {
                Message = message,
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                NoNewLine = true
            };
            List<string> infoTags = dontshow ? [] : ["PSHOST"];
            if (tags != null) { infoTags.AddRange(tags); }
            WriteInformation(msg, infoTags.ToArray());
        }
        /// <summary>
        /// Send a request to retrieve a resource.
        /// (HTTP method: <c>GET</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pathAndQuery"></param>
        /// <param name="acceptType"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? GetResource<TValue>(string pathAndQuery, AcceptType acceptType = AcceptType.Json)
            where TValue : class
        {
            WriteVerboseRequest(pathAndQuery, Method.GET);
            try
            {
                using var apiTask = RestAPI.GetAsync<TValue>(pathAndQuery, acceptType);
                apiTask.Wait();
                var result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result.Contents;
            }
            catch (RestAPIException ex)
            {
                WriteApiError(ex);
            }
            catch(AggregateException aex)
            {
                if (aex.InnerException is RestAPIException ex)
                {
                    WriteVerboseResponse(ex.Response);
                    WriteApiError(ex);
                }
                else
                {
                    throw;
                }
            }
            return null;
        }
        protected virtual IEnumerable<ResultSet<TValue>> GetResultSet<TValue>(string path,
                                                                              NameValueCollection? query = null,
                                                                              bool getAll = false)
            where TValue : class
        {
            var pathAndQuery = path + (query == null ? "" : $"?{query}");
            foreach (var resultSet in GetResultSet<TValue>(pathAndQuery, getAll))
            {
                yield return resultSet;
            }
        }
        /// <summary>
        /// Send requests to retrieve resource list.
        /// (HTTP method: <c>GET</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pathAndQuery"></param>
        /// <param name="getAll"></param>
        /// <returns>Returns successed responses</returns>
        protected virtual IEnumerable<ResultSet<TValue>> GetResultSet<TValue>(string pathAndQuery, bool getAll = false)
            where TValue : class
        {
            string nextPathAndQuery = pathAndQuery;
            do
            {
                WriteVerboseRequest(nextPathAndQuery, Method.GET);
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
                catch(AggregateException aex)
                {
                    if (aex.InnerException is RestAPIException ex)
                    {
                        WriteVerboseResponse(ex.Response);
                        WriteApiError(ex);
                        break;
                    }
                    throw;
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
        /// <param name="pathAndQuery"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        /// <exception cref="RestAPIException"/>
        protected virtual RestAPIResult<TValue> CreateResource<TValue>(string pathAndQuery, object? sendData = null)
            where TValue : class
        {
            WriteVerboseRequest(pathAndQuery, Method.POST);
            try
            {
                using var apiTask = RestAPI.PostJsonAsync<TValue>(pathAndQuery, sendData);
                apiTask.Wait();
                RestAPIResult<TValue> result = apiTask.Result;
                WriteVerboseResponse(result.Response);
                return result;
            }
            catch(RestAPIException ex)
            {
                WriteVerboseResponse(ex.Response);
                WriteApiError(ex);
                throw;
            }
            catch(AggregateException aex)
            {
                if (aex.InnerException is RestAPIException ex)
                {
                    WriteVerboseResponse(ex.Response);
                    WriteApiError(ex);
                    throw ex;
                }
                throw;
            }
        }
        /// <summary>
        /// Send a request to replace the resource.
        /// (HTTP method: <c>PUT</c>)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pathAndQuery"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? UpdateResource<TValue>(string pathAndQuery, object sendData)
            where TValue : class
        {
            WriteVerboseRequest(pathAndQuery, Method.PUT);
            try
            {
                using var apiTask = RestAPI.PutJsonAsync<TValue>(pathAndQuery, sendData);
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
        /// <param name="pathAndQuery"></param>
        /// <param name="sendData"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual TValue? PatchResource<TValue>(string pathAndQuery, object sendData)
            where TValue : class
        {
            WriteVerboseRequest(pathAndQuery, Method.PATCH);
            try
            {
                using var apiTask = RestAPI.PatchJsonAsync<TValue>(pathAndQuery, sendData);
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
        /// <param name="pathAndQuery"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual IRestAPIResponse? DeleteResource(string pathAndQuery)
        {
            WriteVerboseRequest(pathAndQuery, Method.DELETE);
            try
            {
                using var apiTask = RestAPI.DeleteAsync(pathAndQuery);
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
        /// <param name="pathAndQuery"></param>
        /// <returns>Return the result if success, otherwise null</returns>
        protected virtual ApiHelp? GetApiHelp(string pathAndQuery)
        {
            WriteVerboseRequest(pathAndQuery, Method.OPTIONS);
            try
            {
                using var apiTask = RestAPI.OptionsJsonAsync<ApiHelp>(pathAndQuery);
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
        protected virtual void WriteVerboseRequest(string pathAndQuery, Method method)
        {
            var uri = ApiConfig.Instance.Origin;
            WriteVerbose($"> Host: {uri.Host}:{uri.Port}");
            WriteVerbose($"> {method} {pathAndQuery}");
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
    }

}
