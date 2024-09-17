---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Add-Host

## SYNOPSIS
Associate a Host to a Group.

## SYNTAX

```
Add-Host [-Id] <UInt64> [-ToGroup] <UInt64> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Associate a User to a Group.

Implements following Rest API:  
- `/api/v2/groups/{id}/hosts/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-Host -Id 3 -ToGroup 1
```

Associate the Host of ID 3 to the Group of ID 1.

## PARAMETERS

### -Id
Host Id.

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

### -ToGroup
Group Id.

```yaml
Type: UInt64
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
Host Id.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-Host](Get-Host.md)

[Find-Host](Find-Host.md)

[New-Host](New-Host.md)

[Update-Host](Update-Host.md)

[Remove-Host](Remove-Host.md)
