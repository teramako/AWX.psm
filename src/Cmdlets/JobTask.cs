using AWX.Resources;
using System.Web;

namespace AWX.Cmdlets
{
    public class JobTask(IUnifiedJob job)
    {
        public ulong Id { get { return Job.Id; } }
        public IUnifiedJob Job {
            get { return _job; }
            set { 
                if (_job.Id == value.Id)
                {
                    _job = value;
                }
            }
        }
        private IUnifiedJob _job = job;
        public string CurrentLog { get; private set; } = string.Empty;
        protected uint JobLogStartNext { get; private set; } = 0;

        public async Task<JobTask> GetLogAsync()
        {
            if (Job.Type == ResourceType.SystemJob)
            {
                CurrentLog = ((ISystemJob)Job).ResultStdout;
                // throw new NotImplementedException($"Geting SystemJob is not implemented now.");
            }
            else
            {
                var query = HttpUtility.ParseQueryString("format=json");
                query.Add("start_line", $"{JobLogStartNext}");
                var apiResult = await RestAPI.GetAsync<JobLog>($"{Job.Url}stdout/?{query}");
                var log = apiResult.Contents;
                JobLogStartNext = log.Range.End;
                CurrentLog = log.Content;
            }
            return this;
        }
    }
}
