---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-AdHocCommandJob

## SYNOPSIS
Retrieve jobs for AdHocCommand.

## SYNTAX

### All (Default)
```
Find-AdHocCommandJob [-OrderBy <String[]>] [-Search <String[]>] [-Filter <NameValueCollection>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-AdHocCommandJob -Type <ResourceType> -Id <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Filter <NameValueCollection>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of AdHocCommand jobs.

Implements following Rest API:  
- `/api/v2/ad_hoc_commands/`  
- `/api/v2/inventories/{id}/ad_hoc_commands/`  
- `/api/v2/hosts/{id}/ad_hoc_commands/`  
- `/api/v2/groups/{id}/ad_hoc_commands/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-AdHocCommandJob -Search ping
```

Serach `ping` jobs.

### Example 2
```powershell
PS C:\> Find-AdHocCommandJob -Count 2 | Get-JobLog
```

Retreive two of latest jobs and then get the logs.

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

For more details, see [about_AWX.psm_Filter_parameter](about_AWX.psm_Filter_parameter.md).

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
Page Number

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
Search words. (case-insensitive)

Multiple words are available by separating with a comma(`,`).

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: Inventory, Host, Group

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
- `Host`  
- `Group`

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.AdHocCommand
## NOTES

## RELATED LINKS

[Get-AdHocCommand](Get-AdHocCommandJob.md)

[Invoke-AdHocCommand](Invoke-AdHocCommand.md)
