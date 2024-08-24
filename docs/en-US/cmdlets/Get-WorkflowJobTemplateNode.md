---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-WorkflowJobTemplateNode

## SYNOPSIS
Retrieve nodes for WorkflowJobTemplate by ID(s).

## SYNTAX

```
Get-WorkflowJobTemplateNode [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve nodes for WorkflowJobTemplate by specified ID(s).

Implementation of following API:  
- `/api/v2/workflow_job_template_nodes/{id}/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WorkflowJobTemplateNode -Id 1

Id                    Type WorkflowJobTemplate Template ID Template Type Template Name SuccessNodes FailureNodes AlwaysNodes
--                    ---- ------------------- ----------- ------------- ------------- ------------ ------------ -----------
 1 WorkflowJobTemplateNode [13]TestWorkflow              9           Job Test_1        {}           {}           {4}
```

Retrieve a node in WorkflowJob for Database ID 1.

## PARAMETERS

### -Id
List of database IDs for one or more nodes in WorkflowJobTemplate.

```yaml
Type: UInt64[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64[]
One or more database IDs.

## OUTPUTS

### AWX.Resources.WorkflowJobTemplateNode
## NOTES

## RELATED LINKS

[Find-WorkflowJobTemplateNode](Find-WorkflowJobTemplateNode.md)
