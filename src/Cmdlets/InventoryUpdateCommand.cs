using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "InventoryUpdateJob")]
    [OutputType(typeof(InventoryUpdateJob))]
    public class GetInventoryUpdateJobCommand : GetCmdletBase
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
                var res = GetResource<InventoryUpdateJob.Detail>($"{InventoryUpdateJob.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "InventoryUpdateJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(InventoryUpdateJob))]
    public class FindInventoryUpdateJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.ProjectUpdate),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.Host))]
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
                ResourceType.ProjectUpdate => $"{ProjectUpdateJob.PATH}{Id}/scm_inventory_updates/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/inventory_updates/",
                _ => InventoryUpdateJob.PATH
            };
            foreach (var resultSet in GetResultSet<InventoryUpdateJob>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "InventoryUpdate")]
    [OutputType(typeof(InventoryUpdateJob), ParameterSetName = ["Id", "InventorySource", "Inventory"])]
    [OutputType(typeof(InventoryUpdateJob.Detail), ParameterSetName = ["AsyncId", "AsyncInventorySource", "AsyncInventory"])]
    [OutputType(typeof(CanUpdateInventorySource), ParameterSetName = ["CheckId", "CheckInventorySource", "CheckInventory"])]
    public class InvokeInventoryUpdateCommand: InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncId", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "InventorySource", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventorySource", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventorySource", ValueFromPipeline = true, Position = 0)]
        public InventorySource? InventorySource { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Inventory", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventory", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventory", ValueFromPipeline = true, Position = 0)]
        public Inventory? Inventory { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "CheckId")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventorySource")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventory")]
        public SwitchParameter Check { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventorySource")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncInventory")]
        public SwitchParameter Async { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "InventorySource")]
        [Parameter(ParameterSetName = "Inventory")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "InventorySource")]
        [Parameter(ParameterSetName = "Inventory")]
        public SwitchParameter SuppressJobLog { get; set; }


        private void CheckCanUpdateInventorySource(ulong id)
        {
            var res = GetResource<CanUpdateInventorySource>($"{InventorySource.PATH}{id}/update/");
            if (res == null)
            {
                return;
            }
            var result = res with { InventorySource = id };
            WriteObject(result, false);
        }
        private void CheckCanUpdateInventory(Inventory inventory)
        {
            var results = GetResource<CanUpdateInventorySource[]>($"{Inventory.PATH}{inventory.Id}/update_inventory_sources/");
            if (results == null)
            {
                return;
            }
            WriteObject(results, true);
        }
        private void UpdateInventorySource(ulong id)
        {
            var job = CreateResource<InventoryUpdateJob.Detail>($"{InventorySource.PATH}{id}/update/");
            if (job == null)
            {
                return;
            }
            ProcessJob(job);
        }
        private void UpdateInventory(Inventory inventory)
        {
            var jobs = CreateResource<InventoryUpdateJob.Detail[]>($"{Inventory.PATH}{inventory.Id}/update_inventory_sources/");
            if (jobs == null)
            {
                return;
            }
            foreach (var job in jobs)
            {
                ProcessJob(job);
            }
        }
        private void ProcessJob(InventoryUpdateJob.Detail job)
        {
            WriteVerbose($"Update InventorySource:{job.InventorySource} => Job:[{job.Id}]");
            if (Async)
            {
                WriteObject(job, false);
            }
            else
            {
                jobTasks.Add(job.Id, new JobTask(job));
            }
        }
        protected override void ProcessRecord()
        {
            if (Inventory != null)
            {
                if (Check)
                {
                    CheckCanUpdateInventory(Inventory);
                }
                else
                {
                    UpdateInventory(Inventory);
                }
            }
            else
            {
                if (InventorySource != null)
                {
                    Id = InventorySource.Id;
                }
                if (Check)
                {
                    CheckCanUpdateInventorySource(Id);
                }
                else
                {
                    UpdateInventorySource(Id);
                }
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
            WaitJobs("Update InventorySource", IntervalSeconds, SuppressJobLog);
        }
    }
}
