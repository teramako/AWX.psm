---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-WorkflowJobTemplate

## SYNOPSIS
Invoke (update) a WorkflowJobTemplate and wait until the job is finished.

## SYNTAX

### Id
```
Invoke-WorkflowJobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-Id] <UInt64> [-Limit <String>]
 [-Inventory <UInt64>] [-ScmBranch <String>] [-Labels <UInt64[]>] [-Tags <String[]>] [-SkipTags <String[]>]
 [-ExtraVars <String>] [<CommonParameters>]
```

### JobTemplate
```
Invoke-WorkflowJobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog]
 [-WorkflowJobTemplate] <WorkflowJobTemplate> [-Limit <String>] [-Inventory <UInt64>] [-ScmBranch <String>]
 [-Labels <UInt64[]>] [-Tags <String[]>] [-SkipTags <String[]>] [-ExtraVars <String>]
 [<CommonParameters>]
```

## DESCRIPTION
Launch a WorkflowJobTemplate and wait until the job is finished.

Implementation of following API:  
- `/api/v2/workflow_job_templates/{id}/launch/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Invoke-WorkflowJobTemplate -Id 13
[13] TestWorkflow - Workflow Demo
             Inventory : [2] TestInventory
                Labels : [1] test
            Extra vars : ---
====== [111] Test_1 ======

(snip)

====== [112] Demo Job Template ======

(snip)

 Id        Type Name         JobType LaunchType     Status Finished            Elapsed LaunchedBy     Template         Note
 --        ---- ----         ------- ----------     ------ --------            ------- ----------     --------         ----
110 WorkflowJob TestWorkflow             Manual Successful 2024/08/06 16:21:10   4.202 [user][1]admin [13]TestWorkflow {[Labels, test], [Inventory, [2]], [Limit, ], [Branch, ]…}
```

Launch WorkflowJobTemplate ID 13, and wait unti for the job is finished.

## PARAMETERS

### -ExtraVars
Specify extra variables.

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
WorkflowJobTemplate ID to be launched.

```yaml
Type: UInt64
Parameter Sets: Id
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -IntervalSeconds
Interval to confirm job completion (seconds).
Default is 5 seconds.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 5
Accept pipeline input: False
Accept wildcard characters: False
```

### -Inventory
Inventory ID

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Labels
Label IDs

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

```yaml
Type: UInt64[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Limit
Further limit selected hosts to an additional pattern.

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ScmBranch
Specify branch to use in job run. Project default is used if omitted.

> [!NOTE]  
> This parameter will be ignored if the Project's `AllowOverride` flag is on and  "Ask" flag is off, although the request will be sent.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SkipTags
Specify skip tags. (commas `,` separated)

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

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

### -SuppressJobLog
Suppress display job log.

> [!TIP]  
> If you need the job log, use `-InformationVariable` parameter likes following:  
>  
>     PS C:\> Invoke-WorkflowJobTemplate ... -SuppressJobLog -InformationVariable joblog  
>     (snip)  
>     PS C:\> $joblog  
>     ====== [111] Test_1 ======  
>     
>     (snip)  
>     
>     ====== [112] Demo Job Template ======  
>     
>     (snip)

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Tags
Specify tags. (commas `,` separated)

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

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

### -WorkflowJobTemplate
WorkflowJobTempalte object to be launched.

```yaml
Type: WorkflowJobTemplate
Parameter Sets: JobTemplate
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
WorkflowJobTemplate ID to be launched.

### AWX.Resources.WorkflowJobTemplate
WorkflowJobTemplate object to be launched.

## OUTPUTS

### AWX.Resources.WorkflowJob
The result job object of lanched the WorkflowJobTemplate.

## NOTES

## RELATED LINKS

[Start-WorkflowJobTemplate](Start-WorkflowJobTemplate.md)

[Get-WorkflowJob](Get-WorkflowJob.md)

[Find-WorkflowJob](Find-WorkflowJob.md)
