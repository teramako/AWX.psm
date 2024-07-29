---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Wait-UnifiedJob

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

```
Wait-UnifiedJob [-Type] <ResourceType> [-Id] <UInt64> [-IntervalSeconds <Int32>] [-SuppressJobLog]
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
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### -Type
{{ Fill Type Description }}

```yaml
Type: ResourceType
Parameter Sets: (All)
Aliases:
Accepted values: Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand, WorkflowJob

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.ResourceType
### System.UInt64
## OUTPUTS

### AWX.Resources.JobTemplateJob
### AWX.Resources.ProjectUpdateJob
### AWX.Resources.InventoryUpdateJob
### AWX.Resources.SystemJob
### AWX.Resources.AdHocCommand
### AWX.Resources.WorkflowJob
## NOTES

## RELATED LINKS
