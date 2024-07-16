using AWX.Resources;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
{
    public class JobProgressManager : Dictionary<ulong, JobProgress>
    {
        public void Add(IUnifiedJob job, int parnetId = 0)
        {
            if (!ContainsKey(job.Id))
            {
                var jp = new JobProgress(job, parnetId);
                this.Add(job.Id, jp);
            }
        }
        public void UpdateJob()
        {
            var getJobsTask = UnifiedJob.Get([.. Keys]);
            getJobsTask.Wait();
            List<Task> tasks = [];
            foreach (var job in getJobsTask.Result)
            {
                if (TryGetValue(job.Id, out var jp))
                {
                    tasks.Add(jp.UpdateJob(job));
                }
            }
            Task.WaitAll([.. tasks]);
        }
        public IEnumerable<JobProgress> GetAll()
        {
            foreach (var item in Values)
            {
                foreach (var jp in item.GetAll())
                {
                    yield return jp;
                }
            }
        }
        public IEnumerable<JobProgress?> GetJobLog()
        {
            List<Task<JobProgress?>> tasks = [];
            foreach (var jp in GetAll().Where(jp => !jp.Completed && jp.Type != ResourceType.WorkflowJob))
            {
                tasks.Add(jp.GetLogAsync());
            }
            Task.WaitAll([.. tasks]);
            return tasks.Select(t => t.Result);
        }
    }

    public class JobProgress
    {
        public JobProgress(ulong id, ResourceType type, int parentId = 0)
        {
            Id = id;
            Type = type;
            Progress = new ProgressRecord((int)(id % int.MaxValue), $"[{id}]", $"New")
            {
                ParentActivityId = parentId,
            };
            ParentId = parentId;
        }
        public JobProgress(IUnifiedJob job, int parentId = 0)
            : this(job.Id, job.Type, parentId)
        {
            Job = job;
            Progress.Activity = $"[{job.Id}]{job.Name}";
            Progress.StatusDescription = $"{job.Status} {job.Started} Elapsed: {job.Elapsed}";
        }
        public ulong Id { get; }
        public int ParentId { get; private set; } = 0;
        public ResourceType Type { get; private set; }
        public ProgressRecord Progress { get; }
        public IUnifiedJob? Job { get; private set; }
        public bool Finished { get; private set; } = false;
        public bool Completed { get; private set; } = false;
        public Dictionary<ulong, JobProgress> Children = [];

        public IEnumerable<JobProgress> GetAll()
        {
            yield return this;
            if (Type == ResourceType.WorkflowJob && Children.Count > 0)
            {
                foreach (var child in Children.Values)
                {
                    foreach (var jp in child.GetAll())
                    {
                        yield return jp;
                    }
                }
            }
        }
        public string CurrentLog { get; private set; } = string.Empty;
        protected uint JobLogStartNext { get; private set; } = 0;

        public async Task<JobProgress?> GetLogAsync()
        {
            if (Job == null) return null;
            if (Completed) return null;
            if (Job.Type == ResourceType.SystemJob)
            {
                CurrentLog = ((ISystemJob)Job).ResultStdout;
                // throw new NotImplementedException($"Geting SystemJob is not implemented now.");
            }
            else if(Job.Type == ResourceType.WorkflowJob)
            {
                return null;
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
        public async Task UpdateJob(IUnifiedJob job)
        {
            Job = job;
            if (job.Finished != null)
            {
                if (Finished)
                {
                    SetComplete();
                }
                else
                {
                    Finished = true;
                }
            }
            Progress.Activity = $"[{job.Id}]{job.Name}";
            Progress.StatusDescription = $"{job.Status} {job.Started} Elapsed: {job.Elapsed}";
            Progress.PercentComplete = job.Status switch
            {
                JobStatus.New => 0,
                JobStatus.Waiting => 10,
                JobStatus.Pending => 10,
                JobStatus.Running => 30,
                _ => 100
            };
            if (Completed)
            {
                if (Children.Count > 0)
                {
                    Children.Clear();
                }
                return;
            }
            if (job.Type == ResourceType.WorkflowJob)
            {
                await UpdateWorkflowJobNodes();
            }
        }
        public bool SetComplete()
        {
            if (!Finished)
            {
                return false;
            }
            if (Children.Count > 0)
            {
                Completed = Children.Values.All(jp => jp.SetComplete());
            }
            else
            {
                Completed = true;
            }
            return Completed;
        }
        private async Task UpdateWorkflowJobNodes()
        {
            var query = HttpUtility.ParseQueryString("do_not_run=False&page_size=50&order_by=id");
            var completedIds = Children.Values.Where(jp => jp.Completed).Select(jp => jp.Id).ToArray();
            if (completedIds.Length > 0)
            {
                query.Add("not__job__in", string.Join(',', completedIds));
            }
            var result = await RestAPI.GetAsync<ResultSet<WorkflowJobNode>>($"{WorkflowJob.PATH}{Id}/workflow_nodes/?{query}");
            var jobs = result.Contents.Results.Where(n => n.Job != null).Select(n => n.Job ?? 0);
            List<Task> tasks = [];
            foreach (var job in await UnifiedJob.Get(jobs.ToArray()))
            {
                if (Children.TryGetValue(job.Id, out var childJP))
                {
                    tasks.Add(childJP.UpdateJob(job));
                }
                else
                {
                    var jp = new JobProgress(job.Id, job.Type, Progress.ActivityId);
                    Children.Add(job.Id, jp);
                    tasks.Add(jp.UpdateJob(job));
                }
            }
            Task.WaitAll([.. tasks]);
        }
    }
}
