---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Remove-SurveySpec

## SYNOPSIS
Remove SurveySpecs.

## SYNTAX

```
Remove-SurveySpec [-Template] <IResource> [-Force] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Remove SurveySpecs from the JobTemplate or WorkflowJobTemplate

Implements following Rest API:  
- `/api/v2/job_templates/{id}/survey_spec/` (DELETE)  
- `/api/v2/workflow_job_templates/{id}/survey_spec/` (DELETE)

## EXAMPLES

### Example 1
```powershell
PS C:\> $jt = Get-JobTemplate -Id 10
PS C:\> Remove-SurveySpec $jt
```

## PARAMETERS

### -Force
Ignore confirmination.

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

### -Template
JobTemplate or WorkflowJobTemplate object.

```yaml
Type: IResource
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

### AWX.Resources.IResource
JobTemplate or WorkflowJobTemplate object.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-SurveySpec](Get-SurveySpec.md)

[Register-SurveySpec](Register-SurveySpec.md)
