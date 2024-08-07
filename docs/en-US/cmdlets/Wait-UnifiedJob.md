---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Wait-UnifiedJob

## SYNOPSIS
Wait until jobs are finished.

## SYNTAX

```
Wait-UnifiedJob [-Type] <ResourceType> [-Id] <UInt64> [-IntervalSeconds <Int32>] [-SuppressJobLog]
 [<CommonParameters>]
```

## DESCRIPTION
Wait until a job is finished.
While waiting, retrieve the logs periodically and ouput.

## EXAMPLES

### Example 1
```powershell
PS C:\> Wait-UnifiedJob -Type Job -Id 110
====== [110] Demo Job Template ======

PLAY [Hello World Sample] ******************************************************

TASK [Gathering Facts] *********************************************************
ok: [localhost]

TASK [Hello Message] ***********************************************************
ok: [localhost] => {
    "msg": "Hello World!"
}

PLAY RECAP *********************************************************************
localhost                  : ok=2    changed=0    unreachable=0    failed=0    skipped=0    rescued=0    ignored=0

 Id Type Name              JobType LaunchType     Status Finished Elapsed LaunchedBy     Template Note
 -- ---- ----              ------- ----------     ------ -------- ------- ----------     -------- ----
110  Job Demo Job Template     Run     Manual Successful ....         ... ...             ...      ...
```

Wait until Job ID 110 is completed.

### Example 2
```powershell
PS C:\> Find-Job -Status running | Wait-UnifiedJob
```

Retrieve running jobs currently, and wait until those jobs are completed.

## PARAMETERS

### -Id
Job ID of the target resource.
Use in conjection with the `-Type` parameter.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -IntervalSeconds
Interval to confirm job completion (seconds).
Default is 5 seconds.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 5
Accept pipeline input: False
Accept wildcard characters: False
```

### -SuppressJobLog
Suppress display job log.

Note:  
Use `-InformationVariable` parameter, if want to store logs to a variable.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Resource type name of the target.
Use in conjection with the `-Id` parameter.

```yaml
Type: ResourceType
Parameter Sets: (All)
Aliases:
Accepted values: Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand, WorkflowJob

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.ResourceType
Input by `Type` property in the pipeline object.

Acceptable values:  
- `Job`  
- `ProjectUpdate`  
- `InventoryUpdate`  
- `SystemJob`  
- `AdHocCommand`  
- `WorkflowJob`

### System.UInt64
Input by `Id` property in the pipeline object.

## OUTPUTS

### AWX.Resources.JobTemplateJob
### AWX.Resources.ProjectUpdateJob
### AWX.Resources.InventoryUpdateJob
### AWX.Resources.SystemJob
### AWX.Resources.AdHocCommand
### AWX.Resources.WorkflowJob
## NOTES

## RELATED LINKS

[Start-JobTemplateJob](Start-JobTemplate.md)

[Start-ProjectUpdate](Start-ProjectUpdate.md)

[Start-InventoryUpdate](Start-InventoryUpdate.md)

[Start-SystemJobTemplate](Start-SystemJobTemplate.md)

[Start-AdHocCommand](Start-AdHocCommand.md)

[Start-WorkflowJobTemplate](Start-WorkflowJobTemplate.md)

[Stop-UnifiedJob](Stop-UnifiedJob.md)

[Find-UnifiedJob](Find-UnifiedJob.md)
