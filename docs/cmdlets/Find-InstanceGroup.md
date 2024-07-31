---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-InstanceGroup

## SYNOPSIS
Retrieve InstanceGroups.

## SYNTAX

### All (Default)
```
Find-InstanceGroup [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

### AssociatedWith
```
Find-InstanceGroup -Type <ResourceType> -Id <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of InstanceGroups.

Implementation of following API:  
- `/api/v2/instance_groups/`  
- `/api/v2/instances/{id}/instance_groups/`  
- `/api/v2/organizations/{id}/instance_groups/`  
- `/api/v2/inventories/{id}/instance_groups/`  
- `/api/v2/job_templates/{id}/instance_groups/`  
- `/api/v2/schedules/{id}/instance_groups/`  
- `/api/v2/workflow_job_template_nodes/{id}/instance_groups/`  
- `/api/v2/workflow_job_nodes/{id}/instance_groups/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-InstaceGroup
```

### Example 2
```powershell
PS C:\> Find-InstaceGroup -Type Instance -Id 1
```

Retrieve InstanceGroups associated with the Instance of ID 1

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
Default value: None
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

Default value: `id` (ascending order of ID)

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ["id"]
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

Target fields: `name`

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
Accepted values: Instance, Organization, Inventory, JobTemplate, Schedule, WorkflowJobTemplateNode, WorkflowJobNode

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

Acceptable values:  
- `Instance`  
- `Organization`  
- `Inventory`  
- `JobTemplate`  
- `Schedule`  
- `WorkflowJobTemplateNode`  
- `WorkflowJobNode`  

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.InstanceGroup
## NOTES

## RELATED LINKS

[Get-InstanceGroup](Get-InstanceGroup.md)

[Get-Instance](Get-Instance.md)

[Find-Instance](Find-Instance.md)
