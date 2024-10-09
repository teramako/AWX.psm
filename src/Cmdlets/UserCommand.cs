using AWX.Resources;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Me")]
    [OutputType(typeof(User))]
    public class GetMeCommand : APICmdletBase
    {
        protected override void EndProcessing()
        {
            foreach (var resultSet in GetResultSet<User>("/api/v2/me/", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "User")]
    [OutputType(typeof(User))]
    public class GetUserCommand : GetCommandBase<User>
    {
        protected override ResourceType AcceptType => ResourceType.User;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResource(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "User", DefaultParameterSetName = "All")]
    [OutputType(typeof(User))]
    public class FindUserCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Role))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter(Position = 0)]
        public string[]? UserName { get; set; }

        [Parameter(Position = 1)]
        public string[]? Email { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (UserName != null)
            {
                Query.Add("username__in", string.Join(',', UserName));
            }
            if (Email != null)
            {
                Query.Add("email__in", string.Join(",", Email));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/users/",
                ResourceType.Team => $"{Team.PATH}{Id}/users/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/owner_users/",
                ResourceType.Role => $"{Role.PATH}{Id}/users/",
                _ => User.PATH
            };
            foreach (var resultSet in GetResultSet<User>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "AccessList")]
    [OutputType(typeof(User))]
    public class FindAccessListCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.InstanceGroup),
                     nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
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
                ResourceType.InstanceGroup => $"{InstanceGroup.PATH}{Id}/access_list/",
                ResourceType.Organization => $"{Organization.PATH}{Id}/access_list/",
                ResourceType.User => $"{User.PATH}{Id}/access_list/",
                ResourceType.Project => $"{Project.PATH}{Id}/access_list/",
                ResourceType.Team => $"{Team.PATH}{Id}/access_list/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/access_list/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/access_list/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/access_list/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/access_list/",
                _ => throw new ArgumentException($"Can't handle the type: {Type}")
            };
            foreach (var resultSet in GetResultSet<User>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "User", SupportsShouldProcess = true, DefaultParameterSetName = "Credential")]
    [OutputType(typeof(User))]
    public class NewUserCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Credential", Position = 0)]
        [Credential]
        public PSCredential? Credential { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "SecureString", Position = 0)]
        public string UserName { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "SecureString")]
        public SecureString? Password { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string FirstName { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string LastName { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string Email { get; set; } = string.Empty;

        [Parameter()]
        public SwitchParameter IsSuperUser { get; set; }

        [Parameter()]
        public SwitchParameter IsSystemAuditor { get; set; }

        protected override void ProcessRecord()
        {
            string? user = null;
            string? passwordString = null;
            if (Credential != null)
            {
                user = Credential.UserName;
                passwordString = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password));
                Credential.Password.Dispose();
            }
            else 
            {
                user = UserName;
                if (Password != null)
                {
                    passwordString = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password));
                    Password.Dispose();
                }
                else
                {
                    if (CommandRuntime.Host == null)
                        throw new NullReferenceException();

                    var prompt = new AskPrompt(CommandRuntime.Host);
                    if (!prompt.AskPassword($"Password for {user}", "Password", "", out var pass))
                    {
                        return;
                    }
                    passwordString = pass.Input;
                }
            }

            if (string.IsNullOrEmpty(user))
            {
                WriteError(new ErrorRecord(new ArgumentException("UserName should not be empty."),
                                           "Invalid Argument",
                                           ErrorCategory.InvalidArgument,
                                           null));
                return;
            }

            if (string.IsNullOrEmpty(passwordString))
            {
                WriteError(new ErrorRecord(new ArgumentException("Password should not be empty."),
                                           "Invalid Argument",
                                           ErrorCategory.InvalidArgument,
                                           null));
                return;
            }


            var sendData = new Dictionary<string, object>()
            {
                { "username", user },
                { "password", "***" }, // dummy
            };
            if (!string.IsNullOrEmpty(FirstName))
                sendData.Add("first_name", FirstName);
            if (!string.IsNullOrEmpty(LastName))
                sendData.Add("last_name", LastName);
            if (!string.IsNullOrEmpty(Email))
                sendData.Add("email", Email);
            if (IsSuperUser)
                sendData.Add("is_superuser", IsSuperUser);
            if (IsSystemAuditor)
                sendData.Add("is_system_auditor", IsSystemAuditor);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            sendData["password"] = passwordString; // set password string after `sendData` is stringified

            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<User>(User.PATH, sendData);
                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "User", SupportsShouldProcess = true)]
    [OutputType(typeof(User))]
    public class UpdateUserCommand : UpdateCommandBase<User>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.User])]
        public override ulong Id { get; set; }

        [Parameter()]
        public string UserName { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? FirstName { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? LastName { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Email { get; set; }

        [Parameter()]
        public bool? IsSuperUser { get; set; } = null;

        [Parameter()]
        public bool? IsSystemAuditor { get; set; } = null;

        [Parameter()]
        public SecureString? Password { get; set; }

        protected override Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            string dataDescription = string.Empty;
            if (!string.IsNullOrEmpty(UserName))
                sendData.Add("username", UserName);
            if (FirstName != null)
                sendData.Add("first_name", FirstName);
            if (LastName != null)
                sendData.Add("last_name", LastName);
            if (Email != null)
                sendData.Add("email", Email);
            if (IsSuperUser != null)
                sendData.Add("is_superuser", IsSuperUser);
            if (IsSystemAuditor != null)
                sendData.Add("is_system_auditor", IsSystemAuditor);
            if (Password != null)
            {
                var passwordString = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password));
                Password.Dispose();
                if (!string.IsNullOrEmpty(passwordString))
                {
                    sendData.Add("password", "***"); // dummy
                    dataDescription = Json.Stringify(sendData, pretty: true);
                    sendData["password"] = passwordString;
                }
            }

            return sendData;
        }

        protected override void ProcessRecord()
        {
            if (TryPatch(Id, out var result))
            {
                WriteObject(result, false);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Register, "User", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class RegisterUserCommand : RegistrationCommandBase<User>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.User])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Organization,
                ResourceType.Team,
                ResourceType.Role
        ])]
        public IResource To { get; set; } = new Resource(0, 0);

        protected override void ProcessRecord()
        {
            var path = To.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{To.Id}/users/",
                ResourceType.Team => $"{Team.PATH}{To.Id}/users/",
                ResourceType.Role => $"{Role.PATH}{To.Id}/users/",
                _ => throw new ArgumentException($"Invalid resource type: {To.Type}")
            };
            WriteObject(Register(path, Id, To));
        }
    }

    [Cmdlet(VerbsLifecycle.Unregister, "User", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class UnregisterUserCommand : RegistrationCommandBase<User>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.User])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Organization,
                ResourceType.Team,
                ResourceType.Role
        ])]
        public IResource From { get; set; } = new Resource(0, 0);

        protected override void ProcessRecord()
        {
            var path = From.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{From.Id}/users/",
                ResourceType.Team => $"{Team.PATH}{From.Id}/users/",
                ResourceType.Role => $"{Role.PATH}{From.Id}/users/",
                _ => throw new ArgumentException($"Invalid resource type: {From.Type}")
            };
            WriteObject(Unregister(path, Id, From));
        }
    }

    [Cmdlet(VerbsCommon.Remove, "User", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveUserCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.User])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"User [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{User.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"User {Id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
