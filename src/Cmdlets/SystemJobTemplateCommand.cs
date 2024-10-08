using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "SystemJobTemplate")]
    [OutputType(typeof(SystemJobTemplate))]
    public class GetSystemJobTemplateCommand : GetCommandBase<SystemJobTemplate>
    {
        protected override ResourceType AcceptType => ResourceType.SystemJobTemplate;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "SystemJobTemplate")]
    [OutputType(typeof(SystemJobTemplate))]
    public class FindSystemJobTemplateCommand : FindCommandBase
    {
        public override ResourceType Type { get; set; }
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];


        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = SystemJobTemplate.PATH;
            foreach (var resultSet in GetResultSet<SystemJobTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    public class LaunchSystemJobTemplateCommandBase : LaunchJobCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Template", ValueFromPipeline = true, Position = 0)]
        [ResourceTransformation(AcceptableTypes = [ResourceType.SystemJobTemplate])]
        public IResource? SystemJobTemplate { get; set; }

        [Parameter()]
        public IDictionary? ExtraVars { get; set; }

        protected Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (ExtraVars != null)
            {
                dict.Add("extra_vars", ExtraVars);
            }
            return dict;
        }
        protected SystemJob.Detail Launch(ulong id)
        {
            var apiResult = CreateResource<SystemJob.Detail>($"{Resources.SystemJobTemplate.PATH}{id}/launch/", CreateSendData());
            return apiResult.Contents ?? throw new NullReferenceException();
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "SystemJobTemplate")]
    [OutputType(typeof(SystemJob))]
    public class InvokeSystemJobTemplateCommand : LaunchSystemJobTemplateCommandBase
    {
        [Parameter()]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter()]
        public SwitchParameter SuppressJobLog { get; set; }

        protected override void ProcessRecord()
        {
            if (SystemJobTemplate != null)
            {
                Id = SystemJobTemplate.Id;
            }
            try
            {
                var job = Launch(Id);
                WriteVerbose($"Launch SystemJobTemplate:{Id} => Job:[{job.Id}]");
                JobProgressManager.Add(job);
            }
            catch (RestAPIException) { }
        }
        protected override void EndProcessing()
        {
            WaitJobs("Launch SystemJobTemplate", IntervalSeconds, SuppressJobLog);
        }
    }

    [Cmdlet(VerbsLifecycle.Start, "SystemJobTemplate")]
    [OutputType(typeof(SystemJob.Detail))]
    public class StartSystemJobTemplateCommand : LaunchSystemJobTemplateCommandBase
    {
        protected override void ProcessRecord()
        {
            if (SystemJobTemplate != null)
            {
                Id = SystemJobTemplate.Id;
            }
            try
            {
                var job = Launch(Id);
                WriteVerbose($"Launch SystemJobTemplate:{Id} => Job:[{job.Id}]");
                WriteObject(job);
            }
            catch (RestAPIException) { }
        }
    }
}
