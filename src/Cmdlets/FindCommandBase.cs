using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;
using AWX.Resources;

namespace AWX.Cmdlets;

/// <summary>
/// Abstract class for <c>Find-*</c> Cmdlet
/// <br/><br/>
/// <example>
/// Inherited this class should be add like following class attribute:
/// <code>
///     [Cmdlet(VerbsCommon.Find, "Description", DefaultParameterSetName = "All")]
/// </code>
/// </example>
/// </summary>
/// <inheritdoc cref="APICmdletBase"/>
public abstract class FindCommandBase : APICmdletBase
{
    public abstract ResourceType Type { get; set; }
    public abstract ulong Id { get; set; }

    /// <summary>
    /// <c>"search"</c> query parameter for API.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/searching.html">
    /// 5. Searching — Automation Controller API Guide
    /// </a>
    /// </summary>
    [Parameter()]
    public string[]? Search { get; set; }

    /// <summary>
    /// Filtering for various fields for API.
    /// The value is transformed to from one or more various types into a <see cref="NameValueCollection"/>.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/filtering.html">
    /// 6. Filtering — Automation Controller API Guide</a>
    /// </summary>
    /// <seealso cref="FilterArgumentTransformationAttribute"/>
    [Parameter()]
    [FilterArgumentTransformation]
    public NameValueCollection? Filter { get; set; }

    /// <summary>
    /// <c>"order_by"</c> query parameter for API.
    /// <br/>
    /// To sort in reverse (Descending), add <c>"!"</c> prefix instead of <c>"-"</c>.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/sorting.html">
    /// 4. Sorting — Automation Controller API Guide
    /// </a>
    /// </summary>
    public abstract string[] OrderBy { get; set; }

    /// <summary>
    /// Max size of per page.
    /// This parameter is converted to <c>"page_size"</c> query parameter for API.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
    /// 7. Pagination — Automation Controller API Guide
    /// </a>
    /// </summary>
    [Parameter()]
    [ValidateRange(1, 200)]
    public ushort Count { get; set; } = 20;

    /// <summary>
    /// <c>"page"</c> query parameter for API.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/pagination.html">
    /// 7. Pagination — Automation Controller API Guide
    /// </a>
    /// </summary>
    [Parameter()]
    [ValidateRange(1, ushort.MaxValue)]
    public uint Page { get; set; } = 1;

    /// <summary>
    /// Retrieve resources from <see cref="Page">Page</see> to the last, if this switch is true.<br/>
    /// <b>Caoution</b>: "GetAsync" request will be send every per pages.
    /// </summary>
    [Parameter()]
    public SwitchParameter All { get; set; }

    protected readonly NameValueCollection Query = HttpUtility.ParseQueryString("");

    /// <summary>
    /// Add query parameters to <see cref="Query">Query</see>
    /// <list type="bullet">
    ///     <item><see cref="Search">Search</see> to <c>search</c></item>
    ///     <item><see cref="OrderBy">OrderBy</see> to <c>order_by</c></item>
    ///     <item><see cref="Count">Count</see> to <c>page_size</c></item>
    ///     <item><see cref="Page">Page</see> to <c>page</c></item>
    ///     <item><see cref="Filter">Filter</see> to various fields</item>
    /// </list>
    /// </summary>
    protected virtual void SetupCommonQuery()
    {
        if (Search != null)
        {
            Query.Add("search", string.Join(',', Search));
        }
        if (OrderBy != null)
        {
            Query.Add("order_by",
                      string.Join(',', OrderBy.Select(item => item.StartsWith('!') ? $"-{item.Substring(1)}" : item)));
        }
        Query.Add("page_size", $"{Count}");
        Query.Add("page", $"{Page}");
        if (Filter != null)
        {
            Query.Add(Filter);
        }
    }

    protected virtual void Find<T>(string path) where T : class
    {
        foreach (var resultSet in GetResultSet<T>($"{path}?{Query}", All))
        {
            WriteObject(resultSet.Results, true);
        }
    }
}
