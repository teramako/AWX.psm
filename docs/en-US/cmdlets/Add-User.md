---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Add-User

## SYNOPSIS
Associate a Uesr to.

## SYNTAX

```
Add-User [-Id] <UInt64> [-To] <IResource> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Associate a User to the target resource.

Implements following Rest API:  
- `/api/v2/organizations/{id}/users/` (POST)  
- `/api/v2/teams/{id}/users/` (POST)  
- `/api/v2/roles/{id}/users/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-User -Id 2 -To (Get-Organization -Id 1)
```

Associate the User of ID 2 to the Organization of ID 1.

## PARAMETERS

### -Id
User ID to be assocated.

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

### -To
Target resource to be associated to.

Following resource is available:  
- `Organization`  
- `Team`  
- `Role`  

```yaml
Type: IResource
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
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
User ID.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-User](Get-User.md)

[Find-User](Find-User.md)

[New-User](New-User.md)

[Update-User](Update-User.md)

[Remove-User](Remove-User.md)
