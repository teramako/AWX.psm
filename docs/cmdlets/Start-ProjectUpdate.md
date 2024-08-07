---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Start-ProjectUpdate

## SYNOPSIS
Invoke (update) Project.

## SYNTAX

### Id
```
Start-ProjectUpdate [-Id] <UInt64> [<CommonParameters>]
```

### CheckId
```
Start-ProjectUpdate [-Id] <UInt64> [-Check] [<CommonParameters>]
```

### Project
```
Start-ProjectUpdate [-Project] <Project> [<CommonParameters>]
```

### CheckProject
```
Start-ProjectUpdate [-Project] <Project> [-Check] [<CommonParameters>]
```

## DESCRIPTION
Update a Project.

This command only sends a request to update Project, not wait for the job is completed.
So, the returned job object will be non-completed status.
Use `Wait-UnifiedJob` command to wait for the job to complete later.

Implementation of following API:  
- `/api/v2/projects/{id}/update/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Invoke-ProjectUpdate -Id 8

 Id          Type Name   JobType LaunchType  Status Finished            Elapsed LaunchedBy     Template       Note
 --          ---- ----   ------- ----------  ------ --------            ------- ----------     --------       ----
100 ProjectUpdate proj_1   Check     Manual Pending 2024/08/06 15:34:34   1.888 [user][1]admin [8][git]proj_1 {[Branch, master], [Revision, ***], [Url, ***]}
```

Update a Project ID 8.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
Project ID to be updated.

### AWX.Resources.Project
Project object to be updated.

## OUTPUTS

### AWX.Resources.ProjectUpdateJob+Detail
The result job object of updated the Project (non-completed status).

### System.Management.Automation.PSObject
Results of checked wheter the Project can be updated.

## NOTES

## RELATED LINKS

[Invoke-ProjectUpdate](Invoke-ProjectUpdate.md)

[Get-ProjectUpdate](Get-ProjectUpdate.md)

[Find-ProjectUpdate](Find-ProjectUpdate.md)

[Wait-UnifiedJob](Wait-UnifiedJob.md)
