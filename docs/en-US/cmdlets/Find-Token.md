---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-Token

## SYNOPSIS
Retrieve (OAuth2) AccessTokens.

## SYNTAX

### All (Default)
```
Find-Token [-TokenType <ETokenType>] [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>]
 [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-Token -Type <ResourceType> -Id <UInt64> [-TokenType <ETokenType>] [-OrderBy <String[]>]
 [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of OAuth2 Access Tokens.

Implementation of following API:  
- `/api/v2/tokens/`  
- `/api/v2/applications/{id}/tokens/`  
- `/api/v2/users/{id}/tokens/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-Token
```

### Example 2
```powershell
PS C:\> Find-Token -Type User -Id 1
```

Retrieve AccessTokens associated with the User of ID 1

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

### -TokenType
Filter by whether the token type is for Application or for Personal.

```yaml
Type: ETokenType
Parameter Sets: (All)
Aliases:
Accepted values: Both, Personal, Authorized

Required: False
Position: Named
Default value: "Both"
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
Accepted values: OAuth2Application, User

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
- `OAuth2Application`  
- `User`  

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.OAuth2AccessToken
## NOTES

## RELATED LINKS

[Get-Token](Get-Token.md)
