---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-JobEvent

## SYNOPSIS
Retrieve Job Events.

## SYNTAX

```
Find-JobEvent [-Type] <ResourceType> [-Id] <UInt64> [-AdHocCommandEvent] [-OrderBy <String[]>]
 [-Search <String[]>] [-Count <UInt16>] [-Page <UInt32>] [-All]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of Events for Job, ProjectUpdate, InventoryUpdate, SystemJob and AdHocCommand.

Implementation of following API:  
- `/api/v2/jobs/{id}/job_events/`  
- `/api/v2/ad_hoc_commands/{id}/events/`  
- `/api/v2/system_jobs/{id}/events/`  
- `/api/v2/project_updates/{id}/events/`  
- `/api/v2/inventory_updates/{id}/events/`  
- `/api/v2/groups/{id}/job_events/`  
- `/api/v2/hosts/{id}/ad_hoc_command_events/`  
- `/api/v2/hosts/{id}/job_events/`  

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-JobEvent -Type Job -Id 10
```

Retrieve Events for JobTemplate job of ID 1

### Example 2
```powershell
PS C:\> Find-JobEvent -Type Host -Id 1 -AdHocCommandEvent
```

Retrieve AdHocCommand (not JobTemplate job) Events associated with Host of ID 1.

## PARAMETERS

### -AdHocCommandEvent
Retrieve AdHocCommand Events instead of JobTemplate's Events.
Only affected for a **Host** Type

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

### -Id
Datebase ID of the target resource.
Use in conjection with the `-Type` parameter.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -OrderBy
Retrieve list in the specified orders.
Use `!` prefix to sort in reverse.
Multiple sorting fields are available by separating with a comma(`,`).

Default value: `counter` (ascending order of `counter`)

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: ["counter"]
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

Target fields: `stdout`

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
Parameter Sets: (All)
Aliases:
Accepted values: Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand, Host, Group

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.ResourceType
Input by `Type` property in the pipeline object.

Acceptable values:  
- `Job`  
- `ProjectUpdate`  
- `InventoryUpdate`  
- `SystemJob`  
- `AdHocCommand`  
- `Host`  
- `Group`  

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.IJobEventBase
JobEvent objects that extend `IJobEventBase` interface.  
- Job             : `AWX.Resources.JobEvent`  
- ProjectUpdate   : `AWX.Resources.ProjectUpdateJobEvent`  
- InventoryUpdate : `AWX.Resources.InventoryUpdateJobEvent`  
- SystemJob       : `AWX.Resources.SystemJobEvent`  
- AdHocCommand    : `AWX.Resources.AdHocCommandJobEvent`  

## NOTES

## RELATED LINKS

[Get-Job](Get-Job.md)

[Find-Job](Find-Job.md)

[Get-ProjectUpdateJob](Get-ProjectUpdateJob.md)

[Find-ProjectUpdateJob](Find-ProjectUpdateJob.md)

[Get-InventoryUpdateJob](Get-InventoryUpdateJob.md)

[Find-InventoryUpdateJob](Find-InventoryUpdateJob.md)

[Get-SystemJob](Get-SystemJob.md)

[Find-SystemJob](Find-SystemJob.md)

[Get-AdHocCommandJob](Get-AdHocCommandJob.md)

[Find-AdHocCommandJob](Find-AdHocCommandJob.md)
