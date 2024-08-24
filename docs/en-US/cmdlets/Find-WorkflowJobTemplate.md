---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-WorkflowJobTemplate

## SYNOPSIS
Retrieve WorkflowJobTemplates.

## SYNTAX

### All (Default)
```
Find-WorkflowJobTemplate [-OrderBy <String[]>] [-Search <String[]>] [-Filter <NameValueCollection>]
 [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

### AssociatedWith
```
Find-WorkflowJobTemplate [-Type <ResourceType>] -Id <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Filter <NameValueCollection>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of WorkflowJobTemplates.

Implementation of following API:  
- `/api/v2/workflow_job_templates/`  
- `/api/v2/organizations/{id}/workflow_job_templates/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-WorkflowJobTemplate
```

### Example 2
```powershell
PS C:\> Find-WorkflowJobTemplate -Type Organization -Id 1
```

Retrieve WorkflowJobTemplates associated with the Organization of ID 1

`Id` and `Type` parameters can also be given from the pipeline, likes following:  
    Get-Organization -Id 1 | Find-WorkflowJobTemplate

and also can omit `-Type` parameter:  
    Find-WorkflowJobTemplate -Id 1

## PARAMETERS

### -All
Retrieve resources from all pages.

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

### -Count
Number to retrieve per page.

```yaml
Type: UInt16
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 20
Accept pipeline input: False
Accept wildcard characters: False
```

### -Filter
Filtering various fields.

For examples:  
- `name__icontains=test`: "name" field contains "test" (case-insensitive).  
- `"name_ in=test,demo", created _gt=2024-01-01`: "name" field is "test" or "demo" and created after 2024-01-01.  
- `@{ Name = "name"; Value = "test"; Type = "Contains"; Not = $true }`: "name" field NOT contains "test"

For more details, see [about_AWX.psm_Filter_parameter](about_AWX.psm_Filter_parameter.md).

```yaml
Type: NameValueCollection
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Datebase ID of the target resource.
Use in conjection with the `-Type` parameter.

```yaml
Type: UInt64
Parameter Sets: AssociatedWith
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -OrderBy
Retrieve list in the specified orders.
Use `!` prefix to sort in reverse.
Multiple sorting fields are available by separating with a comma(`,`).

Default value: `id` (ascending order of ID)

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ["id"]
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
Page number.

```yaml
Type: UInt32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -Search
Search words. (case-insensitive)

Target fields: `name`, `description`

Multiple words are available by separating with a comma(`,`).

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Resource type name of the target.
Use in conjection with the `-Id` parameter.

```yaml
Type: ResourceType
Parameter Sets: AssociatedWith
Aliases:
Accepted values: Organization

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.ResourceType
Input by `Type` property in the pipeline object.

Acceptable values: `Organization` (only)

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for `Organization`

## OUTPUTS

### AWX.Resources.WorkflowJobTemplate
## NOTES

## RELATED LINKS

[Get-WorkflowJobTemplate](Get-WorkflowJobTemplate.md)

[Find-UnifiedJobTemplate](Find-UnifiedJobTemplate.md)

[Get-WorkflowJobTemplateNode](Get-WorkflowJobTemplateNode.md)

[Find-WorkflowJobTemplateNode](Find-WorkflowJobTemplateNode.md)

[Invoke-WorkflowJobTemplate](Invoke-WorkflowJobTemplate.md)

[Start-WorkflowJobTemplate](Start-WorkflowJobTemplate.md)

[Get-WorkflowJob](Get-WorkflowJob.md)

[Find-WorkflowJob](Find-WorkflowJob.md)
