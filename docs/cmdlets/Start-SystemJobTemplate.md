---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Start-SystemJobTemplate

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Id
```
Start-SystemJobTemplate [-Id] <UInt64> [-ExtraVars <IDictionary>]
 [<CommonParameters>]
```

### Template
```
Start-SystemJobTemplate [-SystemJobTemplate] <SystemJobTemplate> [-ExtraVars <IDictionary>]
 [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -ExtraVars
{{ Fill ExtraVars Description }}

```yaml
Type: IDictionary
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
{{ Fill Id Description }}

```yaml
Type: UInt64
Parameter Sets: Id
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SystemJobTemplate
{{ Fill SystemJobTemplate Description }}

```yaml
Type: SystemJobTemplate
Parameter Sets: Template
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64

### AWX.Resources.SystemJobTemplate

## OUTPUTS

### AWX.Resources.SystemJob+Detail

## NOTES

## RELATED LINKS
