using System.Collections.Specialized;

namespace AWX.Resources
{
    public class SystemJobTemplate(ulong id,
                                   ResourceType type,
                                   string url,
                                   RelatedDictionary related,
                                   SystemJobTemplate.Summary summaryFields,
                                   DateTime created,
                                   DateTime? modified,
                                   string name,
                                   string description,
                                   DateTime? lastJobRun,
                                   bool lastJobFailed,
                                   DateTime? nextJobRun,
                                   JobTemplateStatus status,
                                   ulong? executionEnvironment,
                                   string jobType)
        : UnifiedJobTemplate(id, type, url, created, modified, name, description, lastJobRun,
                             lastJobFailed, nextJobRun, status),
          IUnifiedJobTemplate, IResource<SystemJobTemplate.Summary>
    {
        public new const string PATH = "/api/v2/system_job_templates/";

        /// <summary>
        /// Retrieve a System Job Template.<br/>
        /// API Path: <c>/api/v2/system_job_templates/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<SystemJobTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<SystemJobTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List System Job Templates.<br/>
        /// API Path: <c>/api/v2/system_job_templates/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static new async IAsyncEnumerable<SystemJobTemplate> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<SystemJobTemplate>(PATH, query, getAll))
            {
                foreach (var systemJobTemplate in result.Contents.Results)
                {
                    yield return systemJobTemplate;
                }
            }
        }
        public record Summary(LastJobSummary? LastJob,
                              LastUpdateSummary? LastUpdate,
                              EnvironmentSummary? ResolvedEnvironment);


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public string JobType { get; } = jobType;
    }
}
