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
            if (Type != null && Type != ResourceType.InventoryUpdate)
            {
                return;
            }
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
                     nameof(ResourceType.InventorySource))]
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

    public class LaunchInventoryUpdateCommandBase : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "InventorySource", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventorySource", ValueFromPipeline = true, Position = 0)]
        public InventorySource? InventorySource { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Inventory", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventory", ValueFromPipeline = true, Position = 0)]
        public Inventory? Inventory { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "CheckId")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventorySource")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckInventory")]
        public SwitchParameter Check { get; set; }

        protected void CheckCanUpdateInventorySource(ulong id)
        {
            var res = GetResource<CanUpdateInventorySource>($"{InventorySource.PATH}{id}/update/");
            if (res == null)
            {
                return;
            }
            var psobject = new PSObject();
            psobject.Members.Add(new PSNoteProperty("Id", id));
            psobject.Members.Add(new PSNoteProperty("Type", ResourceType.InventorySource));
            psobject.Members.Add(new PSNoteProperty("CanUpdate", res.CanUpdate));
            WriteObject(psobject, false);
        }
        protected void CheckCanUpdateInventory(Inventory inventory)
        {
            var results = GetResource<CanUpdateInventorySource[]>($"{Inventory.PATH}{inventory.Id}/update_inventory_sources/");
            if (results == null)
            {
                return;
            }
            foreach (var res in results)
            {
                var psobject = new PSObject();
                psobject.Members.Add(new PSNoteProperty("Id", res.InventorySource));
                psobject.Members.Add(new PSNoteProperty("Type", ResourceType.InventorySource));
                psobject.Members.Add(new PSNoteProperty("CanUpdate", res.CanUpdate));
                WriteObject(psobject, false);
            }
        }
        protected InventoryUpdateJob.Detail UpdateInventorySource(ulong id)
        {
            var apiResult = CreateResource<InventoryUpdateJob.Detail>($"{InventorySource.PATH}{id}/update/");
            return apiResult.Contents;
        }
        protected InventoryUpdateJob.Detail[] UpdateInventory(Inventory inventory)
        {
            var apiResult = CreateResource<InventoryUpdateJob.Detail[]>($"{Inventory.PATH}{inventory.Id}/update_inventory_sources/");
            return apiResult.Contents;
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
                    try
                    {
                        UpdateInventory(Inventory);
                    }
                    catch (RestAPIException) {}
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
                    try
                    {
                        UpdateInventorySource(Id);
                    }
                    catch (RestAPIException) {}
                }
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "InventoryUpdate")]
    [OutputType(typeof(InventoryUpdateJob), ParameterSetName = ["Id", "InventorySource", "Inventory"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckInventorySource", "CheckInventory"])]
    public class InvokeInventoryUpdateCommand : LaunchInventoryUpdateCommandBase
    {
        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "InventorySource")]
        [Parameter(ParameterSetName = "Inventory")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "InventorySource")]
        [Parameter(ParameterSetName = "Inventory")]
        public SwitchParameter SuppressJobLog { get; set; }

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
                    try
                    {
                        foreach (var job in UpdateInventory(Inventory))
                        {
                            WriteVerbose($"Update InventorySource:{job.InventorySource} => Job:[{job.Id}]");
                            JobManager.Add(job);
                        }

                    }
                    catch (RestAPIException) {}
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
                    try
                    {
                        var job = UpdateInventorySource(Id);
                        WriteVerbose($"Update InventorySource:{job.InventorySource} => Job:[{job.Id}]");
                        JobManager.Add(job);
                    }
                    catch (RestAPIException) {}
                }
            }
        }
        protected override void EndProcessing()
        {
            if (Check)
            {
                return;
            }
            WaitJobs("Update InventorySource", IntervalSeconds, SuppressJobLog);
        }
    }

    [Cmdlet(VerbsLifecycle.Start, "InventoryUpdate")]
    [OutputType(typeof(InventoryUpdateJob.Detail), ParameterSetName = ["AsyncId", "AsyncInventorySource", "AsyncInventory"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckInventorySource", "CheckInventory"])]
    public class StartInventoryUpdateCommand : LaunchInventoryUpdateCommandBase
    {
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
                    try
                    {
                        var jobs = UpdateInventory(Inventory);
                        WriteObject(jobs, true);
                    }
                    catch (RestAPIException) {}
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
                    try
                    {
                        var job = UpdateInventorySource(Id);
                        WriteObject(job, false);
                    }
                    catch (RestAPIException) {}
                }
            }
        }
    }
}
