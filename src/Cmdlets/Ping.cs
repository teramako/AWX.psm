﻿using AnsibleTower.Resources;
using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Ping")]
    [OutputType([typeof(Ping)])]
    public class GetPingCommand : APICmdletBase
    {
        const string Path = "/api/v2/ping/";
        protected override void EndProcessing()
        {
            var pong = GetResource<Ping>(Path);
            WriteObject(pong);
        }
    }
}
