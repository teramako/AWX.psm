---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Update-Label

## SYNOPSIS
Update a Label.

## SYNTAX

```
Update-Label [-Id] <UInt64> [-Name <String>] [-Organization <UInt64>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Update a Label

Implements following Rest API:  
- `/api/v2/labels/{id}/` (PATCH)

## EXAMPLES

### Example 1
```powershell
PS C:\> Update-Label -Id 1 -Name FixedName

Id  Type Name      Modified            Organization
--  ---- ----      --------            ------------
 1 Label FixedName 09/10/2024 21:50:24 [2]TestOrg
```

## PARAMETERS

### -Id
Target Label ID.

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

### -Name
Label name.

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

### -Organization
Organization ID.

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
Target Label ID.

## OUTPUTS

### AWX.Resources.Label
Updated Label object.

## NOTES

## RELATED LINKS

[Get-Label](Get-Label.md)

[Find-Label](Find-Label.md)

[New-Label](New-Label.md)

[Register-Label](Register-Label.md)

[Unregister-Label](Unregister-Label.md)
