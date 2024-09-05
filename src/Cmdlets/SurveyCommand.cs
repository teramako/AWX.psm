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

}
