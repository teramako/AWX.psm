---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-JobLog

## SYNOPSIS
Retrieve job logs.

## SYNTAX

### StdOut (Default)
```
Get-JobLog -Id <UInt64> [-Type <ResourceType>] [-Format <JobLogFormat>] [-Dark]
 [<CommonParameters>]
```

### Download
```
Get-JobLog -Id <UInt64> [-Type <ResourceType>] -Download <DirectoryInfo> [-Format <JobLogFormat>] [-Dark]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieve job logs for UnifiedJob(s) and output to STDOUT or download to files for each jobs.
The job's information is added to the file when downloading.

You can choose log format with `-Format` parameter:  
- `txt` : Plain text (default)  
- `ansi`: Plain text with ANSI color (need Terminal supported VT100 escape sequence)  
- `html`: HTML format  
- `json`: JSON format (only affected when downloding. otherwise same as `ansi`)

"UnifiedJob(s)" referes to following jobs:  
- `Job`             : JobTempalte's job  
- `ProjectUpdate`   : Project's update job  
- `InventoryUpdate` : InventorySource's update job  
- `AdHocCommand`    : AdHocCommand's job  
- `WorkflowJob`     : WorkflowJobTemplate's job  
- `SystemJob`       : SystemJobTemplate's job

When the specified job is a WorkflowJob, retrieve job logs for each of its nodes.

Implements following Rest API:  
- `/api/v2/jobs/{id}/stdout/`  
- `/api/v2/project_updates/{id}/stdout/`  
- `/api/v2/inventory_updates/{id}/stdout/`  
- `/api/v2/ad_hoc_commands/{id}/stdout/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-JobLog -Type Job -Id 10
==> [10] Job

PLAY [Hello World Sample] ******************************************************

TASK [Gathering Facts] *********************************************************
ok: [localhost]

TASK [Hello Message] ***********************************************************
ok: [localhost] => {
    "msg": "Hello World!"
}

PLAY RECAP *********************************************************************
localhost                  : ok=2    changed=0    unreachable=0    failed=0    skipped=0    rescued=0    ignored=0
```

Show the log for Job of ID 10 as text format.

### Example 2
```powershell
PS C:\> Get-JobLog -Type Job -Id 10 -Format html -Download .

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---          2024/08/05    14:20           4081 10.html
```

Download log to the current directory as HTML format.

### Example 3
```powershell
PS C:\> Find-Job -Status successful,failed -Count 3 | Get-JobLog -Format ansi
==> [13] Job

(snip)

==> [12] Job

(snip)

==> [11] Job

(snip)
```

Retrieve the three jobs finished as successfull or failed, and show the their logs.

## PARAMETERS

### -Dark
Get log with darck(black) background color.
This would only affect HTML format.

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

### -Download
Indicates download to files and download to the argument value's directory.

The file name format: `<job-ID>.<format>`. eg.) 10.txt, 10.ansi, 10.html, 10.json

The only exception is SystemJob, which can only be downloaded as TEXT format.

```yaml
Type: DirectoryInfo
Parameter Sets: Download
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Format
Log format:

- `txt` : Plain text (default)  
- `ansi`: Plain text with ANSI color (need Terminal supported VT100 escape sequence)  
- `html`: HTML format  
- `json`: JSON format (only affected when downloding. otherwise same as `ansi`)  

The only exception is SystemJob, which can only be downloaded as text format.

```yaml
Type: JobLogFormat
Parameter Sets: (All)
Aliases:
Accepted values: txt, ansi, json, html

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Id
Database ID for UnifiedJob.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Type
Resource type name of the target UnifiedJob.

```yaml
Type: ResourceType
Parameter Sets: (All)
Aliases:
Accepted values: Job, ProjectUpdate, InventoryUpdate, SystemJob, WorkflowJob, AdHocCommand

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64
Input by `Id` property in the pipeline object.
Database ID for the UnifiedJob.

### AWX.Resources.ResourceType
Input by `Type` property in the pipeline object.

Acceptable values:  
- `Job`  
- `ProjectUpdate`  
- `InventoryUpdate`  
- `SystemJob`  
- `WorkflowJob`  
- `AdHocCommand`  

## OUTPUTS

### System.String
Job log string. (when not doanloading)

### System.IO.FileInfo
Downloaded file objects.

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

[Get-WorkflowJob](Get-WorkflowJob.md)

[Find-WorkflowJob](Find-WorkflowJob.md)

[Get-AdHocCommandJob](Get-AdHocCommandJob.md)

[Find-AdHocCommandJob](Find-AdHocCommandJob.md)

[Invoke-JobTemplate](Invoke-JobTemplate.md)

[Invoke-ProjectUpdate](Invoke-ProjectUpdate.md)

[Invoke-InventorySource](Invoke-InventoryUpdate.md)

[Invoke-SystemJobTemplate](Invoke-SystemJobTemplate.md)

[Invoke-WorkflowJobTemplate](Invoke-WorkflowJobTemplate.md)

[Wait-UnifiedJob](Wait-UnifiedJob.md)
