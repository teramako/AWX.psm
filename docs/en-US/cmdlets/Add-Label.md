---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Add-Label

## SYNOPSIS
Add a Label.

## SYNTAX

### Association
```
Add-Label [-To] <IResource> [-Id] <UInt64> [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

### New
```
Add-Label [-To] <IResource> [-Name] <String> [-Organization] <UInt64>
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Add or associate a Label to the target resource.

Implements following Rest API:  
- `/api/v2/inventories/{id}/labels/` (POST)  
- `/api/v2/job_templates/{id}/labels/` (POST)  
- `/api/v2/schedules/{id}/labels/` (POST)  
- `/api/v2/workflow_job_templates/{id}/labels/` (POST)  
- `/api/v2/workflow_job_template_nodes/{id}/labels/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Add-Label -To (Get-Inventory -Id 2) -Id 1
```

Associate the Label of ID 1 to the Inventory of ID 2.

## PARAMETERS

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

### -Id
Label ID.

```yaml
Type: UInt64
Parameter Sets: Association
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Label name

```yaml
Type: String
Parameter Sets: New
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Organization
Organization ID.

```yaml
Type: UInt64
Parameter Sets: New
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -To
Target resource to be associated.

Following resource is available:  
- `Inventory`  
- `JobTemplate`  
- `Schedule`  
- `WorkflowJobTemplate`  
- `WorkflowJobTemplateNode`

```yaml
Type: IResource
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf
Shows what would happen if the cmdlet runs. The cmdlet is not run.

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
Label ID.

## OUTPUTS

### None
## NOTES

## RELATED LINKS
