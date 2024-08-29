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
            if (Type != null && Type != ResourceType.SystemJobTemplate)
            {
                return;
            }
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            if (IdSet.Count == 1)
            {
                var res = GetResource<SystemJobTemplate>($"{SystemJobTemplate.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<SystemJobTemplate>(SystemJobTemplate.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
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

    public class LaunchSystemJobTemplateCommandBase : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Template", ValueFromPipeline = true, Position = 0)]
        public SystemJobTemplate? SystemJobTemplate { get; set; }

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
            var apiResult = CreateResource<SystemJob.Detail>($"{SystemJobTemplate.PATH}{id}/launch/", CreateSendData());
            return apiResult.Contents;
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "SystemJobTemplate")]
    [OutputType(typeof(SystemJob))]
    public class InvokeSystemJobTemplateCommand : LaunchSystemJobTemplateCommandBase
    {
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

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
                JobManager.Add(job);
            }
            catch (RestAPIException) {}
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
            catch (RestAPIException) {}
        }
    }
}
