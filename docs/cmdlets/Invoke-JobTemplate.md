---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-JobTemplate

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Id
```
Invoke-JobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-Id] <UInt64> [-Limit <String>]
 [<CommonParameters>]
```

### JobTemplate
```
Invoke-JobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-JobTemplate] <JobTemplate> [-Limit <String>]
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

### -IntervalSeconds
{{ Fill IntervalSeconds Description }}

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -JobTemplate
{{ Fill JobTemplate Description }}

```yaml
Type: JobTemplate
Parameter Sets: JobTemplate
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Limit
{{ Fill Limit Description }}

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

### -SuppressJobLog
{{ Fill SuppressJobLog Description }}

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
### AWX.Resources.JobTemplate
## OUTPUTS

### AWX.Resources.JobTemplateJob
## NOTES

## RELATED LINKS
