---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Find-JobHostSummary

## SYNOPSIS
Retrieve JobHostSummaries.

## SYNTAX

```
Find-JobHostSummary [-Type] <ResourceType> [-Id] <UInt64> [-OrderBy <String[]>] [-Search <String[]>]
 [-Filter <NameValueCollection>] [-Count <UInt16>] [-Page <UInt32>] [-All] [<CommonParameters>]
```

## DESCRIPTION
Retrieve the list of JobHostSummaries for Job, Host or Group.

Implementation of following API:  
- `/api/v2/jobs/{id}/job_host_summaries`  
- `/api/v2/groups/{id}/job_host_summaries/`  
- `/api/v2/hosts/{id}/job_host_summaries/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Find-JobHostSummary -Type Job -Id 10
```

Retrieve JobHostSummaries associated with the Job of ID 10

### Example 2
```powershell
PS C:\> Get-Host -Id 1 | Find-JobHostSummary
```

Retrieve JobHostSummaries associated with the Host of ID 1

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

Target fields: `host_name`

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
Accepted values: Job, Host, Group

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
- `Host`  
- `Group`

### System.UInt64
Input by `Id` property in the pipeline object.

Database ID for the ResourceType

## OUTPUTS

### AWX.Resources.JobHostSummary
## NOTES

## RELATED LINKS

[Get-Job](Get-Job.md)

[Find-Job](Find-Job.md)

[Get-Host](Get-Host.md)

[Find-Host](Find-Host.md)

[Get-Group](Get-Group.md)

[Find-Group](Find-Group.md)
