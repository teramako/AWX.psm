---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-WorkflowJobNode

## SYNOPSIS
Retrieve nodes for WorkflowJob.

## SYNTAX

### All (Default)
```
Find-WorkflowJobNode [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

### AssociatedWith
```
Find-WorkflowJobNode [-Type <ResourceType>] -Id <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of WorkflowJobNodes.

Implementation of following API:  
- `/api/v2/workflow_job_nodes/`  
- `/api/v2/workflow_jobs/{id}/workflow_nodes/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-WorkflowJobNode
```

### Example 2
```powershell
PS C:\> Find-WorkflowJobNode -Type WorkflowJob -Id 10
```

Retrieve nodes associated with the WorkflowJob of ID 10

`Id` and `Type` parameters can also be given from the pipeline, likes following:  
    Get-WorkflowJob -Id 10 | Find-WorkflowJobNode

and also can omit `-Type` parameter:  
    Find-WorkflowJobNode -Id 10

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

Target fields: `unified_job_template__name`, `unified_job_template__description`

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

### -Type
Resource type name of the target.
Use in conjection with the `-Id` parameter.

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: WorkflowJob

Required: False
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

Acceptable values: `WorkflowJob` (only)

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for `WorkflowJob`

## OUTPUTS

### AWX.Resources.WorkflowJobNode
## NOTES

## RELATED LINKS

[Get-WorkflowJobNode](Get-WorkflowJobNode.md)

[Get-WorkflowJob](Get-WorkflowJob.md)

[Find-WorkflowJob](Find-WorkflowJob.md)
