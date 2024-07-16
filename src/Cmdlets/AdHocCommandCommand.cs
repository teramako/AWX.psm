using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "AdHocCommandJob")]
    [OutputType(typeof(AdHocCommand.Detail))]
    public class GetAdHocCommandJobCommand : GetCmdletBase
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
                var res = GetResource<AdHocCommand.Detail>($"{AdHocCommand.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "AdHocCommandJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(AdHocCommand))]
    public class FindAdHocCommandJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.Group))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];


        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/ad_hoc_commands/",
                ResourceType.Host => $"{Host.PATH}{Id}/ad_hoc_commands/",
                ResourceType.Group => $"{Group.PATH}{Id}/ad_hoc_commands/",
                _ => AdHocCommand.PATH
            };
            foreach (var resultSet in GetResultSet<AdHocCommand>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "AdHocCommand")]
    [OutputType(typeof(AdHocCommand))]
    public class InvokeAdHocCommand : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Host", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncHost", ValueFromPipeline = true, Position = 0)]
        public Host? Host { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Group", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncGroup", ValueFromPipeline = true, Position = 0)]
        public Group? Group { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Inventory", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventory", ValueFromPipeline = true, Position = 0)]
        public Inventory? Inventory { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "InventoryId", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventoryId", ValueFromPipeline = true, Position = 0)]
        public ulong? InventoryId { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string ModuleName { get; set; } = string.Empty;

        [Parameter(Position = 2)]
        [AllowEmptyString]
        public string ModuleArgs { get; set; } = string.Empty;

        [Parameter(Mandatory = true, Position = 3)]
        public ulong Credential { get; set; }

        [Parameter()]
        public string Limit { get; set; } = string.Empty;

        [Parameter()]
        public SwitchParameter Check { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventory")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventoryId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncHost")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncGroup")]
        public SwitchParameter Async { get; set; }

        [Parameter(ParameterSetName = "Host")]
        [Parameter(ParameterSetName = "Group")]
        [Parameter(ParameterSetName = "Inventory")]
        [Parameter(ParameterSetName = "InventoryId")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Host")]
        [Parameter(ParameterSetName = "Group")]
        [Parameter(ParameterSetName = "Inventory")]
        [Parameter(ParameterSetName = "InventoryId")]
        public SwitchParameter SuppressJobLog { get; set; }


        protected Hashtable SendData { get; set; } = [];
        protected override void BeginProcessing()
        {
            SendData.Add("module_name", ModuleName);
            SendData.Add("module_args", ModuleArgs);
            SendData.Add("credential", Credential);
            if (Check)
            {
                SendData.Add("job_type", "check");
            }
            if (Limit != null)
            {
                SendData.Add("limit", Limit);
            }
        }
        protected override void ProcessRecord()
        {
            AdHocCommand? job = null;
            if (InventoryId  != null)
            {
                job = CreateResource<AdHocCommand>($"{Inventory.PATH}{InventoryId}/ad_hoc_commands/", SendData);
            }
            else if (Inventory != null)
            {
                job = CreateResource<AdHocCommand>($"{Inventory.PATH}{Inventory.Id}/ad_hoc_commands/", SendData);
            }
            else if (Host != null)
            {
                job = CreateResource<AdHocCommand>($"{Host.PATH}{Host.Id}/ad_hoc_commands/", SendData);
            }
            else if (Group != null)
            {
                job = CreateResource<AdHocCommand>($"{Group.PATH}{Group.Id}/ad_hoc_commands/", SendData);
            }

            if (job == null)
            {
                return;
            }
            WriteVerbose($"Invoke AdHocCommand:{job.Name} => Job:[{job.Id}]");
            if (Async)
            {
                WriteObject(job, false);
            }
            else
            {
                JobManager.Add(job);
            }
        }
        protected override void EndProcessing()
        {
            if (Async)
            {
                return;
            }
            WaitJobs("Invoke AdHocCommand", IntervalSeconds, SuppressJobLog);
        }
    }
}
