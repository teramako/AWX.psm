---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Update-Token

## SYNOPSIS
Update an AccessToken.

## SYNTAX

```
Update-Token [-Id] <UInt64> [-Description <String>] [-Scope <String>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Update an AccessToken. 

Implements following Rest API:  
- `/api/v2/tokens/{id}/` (PATCH)

## EXAMPLES

### Example 1
```powershell
PS C:\> Update-Token -Id 3 -Scope read -Description "Read only token"
```

## PARAMETERS

### -Description
Optional description of the AccessToken.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
AccessToken ID to be updated.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Scope
Allowed scopes, further restricts user's permission.

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: read, write

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
AccessToken ID.

## OUTPUTS

### AWX.Resources.OAuth2AccessToken
Updated AccessToken object.

## NOTES

## RELATED LINKS

[Get-Token](Get-Token.md)

[Find-Token](Find-Token.md)

[New-Token](New-Token.md)

[Remove-Token](Remove-Token.md)
