---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Register-WorkflowJobTemplateNode

## SYNOPSIS
Link WorkflowJobTemplateNode to the parent.

## SYNTAX

```
Register-WorkflowJobTemplateNode [-Id] <UInt64> -To <UInt64> [-RunUpon <String>] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## DESCRIPTION
Link WorkflowJobTemplateNode to the parent.

Implements following Rest API:  
- `/api/v2/workflow_job_template_nodes/{id}/success_nodes/` (POST)  
- `/api/v2/workflow_job_template_nodes/{id}/failure_nodes/` (POST)  
- `/api/v2/workflow_job_template_nodes/{id}/always_nodes/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Register-WorkflowJobTemplateNode -Id 2 -To 1 -RunUpon always
```

## PARAMETERS

### -Id
WorkflowJobTemplateNode ID to be child.

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

### -RunUpon
Specifies which state ("success", "failure", or "always") the job on the parent node should run in.

```yaml
Type: String
Parameter Sets: (All)
Aliases: Upon
Accepted values: success, failure, always

Required: False
Position: Named
Default value: success
Accept pipeline input: False
Accept wildcard characters: False
```

### -To
WorkflowJobTemplateNode ID to be parent.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
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
WorkflowJobTemplateNode ID to be child.

## OUTPUTS

### None
## NOTES

## RELATED LINKS

[Get-WorkflowJobTemplateNode](Get-WorkflowJobTemplateNode.md)

[Find-WorkflowJobTemplateNode](Find-WorkflowJobTemplateNode.md)

[New-WorkflowJobTemplateNode](New-WorkflowJobTemplateNode.md)

[Update-WorkflowJobTemplateNode](Update-WorkflowJobTemplateNode.md)

[Remove-WorkflowJobTemplateNode](Remove-WorkflowJobTemplateNode.md)

[Unregister-WorkflowJobTemplateNode](Unregister-WorkflowJobTemplateNode.md)
