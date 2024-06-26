using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Instance")]
    [OutputType(typeof(Instance))]
    public class GetInstanceCommand : GetCmdletBase
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
            foreach(var instance in Instance.Find(Query).ToBlockingEnumerable())
            {
                WriteObject(instance);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Instance", DefaultParameterSetName = "All")]
    [OutputType(typeof(Instance))]
    public class FindInstanceCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.InstanceGroup))]
        public override ResourceType Type { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var asyncInstances = Type switch
            {
                ResourceType.InstanceGroup => Instance.FindFromInstanceGroup(Id, Query, All),
                _ => Instance.Find(Query, All)
            };
            foreach (var instance in asyncInstances.ToBlockingEnumerable())
            {
                WriteObject(instance);
            }
        }
    }
}

