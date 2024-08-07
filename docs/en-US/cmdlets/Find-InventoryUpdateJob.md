---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-InventoryUpdateJob

## SYNOPSIS
Retrieve jobs for InventoryUpdate.

## SYNTAX

### All (Default)
```
Find-InventoryUpdateJob [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

### AssociatedWith
```
Find-InventoryUpdateJob -Type <ResourceType> -Id <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of InventoryUpdates which are jobs of InventorySources.

Implementation of following API:  
- `/api/v2/inventory_updates/`  
- `/api/v2/project_updates/{id}/scm_inventory_updates/`  
- `/api/v2/inventory_sources/{id}/inventory_updates/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-InventoryUpdate
```

### Example 2
```powershell
PS C:\> Find-InventoryUpdate -Type InventorySource -Id 1
```

Retrieve InstanceUpdate jobs associated with the InventorySource of ID 1

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

### -Type
Resource type name of the target.
Use in conjection with the `-Id` parameter.

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: ProjectUpdate, InventorySource

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
- `ProjectUpdate`  
- `InventorySource`  

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.InventoryUpdateJob
## NOTES

## RELATED LINKS

[Get-InventoryUpdateJob](Get-InventoryUpdateJob.md)

[Invoke-InventoryUpdate](Invoke-InventoryUpdate.md)

[Start-InventoryUpdate](Start-InventoryUpdate.md)

[Get-InventorySource](Get-InventorySource.md)

[Find-InventorySource](Find-InventorySource.md)

[Find-UnifiedJob](Find-UnifiedJob.md)
