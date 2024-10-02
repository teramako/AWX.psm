---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-WorkflowApprovalRequest

## SYNOPSIS
Retrieve request jobs for WorkflowApproval by ID(s).

## SYNTAX

```
Get-WorkflowApprovalRequest [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve JobTemplates by the specified ID(s).

Implements following Rest API:  
- `/api/v2/workflow_approvals/{id}/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-WorkflowApprovalRequest -Id 10

Id             Type Name            Status TimedOut Expire Remaining By       Finished            Elapsed LaunchedBy     WorkflowJob      WorkflowJobTemplate ApprovalTemplate
--             ---- ----            ------ -------- ------ --------- --       --------            ------- ----------     -----------      ------------------- ----------------
10 WorkflowApproval Sample-Approval Failed    False                  [1]admin 2024/07/25 15:46:16  54.573 [user][1]admin [20]ApprovedFlow [13]ApprovedFlow    [12]Sample-Approval
```

Retrieve a WorkflowApproval for Database ID 10.

## PARAMETERS

### -Id
List of database IDs for one or more WorkflowApproval.

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

### AWX.Resources.WorkflowApproval+Detail
## NOTES

## RELATED LINKS

[Find-WorkflowApprovalRequest](Find-WorkflowApprovalRequest.md)

[Approve-WorkflowApprovalRequest](Approve-WorkflowApprovalRequest.md)

[Deny-WorkflowApprovalRequest](Deny-WorkflowApprovalRequest.md)

[Remove-WorkflowApprovalRequest](Remove-WorkflowApprovalRequest.md)
