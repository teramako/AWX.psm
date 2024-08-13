---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-JobTemplate

## SYNOPSIS
Invoke (launch) a JobTemplate and wait unti the job is finished.

## SYNTAX

### Id
```
Invoke-JobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-Id] <UInt64> [-JobType <JobType>]
 [-Limit <String>] [<CommonParameters>]
```

### JobTemplate
```
Invoke-JobTemplate [-IntervalSeconds <Int32>] [-SuppressJobLog] [-JobTemplate] <JobTemplate>
 [-JobType <JobType>] [-Limit <String>] [<CommonParameters>]
```

## DESCRIPTION
Launch the specified JobTemplate and wait until the job is finished.

Implementation of following API:  
- `/api/v2/job_templates/{id}/launch/`

## EXAMPLES

### Example 1
```powershell
PS C:\> > Invoke-Jobtemplate -Id 7
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
====== [100] Demo Job Template ======

PLAY [Hello World Sample] ******************************************************

TASK [Gathering Facts] *********************************************************
ok: [localhost]

TASK [Hello Message] ***********************************************************
ok: [localhost] => {
    "msg": "Hello World!"
}

PLAY RECAP *********************************************************************
localhost                  : ok=2    changed=0    unreachable=0    failed=0    skipped=0    rescued=0    ignored=0

 Id Type Name              JobType LaunchType     Status Finished            Elapsed LaunchedBy     Template             Note
 -- ---- ----              ------- ----------     ------ --------            ------- ----------     --------             ----
100 Job Demo Job Template     Run     Manual Successful 2024/08/06 15:19:01   1.983 [user][1]admin [7]Demo Job Template {[Playbook, hello_world.yml], [Artifacts, {}], [Labels, ]}
```

Launch JobTemplate ID 7, and wait unti for the job is finished.

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

### -SuppressJobLog
Suppress display job log.

> [!TIP]  
> If you need the job log, use `-InformationVariable` parameter likes following:  
>  
>     PS C:\> Invoke-JobTemplate ... -SuppressJobLog -InformationVariable joblog  
>     (snip)  
>     PS C:\> $joblog  
>     ====== [100] Demo Job Template ======  
>     
>     PLAY [Hello World Sample] ******************************************************  
>     
>     (snip)  
>     
>     PLAY RECAP *********************************************************************  
>     localhost                  : ok=2    changed=0    unreachable=0    failed=0    skipped=0    rescued=0    ignored=0

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
JobTemplate ID to be launched.

### AWX.Resources.JobTemplate
JobTemplate object to be launched.

## OUTPUTS

### AWX.Resources.JobTemplateJob
The result job object of lanched the JobTemplate.

## NOTES

## RELATED LINKS

[Start-JobTemplate](Start-JobTemplate.md)

[Get-JobTemplate](Get-JobTemplate.md)

[Find-JobTemplate](Find-JobTemplate.md)
