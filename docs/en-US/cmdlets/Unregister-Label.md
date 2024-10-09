---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Unregister-Label

## SYNOPSIS
Unregister a Label from other resource.

## SYNTAX

```
Unregister-Label [-Id] <UInt64> [-From] <IResource> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Unregister a Label from the target resource.

Implements following Rest API:  
- `/api/v2/inventories/{id}/labels/` (POST)  
- `/api/v2/job_templates/{id}/labels/` (POST)  
- `/api/v2/schedules/{id}/labels/` (POST)  
- `/api/v2/workflow_job_templates/{id}/labels/` (POST)  
- `/api/v2/workflow_job_template_nodes/{id}/labels/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> Unregister-Label -From (Get-Inventory -Id 2) -Id 1
```

Disassociate the Label of ID 1 from the Inventory of Id 2.

## PARAMETERS

### -From
Target resource to be unregistered from.

```yaml
Type: IResource
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Label ID to be unregistered.

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
Label ID.

## OUTPUTS

### System.Boolean
Success or Failure

## NOTES

## RELATED LINKS

[Get-Label](Get-Label.md)

[Find-Label](Find-Label.md)

[New-Label](New-Label.md)

[Register-Label](Register-Label.md)

[Update-Label](Update-Label.md)
