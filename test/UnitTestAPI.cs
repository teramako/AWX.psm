using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;
using System.Text.Json.Nodes;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace API_Test
{
    public class Util
    {
        public static void DumpResponse(IRestAPIResponse res)
        {
            Console.WriteLine();
            Console.WriteLine("{0} {1} (HTTP {2})", res.Method, res.RequestUri, res.HttpVersion);
            Console.WriteLine("Status: {0:d} {1}", res.StatusCode, res.ReasonPhrase);
            Console.WriteLine("Response Header:");
            foreach (var header in res.ContentHeaders)
            {
                Console.WriteLine("    {0}: {1}", header.Key, string.Join(", ", header.Value));
            }
            foreach (var header in res.ResponseHeaders)
            {
                Console.WriteLine("    {0}: {1}", header.Key, string.Join(", ", header.Value));
            }
            Console.WriteLine("Request Header:");
            if (res.RequestHeaders == null) return;
            foreach (var header in res.RequestHeaders)
            {
                Console.WriteLine("    {0}: {1}", header.Key, string.Join(", ", header.Value));
            }
        }
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        public static void DumpObject(object json)
        {
            Console.WriteLine($"-- {json.GetType().FullName} --");
            Console.WriteLine(JsonSerializer.Serialize(json, jsonSerializerOptions));
            Console.WriteLine("--------------");
        }

    }
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void Test_1_DefaultFile()
        {
            var path = ApiConfig.DefaultConfigPath;
            Console.WriteLine(path);
            Assert.IsNotNull(path);
            Assert.IsTrue(File.Exists(path));
        }
        [TestMethod]
        public void Test_2_DefaultConfig()
        {
            var config = ApiConfig.Instance;
            Assert.IsNotNull(config);
            Console.WriteLine(config.File);
            Console.WriteLine(config.Origin);
            Assert.IsInstanceOfType<Uri>(config.Origin);
            Assert.IsNotNull(config.File);
            Assert.IsNotNull(config.Origin);
            Assert.IsNotNull(config.Token);

            config.Save();
            Assert.IsNotNull(config.LastSaved);
            Console.WriteLine(config.LastSaved?.ToString("o"));
        }
        public static readonly DirectoryInfo? projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent;
        [TestMethod]
        public void Test_3_LoadConfig()
        {
            var file = Path.Join(projectDirectory?.ToString(), ".ansible_psm_config.json");
            Console.WriteLine(file);
            Assert.IsTrue(File.Exists(file));
            var fileInfo = new FileInfo(file);
            var config = ApiConfig.Load(fileInfo);
            Assert.AreSame(fileInfo, config.File);
        }
    }
    [TestClass]
    public class Test_ApiClient
    {
        public Test_ApiClient()
        {
            var configFile = Path.Join(ConfigTest.projectDirectory?.ToString(), ".ansible_psm_config.json");
            RestAPI.SetClient(ApiConfig.Load(new FileInfo(configFile)));
        }
        [TestMethod]
        public async Task Error404_1_AsJsonResponse()
        {
            var ex = await Assert.ThrowsExceptionAsync<RestAPIException>(() => RestAPI.GetAsync<User>("/api/v2/users/0/"));
            Assert.AreEqual(HttpStatusCode.NotFound, ex.StatusCode);
            Console.WriteLine(ex.ToString());
            Console.WriteLine("====================");
            Assert.IsNull(ex.InnerException);
            Assert.IsTrue(ex.Message.IndexOf("{\"detail\":") > 0);
            /*
            var ex = await Assert.ThrowsExceptionAsync<RestAPIException>(() => RestAPI.GetAsync<User>("/api/v2/users/0/"));
            var apiResponse = await RestAPI.GetAsync<User>("/api/v2/users/0/");
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.IsNotNull(apiResponse.Exception);
            Console.WriteLine(apiResponse.Exception.ToString());
            Assert.IsInstanceOfType<RestAPIException>(apiResponse.Exception);
            Assert.IsTrue(apiResponse.Exception.Message.IndexOf("{\"detail\":") > 0);
            */
        }
        [TestMethod]
        public async Task Error404_2_AsHtmlResponse()
        {
            var ex = await Assert.ThrowsExceptionAsync<RestAPIException>(() => RestAPI.GetAsync<User>("/404NotFound/"));
            Assert.AreEqual(HttpStatusCode.NotFound, ex.StatusCode);
            Console.WriteLine(ex.ToString());
            Console.WriteLine("====================");
            Assert.IsNull(ex.InnerException);
            Assert.IsTrue(ex.Message.IndexOf("text/html") > 0);
            /*
            var apiResponse = await RestAPI.GetAsync<User>("/404NotFound/");
            Assert.IsFalse(apiResponse.IsSuccess);
            Assert.IsNotNull(apiResponse.Exception);
            Console.WriteLine(apiResponse.Exception.ToString());
            Assert.IsInstanceOfType<RestAPIException>(apiResponse.Exception);
            Assert.IsTrue(apiResponse.Exception.Message.IndexOf("text/html") > 0);
            */
        }
        [TestMethod]
        public async Task GetJson()
        {
            var apiResult = await RestAPI.GetAsync<Ping>("/api/v2/ping/");
            Assert.IsNotNull(apiResult);
            Assert.AreEqual(HttpStatusCode.OK, apiResult.Response.StatusCode);
            Assert.IsTrue(apiResult.Response.IsSuccessStatusCode);
            Assert.IsTrue(apiResult.Response.ContentLength > 0);
            Assert.AreSame(RestAPI.JsonContentType, apiResult.Response.ContentType);
            var res = apiResult.Contents;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.HA);
            Assert.IsFalse(string.IsNullOrEmpty(res.Version));
            Console.WriteLine($"Version: {res.Version}");
            Assert.IsFalse(string.IsNullOrEmpty(res.ActiveNode));
            Assert.IsFalse(string.IsNullOrEmpty(res.InstallUuid));
            Assert.IsTrue(res.Instances.Length > 0);
            Assert.IsFalse(string.IsNullOrEmpty(res.Instances[0].Node));
            Assert.IsFalse(string.IsNullOrEmpty(res.Instances[0].NodeType));
            Assert.IsFalse(string.IsNullOrEmpty(res.Instances[0].Uuid));
            Assert.IsFalse(string.IsNullOrEmpty(res.Instances[0].Heartbeat));
            Assert.IsTrue(res.Instances[0].Capacity > 0);
            Assert.IsFalse(string.IsNullOrEmpty(res.Instances[0].Version));
            Assert.IsTrue(res.InstanceGroups.Length > 0);
            Assert.IsFalse(string.IsNullOrEmpty(res.InstanceGroups[0].Name));
            Assert.IsTrue(res.InstanceGroups[0].Capacity > 0);
            Assert.IsTrue(res.InstanceGroups[0].Instances.Length > 0);
            Assert.IsFalse(string.IsNullOrEmpty(res.InstanceGroups[0].Instances[0]));
            Util.DumpObject(res);
            Util.DumpResponse(apiResult.Response);
        }
        [TestMethod]
        public async Task OptionsJson()
        {
            var apiResult = await RestAPI.OptionsJsonAsync<ApiHelp>("/api/v2/ping/");
            Assert.IsNotNull(apiResult);
            var help = apiResult.Contents;
            Assert.IsNotNull(help);
            Util.DumpObject(help);
            Util.DumpResponse(apiResult.Response);
            Assert.IsInstanceOfType<ApiHelp>(help);
            Assert.IsTrue(help.Name.Length > 0);
            Assert.IsTrue(help.Description.Length > 0);
        }


        [TestMethod]
        public async Task GetText()
        {
            var jobResult = await RestAPI.GetAsync<string>("/api/v2/jobs/4/stdout/?format=txt", AcceptType.Text);
            Assert.IsNotNull(jobResult.Contents);
            Assert.IsTrue(jobResult.Response.IsSuccessStatusCode);
            Console.WriteLine(jobResult.Response.IsSuccessStatusCode);
            Assert.IsTrue(RestAPI.TextContentType == jobResult.Response.ContentType);
            Assert.IsInstanceOfType<string>(jobResult.Contents);
            Util.DumpResponse(jobResult.Response);
            Console.WriteLine("----------------");
            Console.WriteLine(jobResult.Contents);
        }
        [TestMethod]
        public async Task GetHtml()
        {
            var jobResult = await RestAPI.GetAsync<string>("/api/v2/jobs/4/stdout/?format=html", AcceptType.Html);
            Assert.IsNotNull(jobResult.Contents);
            Assert.IsTrue(jobResult.Response.IsSuccessStatusCode);
            Assert.IsTrue(RestAPI.HtmlContentType == jobResult.Response.ContentType);
            Assert.IsInstanceOfType<string>(jobResult.Contents);
            Util.DumpResponse(jobResult.Response);
            Console.WriteLine("----------------");
            Console.WriteLine(jobResult.Contents);
        }
        [TestMethod]
        public async Task GetResultSet()
        {
            await foreach (var apiResult in RestAPI.GetResultSetAsync<User>("/api/v2/me/", null, false))
            {
                var resultSet = apiResult.Contents;
                Assert.IsNotNull(resultSet);
                Assert.IsNull(resultSet.Previous);
                Assert.IsNull(resultSet.Next);
                Assert.IsTrue(resultSet.Count > 0);
                foreach (var user in resultSet.Results)
                {
                    Assert.IsTrue(user.Id > 0);
                    Assert.AreEqual(ResourceType.User, user.Type);
                    Assert.IsFalse(string.IsNullOrEmpty(user.Username));
                    Util.DumpObject(user);
                }
            }
        }

    }
    [TestClass]
    public class Test_ActivityStream
    {
        static void DumpResource(ActivityStream a)
        {
            Console.WriteLine($"{a.Id} {a.Type} {a.Timestamp}");
            Console.WriteLine($"Operation: {a.Operation}");
            Console.WriteLine($"Object   : 1:{a.Object1}, 2:{a.Object2}");
        }
        static void DumpSummary(ActivityStream.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Actor : [{summary.Actor?.Id}] {summary.Actor?.Username}");
            if (summary.ExtensionData != null)
                Util.DumpObject(summary.ExtensionData);
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var activity = await ActivityStream.Get(1);
            Assert.IsNotNull(activity);
            Assert.IsNotNull(activity.Id);
            Assert.AreEqual(ResourceType.ActivityStream, activity.Type);
            Assert.IsNotNull(activity.Timestamp);
            Assert.IsInstanceOfType<ActivityStreamOperation>(activity.Operation);
            DumpResource(activity);
            DumpSummary(activity.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var expectCount = 2;
            var c = 0;
            var query = HttpUtility.ParseQueryString($"page_size={expectCount}");
            await foreach(var activity in ActivityStream.Find(query, false))
            {
                c++;
                Assert.IsInstanceOfType<ActivityStream>(activity);
                DumpResource(activity);
                DumpSummary(activity.SummaryFields);
            }
            Assert.AreEqual(expectCount, c);
        }

    }
    [TestClass]
    public class Test_Application
    {
        private static void DumpSummary(Application.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            var org = summary.Organization;
            Console.WriteLine($"Org: [{org.Id}] {org.Name} - {org.Description}");
            Console.WriteLine($"Cap: {summary.UserCapabilities}");
            Console.WriteLine($"Tokens: {summary.Tokens.Count}");
            foreach (var token in summary.Tokens.Results)
            {
                Console.WriteLine($"Token: [{token.Id}] {token.Token} {token.Scope}");
            }

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var app = await Application.Get(1);
            Assert.IsInstanceOfType<Application>(app);
            Console.WriteLine($"Id           : {app.Id}");
            Console.WriteLine($"Name         : {app.Name}");
            Console.WriteLine($"Description  : {app.Description}");
            Console.WriteLine($"Created      : {app.Created}");
            Console.WriteLine($"Modified     : {app.Modified}");
            Console.WriteLine($"AuthGrantType: {app.AuthorizationGrantType}");
            Console.WriteLine($"ClientId     : {app.ClientId}");
            Console.WriteLine($"ClientType   : {app.ClientType}");
            Console.WriteLine($"ClientSecret : {app.ClientSecret ?? "(null)"}");
            Console.WriteLine($"Redirect     : {app.RedirectUris}");
            Console.WriteLine($"SkipAuth     : {app.SkipAuthorization}");
            Console.WriteLine($"Organization : {app.Organization}");

            DumpSummary(app.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var expectCount = 2;
            var c = 0;
            var query = HttpUtility.ParseQueryString($"page_size={expectCount}");

            await foreach (var app in Application.Find(query, false))
            {
                c++;
                Assert.IsInstanceOfType<Application>(app);
                Console.WriteLine($"{app.Id,5:d}: {app.Name} {app.Description}");

                DumpSummary(app.SummaryFields);
            }
            Assert.AreEqual(expectCount, c);
        }

    }

    [TestClass]
    public class Test_OAuth2AccessToken
    {
        private static void DumpSummary(OAuth2AccessToken.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"User        : [{summary.User.Id}] {summary.User.Username}");
            Console.WriteLine($"Application : [{summary.Application?.Id}] {summary.Application?.Name}");
        }
        private static void DumpToken(OAuth2AccessToken token)
        {
            Console.WriteLine($"{token.Id} {token.Token} - {token.Description}");
            Console.WriteLine($"Application : {token.Application}");
            Console.WriteLine($"Socpe       : {token.Scope}");
            Console.WriteLine($"Expires     : {token.Expires}");
            Console.WriteLine($"Created     : {token.Created}");
            Console.WriteLine($"Modified    : {token.Modified?.ToString() ?? "(null)"}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var token = await OAuth2AccessToken.Get(1);
            Assert.IsInstanceOfType<OAuth2AccessToken>(token);
            DumpToken(token);
            DumpSummary(token.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            await foreach(var token in OAuth2AccessToken.Find(null))
            {
                DumpToken(token);
                DumpSummary(token.SummaryFields);
                Console.WriteLine();
            }
        }
    }

    [TestClass]
    public class Test_Instance
    {
        private static void DumpSummary(Instance.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Caps : {summary.UserCapabilities}");
        }
        private static void DumpInstance(Instance instance)
        {
            Console.WriteLine($"Id                : {instance.Id}");
            Console.WriteLine($"Type              : {instance.Type}");
            Console.WriteLine($"Hostname          : {instance.Hostname}");
            Console.WriteLine($"UUID              : {instance.Uuid}");
            Console.WriteLine($"Created           : {instance.Created}");
            Console.WriteLine($"Modified          : {instance.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"LastSeen          : {instance.LastSeen}");
            Console.WriteLine($"HelthCheckStarted : {instance.HealthCheckStarted?.ToString() ?? "(null)"}");
            Console.WriteLine($"HelthCheckPending : {instance.HealthCheckPending}");
            Console.WriteLine($"LastHealthCheck   : {instance.LastHealthCheck?.ToString() ?? "(null)"}");
            Console.WriteLine($"Errors            : {instance.Errors}");
            Console.WriteLine($"CapacityAdjustment: {instance.CapacityAdjustment}");
            Console.WriteLine($"Version           : {instance.Version}");
            Console.WriteLine($"Capacity          : {instance.Capacity}");
            Console.WriteLine($"ConsumedCapacity  : {instance.ConsumedCapacity}");
            Console.WriteLine($"PercentCapacityRemaining: {instance.PercentCapacityRemaining}");
            Console.WriteLine($"JobRunning        : {instance.JobsRunning}");
            Console.WriteLine($"JobTotal          : {instance.JobsTotal}");
            Console.WriteLine($"CPU               : {instance.Cpu}");
            Console.WriteLine($"Memory            : {instance.Memory}");
            Console.WriteLine($"CPU Capacity      : {instance.CpuCapacity}");
            Console.WriteLine($"Mem Capacity      : {instance.MemCapacity}");
            Console.WriteLine($"Enabled           : {instance.Enabled}");
            Console.WriteLine($"ManagedByPolicy   : {instance.ManagedByPolicy}");
            Console.WriteLine($"NodeType          : {instance.NodeType}");
            Console.WriteLine($"NodeState         : {instance.NodeState}");
            Console.WriteLine($"IpAddress         : {instance.IpAddress}");
            Console.WriteLine($"Listener Port     : {instance.ListenerPort}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var instance = await Instance.Get(1);
            Assert.IsInstanceOfType<Instance>(instance);
            DumpInstance(instance);
            DumpSummary(instance.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var expectCount = 2;
            var c = 0;
            var query = HttpUtility.ParseQueryString($"page_size={expectCount}");

            await foreach (var instance in Instance.Find(query, false))
            {
                c++;
                Assert.IsInstanceOfType<Instance>(instance);
                DumpInstance(instance);
                DumpSummary(instance.SummaryFields);
                Console.WriteLine();
            }
            Assert.IsTrue(c <= expectCount);
        }
    }
    [TestClass]
    public class Test_InstanceGroup
    {
        private static void DumpSummary(InstanceGroup.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Caps : {summary.UserCapabilities}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
        }
        private static void DumpResource(InstanceGroup ig)
        {
            Console.WriteLine($"Id                : {ig.Id}");
            Console.WriteLine($"Type              : {ig.Type}");
            Console.WriteLine($"Created           : {ig.Created}");
            Console.WriteLine($"Modified          : {ig.Modified}");
            Console.WriteLine($"Name              : {ig.Name}");
            Console.WriteLine($"Capacity          : {ig.Capacity}");
            Console.WriteLine($"ConsumedCapacity  : {ig.ConsumedCapacity}");
            Console.WriteLine($"PercentCapacityRemaining: {ig.PercentCapacityRemaining}");
            Console.WriteLine($"JobsRunning       : {ig.JobsRunning}");
            Console.WriteLine($"MaxConcurrentJobs : {ig.MaxConcurrentJobs}");
            Console.WriteLine($"MaxForkcs         : {ig.MaxForks}");
            Console.WriteLine($"JobsTotal         : {ig.JobsTotal}");
            Console.WriteLine($"Instances         : {ig.Instances}");
            Console.WriteLine($"IsContainerGroup  : {ig.IsContainerGroup}");
            Console.WriteLine($"Credential        : {ig.Credential?.ToString() ?? "(null)"}");
            Console.WriteLine($"PolicyInstancePercentage: {ig.PolicyInstancePercentage}");
            Console.WriteLine($"PolicyInstanceMinimum   : {ig.PolicyInstanceMinimum}");
            Console.WriteLine($"PolicyInstanceList      : {ig.PolicyInstanceList}");
            Console.WriteLine($"PodSpecOverride   : {ig.PodSpecOverride}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var ig = await InstanceGroup.Get(1);
            Assert.IsInstanceOfType<InstanceGroup>(ig);
            DumpResource(ig);
            DumpSummary(ig.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var expectCount = 2;
            var c = 0;
            var query = HttpUtility.ParseQueryString($"page_size={expectCount}");

            await foreach (var ig in InstanceGroup.Find(query, false))
            {
                c++;
                Assert.IsInstanceOfType<InstanceGroup>(ig);
                DumpResource(ig);
                DumpSummary(ig.SummaryFields);
                Console.WriteLine();
            }
            Assert.IsTrue(c <= expectCount);
        }
    }
    [TestClass]
    public class Test_Organization
    {
        private static void DumpSummary(Organization.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"DefaultEnvironment : [{summary.DefaultEnvironment?.Id}] {summary.DefaultEnvironment?.Name}");
            Console.WriteLine($"CreatedBy  : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy : [{summary.ModifiedBy.Id}] {summary.ModifiedBy.Username}");
            Console.WriteLine($"Caps       : {summary.UserCapabilities}");
            Console.WriteLine($"Roles:");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"RelatedFieldCounts:");
            Console.WriteLine($"  {summary.RelatedFieldCouns}");
        }
        private static void DumpResource(Organization org)
        {
            Console.WriteLine($"Id                : {org.Id}");
            Console.WriteLine($"Type              : {org.Type}");
            Console.WriteLine($"Created           : {org.Created}");
            Console.WriteLine($"Modified          : {org.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name              : {org.Name}");
            Console.WriteLine($"Description       : {org.Description}");
            Console.WriteLine($"MaxHosts          : {org.MaxHosts}");
            Console.WriteLine($"DefaultEnvironment: {org.DefaultEnvironment?.ToString() ?? "(null)"}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var org = await Organization.Get(1);
            Assert.IsInstanceOfType<Organization>(org);
            DumpResource(org);
            DumpSummary(org.SummaryFields);
            Util.DumpObject(org);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var expectCount = 2;
            var c = 0;
            var query = HttpUtility.ParseQueryString($"page_size={expectCount}");

            await foreach (var org in Organization.Find(query, false))
            {
                c++;
                Assert.IsInstanceOfType<Organization>(org);
                DumpResource(org);
                DumpSummary(org.SummaryFields);
            }
            Assert.IsTrue(c <= expectCount);
        }

    }
    [TestClass]
    public class Test_User
    {
        [TestMethod("既存ユーザーの作成を試行")]
        public async Task User_CreateError_1()
        {
            var user = new UserData()
            {
                Username = "teramako",
                Email = "teramako@gmail.com",
                Password = "P@ssw0rd"
            };
            var ex = await Assert.ThrowsExceptionAsync<RestAPIException>(() => RestAPI.PostJsonAsync<User>("/api/v2/users/", user));
            Console.WriteLine(ex.ToString());
            Assert.AreEqual(HttpStatusCode.BadRequest, ex.StatusCode);
            /*
            var res = await RestAPI.PostJsonAsync<User>("/api/v2/users/", jt);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsNotNull(res.Exception);
            Console.WriteLine(res.Exception.ToString());
            Assert.IsInstanceOfType<RestAPIException>(res.Exception);
            Assert.IsTrue(res.Exception.Message.IndexOf("{\"username\":") > 0);
            */
        }
        public async Task User_CreateAndDelete()
        {
            var user = new UserData()
            {
                Username = "Test_User_1",
                FirstName = "User 1",
                LastName = "Test",
                Email = "",
                IsSuperuser = false,
                IsSystemAuditor = false,
                Password = "Password",
            };
            Console.WriteLine("================= Create =================");
            var apiResult = await RestAPI.PostJsonAsync<User>("/api/v2/users/", user);
            Assert.IsNotNull(apiResult.Contents);
            var createdUser = apiResult.Contents;
            Assert.IsInstanceOfType<User>(createdUser);
            Assert.IsNotNull(createdUser.Id);
            Util.DumpObject(createdUser);
            Util.DumpResponse(apiResult.Response);

            Console.WriteLine("================= Deleate =================");
            var id = createdUser.Id;
            var deleteResult = await RestAPI.DeleteAsync($"/api/v2/users/{id}/");
            Assert.IsTrue(deleteResult.Response.ContentLength == 0);
            if (deleteResult.Contents != null)
                Util.DumpObject(deleteResult.Contents);
            else
                Console.WriteLine($"{nameof(deleteResult.Contents)} is null");
            Util.DumpResponse(deleteResult.Response);
        }
        private static void DumpResource(User user)
        {
            Console.WriteLine($"ID   : {user.Id}");
            Console.WriteLine($"Type : {user.Type}");
            Console.WriteLine($"Created  : {user.Created}");
            Console.WriteLine($"Modifed  : {user.Modified}");
            Console.WriteLine($"Username : {user.Username}");
            Console.WriteLine($"FirstName: {user.FirstName}");
            Console.WriteLine($"LastName : {user.LastName}");
            Console.WriteLine($"Email    : {user.Email}");
            Console.WriteLine($"LastLogin: {user.LastLogin?.ToString() ?? "(null)"}");
            Console.WriteLine($"Auth     : {user.Auth}");
            Console.WriteLine($"Password : {user.Password}");
            Console.WriteLine($"LdapDn   : {user.LdapDn}");
            Console.WriteLine($"IsSuperuser      : {user.IsSuperuser}");
            Console.WriteLine($"IsSystemAutoditor: {user.IsSystemAuditor}");
            Console.WriteLine($"ExternalAccount  : {user.ExternalAccount}");
            DumpSummary(user.SummaryFields);
        }
        private static void DumpSummary(User.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Caps : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var user = await User.Get(2);
            Assert.IsInstanceOfType<User>(user);
            DumpResource(user);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=2");
            await foreach(var user in User.Find(query, false))
            {
                DumpResource(user);
            }
        }
        [TestMethod]
        public async Task Get_3_Me()
        {
            var user = await User.GetMe();
            Assert.IsInstanceOfType<User>(user);
            DumpResource(user);
            DumpSummary(user.SummaryFields);
        }
    }
    [TestClass]
    public class Test_Project
    {
        private static void DumpResource(Project proj)
        {
            Console.WriteLine($"Id                   : {proj.Id}");
            Console.WriteLine($"Type                 : {proj.Type}");
            Console.WriteLine($"Created              : {proj.Created}");
            Console.WriteLine($"Modified             : {proj.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name                 : {proj.Name}");
            Console.WriteLine($"Description          : {proj.Description}");
            Console.WriteLine($"LocalPath            : {proj.LocalPath}");
            Console.WriteLine($"ScmType              : {proj.ScmType}");
            Console.WriteLine($"ScmUrl               : {proj.ScmUrl}");
            Console.WriteLine($"ScmBranch            : {proj.ScmBranch}");
            Console.WriteLine($"ScmRefspac           : {proj.ScmRefspec}");
            Console.WriteLine($"ScmClean             : {proj.ScmClean}");
            Console.WriteLine($"ScmTrackSubmodules   : {proj.ScmTrackSubmodules}");
            Console.WriteLine($"ScmDeleteOnUpdate    : {proj.ScmDeleteOnUpdate}");
            Console.WriteLine($"Credential           : {proj.Credential?.ToString() ?? "(null)"}");
            Console.WriteLine($"Timeout              : {proj.Timeout}");
            Console.WriteLine($"ScmRevision          : {proj.ScmRevision}");
            Console.WriteLine($"LastJobRun           : {proj.LastJobRun?.ToString() ?? "(null)"}");
            Console.WriteLine($"LastJobFailed        : {proj.LastJobFailed}");
            Console.WriteLine($"NextJobFun           : {proj.NextJobRun?.ToString() ?? "(null)"}");
            Console.WriteLine($"Status               : {proj.Status}");
            Console.WriteLine($"Organization         : {proj.Organization}");
            Console.WriteLine($"ScmUpdateOnLaunch    : {proj.ScmUpdateOnLaunch}");
            Console.WriteLine($"ScmUpdateCacheTimeout: {proj.ScmUpdateCacheTimeout}");
            Console.WriteLine($"AllowOverride        : {proj.AllowOverride}");
            Console.WriteLine($"CustomVirtualenv     : {proj.CustomVirtualenv??"(null)"}");
            Console.WriteLine($"DefaultEnvironment   : {proj.DefaultEnvironment?.ToString()??"(null)"}");
            Console.WriteLine($"LastUpdateFailed     : {proj.LastUpdateFailed}");
            Console.WriteLine($"LastUpdated          : {proj.LastUpdated?.ToString()??"(null)"}");
            Console.WriteLine($"SignatureValidateionCredential: {proj.SignatureValidationCredential?.ToString()??"(null)"}");
            DumpSummary(proj.SummaryFields);
        }
        private static void DumpSummary(Project.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"DefaultEnvironment : [{summary.DefaultEnvironment?.Id}] {summary.DefaultEnvironment?.Name}");
            Console.WriteLine($"Credential         : [{summary.Credential?.Id}] {summary.Credential?.Kind} {summary.Credential?.Name}");
            Console.WriteLine($"LastJob            : [{summary.LastJob?.Id}] {summary.LastJob?.Name} {summary.LastJob?.Status} {summary.LastJob?.Finished}");
            Console.WriteLine($"LastUpdate         : [{summary.LastUpdate?.Id}] {summary.LastUpdate?.Name} {summary.LastUpdate?.Status}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy         : [{summary.ModifiedBy.Id}] {summary.ModifiedBy.Username}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var proj = await Project.Get(8);
            Assert.IsInstanceOfType<Project>(proj);
            DumpResource(proj);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=2");
            await foreach(var proj in Project.Find(query, false))
            {
                DumpResource(proj);
            }
        }
    }

    [TestClass]
    public class Test_ProjectUpdate
    {
        private static void DumpResource(IProjectUpdateJob job)
        {
            Console.WriteLine($"ID         : {job.Id}");
            Console.WriteLine($"Name       : {job.Name}");
            Console.WriteLine($"Description: {job.Description}");
            Console.WriteLine($"LocalPath  : {job.LocalPath}");
            Console.WriteLine($"ScmType    : {job.ScmType}");
            Console.WriteLine($"ScmUrl     : {job.ScmUrl}");
            Console.WriteLine($"ScmBranch  : {job.ScmBranch}");
            Console.WriteLine($"ScmRefspec : {job.ScmRefspec}");
            Console.WriteLine($"ScmClean   : {job.ScmClean}");
            Console.WriteLine($"ScmTrackSubmodules: {job.ScmTrackSubmodules}");
            Console.WriteLine($"ScmDeleteOnUpdate : {job.ScmDeleteOnUpdate}");
            Console.WriteLine($"Credential : {job.Credential?.ToString()??"(null)"}");
            Console.WriteLine($"Timeout    : {job.Timeout}");
            Console.WriteLine($"Project    : {job.Project}");
        }
        private static void DumpSummary(ProjectUpdateJob.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization       : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"DefaultEnvironment : [{summary.DefaultEnvironment?.Id}] {summary.DefaultEnvironment?.Name}");
            Console.WriteLine($"Project            : [{summary.Project.Id}] {summary.Project.Name} {summary.Project.ScmType}");
            Console.WriteLine($"Credential         : [{summary.Credential?.Id}] {summary.Credential?.Kind} {summary.Credential?.Name}");
            Console.WriteLine($"UnifiedJobTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();
        }

        [TestMethod]
        public async Task Get_1_Single()
        {
            var job = await ProjectUpdateJob.Get(5);
            Assert.IsInstanceOfType<ProjectUpdateJob.Detail>(job);
            DumpResource(job);
            Console.WriteLine($"JobArgs    : {job.JobArgs}");
            Console.WriteLine($"JobCwd     : {job.JobCwd}");
            Console.WriteLine($"JobEnv     : {job.JobEnv.Count}");
            foreach (var (k,v) in job.JobEnv)
            {
                Console.WriteLine($"   {k}: {v}");
            }
            DumpSummary(job.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=2&order_by=-id");
            await foreach(var job in ProjectUpdateJob.Find(query, false))
            {
                DumpResource(job);
                DumpSummary(job.SummaryFields);
            }
        }
    }
    [TestClass]
    public class Test_Team
    {
        private static void DumpResource(Team team)
        {
            Console.WriteLine($"Id          : {team.Id}");
            Console.WriteLine($"Type        : {team.Type}");
            Console.WriteLine($"Created     : {team.Created}");
            Console.WriteLine($"Modified    : {team.Modified?.ToString()??"(null)"}");
            Console.WriteLine($"Name        : {team.Name}");
            Console.WriteLine($"Description : {team.Description}");
            Console.WriteLine($"Organization: {team.Organization}");
            DumpSummary(team.SummaryFields);
        }
        private static void DumpSummary(Team.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization       : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy         : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();
        }

        [TestMethod]
        public async Task Get_1_Single()
        {
            var team = await Team.Get(1);
            Assert.IsInstanceOfType<Team>(team);
            DumpResource(team);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=2");
            await foreach(var team in Team.Find(query, false))
            {
                DumpResource(team);
            }
        }
    }
    [TestClass]
    public class Test_Credential
    {
        private static void DumpResource(Credential cred)
        {
            Console.WriteLine($"Id            : {cred.Id}");
            Console.WriteLine($"Type          : {cred.Type}");
            Console.WriteLine($"Created       : {cred.Created}");
            Console.WriteLine($"Modified      : {cred.Modified}");
            Console.WriteLine($"Name          : {cred.Name}");
            Console.WriteLine($"Description   : {cred.Description}");
            Console.WriteLine($"Organization  : {cred.Organization?.ToString() ?? "(null)"}");
            Console.WriteLine($"CredentialType: {cred.CredentialType}");
            Console.WriteLine($"Managed       : {cred.Managed}");
            Console.WriteLine($"Kind          : {cred.Kind}");
            Console.WriteLine($"Cloud         : {cred.Cloud}");
            Console.WriteLine($"Kubernetes    : {cred.Kubernetes}");
            DumpSummary(cred.SummaryFields);
        }
        private static void DumpSummary(Credential.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization    : [{summary.Organization?.Id}] {summary.Organization?.Name}");
            Console.WriteLine($"CredentialType  : [{summary.CredentialType.Id}] {summary.CredentialType.Name}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy      : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine("Owners:");
            foreach (var owner in summary.Owners)
            {
                Console.WriteLine($"  [{owner.Type}] {owner.Id} {owner.Name}");
            }
            Console.WriteLine();

        }

        [TestMethod]
        public async Task Get_1_Single()
        {
            var cred = await Credential.Get(2);
            Assert.IsInstanceOfType<Credential>(cred);
            DumpResource(cred);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=id");
            await foreach(var cred in Credential.Find(query, false))
            {
                DumpResource(cred);
            }
        }
    }
    [TestClass]
    public class Test_CredentialType
    {
        private static void DumpResource(CredentialType ct)
        {
            Console.WriteLine($"Id          : {ct.Id}");
            Console.WriteLine($"Type        : {ct.Type}");
            Console.WriteLine($"Created     : {ct.Created}");
            Console.WriteLine($"Modified    : {ct.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {ct.Name}");
            Console.WriteLine($"Description : {ct.Description}");
            Console.WriteLine($"Kind        : {ct.Kind}");
            Console.WriteLine($"Namespace   : {ct.Namespace}");
            Console.WriteLine($"Managed     : {ct.Managed}");
            Console.WriteLine($"==== Inputs ({ct.Inputs.Count})======");
            if (ct.Inputs.Count > 0)
                Util.DumpObject(ct.Inputs);
            Console.WriteLine($"==== Injectors ({ct.Injectors.Count})===");
            if (ct.Injectors.Count > 0)
                Util.DumpObject(ct.Injectors);
            DumpSummary(ct.SummaryFields);
        }
        private static void DumpSummary(CredentialType.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var ct = await CredentialType.Get(1);
            Assert.IsInstanceOfType<CredentialType>(ct);
            DumpResource(ct);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=20&order_by=id");
            await foreach(var ct in CredentialType.Find(query, false))
            {
                DumpResource(ct);
            }
        }
    }

    [TestClass]
    public class Test_Inventory
    {
        private static void DumpResource(Inventory inventory)
        {
            Console.WriteLine($"Id          : {inventory.Id}");
            Console.WriteLine($"Type        : {inventory.Type}");
            Console.WriteLine($"Created     : {inventory.Created}");
            Console.WriteLine($"Modified    : {inventory.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {inventory.Name}");
            Console.WriteLine($"Description : {inventory.Description}");
            Console.WriteLine($"Kind        : {inventory.Kind}");
            Console.WriteLine($"HostFilter  : {inventory.HostFilter}");
            Console.WriteLine($"Variables   : {inventory.Variables}");
            DumpSummary(inventory.SummaryFields);
        }
        private static void DumpSummary(Inventory.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization    : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy      : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine("Labels:");
            foreach (var label in summary.Labels.Results)
            {
                Console.WriteLine($"  {label.Id} {label.Name}");
            }
            Console.WriteLine();
        }

        [TestMethod]
        public async Task Get_1_Single()
        {
            var inventory = await Inventory.Get(1);
            Assert.IsInstanceOfType<Inventory>(inventory);
            DumpResource(inventory);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=20&order_by=id");
            await foreach(var inventory in Inventory.Find(query, false))
            {
                DumpResource(inventory);
            }
        }
    }
    [TestClass]
    public class Test_ConstructedInventory
    {
        private static void DumpResource(ConstructedInventory inventory)
        {
            Console.WriteLine($"Id          : {inventory.Id}");
            Console.WriteLine($"Type        : {inventory.Type}");
            Console.WriteLine($"Created     : {inventory.Created}");
            Console.WriteLine($"Modified    : {inventory.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {inventory.Name}");
            Console.WriteLine($"Description : {inventory.Description}");
            Console.WriteLine($"Kind        : {inventory.Kind}");
            Console.WriteLine($"Variables   : {inventory.Variables}");
            Console.WriteLine($"Sourcevars  : {inventory.SourceVars}");
            DumpSummary(inventory.SummaryFields);
        }
        private static void DumpSummary(Inventory.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization    : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy      : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            var roles = summary.ObjectRoles;
            foreach (var (key,val) in roles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine("Labels:");
            foreach (var label in summary.Labels.Results)
            {
                Console.WriteLine($"  {label.Id} {label.Name}");
            }
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var inventory = await ConstructedInventory.Get(4);
            Assert.IsInstanceOfType<ConstructedInventory>(inventory);
            Assert.AreEqual("constructed", inventory.Kind);
            DumpResource(inventory);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            await foreach(var inventory in ConstructedInventory.Find(null))
            {
                Assert.AreEqual("constructed", inventory.Kind);
                DumpResource(inventory);
            }
        }
    }
    [TestClass]
    public class Test_InventorySource
    {
        private static void DumpResource(InventorySource res)
        {
            Console.WriteLine($"Id          : {res.Id}");
            Console.WriteLine($"Type        : {res.Type}");
            Console.WriteLine($"Name        : {res.Name}");
            Console.WriteLine($"Status      : {res.Status}");
            Console.WriteLine($"Source      : {res.Source}");
            Console.WriteLine($"SourcePath  : {res.SourcePath}");
            Console.WriteLine($"SourceVars  : {res.SourceVars}");
            Console.WriteLine($"Enabled  Var: {res.EnabledVar}, Value: {res.EnabledValue}");
            Console.WriteLine($"Overwrite   : {res.Overwrite}, Vars: {res.OverwriteVars}");
            DumpSummary(res.SummaryFields);
        }
        private static void DumpSummary(InventorySource.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization    : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"Inventory       : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"ExecutionEnv    : [{summary.ExecutionEnvironment?.Id}] {summary.ExecutionEnvironment?.Name}");
            Console.WriteLine($"SourceProject   : [{summary.SourceProject?.Id}] {summary.SourceProject?.Name} {summary.SourceProject?.AllowOverride}");
            Console.WriteLine($"LastJob         : [{summary.LastJob?.Id}] {summary.LastJob?.Name} {summary.LastJob?.Status} {summary.LastJob?.Finished}");
            Console.WriteLine($"LastUpdate      : [{summary.LastUpdate?.Id}] {summary.LastUpdate?.Name} {summary.LastUpdate?.Status}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"ModifiedBy      : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"Caps            : {summary.UserCapabilities}");
            Console.WriteLine("Credentials:");
            foreach (var cred in summary.Credentials)
            {
                Console.WriteLine($"  [{cred.Id}][{cred.Kind}] {cred.Name}");
            }
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await InventorySource.Get(11);
            Assert.IsInstanceOfType<InventorySource>(res);
            DumpResource(res);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            await foreach(var res in InventorySource.Find(HttpUtility.ParseQueryString("order_by=id")))
            {
                DumpResource(res);
            }
        }
    }
    [TestClass]
    public class Test_InventoryUpdate
    {
        private static void DumpResource(InventoryUpdateJob res)
        {
            Console.WriteLine($"Id          : {res.Id}");
            Console.WriteLine($"Type        : {res.Type}");
            Console.WriteLine($"Name        : {res.Name}");
            Console.WriteLine($"Status      : {res.Status}");
            Console.WriteLine($"Source      : {res.Source}");
            Console.WriteLine($"SourcePath  : {res.SourcePath}");
            Console.WriteLine($"SourceVars  : {res.SourceVars}");
            Console.WriteLine($"Enabled  Var: {res.EnabledVar}, Value: {res.EnabledValue}");
            Console.WriteLine($"Overwrite   : {res.Overwrite}, Vars: {res.OverwriteVars}");
            DumpSummary(res.SummaryFields);
        }
        private static void DumpSummary(InventoryUpdateJob.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization       : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"Inventory          : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"ExecutionEnv       : [{summary.ExecutionEnvironment?.Id}] {summary.ExecutionEnvironment?.Name}");
            Console.WriteLine($"UnifiedJobTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"InventorySource    : [{summary.InventorySource.Id}][{summary.InventorySource.Source}]{summary.InventorySource.Name} {summary.InventorySource.Status}");
            Console.WriteLine($"InstanceGroup      : [{summary.InstanceGroup.Id}] {summary.InstanceGroup.Name}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy.Id}] {summary.CreatedBy.Username}");
            Console.WriteLine($"Caps            : {summary.UserCapabilities}");
            Console.WriteLine("Credentials:");
            foreach (var cred in summary.Credentials)
            {
                Console.WriteLine($"  [{cred.Id}][{cred.Kind}] {cred.Name}");
            }
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await InventoryUpdateJob.Get(46);
            Assert.IsInstanceOfType<InventoryUpdateJob>(res);
            Assert.IsInstanceOfType<IUnifiedJob>(res);
            DumpResource(res);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            await foreach(var res in InventoryUpdateJob.Find(HttpUtility.ParseQueryString("order_by=id")))
            {
                DumpResource(res);
            }
        }
    }

    [TestClass]
    public class Test_Group
    {
        private static void DumpResource(Group group)
        {
            Console.WriteLine($"Id          : {group.Id}");
            Console.WriteLine($"Type        : {group.Type}");
            Console.WriteLine($"Created     : {group.Created}");
            Console.WriteLine($"Modified    : {group.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {group.Name}");
            Console.WriteLine($"Description : {group.Description}");
            Console.WriteLine($"Inventory   : {group.Inventory}");
            Console.WriteLine($"Variables   : {group.Variables}");
            DumpSummary(group.SummaryFields);
        }
        private static void DumpSummary(Group.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Inventory   : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"CreatedBy   : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy  : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"Caps        : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var group = await Group.Get(1);
            Assert.IsInstanceOfType<Group>(group);
            DumpResource(group);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=20");
            await foreach(var group in Group.Find(query, false))
            {
                DumpResource(group);
            }
        }
    }
    [TestClass]
    public class Test_Host
    {
        private static void DumpResource(Host host)
        {
            Console.WriteLine($"Id          : {host.Id}");
            Console.WriteLine($"Type        : {host.Type}");
            Console.WriteLine($"Created     : {host.Created}");
            Console.WriteLine($"Modified    : {host.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {host.Name}");
            Console.WriteLine($"Description : {host.Description}");
            Console.WriteLine($"Inventory   : {host.Inventory}");
            Console.WriteLine($"Enabled     : {host.Enabled}");
            Console.WriteLine($"InstanceId  : {host.InstanceId}");
            Console.WriteLine($"Variables   : {host.Variables}");
            DumpSummary(host.SummaryFields);
        }
        private static void DumpSummary(Host.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Inventory   : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"Caps        : {summary.UserCapabilities}");
            Console.WriteLine($"Groups: ({summary.Groups.Count})");
            foreach (var group in summary.Groups.Results)
            {
                Console.WriteLine($"  [{group.Id}] {group.Name}");
            }
            Console.WriteLine($"RecentJob: ({summary.RecentJobs.Length})");
            foreach (var job in summary.RecentJobs)
            {
                Console.WriteLine($"  [{job.Id}] {job.Name} {job.Status} {job.Finished}");
            }
            Console.WriteLine();

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var host = await Host.Get(1);
            Assert.IsInstanceOfType<Host>(host);
            DumpResource(host);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=20");
            await foreach(var host in Host.Find(query, false))
            {
                DumpResource(host);
            }
        }
    }
    [TestClass]
    public class Test_JobTemplate
    {
        private static void DumpResource(JobTemplate jt)
        {
            Console.WriteLine($"Id          : {jt.Id}");
            Console.WriteLine($"Type        : {jt.Type}");
            Console.WriteLine($"Created     : {jt.Created}");
            Console.WriteLine($"Modified    : {jt.Modified?.ToString() ?? "(null)"}");
            Console.WriteLine($"Name        : {jt.Name}");
            Console.WriteLine($"Description : {jt.Description}");
            Console.WriteLine($"Inventory   : {jt.Inventory}");
            Console.WriteLine($"Project     : {jt.Project}");
            Console.WriteLine($"Playbook    : {jt.Playbook}");
            Console.WriteLine($"ExtraVars   : {jt.ExtraVars}");
            DumpSummary(jt.SummaryFields);
        }
        private static void DumpSummary(JobTemplate.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization  : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"Inventory     : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"Project       : [{summary.Project.Id}][{summary.Project.ScmType}] {summary.Project.Name}");
            Console.WriteLine($"LastJob       : [{summary.LastJob?.Id}] {summary.LastJob?.Name} {summary.LastJob?.Status} {summary.LastJob?.Finished}");
            Console.WriteLine($"LastUpdate    : [{summary.LastUpdate?.Id}] {summary.LastUpdate?.Name} {summary.LastUpdate?.Status}");
            Console.WriteLine($"CreatedBy     : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy    : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"ObjectRoles   :");
            foreach (var (key,val) in summary.ObjectRoles)
            {
                Console.WriteLine($"  {key} [{val?.Id}] {val?.Name} - {val?.Description}");
            }
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine($"ResolvedEnv   : [{summary.ResolvedEnvironment?.Id}] {summary.ResolvedEnvironment?.Name}");
            Console.WriteLine($"RecentJobs    : ({summary.RecentJobs.Length})");
            foreach (var job in summary.RecentJobs)
            {
                Console.WriteLine($"  [{job.Id}][{job.Type}] {job.Status} {job.Finished}");
            }
            Console.WriteLine();

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var jt = await JobTemplate.Get(9);
            Assert.IsInstanceOfType<JobTemplate>(jt);
            Assert.IsInstanceOfType<IUnifiedJobTemplate>(jt);
            DumpResource(jt);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=20&order_by=id");
            await foreach(var jt in JobTemplate.Find(query, false))
            {
                DumpResource(jt);
            }
        }
    }
    [TestClass]
    public class Test_Job
    {
        const ulong jobId = 4;

        private static void DumpResource(JobTemplateJob.Detail job)
        {
            Console.WriteLine($"[{job.Id}] {job.Name} - {job.Description}");
            Console.WriteLine($"JobArgs: {job.JobArgs}");
            Console.WriteLine($"JobCwd : {job.JobCwd}");
            Console.WriteLine($"JobEnv : {string.Join(", ", job.JobEnv.Keys)}");
            Console.WriteLine($"EventProcessingFinished: {job.EventProcessingFinished}");
            foreach (var kv in job.HostStatusCounts)
            {
                Console.WriteLine($"HostStatus: {kv.Key}: {kv.Value}");
            }
            foreach (var kv in job.PlaybookCounts)
            {
                Console.WriteLine($"PlaybookCounts: {kv.Key}: {kv.Value}");
            }
        }
        private static void DumpResource(IJobTemplateJob job)
        {
            Console.WriteLine("===== Job =====");
            Console.WriteLine($"[{job.Id}] {job.Name} - {job.Description}");
            Console.WriteLine($"Created   : {job.Created}");
            Console.WriteLine($"Modified  : {job.Modified}");
            Console.WriteLine($"Finished  : {job.Finished}");
            Assert.IsInstanceOfType<JobType>(job.JobType);
            Console.WriteLine($"JobType   : {job.JobType}");
            Assert.IsInstanceOfType<JobLaunchType>(job.LaunchType);
            Console.WriteLine($"LaunchType: {job.LaunchType}");
            Assert.IsInstanceOfType<JobStatus>(job.Status);
            Console.WriteLine($"Status    : {job.Status}");

            Assert.IsInstanceOfType<JobVerbosity>(job.Verbosity);
            Console.WriteLine($"Verbosity : {job.Verbosity}");
            Assert.IsInstanceOfType<JobVerbosity>(job.Verbosity);
            Console.WriteLine("=== Launched By ===");
            Console.WriteLine($"  [{job.LaunchedBy.Type}]{job.LaunchedBy.Name} [{job.LaunchedBy.Id}] {job.LaunchedBy.Url}");
            Assert.AreEqual($"[{job.LaunchedBy.Type}]{job.LaunchedBy.Name}", job.LaunchedBy.ToString());
        }
        private static void DumpSummary(JobTemplateJob.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization       : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"Inventory          : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"ExecutionEnv       : [{summary.ExecutionEnvironment?.Id}] {summary.ExecutionEnvironment?.Name}");
            Console.WriteLine($"Project            : [{summary.Project.Id}][{summary.Project.ScmType}] {summary.Project.Name}");
            Console.WriteLine($"JobTemplate        : [{summary.JobTemplate.Id}] {summary.Project.Name}");
            Console.WriteLine($"UnifiedJobTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"InstanceGroup      : [{summary.InstanceGroup.Id}] {summary.InstanceGroup.Name}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var job = await JobTemplateJob.Get(jobId);
            Assert.IsInstanceOfType<JobTemplateJob.Detail>(job);
            DumpResource(job);
            Console.WriteLine($"JobArgs   : {job.JobArgs}");
            Console.WriteLine($"JobCwd    : {job.JobCwd}");
            DumpSummary(job.SummaryFields);
        }

        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=2&order_by=-id");
            await foreach (var job in  JobTemplateJob.Find(query, false))
            {
                DumpResource(job);
                DumpSummary(job.SummaryFields);
            }
        }
        [TestMethod]
        public async Task JobLogTest_Text()
        {
            var apiResult = await RestAPI.GetAsync<string>($"/api/v2/jobs/{jobId}/stdout/", AcceptType.Text);
            Assert.IsTrue(apiResult.Response.IsSuccessStatusCode);
            Assert.IsNotNull(apiResult.Contents);
            Assert.IsInstanceOfType<string>(apiResult.Contents);
            var jobLog = apiResult.Contents;
            Console.WriteLine(jobLog);
        }
        [TestMethod]
        public async Task JobLogTest_Ansi()
        {
            var apiResult = await RestAPI.GetAsync<string>($"/api/v2/jobs/{jobId}/stdout/?format=ansi", AcceptType.Text);
            Assert.IsTrue(apiResult.Response.IsSuccessStatusCode);
            Assert.IsNotNull(apiResult.Contents);
            Assert.IsInstanceOfType<string>(apiResult.Contents);
            var jobLog = apiResult.Contents;
            Console.WriteLine(jobLog);
        }

        [TestMethod]
        public async Task JobLogTest_Html()
        {
            var apiResult = await RestAPI.GetAsync<string>($"/api/v2/jobs/{jobId}/stdout/?format=html", AcceptType.Html);
            Assert.IsTrue(apiResult.Response.IsSuccessStatusCode);
            Assert.IsNotNull(apiResult.Contents);
            Assert.IsInstanceOfType<string>(apiResult.Contents);
            var jobLog = apiResult.Contents;
            Console.WriteLine(jobLog);
        }
        [TestMethod]
        public async Task JobLogTest_Json()
        {
            var apiResult = await RestAPI.GetAsync<JobLog>($"/api/v2/jobs/{jobId}/stdout/?format=json");
            Assert.IsTrue(apiResult.Response.IsSuccessStatusCode);
            Assert.IsNotNull(apiResult.Contents);
            Assert.IsInstanceOfType<JobLog>(apiResult.Contents);
            var jobLog = apiResult.Contents;
            Assert.IsInstanceOfType<JobLog.JobLogRange>(jobLog.Range);
            Assert.AreEqual<uint>(0, jobLog.Range.Start);
            Assert.IsInstanceOfType<string>(jobLog.Content);
            Console.WriteLine(jobLog.Content);
        }
    }

    [TestClass]
    public class Test_JobEvent
    {
        private static void DumpResource(JobEvent e)
        {
            Console.WriteLine($"{e.Id} {e.Counter} {e.Event} {e.EventDisplay}");
            Console.WriteLine($"  {e.Playbook} {e.Play} {e.Task} {e.Role} {e.HostName}");
            if (!string.IsNullOrEmpty(e.Stdout))
            {
                Console.WriteLine($"  StdOut: {e.Stdout}");
            }
            DumpSummary(e.SummaryFields);
        }
        private static void DumpSummary(JobEvent.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Job         : [{summary.Job.Id}] {summary.Job.Name}");
            Console.WriteLine($"JobTemplate : [{summary.Job.JobTemplateId}] {summary.Job.JobTemplateName}");
            Console.WriteLine($"Host        : [{summary.Host?.Id}] {summary.Host?.Name}");
            Console.WriteLine($"Role        : {{{summary.Role.Count}}}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Jobs()
        {
            var eventQuery = HttpUtility.ParseQueryString("order_by=counter");
            await foreach (var job in JobTemplateJob.Find(HttpUtility.ParseQueryString("order_by=-id&page_size=1"), false))
            {
                Console.WriteLine($"{job.Id} {job.Name}");
                await foreach(var e in JobEvent.Find(job, eventQuery, false))
                {
                    DumpResource(e);
                }
            }
        }
    }

    [TestClass]
    public class Test_AdHocCommand
    {
        private static void DumpResource(AdHocCommand res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name}");
            Console.WriteLine($"  {res.JobType} {res.Created} {res.Modified}");
            Console.WriteLine($"  {res.ModuleName} {res.ModuleArgs}");
            DumpSummary(res.SummaryFields);
        }
        private static void DumpSummary(AdHocCommand.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Inventory          : [{summary.Inventory.Id}][{summary.Inventory.Kind}] {summary.Inventory.Name}");
            Console.WriteLine($"ExecutionEnv       : [{summary.ExecutionEnvironment?.Id}] {summary.ExecutionEnvironment?.Name}");
            Console.WriteLine($"Credential         : [{summary.Credential?.Id}] {summary.Credential?.Kind} {summary.Credential?.Name}");
            Console.WriteLine($"InstanceGroup      : [{summary.InstanceGroup.Id}] {summary.InstanceGroup.Name}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await AdHocCommand.Get(69);
            Assert.IsInstanceOfType<AdHocCommand>(res);
            Assert.IsInstanceOfType<AdHocCommand.Detail>(res);
            DumpResource(res);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=-id&page_size=2");
            await foreach (var res in AdHocCommand.Find(query, false))
            {
                DumpResource(res);
            }
        }
    }

    [TestClass]
    public class Test_SystemJobTemplate
    {
        private static void DumpResource(SystemJobTemplate res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
            Console.WriteLine($"  {res.JobType} {res.Created} {res.Modified}");
            Console.WriteLine($"  {res.LastJobRun} {res.NextJobRun}");
            DumpSummary(res.SummaryFields);
        }
        private static void DumpSummary(SystemJobTemplate.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"LastJob       : [{summary.LastJob?.Id}] {summary.LastJob?.Name} {summary.LastJob?.Status} {summary.LastJob?.Finished}");
            Console.WriteLine($"LastUpdate    : [{summary.LastUpdate?.Id}] {summary.LastUpdate?.Name} {summary.LastUpdate?.Status}");
            Console.WriteLine($"ResolvedEnv   : [{summary.ResolvedEnvironment?.Id}] {summary.ResolvedEnvironment?.Name}");
            Console.WriteLine();

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await SystemJobTemplate.Get(1);
            Assert.IsInstanceOfType<SystemJobTemplate>(res);
            DumpResource(res);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("");
            await foreach (var res in SystemJobTemplate.Find(query, false))
            {
                DumpResource(res);
            }

        }
    }
    [TestClass]
    public class Test_SystemJob
    {
        private static void DumpResource(ISystemJob res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
            Console.WriteLine($"UnifiedJT     : {res.UnifiedJobTemplate}");
            Console.WriteLine($"LaunchType    : {res.LaunchType}");
            Console.WriteLine($"Status        : {res.Status}");
            Console.WriteLine($"EV            : {res.ExecutionEnvironment}");
            Console.WriteLine($"Failed        : {res.Failed}");
            Console.WriteLine($"Started       : {res.Started}");
            Console.WriteLine($"Fnished       : {res.Finished}");
            Console.WriteLine($"CancledOn     : {res.CanceledOn}");
            Console.WriteLine($"Elapsed       : {res.Elapsed}");
            Console.WriteLine($"JobExplain    : {res.JobExplanation}");
            Console.WriteLine($"ExecutionNode : {res.ExecutionNode}");
            Console.WriteLine($"LaunchedBy    : {res.LaunchedBy}");
            Console.WriteLine($"SystemJT      : {res.SystemJobTemplate}");
            Console.WriteLine($"JobType       : {res.JobType}");
            Console.WriteLine($"ExtraVars     : {res.ExtraVars}");
            Console.WriteLine($"ResultStdout  : {res.ResultStdout}");
        }
        private static void DumpSummary(SystemJob.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"ExecutionEnv       : [{summary.ExecutionEnvironment?.Id}] {summary.ExecutionEnvironment?.Name}");
            Console.WriteLine($"Schedule           : [{summary.Schedule?.Id}] {summary.Schedule?.Name} {summary.Schedule?.NextRun}");
            Console.WriteLine($"UnifiedJobTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"InstanceGroup      : [{summary.InstanceGroup.Id}] {summary.InstanceGroup.Name}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await SystemJob.Get(1);
            Assert.IsInstanceOfType<SystemJob.Detail>(res);
            Assert.IsInstanceOfType<IUnifiedJob>(res);
            DumpResource(res);
            Console.WriteLine($"JobArgs   : {res.JobArgs}");
            Console.WriteLine($"JobCwd    : {res.JobCwd}");
            Console.WriteLine($"JobEnv    : ({res.JobEnv.Count})");
            foreach (var (k,v) in res.JobEnv)
            {
                Console.WriteLine($"  {k}: {v}");
            }
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=id");
            await foreach(var res in SystemJob.Find(query, false))
            {
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_Schedule
    {
        static void DumpResource(Schedule res)
        {
            Console.WriteLine($"{res.Id} [{res.Type}] {res.Name} - {res.Description}");
            Console.WriteLine($"RRule   : {res.Rrule}");
            Console.WriteLine($"Job     : {res.UnifiedJobTemplate}");
            Console.WriteLine($"Start   : {res.DTStart}");
            Console.WriteLine($"NextRun : {res.NextRun}");
            Console.WriteLine($"End     : {res.DTEnd}");
        }
        static void DumpSummary(Schedule.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"UnifiedJobTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy         : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine($"Inventory          : [{summary.Inventory?.Id}] {summary.Inventory?.Name}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await Schedule.Get(1);
            Assert.IsInstanceOfType<Schedule>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=id");
            await foreach(var res in Schedule.Find(query))
            {
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }

        }
    }
    [TestClass]
    public class Test_Role
    {
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await Role.Get(1);
            Assert.IsInstanceOfType<Role>(res);
            Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
            var summary = res.SummaryFields;
            Console.WriteLine($"  Resource: {summary.ResourceId} {summary.ResourceType} {summary.ResourceName}");
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=id");
            await foreach (var res in Role.Find(query))
            {
                Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
                var summary = res.SummaryFields;
                Console.WriteLine($"  Resource: {summary.ResourceId} {summary.ResourceType} {summary.ResourceName}");
            }
        }
    }

    [TestClass]
    public class Test_NotificationTemplate
    {
        static void DumpResource(NotificationTemplate res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
            Console.WriteLine($"Origanization    : {res.Organization}");
            Console.WriteLine($"NotificationType : {res.NotificationType}");
            Console.WriteLine($"NotificationConfig:");
            Util.DumpObject(res.NotificationConfiguration);
            if (res.Messages != null)
                Util.DumpObject(res.Messages);

        }
        static void DumpSummary(NotificationTemplate.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization       : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"CreatedBy          : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy         : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"Caps               : {summary.UserCapabilities}");
            Console.WriteLine($"RecentNotification : ({summary.RecentNotification.Length})");
            foreach (var notification in summary.RecentNotification)
            {
                Console.WriteLine($"[{notification.Id,3:d}] {notification.Status} Error: {notification.Error}");
            }
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await NotificationTemplate.Get(1);
            Assert.IsInstanceOfType<NotificationTemplate>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=id");
            await foreach (var res in NotificationTemplate.Find(query))
            {
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }
    [TestClass]
    public class Test_Notification
    {
        static void DumpResource(Notification res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.NotificationType}");
            Console.WriteLine($"{res.Created} {res.Modified}");
            Console.WriteLine($"NotificationTemplate : {res.NotificationTemplate}");
            Console.WriteLine($"Error                : {res.Error}");
            Console.WriteLine($"Status               : {res.Status}");
            Console.WriteLine($"NotificationSent     : {res.NotificationsSent}");
            Console.WriteLine($"Recipients           : {res.Recipients}");
            Console.WriteLine($"Subject              : {res.Subject}");
            Console.WriteLine($"Body                 : {(res.Body ?? "(null)")}");
        }
        static void DumpSummary(Notification.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Template  : [{summary.NotificationTemplate.Id}] {summary.NotificationTemplate.Name}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await Notification.Get(1);
            Assert.IsInstanceOfType<Notification>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("order_by=id");
            await foreach(var res in Notification.Find(query))
            {
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_Label
    {
        static void DumpResource(Label res)
        {
            Console.WriteLine($"{res.Id} {res.Name} {res.Url}");
            Console.WriteLine($"Organization: {res.Organization}");
            Console.WriteLine($"Created: {res.Created} Modified: {res.Modified}");
        }
        static void DumpSummary(Label.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Organization : [{summary.Organization.Id}] {summary.Organization.Name}");
            Console.WriteLine($"CreatedBy    : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy   : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await Label.Get(1);
            Assert.IsInstanceOfType<Label>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            await foreach(var res in Label.Find(null))
            {
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_UnifiedJobTemplate
    {
        static void DumpResource(IUnifiedJobTemplate jt)
        {
            Console.WriteLine($"---- Type: {jt.GetType().Name} ----");
            Console.WriteLine($"{jt.Id} [{jt.Type}] {jt.Name}");
            Console.WriteLine($"  Status: {jt.Status}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await UnifiedJobTemplate.Get(1);
            Console.WriteLine($"{res.Id} {res.Type} {res.Name}");
            Assert.IsInstanceOfType<IUnifiedJobTemplate>(res);
            DumpResource(res);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var ujtList = await UnifiedJobTemplate.Get(1, 6, 9, 11, 13);
            foreach (var res in ujtList)
            {
                DumpResource(res);
                switch (res)
                {
                    case JobTemplate jt:
                        Console.WriteLine($"  {jt.Playbook}");
                        break;
                    case SystemJobTemplate systemjt:
                        Console.WriteLine($"  {systemjt.JobType} {systemjt.Name}");
                        break;
                    case Project pj:
                        Console.WriteLine($"  {pj.ScmType} {pj.ScmUrl} {pj.ScmBranch}");
                        break;
                    case InventorySource inv:
                        Console.WriteLine($"  {inv.SourceProject} {inv.SourcePath}");
                        break;
                    case WorkflowJobTemplate wjt:
                        Console.WriteLine($"  {wjt.Url}");
                        break;
                    default:
                        Assert.Fail($"Unkown type: {res.Type}");
                        break;
                }
            }
            Util.DumpObject(ujtList);
        }

        [TestMethod]
        public async Task Get_3_List_JobTemplate()
        {
            var query = HttpUtility.ParseQueryString("type=job_template&order_by=-id&page_size=2");
            await foreach (var res in UnifiedJobTemplate.Find(query, false))
            {
                DumpResource(res);
                Assert.IsInstanceOfType<JobTemplate>(res);
            }
        }
        [TestMethod]
        public async Task Get_4_List_Project()
        {
            var query = HttpUtility.ParseQueryString("type=project&order_by=-id&page_size=2");
            await foreach (var res in UnifiedJobTemplate.Find(query, false))
            {
                DumpResource(res);
                Assert.IsInstanceOfType<Project>(res);
            }
        }
        [TestMethod]
        public async Task Get_5_List_InventorySource()
        {
            var query = HttpUtility.ParseQueryString("type=inventory_source&order_by=-id&page_size=2");
            await foreach (var res in UnifiedJobTemplate.Find(query, false))
            {
                DumpResource(res);
                Assert.IsInstanceOfType<InventorySource>(res);
            }
        }
        [TestMethod]
        public async Task Get_6_List_SystemJobTemplate()
        {
            var query = HttpUtility.ParseQueryString("type=system_job_template&order_by=-id&page_size=2");
            await foreach (var res in UnifiedJobTemplate.Find(query, false))
            {
                DumpResource(res);
                Assert.IsInstanceOfType<SystemJobTemplate>(res);
            }
        }
        [TestMethod]
        public async Task Get_7_List_WorkflowJobTemplate()
        {
            var query = HttpUtility.ParseQueryString("type=workflow_job_template&order_by=-id&page_size=2");
            await foreach (var res in UnifiedJobTemplate.Find(query, false))
            {
                DumpResource(res);
                Assert.IsInstanceOfType<WorkflowJobTemplate>(res);
            }
        }
    }


    [TestClass]
    public class Test_UnifiedJob
    {
        static void DumpResource(IUnifiedJob job)
        {
            Console.WriteLine($"---- Type: {job.GetType().Name} ----");
            Console.WriteLine($"{job.Id} [{job.Type}] {job.Name}");
            Console.WriteLine($"  Start: {job.Started} - {job.Finished} ({job.Elapsed})");
            Console.WriteLine($"  Status: {job.Status}");
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var job = await UnifiedJob.Get(20);
            Console.WriteLine($"{job.Id} {job.Type} {job.Name}");
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
            }
        }
        [TestMethod]
        public async Task Get_3_JobTemplateJob()
        {
            var query = HttpUtility.ParseQueryString("type=job&page_size=2&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
                Assert.IsInstanceOfType<JobTemplateJob>(job);
            }
        }
        [TestMethod]
        public async Task Get_4_ProjectUpdateJob()
        {
            var query = HttpUtility.ParseQueryString("type=project_update&page_size=2&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
                Assert.IsInstanceOfType<ProjectUpdateJob>(job);
            }
        }
        [TestMethod]
        public async Task Get_5_InventoryUpdate()
        {
            var query = HttpUtility.ParseQueryString("type=inventory_update&page_size=2&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
                Assert.IsInstanceOfType<InventoryUpdateJob>(job);
            }
        }
        [TestMethod]
        public async Task Get_6_WorkflobJob()
        {
            var query = HttpUtility.ParseQueryString("type=workflow_job&page_size=2&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
                Assert.IsInstanceOfType<WorkflowJob>(job);
            }
        }
        [TestMethod]
        public async Task Get_7_SystemJob()
        {
            var query = HttpUtility.ParseQueryString("type=system_job&page_size=2&order_by=-id");
            await foreach (var job in UnifiedJob.Find(query, false))
            {
                DumpResource(job);
                Assert.IsInstanceOfType<SystemJob>(job);
            }
        }
    }

    [TestClass]
    public class Test_WorkflowJobTemplate
    {
        static void DumpResource(WorkflowJobTemplate res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name}");
            Console.WriteLine($"Description : {res.Description}");
            Console.WriteLine($"Status      : {res.Status}");
        }
        static void DumpSummary(WorkflowJobTemplate.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"LastJob      : [{summary.LastJob?.Id}] {summary.LastJob?.Name} {summary.LastJob?.Status} {summary.LastJob?.Finished}");
            Console.WriteLine($"LastUpdate   : [{summary.LastUpdate?.Id}] {summary.LastUpdate?.Name} {summary.LastUpdate?.Status}");
            Console.WriteLine($"CreatedBy    : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy   : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine($"ObjectRoles  : ({summary.ObjectRoles.Count})");
            foreach (var (k, role) in summary.ObjectRoles)
            {
                Console.WriteLine($"  {k}: {role}");
            }
            Console.WriteLine();

        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await WorkflowJobTemplate.Get(13);
            Assert.IsInstanceOfType<WorkflowJobTemplate>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=-id");
            await foreach (var res in WorkflowJobTemplate.Find(query, false))
            {
                Assert.IsInstanceOfType<WorkflowJobTemplate>(res);
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_WofkflowJob
    {
        static void DumpResource(WorkflowJob res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name}");
            Console.WriteLine($"Description : {res.Description}");
            Console.WriteLine($"Status      : {res.Status}");
        }
        static void DumpSummary(WorkflowJob.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Template        : [{summary.WorkflowJobTemplate.Id}] {summary.WorkflowJobTemplate.Name}");
            Console.WriteLine($"Schedule        : [{summary.Schedule?.Id}] {summary.Schedule?.Name} {summary.Schedule?.NextRun}");
            Console.WriteLine($"UnifiedTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine($"CreatedBy       : [{summary.CreatedBy?.Id}] {summary.CreatedBy?.Username}");
            Console.WriteLine($"ModifiedBy      : [{summary.ModifiedBy?.Id}] {summary.ModifiedBy?.Username}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await WorkflowJob.Get(51);
            Assert.IsInstanceOfType<WorkflowJob>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=-id");
            await foreach (var res in WorkflowJob.Find(query, false))
            {
                Assert.IsInstanceOfType<WorkflowJob>(res);
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_WorkflowJobTemplateNode
    {
        static void DumpResource(WorkflowJobTemplateNode res)
        {
            Console.WriteLine($"{res.Id} {res.Type}");
            Console.WriteLine($"WorkflowJobTemplate : {res.WorkflowJobTemplate}");
            Console.WriteLine($"UnifiedJobTemplate  : {res.UnifiedJobTemplate}");
            Console.WriteLine($"SuccessNodes        : {string.Join(", ", res.SuccessNodes)}");
            Console.WriteLine($"FailureNodes        : {string.Join(", ", res.FailureNodes)}");
            Console.WriteLine($"AlwaysNodes         : {string.Join(", ", res.AlwaysNodes)}");
        }
        static void DumpSummary(WorkflowJobTemplateNode.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Template        : [{summary.WorkflowJobTemplate.Id}] {summary.WorkflowJobTemplate.Name}");
            Console.WriteLine($"UnifiedTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await WorkflowJobTemplateNode.Get(1);
            Assert.IsInstanceOfType<WorkflowJobTemplateNode>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=-id");
            await foreach (var res in WorkflowJobTemplateNode.Find(query, false))
            {
                Assert.IsInstanceOfType<WorkflowJobTemplateNode>(res);
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }
    [TestClass]
    public class Test_WorkflowJobNode
    {
        static void DumpResource(WorkflowJobNode res)
        {
            Console.WriteLine($"{res.Id} {res.Type}");
            Console.WriteLine($"Job                 : {res.Job}");
            Console.WriteLine($"UnifiedJobTemplate  : {res.UnifiedJobTemplate}");
            Console.WriteLine($"SuccessNodes        : {string.Join(", ", res.SuccessNodes)}");
            Console.WriteLine($"FailureNodes        : {string.Join(", ", res.FailureNodes)}");
            Console.WriteLine($"AlwaysNodes         : {string.Join(", ", res.AlwaysNodes)}");
        }
        static void DumpSummary(WorkflowJobNode.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Job             : [{summary.Job.Id}][{summary.Job.Type}] {summary.Job.Status} {summary.Job.Name}");
            Console.WriteLine($"WorkflowJob     : [{summary.WorkflowJob.Id}] {summary.WorkflowJob.Name}");
            Console.WriteLine($"UnifiedTemplate : [{summary.UnifiedJobTemplate.Id}][{summary.UnifiedJobTemplate.UnifiedJobType}] {summary.UnifiedJobTemplate.Name}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await WorkflowJobNode.Get(1);
            Assert.IsInstanceOfType<WorkflowJobNode>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=-id");
            await foreach (var res in WorkflowJobNode.Find(query, false))
            {
                Assert.IsInstanceOfType<WorkflowJobNode>(res);
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }

    [TestClass]
    public class Test_ExecutionEnvironment
    {
        static void DumpResource(ExecutionEnvironment res)
        {
            Console.WriteLine($"{res.Id} {res.Type} {res.Name} {res.Description}");
            Console.WriteLine($"Image   : {res.Image}");
            Console.WriteLine($"Managed : {res.Managed}");
        }
        static void DumpSummary(ExecutionEnvironment.Summary summary)
        {
            Console.WriteLine("-----SummaryFields-----");
            Console.WriteLine($"Caps : {summary.UserCapabilities}");
            Console.WriteLine();
        }
        [TestMethod]
        public async Task Get_1_Single()
        {
            var res = await ExecutionEnvironment.Get(1);
            Assert.IsInstanceOfType<ExecutionEnvironment>(res);
            DumpResource(res);
            DumpSummary(res.SummaryFields);
        }
        [TestMethod]
        public async Task Get_2_List()
        {
            var query = HttpUtility.ParseQueryString("page_size=10&order_by=id");
            await foreach (var res in ExecutionEnvironment.Find(query, false))
            {
                Assert.IsInstanceOfType<ExecutionEnvironment>(res);
                DumpResource(res);
                DumpSummary(res.SummaryFields);
            }
        }
    }


    [TestClass]
    public class Test_Config
    {
        [TestMethod]
        public async Task Config_Get()
        {
            var apiResult = await RestAPI.GetAsync<Config>("/api/v2/config/");
            Assert.IsNotNull(apiResult);
            var config = apiResult.Contents;
            Assert.IsNotNull(config);
            Util.DumpObject(config);
            Util.DumpResponse(apiResult.Response);
            Assert.IsInstanceOfType<Config>(config);
            Assert.AreEqual("UTC", config.TimeZone);
            Assert.AreEqual("open", config.LicenseInfo.LicenseType);

            Assert.AreEqual("config", config.AnalyticsCollectors["config"].Name);
        }
    }

    [TestClass]
    public class Test_Settings
    {
        [TestMethod]
        public async Task Settings_Get()
        {
            var apiResult = await RestAPI.GetAsync<ResultSet<Setting>>("/api/v2/settings/");
            Assert.IsNotNull(apiResult);
            var resultSet = apiResult.Contents;
            Assert.IsNotNull(resultSet);
            Util.DumpObject(resultSet);
            Util.DumpResponse(apiResult.Response);

            Assert.IsTrue(resultSet.Results.Length > 0);
            foreach (var setting in resultSet.Results)
            {
                Assert.IsInstanceOfType<Setting>(setting);
                Console.WriteLine($"{setting.Name}: Slug: {setting.Slug} URL: {setting.Url}");
            }
        }
        [TestMethod]
        public async Task Settings_Get_Github()
        {
            var apiResult = await RestAPI.GetAsync<object>("/api/v2/settings/github/");
            Assert.IsNotNull(apiResult);
            var setting = apiResult.Contents;
            Assert.IsNotNull(setting);
            Util.DumpObject(setting);
        }
    }

}
