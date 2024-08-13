---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-Group

## SYNOPSIS
Retrieve Groups.

## SYNTAX

### All (Default)
```
Find-Group [-OrderBy <String[]>] [-Search <String[]>] [-Filter <NameValueCollection>] [-Count <UInt16>]
 [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-Group -Type <ResourceType> -Id <UInt64> [-OnlyRoot] [-OnlyParnets] [-OrderBy <String[]>]
 [-Search <String[]>] [-Filter <NameValueCollection>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of Groups.

Implementation of following API:  
- `/api/v2/groups/`  
- `/api/v2/inventories/{id}/groups/`  
- `/api/v2/inventories/{id}/root_groups/`  
- `/api/v2/groups/{id}/children/`  
- `/api/v2/inventory_sources/{id}/groups/`  
- `/api/v2/hosts/{id}/groups/`  
- `/api/v2/hosts/{id}/all_groups/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-Group
```

### Example 2
```powershell
PS C:\> Find-Group -Type Inventory -Id 1
```

Retrieve Groups associated with the Inventory of ID 1

`Id` and `Type` parameters can also be given from the pipeline, likes following:  
    Get-Inventory -Id 1 | Find-Group

### Example 3
```powershell
PS C:\> Find-Group -Type Inventory -Id 1 -OnlyRoot
```

Retrieve **root** (top-level) Groups associated with the Inventory of ID 1

### Example 4
```powershell
PS C:\> Find-Group -Type Host -Id 1
```

Retrieve Groups of which the target Host (ID 1) is directly or indirectly a member.

If you need only directly a member, use `-OnlyParents` parameter.

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

### -Filter
Filtering various fields.

For examples:  
- `name__icontains=test`: "name" field contains "test" (case-insensitive).  
- `"name_ in=test,demo", created _gt=2024-01-01`: "name" field is "test" or "demo" and created after 2024-01-01.  
- `@{ Name = "name"; Value = "test"; Type = "Contains"; Not = $true }`: "name" field NOT contains "test"

For more details, see about_AWX.psm_Filter_parameter (about_AWX.psm_Filter_parameter.md).

```yaml
Type: NameValueCollection
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

### -OnlyParnets
List only directly member Groups.
Only affected for a **Host** Type

```yaml
Type: SwitchParameter
Parameter Sets: AssociatedWith
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OnlyRoot
List only root(Top-level) Groups.
Only affected for an **Inventory** Type

```yaml
Type: SwitchParameter
Parameter Sets: AssociatedWith
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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
Accepted values: Inventory, Group, InventorySource, Host

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
- `Inventory`  
- `Group`  
- `InventorySource`  
- `Host`

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.Group
## NOTES

## RELATED LINKS

[Get-Group](Get-Group.md)

[Get-Inventory](Get-Inventory.md)

[Find-Inventory](Find-Inventory.md)

[Get-Host](Get-Host.md)

[Find-Host](Find-Host.md)
