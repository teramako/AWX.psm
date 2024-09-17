---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Remove-Host

## SYNOPSIS
Remove a Host

## SYNTAX

```
Remove-Host [-Id] <UInt64> [-FromGroup <UInt64>] [-Force] [-WhatIf]
 [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Remove a Host or disassociate from the group.

Implements following Rest API:  
- `/api/v2/hosts/{id}/` (DELETE)  
- `/api/v2/groups/{id}/hosts/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-Host -Id 3 -FromGroup 1
```

Disassociate the Host of ID 3 from the Group ID 1.

### Example 2
```powershell
PS C:\> Remove-Host -Id 3
```

Delete completly the Host of ID 3.

## PARAMETERS

### -Force
Don't confirm. (Ignore `-Confirm` and `-WhatIf`)

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

### -FromGroup
Group ID.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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

[Add-Host](Add-Host.md)

[Update-Host](Update-Host.md)
