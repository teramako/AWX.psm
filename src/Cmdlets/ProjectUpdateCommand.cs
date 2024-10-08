using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ProjectUpdateJob")]
    [OutputType(typeof(ProjectUpdateJob.Detail))]
    public class GetProjectUpdateJobCommand : GetCommandBase<ProjectUpdateJob.Detail>
    {
        protected override ResourceType AcceptType => ResourceType.ProjectUpdate;

        protected override void ProcessRecord()
        {
            WriteObject(GetResource(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "ProjectUpdateJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(ProjectUpdateJob))]
    public class FindProjectUpdateJobCommand : FindCommandBase
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

    public abstract class LaunchProjectUpdateCommandBase : LaunchJobCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Project", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckProject", ValueFromPipeline = true, Position = 0)]
        [ResourceTransformation(AcceptableTypes = [ResourceType.Project])]
        public IResource? Project { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "CheckId")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckProject")]
        public SwitchParameter Check { get; set; }

        protected void CheckCanUpdate(ulong projectId)
        {
            var res = GetResource<CanUpdateProject>($"{Resources.Project.PATH}{projectId}/update/");
            if (res == null)
            {
                return;
            }
            var psobject = new PSObject();
            psobject.Members.Add(new PSNoteProperty("Id", projectId));
            psobject.Members.Add(new PSNoteProperty("Type", ResourceType.Project));
            psobject.Members.Add(new PSNoteProperty("CanUpdate", res.CanUpdate));
            WriteObject(psobject, false);
        }
        protected ProjectUpdateJob.Detail UpdateProject(ulong projectId)
        {
            var apiResult = CreateResource<ProjectUpdateJob.Detail>($"{Resources.Project.PATH}{projectId}/update/");
            return apiResult.Contents ?? throw new NullReferenceException();
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "ProjectUpdate")]
    [OutputType(typeof(ProjectUpdateJob), ParameterSetName = ["Id", "Project"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckProject"])]
    public class InvokeProjectUpdateCommand : LaunchProjectUpdateCommandBase
    {
        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Project")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Project")]
        public SwitchParameter SuppressJobLog { get; set; }

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
                try
                {
                    var job = UpdateProject(Id);
                    WriteVerbose($"Update Project:{Id} => Job:[{job.Id}]");
                    JobProgressManager.Add(job);
                }
                catch (RestAPIException) { }
            }
        }
        protected override void EndProcessing()
        {
            if (Check)
            {
                return;
            }
            WaitJobs("Update Project", IntervalSeconds, SuppressJobLog);
        }
    }

    [Cmdlet(VerbsLifecycle.Start, "ProjectUpdate")]
    [OutputType(typeof(ProjectUpdateJob.Detail), ParameterSetName = ["Id", "Project"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckProject"])]
    public class StartProjectUpdateCommand : LaunchProjectUpdateCommandBase
    {
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
                try
                {
                    var job = UpdateProject(Id);
                    WriteVerbose($"Update Project:{Id} => Job:[{job.Id}]");
                    WriteObject(job, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "ProjectUpdateJob", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProjectUpdateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ProjectUpdate])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"ProjectUpdate [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{ProjectUpdateJob.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"ProjectUpdate {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
