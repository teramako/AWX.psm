---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-ProjectUpdateJob

## SYNOPSIS
Retrieve jobs for ProjectUpdate.

## SYNTAX

### All (Default)
```
Find-ProjectUpdateJob [[-Name] <String[]>] [-Status <String[]>] [-OrderBy <String[]>] [-Search <String[]>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-ProjectUpdateJob -Type <ResourceType> -Id <UInt64> [[-Name] <String[]>] [-Status <String[]>]
 [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of ProjectUpdate jobs.

Implementation of following API:  
- `/api/v2/project_updates/`  
- `/api/v2/projects/{id}/project_updates/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-ProjectUpdateJob -Status successful
```

Retreive successfully completed ProjectUpdate jobs.

### Example 2
```powershell
PS C:\> Find-ProjectUpdateJob -Type Project -Id 1
```

Retrieve ProjectUpdate jobs associated with the Project of ID 1

`Id` and `Type` parameters can also be given from the pipeline, likes following:  
    Get-Project -Id 1 | Find-ProjectUpdateJob

and also can omit `-Type` parameter:  
    Find-ProjectUpdateJob -Id 1

## PARAMETERS

### -All
Retrieve resources from all pages.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Count
Number to retrieve per page.

```yaml
Type: UInt16
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 20
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Datebase ID of the target resource.
Use in conjection with the `-Type` parameter.

```yaml
Type: UInt64
Parameter Sets: AssociatedWith
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
Filter by ProjectUpdate job name.
The names must be an exact match. (case-sensitive)

Multiple words are available by separating with a comma(`,`).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OrderBy
Retrieve list in the specified orders.
Use `!` prefix to sort in reverse.
Multiple sorting fields are available by separating with a comma(`,`).

Default value: `!id` (descending order of ID)

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ["!id"]
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
Page number.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -Search
Search words. (case-insensitive)

Target fields: `name`, `description`

Multiple words are available by separating with a comma(`,`).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Status
Filter by `status` field.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:
Accepted values: new, started, pending, waiting, running, successful, failed, error, canceled

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Resource type name of the target.
Use in conjection with the `-Id` parameter.

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: Project

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.ResourceType
Input by `Type` property in the pipeline object.

Acceptable values: `Project` (only)

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for `Project`

## OUTPUTS

### AWX.Resources.ProjectUpdateJob
## NOTES

## RELATED LINKS

[Get-ProjectUpdateJob](Get-ProjectUpdateJob.md)

[Get-Project](Get-Project.md)

[Find-Project](Find-Project.md)
