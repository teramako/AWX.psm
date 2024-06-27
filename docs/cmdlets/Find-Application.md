---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-Application

## SYNOPSIS
Retrieve Applications from AWX/AnsibleTower

## SYNTAX

### All (Default)
```
Find-Application [-OrderBy <String[]>] [[-Search] <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-Application -Id <UInt64> -Type <ResourceType> [-OrderBy <String[]>] [[-Search] <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Retrieve and list Applications from AWX/AnsibleTower.

Implementation of following API:

* `/api/v2/applications/`
* `/api/v2/organizations/{id}/applications/`
* `/api/v2/users/{id}/applications/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-Application
```

### Example 2
```powershell
PS C:\> Find-Application -Type Organization -Id 1
```

Retrieve Applications associated with the Organization of ID `1`.

`Id` and `Type` parameters can also be given from the pipeline, likes following:

    Get-Organization 1 | Find-Application

## PARAMETERS

### -All
Retreive all items after the specified `-Page`.
This may take many HTTP requests to AWX/AnsibleTower.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Count

Max item counts per a `Get` request.
(Range: 1 - 200)

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
Used with `-Type` parameter to retreive items associated with the ID of the target type.

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
Key name list for sortng order. (eg. `id` , `name`, `modified`)

To sort by descending, add `!` prefix. (eg. `!id`)

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
Search keywords.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Used with `-Id` parameter to retreive items associated with the ID of the target type.

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: Organization, User

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
See: `-Id` parameter

### AWX.Resources.ResourceType
See: `-Type` parameter

## OUTPUTS

### AWX.Resources.Application

## NOTES

## RELATED LINKS
