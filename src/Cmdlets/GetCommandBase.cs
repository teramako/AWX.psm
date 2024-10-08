using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;
using AWX.Resources;

namespace AWX.Cmdlets;

public abstract class GetCommandBase<TResource> : APICmdletBase where TResource: class
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

    private string? _apiPath = null;
    protected virtual string ApiPath
    {
        get
        {
            if (_apiPath != null)
                return _apiPath;

            _apiPath = GetApiPath(typeof(TResource));
            return _apiPath;
        }
        set
        {
            _apiPath = value;
        }
    }

    protected abstract ResourceType AcceptType { get; }
    /// <summary>
    /// Gather resource IDs to retrieve
    /// Primarily called from within the <see cref="Cmdlet.ProcessRecord"/> method
    /// </summary>
    protected void GatherResourceId()
    {
        if (Type != null && Type != AcceptType)
        {
            return;
        }
        foreach (var id in Id)
        {
            IdSet.Add(id);
        }
    }

    /// <summary>
    /// Retrieve and output the resource for the gathered ID.
    /// Primarily called from within the <see cref="Cmdlet.EndProcessing"/> method
    /// </summary>
    protected IEnumerable<TResource> GetResultSet()
    {
        if (IdSet.Count == 1)
        {
            var res = GetResource<TResource>($"{ApiPath}{IdSet.First()}/");
            if (res == null)
                yield break;
            yield return res;
        }
        else
        {
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<TResource>(ApiPath, Query, true))
            {
                foreach (var res in resultSet.Results)
                {
                    yield return res;
                }
            }
        }
    }

    /// <summary>
    /// Get and output resources individually.
    /// Primarily called from within the <see cref="Cmdlet.ProcessRecord"/> method
    /// </summary>
    /// <param name="subPath">sub path</param>
    protected IEnumerable<TResource> GetResource(string subPath = "")
    {
        if (Type != null && Type != AcceptType)
        {
            yield break;
        }
        foreach (var id in Id)
        {
            if (!IdSet.Add(id))
            {
                // skip already processed
                continue;
            }
            var res = GetResource<TResource>($"{ApiPath}{id}/{subPath}");
            if (res != null)
            {
                yield return res;
            }
        }
    }
}
