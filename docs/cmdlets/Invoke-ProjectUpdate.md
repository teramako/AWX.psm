---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-ProjectUpdate

## SYNOPSIS
Invoke (update) a Project and wait until the job is finished.

## SYNTAX

### Id
```
Invoke-ProjectUpdate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-Id] <UInt64>
 [<CommonParameters>]
```

### Project
```
Invoke-ProjectUpdate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-Project] <Project>
 [<CommonParameters>]
```

### CheckId
```
Invoke-ProjectUpdate [-Id] <UInt64> [-Check] [<CommonParameters>]
```

### CheckProject
```
Invoke-ProjectUpdate [-Project] <Project> [-Check] [<CommonParameters>]
```

## DESCRIPTION
Update a Project and wait until the job is finished.

Implementation of following API:  
- `/api/v2/projects/{id}/update/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Invoke-ProjectUpdate -Id 8
====== [100] proj_1 ======
Identity added: /runner/artifacts/336/ssh_key_data (***@********)

PLAY [Update source tree if necessary] *****************************************

(snip)

PLAY RECAP *********************************************************************
localhost                  : ok=3    changed=0    unreachable=0    failed=0    skipped=4    rescued=0    ignored=0

 Id          Type Name   JobType LaunchType     Status Finished            Elapsed LaunchedBy     Template       Note
 --          ---- ----   ------- ----------     ------ --------            ------- ----------     --------       ----
100 ProjectUpdate proj_1   Check     Manual Successful 2024/08/06 15:34:34   1.888 [user][1]admin [8][git]proj_1 {[Branch, master], [Revision, 3cc7efff0ab80a0108456317c47214509728c9d3], [Url, git@gitrepo:repo1.git]}
```

Update a Project ID 8, and wait until for the job is finished.

### Example 2
```powershell
PS C:\> Invoke-ProjectUpdate -Id 8 -Check

Id    Type CanUpdate
--    ---- ---------
 8 Project      True
```

Check wheter Project ID 8 can be updated.

## PARAMETERS

### -Check
Check wheter a Project can be updated.

```yaml
Type: SwitchParameter
Parameter Sets: CheckId, CheckProject
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Project ID to be updated.

```yaml
Type: UInt64
Parameter Sets: Id, CheckId
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -IntervalSeconds
Interval to confirm job completion (seconds).
Default is 5 seconds.

```yaml
Type: Int32
Parameter Sets: Id, Project
Aliases:

Required: False
Position: Named
Default value: 5
Accept pipeline input: False
Accept wildcard characters: False
```

### -Project
Project object to be updated.

```yaml
Type: Project
Parameter Sets: Project, CheckProject
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SuppressJobLog
Suppress display job log.

Note:  
Use `-InformationVariable` parameter, if want to store logs to a variable.

```yaml
Type: SwitchParameter
Parameter Sets: Id, Project
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
Project ID to be updated.

### AWX.Resources.Project
Project object to be updated.

## OUTPUTS

### AWX.Resources.ProjectUpdateJob
The result job object of updated the Project.

### System.Management.Automation.PSObject
Results of checked wheter the Project can be updated.

## NOTES

## RELATED LINKS

[Start-ProjectUpdate](Start-ProjectUpdate.md)

[Get-ProjectUpdate](Get-ProjectUpdate.md)

[Find-ProjectUpdate](Find-ProjectUpdate.md)
