using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "SystemJobTemplate")]
    [OutputType(typeof(SystemJobTemplate))]
    public class GetSystemJobTemplateCommand : GetCmdletBase
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
            foreach (var resultSet in GetResultSet<SystemJobTemplate>($"{SystemJobTemplate.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "SystemJobTemplate")]
    [OutputType(typeof(SystemJobTemplate))]
    public class FindSystemJobTemplateCommand : FindCmdletBase
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

    [Cmdlet(VerbsLifecycle.Invoke, "SystemJobTemplate")]
    [OutputType(typeof(SystemJob))]
    public class InvokeSystemJobTemplateCommand : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Template", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncTemplate", ValueFromPipeline = true, Position = 0)]
        public SystemJobTemplate? SystemJobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncTemplate")]
        public SwitchParameter Async { get; set; }

        [Parameter()]
        public IDictionary? ExtraVars { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Template")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Template")]
        public SwitchParameter SuppressJobLog { get; set; }

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (ExtraVars != null)
            {
                dict.Add("extra_vars", ExtraVars);
            }
            return dict;
        }
        private void Launch(ulong id)
        {
            var launchResult = CreateResource<SystemJob>($"{SystemJobTemplate.PATH}{id}/launch/", CreateSendData());
            if (launchResult == null)
            {
                return;
            }
            WriteVerbose($"Launch SystemJobTemplate:{id} => Job:[{launchResult.Id}]");
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
            if (SystemJobTemplate != null)
            {
                Id = SystemJobTemplate.Id;
            }
            Launch(Id);
        }
        protected override void EndProcessing()
        {
            if (Async)
            {
                return;
            }
            WaitJobs("Launch SystemJobTemplate", IntervalSeconds, SuppressJobLog);
        }
    }
}
