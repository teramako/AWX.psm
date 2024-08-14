---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Start-JobTemplate

## SYNOPSIS
Invoke (launch) a JobTemplate.

## SYNTAX

### Id
```
Start-JobTemplate [-Id] <UInt64> [-Inventory <UInt64>] [-JobType <JobType>] [-ScmBranch <String>]
 [-Limit <String>] [-Labels <UInt64[]>] [-Tags <String[]>] [-SkipTags <String[]>]
 [<CommonParameters>]
```

### JobTemplate
```
Start-JobTemplate [-JobTemplate] <JobTemplate> [-Inventory <UInt64>] [-JobType <JobType>] [-ScmBranch <String>]
 [-Limit <String>] [-Labels <UInt64[]>] [-Tags <String[]>] [-SkipTags <String[]>]
 [<CommonParameters>]
```

## DESCRIPTION
Launch a JobTemplate.

This command only sends a request to start JobTemplate, not wait for the job is completed.
So, the returned job object will be non-completed status.
Use `Wait-UnifiedJob` command to wait for the job to complete later.

Implementation of following API:  
- `/api/v2/job_templates/{id}/launch/`

## EXAMPLES

### Example 1
```powershell
PS C:\> > Start-Jobtemplate -Id 7

 Id Type Name              JobType LaunchType  Status Finished            Elapsed LaunchedBy     Template             Note
 -- ---- ----              ------- ----------  ------ --------            ------- ----------     --------             ----
101 Job Demo Job Template     Run     Manual  Pending 2024/08/06 15:19:01   1.983 [user][1]admin [7]Demo Job Template {[Playbook, hello_world.yml], [Artifacts, {}], [Labels, ]}
```

Launch JobTemplate ID 7.

### Example 2
```powershell
PS C:\> > Start-Jobtemplate -Id 7 | Wait-UnifiedJob
[7] Demo Job Template -
             Inventory : [1] Demo Inventory
            Extra vars : ---
             Diff Mode : False
              Job Type : Run
             Verbosity : 0 (Normal)
           Credentials : [1] Demo Credential
                 Forks : 0
       Job Slice Count : 1
               Timeout : 0
====== [102] Demo Job Template ======

PLAY [Hello World Sample] ******************************************************

TASK [Gathering Facts] *********************************************************
ok: [localhost]

TASK [Hello Message] ***********************************************************
ok: [localhost] => {
    "msg": "Hello World!"
}

PLAY RECAP *********************************************************************
localhost                  : ok=2    changed=0    unreachable=0    failed=0    skipped=0    rescued=0    ignored=0

 Id Type Name              JobType LaunchType     Status Finished           Elapsed LaunchedBy     Template             Note
 -- ---- ----              ------- ----------     ------ --------           ------- ----------     --------             ----
102 Job Demo Job Template     Run     Manual Successful 2024/08/07 9:53:56   1.968 [user][1]admin [7]Demo Job Template {[Playbook, hello_world.yml], [Artifacts, {}], [Labels, ]}
```

Launch JobTemplate ID 7, and wait unti for the job is finished.
This is almost same as `Invoke-JobTemplate` command.

## PARAMETERS

### -Id
JobTemplate ID to be launched.

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

### -JobTemplate
JobTempalte object to be launched.

```yaml
Type: JobTemplate
Parameter Sets: JobTemplate
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -JobType
Specify JobType ("Run" or "Check")

> [!NOTE]  
> This parameter will be ignored if "Ask" flag is off, although the request will be sent.

```yaml
Type: JobType
Parameter Sets: (All)
Aliases:
Accepted values: Run, Check

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
JobTemplate ID to be launched.

### AWX.Resources.JobTemplate
JobTemplate object to be launched.

## OUTPUTS

### AWX.Resources.JobTemplateJob+LaunchResult
The result job object of lanched the JobTemplate (non-completed status)

## NOTES

## RELATED LINKS

[Invoke-JobTemplate](Invoke-JobTemplate.md)

[Get-JobTemplate](Get-JobTemplate.md)

[Find-JobTemplate](Find-JobTemplate.md)

[Wait-UnifiedJob](Wait-UnifiedJob.md)
