---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Remove-Credential

## SYNOPSIS
Remove a Credential.

## SYNTAX

```
Remove-Credential [-Id] <UInt64> [-From <IResource>] [-Force] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Remove a Credential.

Implements following Rest API:  
- `/api/v2/credentials/{id}/` (DELETE)

## EXAMPLES

### Example 1
```powershell
PS C:\> Remove-Credential -Id 2
```

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

### -From
Target resource to be removed from.

Following resource is available:  
- `Organization` (galaxy-token only)  
- `InventorySource`  
- `JobTemplate`  
- `Schedule`  
- `WorkflowJobTemplateNode`

```yaml
Type: IResource
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Credential ID to be removed.

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
Credential ID.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-Credential](Get-Credential.md)

[Find-Credential](Find-Credential.md)

[New-Credential](New-Credential.md)

[Add-Credential](Add-Credential.md)

[Update-Credential](Update-Credential.md)
