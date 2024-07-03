using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ProjectUpdateJob")]
    [OutputType(typeof(ProjectUpdateJob.Detail))]
    public class GetProjectUpdateJobCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                if (!IdSet.Add(id))
                {
                    // skip already processed
                    continue;
                }
                var res = GetResource<ProjectUpdateJob.Detail>($"{ProjectUpdateJob.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "ProjectUpdateJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(ProjectUpdateJob))]
    public class FindProjectUpdateJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Project))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        [ValidateSet(typeof(EnumValidateSetGenerator<JobStatus>))]
        public string[]? Status { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];


        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(',', Name));
            }
            if (Status != null)
            {
                Query.Add("status__in", string.Join(',', Status));
            }
            SetupCommonQuery();
        }
        protected override void EndProcessing()
        {
            var path = Type switch
            {
                ResourceType.Project => $"{Project.PATH}{Id}/project_updates/",
                _ => ProjectUpdateJob.PATH
            };
            foreach (var resultSet in GetResultSet<ProjectUpdateJob>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "ProjectUpdate")]
    [OutputType(typeof(ProjectUpdateJob), ParameterSetName = ["Id", "Project"])]
    [OutputType(typeof(ProjectUpdateJob.Detail), ParameterSetName = ["AsyncId", "AsyncProject"])]
    [OutputType(typeof(CanUpdateProject), ParameterSetName = ["CheckId", "CheckProject"])]
    public class InvokeProjectUpdateCommand: InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncId", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Project", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncProject", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckProject", ValueFromPipeline = true, Position = 0)]
        public Project? Project { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "CheckId")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckProject")]
        public SwitchParameter Check { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncProject")]
        public SwitchParameter Async { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Project")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Project")]
        public SwitchParameter SuppressJobLog { get; set; }


        private void CheckCanUpdate(ulong projectId)
        {
            var res = GetResource<CanUpdateProject>($"{Project.PATH}{projectId}/update/");
            if (res == null)
            {
                return;
            }
            var result = res with { Project = projectId };
            WriteObject(result, false);
        }
        private void UpdateProject(ulong projectId)
        {
            var launchResult = CreateResource<ProjectUpdateJob.Detail>($"{Project.PATH}{projectId}/update/");
            if (launchResult == null)
            {
                return;
            }
            WriteVerbose($"Update Project:{Id} => Job:[{launchResult.Id}]");
            if (Async)
            {
                WriteObject(launchResult, false);
            }
            else
            {
                jobTasks.Add(launchResult.Id, new JobTask(launchResult));
            }
        }
        protected override void ProcessRecord()
        {
            if (Project != null)
            {
                Id = Project.Id;
            }
            if (Check)
            {
                CheckCanUpdate(Id);
            }
            else
            {
                UpdateProject(Id);
            }
        }
        protected override void EndProcessing()
        {
            if (Check)
            {
                return;
            }
            if (Async)
            {
                return;
            }
            WaitJobs("Update Project", IntervalSeconds, SuppressJobLog);
        }
    }
}
