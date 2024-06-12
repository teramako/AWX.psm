using AnsibleTower.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ActivityStream")]
    [OutputType(typeof(ActivityStream))]
    public class GetActivityStreamCommand : GetCmdletBase<ActivityStream>
    {
    }

    [Cmdlet(VerbsCommon.Find, "ActivityStream", DefaultParameterSetName = "All")]
    [OutputType(typeof(ActivityStream))]
    public class FindActivityStreamCommand : FindCmdletBase<ActivityStream>
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ResourceType Type { get; set; }
        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];
    }
}
