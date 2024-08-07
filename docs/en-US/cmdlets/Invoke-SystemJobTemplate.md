---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-SystemJobTemplate

## SYNOPSIS
Invoke (launch) a SystemJobTemplate and wait unti the job is finished.

## SYNTAX

### Id
```
Invoke-SystemJobTemplate [-Id] <UInt64> [-ExtraVars <IDictionary>]
 [<CommonParameters>]
```

### Template
```
Invoke-SystemJobTemplate [-SystemJobTemplate] <SystemJobTemplate> [-ExtraVars <IDictionary>]
 [<CommonParameters>]
```

## DESCRIPTION
Launch the specified SystemJobTemplate and wait until the job is finished.

Implementation of following API:  
- `/api/v2/system_job_templates/{id}/launch/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Invoke-SystemJobTemplate -Id 4
====== [110] Cleanup Expired Sessions ======
Expired Sessions deleted 2


 Id      Type Name                              JobType LaunchType     Status Finished            Elapsed LaunchedBy     Template                    Note
 --      ---- ----                              ------- ----------     ------ --------            ------- ----------     --------                    ----
110 SystemJob Cleanup Expired Sessions cleanup_sessions     Manual Successful 2024/08/06 15:56:27   1.793 [user][1]admin [4]Cleanup Expired Sessions {[ExtraVars, {}], [Stdout, Expired Sessions deleted 2…
```

Launch JobTemplate ID 4, and wait unti for the job is finished.

### Example 2
```powershell
PS C:\> Invoke-SystemJobTemplate -Id 2 -ExtraVars @{ dry_run = $true }
====== [120] Cleanup Activity Stream ======
would skip "update-2024-05-18T06:21:07.998340+00:00-pk=23" id: 23
would skip "create-2024-05-18T06:29:33.680642+00:00-pk=24" id: 24

(snip)

Removed 0 items


 Id      Type Name                                   JobType LaunchType     Status Finished            Elapsed LaunchedBy     Template                   Note
 --      ---- ----                                   ------- ----------     ------ --------            ------- ----------     --------                   ----
120 SystemJob Cleanup Activity Stream cleanup_activitystream     Manual Successful 2024/08/06 16:04:30   2.171 [user][1]admin [2]Cleanup Activity Stream {[ExtraVars, {"dry_run": true}], *** }
```

Launch SystemJobTemplate ID 2(Cleanup ActivityStream) as dry-run mode.

## PARAMETERS

### -ExtraVars
Variables to be passed to the system job task as command line parameters.

For excample:  
- `@{ dry_run: $true }` : for `cleanup_jobs` and `cleanup_activitystream`  
- `@{ days: 90 }'`      : for `cleanup_jobs` and `cleanup_activitystream`

```yaml
Type: IDictionary
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
SystemJobTemplate ID to be launched.

```yaml
Type: UInt64
Parameter Sets: Id
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SystemJobTemplate
SystemJobTempalte object to be launched.

```yaml
Type: SystemJobTemplate
Parameter Sets: Template
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
SystemJobTemplate ID to be launched.

### AWX.Resources.SystemJobTemplate
SystemJobTemplate object to be launched.

## OUTPUTS

### AWX.Resources.SystemJob
The result job object of lanched the SystemJobTemplate.

## NOTES

## RELATED LINKS

[Start-SystemJobTemplate](Start-SystemJobTemplate.md)

[Get-SystemJob](Get-SystemJob.md)

[Find-SystemJob](Find-SystemJob)
