---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Revoke-Role

## SYNOPSIS
Revoke Roles.

## SYNTAX

```
Revoke-Role [-Roles] <IResource[]> [-From] <IResource> [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Revoke Roles from a User or Team.

Implements following Rest API:  
- `/api/v2/users/{id}/roles/` (POST)  
- `/api/v2/teams/{id}/roles/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> $grantedRoles = Find-Role -Type User -Id 2
PS C:\> $user = Get-User -Id 2
PS C:\> Revoke-Role -Roles ($grantedRoles | Where Name -match admin) -From $user
```

Revoke all roles that contain "admin" in the role name from the user of ID 2.

## PARAMETERS

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

### -From
Target resource (`User` or `Team`) to be revoked from.

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

### -Roles
Target role objects to be revoked.

```yaml
Type: IResource[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
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

### AWX.Resources.IResource[]
Role objects to be revoked.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-Role](Get-Role.md)

[Find-Role](Find-Role.md)

[Find-ObjectRole](Find-ObjectRole.md)

[Grant-Role](Grant-Roke.md)
