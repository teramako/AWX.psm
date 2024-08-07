---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-Organization

## SYNOPSIS
Retrieve Organizations.

## SYNTAX

### All (Default)
```
Find-Organization [[-Name] <String[]>] [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>]
 [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-Organization -Type <ResourceType> -Id <UInt64> [-Admin] [[-Name] <String[]>] [-OrderBy <String[]>]
 [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of Organizations.

Implementation of following API:  
- `/api/v2/organizations/`  
- `/api/v2/users/{id}/organizations/`  
- `/api/v2/users/{id}/admin_of_organizations/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-Organization
```

### Example 2
```powershell
PS C:\> Find-Organization -Type User -Id 1
```

Retrieve Organizations which the User of ID 1 belong to.

### Example 3
```powershell
PS C:\> Find-JobTemplate -Type User -Id 1 -Admin
```

Retrieve Organizations administrered by the User of ID 1.

## PARAMETERS

### -Admin
Filter to Organizations administerted by the target User.

```yaml
Type: SwitchParameter
Parameter Sets: AssociatedWith
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

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
Filter by Organization name.
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
Accepted values: User

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

Acceptable values: `User` (only)

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for `User`

## OUTPUTS

### AWX.Resources.Organization
## NOTES

## RELATED LINKS

[Get-Organization](Get-Organization.md)
