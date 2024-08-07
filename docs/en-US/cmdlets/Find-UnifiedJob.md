---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-UnifiedJob

## SYNOPSIS
Retrieve Unified Jobs.

## SYNTAX

```
Find-UnifiedJob [-OrderBy <String[]>] [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve Jobs which are Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand or WorkflowJob.

Implementation of following API:  
- `/api/v2/unified_jobs/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-UnifiedJob
```

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

### -OrderBy
Retrieve list in the specified orders.
Use `!` prefix to sort in reverse.
Multiple sorting fields are available by separating with a comma(`,`).

Default value: `!id` (descending order of ID)

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ["!id"]
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

Target fields: `name`, `description`, `job__playbook`

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
## OUTPUTS

### AWX.Resources.IUnifiedJob
Unified Job objects which are following instances implemented `IUnifiedJob`:  
- `Job`             : JobTemplate's job  
- `ProjectUpdate`   : Project Update job  
- `InventoryUpdate` : Inventory Update job  
- `SystemJob`       : SystemJobTemplate's job  
- `AdHocCommand`    : AdHocCommand job  
- `WorkflowJob`     : WorkflowJobTemplate's job  

## NOTES

## RELATED LINKS

[Find-Job](Find-Job.md)

[Find-ProjectUpdateJob](Find-ProjectUpdateJob.md)

[Find-InventoryUpdateJob](Find-InventoryUpdateJob.md)

[Find-SystemJob](Find-SystemJob.md)

[Find-AdHocCommandJob](Find-AdHocCommandJob.md)

[Find-WorkflowJob](Find-WorkflowJob.md)

[Find-UnifiedJobTemplate](Find-UnifiedJobTemplate.md)
