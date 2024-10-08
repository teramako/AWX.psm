using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "InventoryUpdateJob")]
    [OutputType(typeof(InventoryUpdateJob.Detail))]
    public class GetInventoryUpdateJobCommand : GetCommandBase<InventoryUpdateJob.Detail>
    {
        protected override string ApiPath => InventoryUpdateJob.PATH;
        protected override ResourceType AcceptType => ResourceType.InventoryUpdate;

        protected override void ProcessRecord()
        {
            WriteObject(GetResource(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "InventoryUpdateJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(InventoryUpdateJob))]
    public class FindInventoryUpdateJobCommand : FindCommandBase
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

    public class LaunchInventoryUpdateCommandBase : LaunchJobCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Resource", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "CheckResource", ValueFromPipeline = true, Position = 0)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Inventory,
                ResourceType.InventorySource
        ])]
        public IResource? Source { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "CheckId")]
        [Parameter(Mandatory = true, ParameterSetName = "CheckResource")]
        public SwitchParameter Check { get; set; }

        protected void CheckCanUpdate(IResource source)
        {
            switch (source.Type)
            {
                case ResourceType.Inventory:
                    CheckCanUpdateInventory(source.Id);
                    break;
                case ResourceType.InventorySource:
                    CheckCanUpdateInventorySource(source.Id);
                    break;
            }
        }
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
        protected void CheckCanUpdateInventory(ulong id)
        {
            var results = GetResource<CanUpdateInventorySource[]>($"{Inventory.PATH}{id}/update_inventory_sources/");
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
            return apiResult.Contents ?? throw new NullReferenceException();
        }
        protected InventoryUpdateJob.Detail[] UpdateInventory(ulong id)
        {
            var apiResult = CreateResource<InventoryUpdateJob.Detail[]>($"{Inventory.PATH}{id}/update_inventory_sources/");
            return apiResult.Contents ?? throw new NullReferenceException();
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "InventoryUpdate")]
    [OutputType(typeof(InventoryUpdateJob), ParameterSetName = ["Id", "Resource"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckResource"])]
    public class InvokeInventoryUpdateCommand : LaunchInventoryUpdateCommandBase
    {
        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Resource")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "Resource")]
        public SwitchParameter SuppressJobLog { get; set; }

        protected override void ProcessRecord()
        {
            if (Source == null)
            {
                Source = new Resource(ResourceType.InventorySource, Id);
            }

            if (Check)
            {
                CheckCanUpdate(Source);
                return;
            }

            switch (Source.Type)
            {
                case ResourceType.Inventory:
                    try
                    {
                        foreach (var job in UpdateInventory(Source.Id))
                        {
                            WriteVerbose($"Update InventorySource:{job.InventorySource} => Job:[{job.Id}]");
                            JobProgressManager.Add(job);
                        }
                    }
                    catch (RestAPIException) { }
                    break;
                case ResourceType.InventorySource:
                    try
                    {
                        var job = UpdateInventorySource(Id);
                        WriteVerbose($"Update InventorySource:{job.InventorySource} => Job:[{job.Id}]");
                        JobProgressManager.Add(job);
                    }
                    catch (RestAPIException) { }
                    break;
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
    [OutputType(typeof(InventoryUpdateJob.Detail), ParameterSetName = ["Id", "Resource"])]
    [OutputType(typeof(PSObject), ParameterSetName = ["CheckId", "CheckResource"])]
    public class StartInventoryUpdateCommand : LaunchInventoryUpdateCommandBase
    {
        protected override void ProcessRecord()
        {
            if (Source == null)
            {
                Source = new Resource(ResourceType.InventorySource, Id);
            }

            if (Check)
            {
                CheckCanUpdate(Source);
                return;
            }

            switch (Source.Type)
            {
                case ResourceType.Inventory:
                    try
                    {
                        var jobs = UpdateInventory(Source.Id);
                        WriteObject(jobs, true);
                    }
                    catch (RestAPIException) { }
                    break;
                case ResourceType.InventorySource:
                    try
                    {
                        var job = UpdateInventorySource(Source.Id);
                        WriteObject(job, false);
                    }
                    catch (RestAPIException) { }
                    break;
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "InventoryUpdateJob", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveInventoryUpdateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.InventoryUpdate])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"InventoryUpdate [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{InventoryUpdateJob.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"InventoryUpdate {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
