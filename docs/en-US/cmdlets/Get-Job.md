---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-Job

## SYNOPSIS
Retrieve JobTemplate's job details by ID(s).

## SYNTAX

```
Get-Job [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve JobTemplate's job details by the specified ID(s).

Implements following Rest API:  
- `/api/v2/jobs/{id}/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-Job -Id 1
```

Retrieve a JobTemplate's job detail for Database ID 1.

## PARAMETERS

### -Id
List of database IDs for one or more jobs.

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

### AWX.Resources.JobTemplateJob+Detail
## NOTES

## RELATED LINKS

[Find-Job](Find-Job.md)

[Remove-Job](Remove-Job.md)
