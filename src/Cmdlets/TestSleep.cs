using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsDiagnostic.Test, "Sleep")]
    public class TestSleep : Cmdlet
    {
        private DateTime startTime = DateTime.Now;
        private int time = 5;
        private Sleep? _sleep;
        protected override void BeginProcessing()
        {
            WriteObject($"Start: {startTime}");
        }
        protected override void ProcessRecord()
        {
            using (_sleep = new Sleep())
            {
                for (var i = 0; i < time; i++)
                {
                    WriteObject($"Time {i,5:d}: {DateTime.Now}");
                    _sleep.Do(1000);
                }

            }
        }
        protected override void EndProcessing()
        {
            var endTime = DateTime.Now;
            var span = endTime - startTime;
            WriteObject($"End  : {endTime}");
            WriteObject($"Span : {span}");
        }
        protected override void StopProcessing()
        {
            EndProcessing();
            _sleep?.Stop();
        }
    }
}
