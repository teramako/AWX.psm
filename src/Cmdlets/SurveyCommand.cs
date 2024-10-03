using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SurveySpec")]
    [OutputType(typeof(Survey))]
    public class GetSurveySpecCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [ValidateSet(nameof(ResourceType.JobTemplate), nameof(ResourceType.WorkflowJobTemplate))]
        public ResourceType Type { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public ulong Id { get; set; }

        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/survey_spec/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/survey_spec/",
                _ => throw new ArgumentException($"Unkown Resource Type: {Type}")
            };
            var survey = GetResource<Survey>(path);
            WriteObject(survey);
        }
    }

    [Cmdlet(VerbsLifecycle.Register, "SurveySpec", SupportsShouldProcess = true)]
    public class RegisterSurverySpecCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.JobTemplate,
                ResourceType.WorkflowJobTemplate
        ])]
        public IResource Template { get; set; } = new Resource(0, 0);

        [Parameter()]
        [AllowEmptyString]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string Description { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        public SurveySpec[] Spec { get; set; } = [];

        protected override void ProcessRecord()
        {
            var path = Template.Type switch
            {
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Template.Id}/survey_spec/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Template.Id}/survey_spec/",
                _ => throw new ArgumentException($"Invalid Resource Type: {Template.Type}")
            };
            var sendData = new Survey() { Name = Name, Description = Description, Spec = Spec };
            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Survey>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose("Success");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
