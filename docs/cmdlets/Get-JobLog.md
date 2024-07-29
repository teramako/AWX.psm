---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-JobLog

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### StdOut (Default)
```
Get-JobLog -Id <UInt64> [-Type <ResourceType>] [-Format <JobLogFormat>] [-Dark]
 [<CommonParameters>]
```

### Download
```
Get-JobLog -Id <UInt64> [-Type <ResourceType>] -Download <DirectoryInfo> [-Format <JobLogFormat>] [-Dark]
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

### -Dark
{{ Fill Dark Description }}

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

### -Download
{{ Fill Download Description }}

```yaml
Type: DirectoryInfo
Parameter Sets: Download
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Format
{{ Fill Format Description }}

```yaml
Type: JobLogFormat
Parameter Sets: (All)
Aliases:
Accepted values: txt, ansi, json, html

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
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Type
{{ Fill Type Description }}

```yaml
Type: ResourceType
Parameter Sets: (All)
Aliases:
Accepted values: Job, ProjectUpdate, InventoryUpdate, SystemJob, WorkflowJob, AdHocCommand

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
### AWX.Resources.ResourceType
## OUTPUTS

### System.String
### System.IO.FileInfo
## NOTES

## RELATED LINKS
