---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-SystemJob

## SYNOPSIS
Retrieve SystemJob details by ID(s).

## SYNTAX

```
Get-SystemJob [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve SystemJob details by the specified ID(s).

Implements following Rest API:  
- `/api/v2/system_job/{id}/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-SystemJob -Id 10

Id      Type Name                              JobType LaunchType     Status Finished            Elapsed LaunchedBy                            Template                    Note
--      ---- ----                              ------- ----------     ------ --------            ------- ----------                            --------                    ----
10 SystemJob Cleanup Expired Sessions cleanup_sessions  Scheduled Successful 2024/07/06 22:01:18   2.494 [schedule][4]Cleanup Expired Sessions [4]Cleanup Expired Sessions {[ExtraVars, ], [Stdout, Expired Sessions deleted 16…
```

Retrieve a SystemJob for Database ID 10.

## PARAMETERS

### -Id
List of database IDs for one or more SystemJobs.

```yaml
Type: UInt64[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64[]
One or more database IDs.

## OUTPUTS

### AWX.Resources.SystemJob+Detail
## NOTES

## RELATED LINKS

[Find-SystemJob](Find-SystemJob.md)

[Remove-SystemJob](Remove-SystemJob.md)
