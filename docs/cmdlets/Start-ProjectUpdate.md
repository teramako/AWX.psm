---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Start-ProjectUpdate

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Id
```
Start-ProjectUpdate [-Id] <UInt64> [<CommonParameters>]
```

### CheckId
```
Start-ProjectUpdate [-Id] <UInt64> [-Check] [<CommonParameters>]
```

### Project
```
Start-ProjectUpdate [-Project] <Project> [<CommonParameters>]
```

### CheckProject
```
Start-ProjectUpdate [-Project] <Project> [-Check] [<CommonParameters>]
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

### -Check
{{ Fill Check Description }}

```yaml
Type: SwitchParameter
Parameter Sets: CheckId, CheckProject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
{{ Fill Id Description }}

```yaml
Type: UInt64
Parameter Sets: Id, CheckId
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Project
{{ Fill Project Description }}

```yaml
Type: Project
Parameter Sets: Project, CheckProject
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
### AWX.Resources.Project
## OUTPUTS

### AWX.Resources.ProjectUpdateJob+Detail
### System.Management.Automation.PSObject
## NOTES

## RELATED LINKS
