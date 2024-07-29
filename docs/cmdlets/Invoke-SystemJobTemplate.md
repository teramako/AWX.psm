---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-SystemJobTemplate

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Id
```
Invoke-SystemJobTemplate [-Id] <UInt64> [-ExtraVars <IDictionary>] [-IntervalSeconds <Int32>] [-SuppressJobLog]
 [<CommonParameters>]
```

### AsyncId
```
Invoke-SystemJobTemplate [-Id] <UInt64> [-Async] [-ExtraVars <IDictionary>]
 [<CommonParameters>]
```

### Template
```
Invoke-SystemJobTemplate [-SystemJobTemplate] <SystemJobTemplate> [-ExtraVars <IDictionary>]
 [-IntervalSeconds <Int32>] [-SuppressJobLog] [<CommonParameters>]
```

### AsyncTemplate
```
Invoke-SystemJobTemplate [-SystemJobTemplate] <SystemJobTemplate> [-Async] [-ExtraVars <IDictionary>]
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

### -Async
{{ Fill Async Description }}

```yaml
Type: SwitchParameter
Parameter Sets: AsyncId, AsyncTemplate
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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
Parameter Sets: Id, AsyncId
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -IntervalSeconds
{{ Fill IntervalSeconds Description }}

```yaml
Type: Int32
Parameter Sets: Id, Template
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SuppressJobLog
{{ Fill SuppressJobLog Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Id, Template
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SystemJobTemplate
{{ Fill SystemJobTemplate Description }}

```yaml
Type: SystemJobTemplate
Parameter Sets: Template, AsyncTemplate
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

### AWX.Resources.SystemJob
## NOTES

## RELATED LINKS
