---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Unregister-Group

## SYNOPSIS
Unregister a Group from parent Group.

## SYNTAX

```
Unregister-Group [-Id] <UInt64> [-From] <UInt64> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Unregister a Group from the parent Group.

Implements following Rest API:  
- `/api/v2/groups/{id}/children/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Unregister-Group -Id 3 -From 1
```

Disassociate the Group of ID 3 from the parent Group ID 1.

## PARAMETERS

### -From
Parent Group ID.

> [!NOTE]  
> Can specify `IResource` object.  
> For example: `-From (Get-Group -Id 10)`, `-From @{ type="group"; id = 10 }`

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

### -Id
Group Id.

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
Group Id.

## OUTPUTS

### System.Boolean
Success or Failure

## NOTES

## RELATED LINKS

[Get-Group](Get-Group.md)

[Find-Group](Find-Group.md)

[New-Group](New-Group.md)

[Register-Group](Register-Group.md)

[Update-Group](Update-Group.md)
