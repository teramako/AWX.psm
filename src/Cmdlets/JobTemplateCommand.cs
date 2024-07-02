using AWX.Resources;
using System.Collections;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "JobTemplate")]
    [OutputType(typeof(JobTemplate))]
    public class GetJobTemplate : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<JobTemplate>($"{JobTemplate.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "JobTemplate", DefaultParameterSetName = "All")]
    [OutputType(typeof(JobTemplate))]
    public class FindJobTemplateCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.JobTemplate))]
        public override ResourceType Type { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(',', Name));
            }
            SetupCommonQuery();
        }
        protected override void EndProcessing()
        {
            Find<JobTemplate>(JobTemplate.PATH);
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "JobTemplate")]
    public class StartJobTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id")]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true)]
        public JobTemplate? JobTemplate { get; set; }

        [Parameter()]
        public string? Limit { get; set; }

        [Parameter()]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        private readonly Dictionary<ulong, JobTask> jobTasks = [];
        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            return dict;
        }
        protected override void ProcessRecord()
        {
            if (JobTemplate != null)
            {
                Id = JobTemplate.Id;
            }
            var launchResult = CreateResource<JobTemplateJob.LaunchResult>($"{JobTemplate.PATH}{Id}/launch/", CreateSendData());
            if (launchResult == null)
            {
                return;
            }
            WriteVerbose($"Launch JobTemplate:{Id} => Job:[{launchResult.Job}]");
            // launchJobs.Add(launchResult.Job);
            jobTasks.Add(launchResult.Job, new JobTask(launchResult));
        }
        protected override void EndProcessing()
        {
            var start = DateTime.Now;
            var rootProgress = new ProgressRecord(0, "Launch Job", "Waiting...")
            {
                SecondsRemaining = IntervalSeconds
            };
            do
            {
                for(var i = 1; i <= IntervalSeconds; i++)
                {
                    Sleep(1000);
                    var elapsed = DateTime.Now - start;
                    rootProgress.PercentComplete = i * 100 / IntervalSeconds;
                    rootProgress.SecondsRemaining = IntervalSeconds - i;
                    rootProgress.StatusDescription = $"Waiting... Elapsed: {elapsed:hh\\:mm\\:ss\\.ff}";
                    WriteProgress(rootProgress);
                }
                using var task = UnifiedJob.Get(jobTasks.Keys.ToArray());
                var tasks = jobTasks.Values.Select(jobTask => jobTask.GetLogAsync()).ToArray();
                task.Wait();
                Task.WaitAll(tasks);

                // Remove Progressbar
                rootProgress.RecordType = ProgressRecordType.Completed;
                WriteProgress(rootProgress);

                foreach (var t in tasks)
                {
                    var jobTask = t.Result;
                    var color = Console.ForegroundColor;
                    if (tasks.Length > 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"====== {jobTask.Job.Name} ======");
                        Console.ForegroundColor = color;
                    }
                    Console.Write(jobTask.CurrentLog);
                }
                foreach (var job in task.Result)
                {
                    jobTasks[job.Id].Job = job;
                    switch (job.Status)
                    {
                        case JobStatus.New:
                        case JobStatus.Pending:
                        case JobStatus.Waiting:
                        case JobStatus.Running:
                            break;
                        default:
                            jobTasks.Remove(job.Id);
                            WriteObject(job, false);
                            break;
                    }
                }

            } while(jobTasks.Count != 0);
        }
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

    /*
    class JobProgress
    {
        public ulong Id { get; private set; }
        public ResourceType Type { get; private set; }
        bool Finished = false;
        bool Completed = false;
        Dictionary<ulong, JobProgress>? Children = null;
        int StartNext = 0;
        public ProgressRecord Progress { get; }
        public JobProgress(ulong id, int parentId = 0, UnifiedJob? job = null)
        {
            Id = id;
            var recordeId = (int)id >> 32;
            Progress = new ProgressRecord(recordeId, $"Wait Job {id}", $"Start")
            {
                ParentActivityId = parentId
            };

        }
    }
    */
}
