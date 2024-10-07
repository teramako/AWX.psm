using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;
using AWX.Resources;

namespace AWX.Cmdlets;

public abstract class GetCommandBase : APICmdletBase
{
    [Parameter(Mandatory = true,
               Position = 0,
               ValueFromRemainingArguments = true,
               ValueFromPipeline = true,
               ValueFromPipelineByPropertyName = true)]
    [PSDefaultValue(Value = 1, Help = "The resource ID")]
    public ulong[] Id { get; set; } = [];

    [Parameter(ValueFromPipelineByPropertyName = true, DontShow = true)]
    public ResourceType? Type { get; set; }

    protected readonly HashSet<ulong> IdSet = [];
    protected readonly NameValueCollection Query = HttpUtility.ParseQueryString("");
}
