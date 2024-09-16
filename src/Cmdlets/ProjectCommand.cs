using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Project")]
    [OutputType(typeof(Project))]
    public class GetProjectCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
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
                var res = GetResource<Project>($"{Project.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Project>(Project.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Project", DefaultParameterSetName = "All")]
    [OutputType(typeof(Project))]
    public class FindProjectCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Team))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/projects/",
                ResourceType.User => $"{User.PATH}{Id}/projects/",
                ResourceType.Team => $"{Team.PATH}{Id}/projects/",
                _ => Project.PATH
            };
            foreach (var resultSet in GetResultSet<Project>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "Playbook")]
    [OutputType(typeof(string))]
    public class GetPlaybookCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
            {
                return;
            }
            foreach (var id in Id)
            {
                if (IdSet.Add(id))
                {
                    var playbooks = GetResource<string[]>($"{Project.PATH}{id}/playbooks/");
                    WriteObject(playbooks, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "InventoryFile")]
    [OutputType(typeof(string))]
    public class GetInventoryFileCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
            {
                return;
            }
            foreach (var id in Id)
            {
                if (IdSet.Add(id))
                {
                    var inventoryFiles = GetResource<string[]>($"{Project.PATH}{id}/inventories/");
                    WriteObject(inventoryFiles, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Project", DefaultParameterSetName = "Manual", SupportsShouldProcess = true)]
    [OutputType(typeof(Project))]
    public class AddProjectCommand : APICmdletBase
    {
        [Parameter(ParameterSetName = "Manual", Mandatory = true)]
        public SwitchParameter Local { get; set; }

        [Parameter(ParameterSetName = "Git", Mandatory = true)]
        public SwitchParameter Git { get; set; }

        [Parameter(ParameterSetName = "Svn", Mandatory = true)]
        public SwitchParameter Subversion { get; set; }

        [Parameter(ParameterSetName = "Insights", Mandatory = true)]
        public SwitchParameter Insights { get; set; }

        [Parameter(ParameterSetName = "Archive", Mandatory = true)]
        public SwitchParameter RemoteArchive { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        public string Description { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Organization { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong DefaultEnvironment { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong SignatureValidationCredential { get; set; }

        [Parameter(ParameterSetName = "Manual", Mandatory = true)]
        public string LocalPath { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "Git", Mandatory = true)]
        [Parameter(ParameterSetName = "Svn", Mandatory = true)]
        [Parameter(ParameterSetName = "Archive", Mandatory = true)]
        public string ScmUrl { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        public string ScmBranch { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "Git")]
        public string ScmRefspec { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        [Parameter(ParameterSetName = "Insights", Mandatory = true)]
        [Parameter(ParameterSetName = "Archive")]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong Credential { get; set; }

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        [Parameter(ParameterSetName = "Insights")]
        [Parameter(ParameterSetName = "Archive")]
        public SwitchParameter ScmClean { get; set; }

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        [Parameter(ParameterSetName = "Insights")]
        [Parameter(ParameterSetName = "Archive")]
        public SwitchParameter ScmDeleteOnUpdate { get; set; }

        [Parameter(ParameterSetName = "Git")]
        public SwitchParameter ScmTrackSubmodules { get; set; }

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        [Parameter(ParameterSetName = "Insights")]
        [Parameter(ParameterSetName = "Archive")]
        public SwitchParameter ScmUpdateOnLaunch { get; set; }

        [Parameter(ParameterSetName = "Git")]
        [Parameter(ParameterSetName = "Svn")]
        [Parameter(ParameterSetName = "Archive")]
        public SwitchParameter AllowOverride { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int Timeout { get; set; }

        private Dictionary<string, object> CreateSendData()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "organization", Organization },
            };
            if (DefaultEnvironment > 0)
                sendData.Add("default_environment", DefaultEnvironment);
            if (SignatureValidationCredential > 0)
                sendData.Add("signature_validation_credential", SignatureValidationCredential);

            if (Local)
                sendData.Add("scm_type", "");
            else if (Git)
                sendData.Add("scm_type", "git");
            else if (Subversion)
                sendData.Add("scm_type", "svn");
            else if (Insights)
                sendData.Add("scm_type", "insights");
            else if (RemoteArchive)
                sendData.Add("scm_type", "archive");

            if (!string.IsNullOrEmpty(LocalPath))
                sendData.Add("local_path", LocalPath);
            if (!string.IsNullOrEmpty(ScmUrl))
                sendData.Add("scm_url", ScmUrl);
            if (!string.IsNullOrEmpty(ScmBranch))
                sendData.Add("scm_branch", ScmBranch);
            if (!string.IsNullOrEmpty(ScmRefspec))
                sendData.Add("scm_refspec", ScmRefspec);
            if (Credential > 0)
                sendData.Add("credential", Credential);
            if (ScmClean)
                sendData.Add("scm_clean", true);
            if (ScmDeleteOnUpdate)
                sendData.Add("scm_delete_on_update", true);
            if (ScmTrackSubmodules)
                sendData.Add("scm_track_submodules", true);
            if (ScmUpdateOnLaunch)
                sendData.Add("scm_update_on_launch", true);
            if (AllowOverride)
                sendData.Add("allow_override", true);
            if (Timeout > 0)
                sendData.Add("timeout", Timeout);

            return sendData;
        }
        protected override void ProcessRecord()
        {
            var sendData = CreateSendData();

            var dataDescription = string.Join(", ", sendData.Select(kv => $"{kv.Key} = {kv.Value}"));
            if (ShouldProcess($"{{ {dataDescription} }}"))
            {
                try
                {
                    var apiResult = CreateResource<Project>(Project.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }

    }
}
