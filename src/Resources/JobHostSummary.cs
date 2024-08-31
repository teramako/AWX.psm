using System.Collections.Specialized;

namespace AWX.Resources
{

    public class JobHostSummary(ulong id, ResourceType type, string url, RelatedDictionary related, JobHostSummary.Summary summaryFields,
                          DateTime created, DateTime? modified, ulong job, ulong host, ulong? constructedHost,
                          string hostName, int changed, int dark, int failures, int oK, int processed, int skipped,
                          bool failed, int ignored, int rescued)
                : IResource<JobHostSummary.Summary>
    {
        public const string PATH = "/api/v2/job_host_summaries/";
        /// <summary>
        /// Retrieve a Job Host Summary.<br/>
        /// API Path: <c>/api/v2/job_host_summaries/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<JobHostSummary> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<JobHostSummary>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Job Host Summaries for a Group.<br/>
        /// API Path: <c>/api/v2/groups/<paramref name="groupId"/>/job_host_summaries/</c>
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<JobHostSummary> FindFromGroup(ulong groupId,
                                                                           NameValueCollection? query = null,
                                                                           bool getAll = false)
        {
            var path = $"{Group.PATH}{groupId}/job_host_summaries/";
            await foreach (var result in RestAPI.GetResultSetAsync<JobHostSummary>(path , query, getAll))
            {
                foreach(var jobHostSummary in result.Contents.Results)
                {
                    yield return jobHostSummary;
                }
            }
        }
        /// <summary>
        /// List Job Host Summaries for a Host.<br/>
        /// API Path: <c>/api/v2/hosts/<paramref name="hostId"/>/job_host_summaries/</c>
        /// </summary>
        /// <param name="hostId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<JobHostSummary> FindFromHost(ulong hostId,
                                                                          NameValueCollection? query = null,
                                                                          bool getAll = false)
        {
            var path = $"{Resources.Host.PATH}{hostId}/job_host_summaries/";
            await foreach (var result in RestAPI.GetResultSetAsync<JobHostSummary>(path , query, getAll))
            {
                foreach(var jobHostSummary in result.Contents.Results)
                {
                    yield return jobHostSummary;
                }
            }
        }
        /// <summary>
        /// List Job Host Summaries for a Job.<br/>
        /// API Path: <c>/api/v2/jobs/<paramref name="jobId"/>/job_host_summaries/</c>
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<JobHostSummary> FindFromJob(ulong jobId,
                                                                         NameValueCollection? query = null,
                                                                         bool getAll = false)
        {
            var path = $"{JobTemplateJob.PATH}{jobId}/job_host_summaries/";
            await foreach (var result in RestAPI.GetResultSetAsync<JobHostSummary>(path , query, getAll))
            {
                foreach(var jobHostSummary in result.Contents.Results)
                {
                    yield return jobHostSummary;
                }
            }
        }

        public record Summary(HostSummary Host, JobExSummary Job);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public ulong Job { get; } = job;
        public ulong Host { get; } = host;
        public ulong? ConstructedHost { get; } = constructedHost;
        public string HostName { get; } = hostName;
        public int Changed { get; } = changed;
        public int Dark { get; } = dark;
        public int Failures { get; } = failures;
        public int OK { get; } = oK;
        public int Processed { get; } = processed;
        public int Skipped { get; } = skipped;
        public bool Failed { get; } = failed;
        public int Ignored { get; } = ignored;
        public int Rescued { get; } = rescued;
    }
}
