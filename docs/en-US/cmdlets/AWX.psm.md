---
Module Name: AWX.psm
Module Guid: a60acf94-7e42-4b77-bf97-1cdaf17b822b
Download Help Link: null
Help Version: 1.1.0
Locale: en-US
---

# AWX.psm Module
## Description
PowerShell module to operate AWX/AnsibleTower using Rest API.

## AWX.psm Cmdlets
### [Add-Label](Add-Label.md)
Add a Label.

### [Approve-WorkflowApprovalRequest](Approve-WorkflowApprovalRequest.md)
Approve requests for WorkflowApproval.

### [Deny-WorkflowApprovalRequest](Deny-WorkflowApprovalRequest.md)
Deny requests for WorkflowApproval.

### [Find-AccessList](Find-AccessList.md)
Retrieve Users accessible to a resource.

### [Find-ActivityStream](Find-ActivityStream.md)
Retrieve ActivityStreams.

### [Find-AdHocCommandJob](Find-AdHocCommandJob.md)
Retrieve jobs for AdHocCommand.

### [Find-Application](Find-Application.md)
Retrieve (OAuth2) Applications.

### [Find-Credential](Find-Credential.md)
Retrieve Credentials.

### [Find-CredentialInputSource](Find-CredentialInputSource.md)
Retrieve CredentialsInputSources.

### [Find-CredentialType](Find-CredentialType.md)
Retrieve CredentialTypes.

### [Find-ExecutionEnvironment](Find-ExecutionEnvironment.md)
Retrieve ExecutionEnvironments.

### [Find-Group](Find-Group.md)
Retrieve Groups.

### [Find-Host](Find-Host.md)
Retrieve Hosts.

### [Find-HostMetric](Find-HostMetric.md)
Retrieve HostMerics.

### [Find-Instance](Find-Instance.md)
Retrieve Instances.

### [Find-InstanceGroup](Find-InstanceGroup.md)
Retrieve InstanceGroups.

### [Find-Inventory](Find-Inventory.md)
Retrieve Inventories.

### [Find-InventorySource](Find-InventorySource.md)
Retrieve InventorySources.

### [Find-InventoryUpdateJob](Find-InventoryUpdateJob.md)
Retrieve jobs for InventoryUpdate.

### [Find-Job](Find-Job.md)
Retrieve jobs for JobTemplate.

### [Find-JobEvent](Find-JobEvent.md)
Retrieve Job Events.

### [Find-JobHostSummary](Find-JobHostSummary.md)
Retrieve JobHostSummaries.

### [Find-JobTemplate](Find-JobTemplate.md)
Retrieve JobTemplates.

### [Find-Label](Find-Label.md)
Retrieve Labels.

### [Find-Notification](Find-Notification.md)
Retrieve Notifications.

### [Find-NotificationTemplate](Find-NotificationTemplate.md)
Retrieve NotificationTemplates.

### [Find-NotificationTemplateForApproval](Find-NotificationTemplateForApproval.md)
Retrieve Approval NotificationTemplates.

### [Find-NotificationTemplateForError](Find-NotificationTemplateForError.md)
Retrieve Error NotificationTemplates.

### [Find-NotificationTemplateForStarted](Find-NotificationTemplateForStarted.md)
Retrieve Started NotificationTemplates.

### [Find-NotificationTemplateForSuccess](Find-NotificationTemplateForSuccess.md)
Retrieve Success NotificationTemplates.

### [Find-ObjectRole](Find-ObjectRole.md)
Retrieve Roles for the target resource.

### [Find-Organization](Find-Organization.md)
Retrieve Organizations.

### [Find-Project](Find-Project.md)
Retrieve Projects.

### [Find-ProjectUpdateJob](Find-ProjectUpdateJob.md)
Retrieve jobs for ProjectUpdate.

### [Find-Role](Find-Role.md)
Retrieve Roles all or granted to the target resource.

### [Find-Schedule](Find-Schedule.md)
Retrieve Schedules.

### [Find-SystemJob](Find-SystemJob.md)
Retrieve jobs for SystemJobTemplate.

### [Find-SystemJobTemplate](Find-SystemJobTemplate.md)
Retrieve SystemJobTemplates.

### [Find-Team](Find-Team.md)
Retrieve Teams.

### [Find-Token](Find-Token.md)
Retrieve (OAuth2) AccessTokens.

### [Find-UnifiedJob](Find-UnifiedJob.md)
Retrieve Unified Jobs.

### [Find-UnifiedJobTemplate](Find-UnifiedJobTemplate.md)
Retrieve Unified Job Templates.

### [Find-User](Find-User.md)
Retrieve Users.

### [Find-WorkflowApprovalRequest](Find-WorkflowApprovalRequest.md)
Retrieve request jobs for WorkflowApproval.

### [Find-WorkflowJob](Find-WorkflowJob.md)
Retrieve jobs for WorkflowJobTemplate.

### [Find-WorkflowJobNode](Find-WorkflowJobNode.md)
Retrieve nodes for WorkflowJob.

### [Find-WorkflowJobNodeFor](Find-WorkflowJobNodeFor.md)
Retrieve WorkflowJobNodes linked to the target WorkflowJobNode.

### [Find-WorkflowJobTemplate](Find-WorkflowJobTemplate.md)
Retrieve WorkflowJobTemplates.

### [Find-WorkflowJobTemplateNode](Find-WorkflowJobTemplateNode.md)
Retrieve WorkflowJobTemplateNodes.

### [Find-WorkflowJobTemplateNodeFor](Find-WorkflowJobTemplateNodeFor.md)
Retrieve WorkflowJobTemplateNodes linked to the target WorkflowJobTemplateNode.

### [Get-ActivityStream](Get-ActivityStream.md)
Retrieves ActivityStreams by ID(s).

### [Get-AdHocCommandJob](Get-AdHocCommandJob.md)
Retrieve AdHocCommand job details by ID(s).

### [Get-ApiConfig](Get-ApiConfig.md)
Get loaded config data currently.

### [Get-ApiHelp](Get-ApiHelp.md)
Get and show Ansible's API Help.

### [Get-Application](Get-Application.md)
Retrieve Applications by the ID(s).

### [Get-Config](Get-Config.md)
Retrieve various sitewide configuration settings.

### [Get-Credential](Get-Credential.md)
Retrieve Credentials by the ID(s).

### [Get-CredentialInputSource](Get-CredentialInputSource.md)
Retrieve CredentialInputSources by the ID(s).

### [Get-CredentialType](Get-CredentialType.md)
Retrieve CredentialTypes by the ID(s).

### [Get-Dashboard](Get-Dashboard.md)
Retrieve Dashboard.

### [Get-ExecutionEnvironment](Get-ExecutionEnvironment.md)
Retrieve ExecutionEnvironments by the ID(s).

### [Get-Group](Get-Group.md)
Retrieve Groups by the ID(s).

### [Get-Host](Get-Host.md)
Retrieve Hosts by the ID(s).

### [Get-HostFactsCache](Get-HostFactsCache.md)
Retrieve Ansible Facts for a Host.

### [Get-HostMetric](Get-HostMetric.md)
Retrieve HostMetrics by the ID(s).

### [Get-Instance](Get-Instance.md)
Retrieve Instances by the ID(s).

### [Get-InstanceGroup](Get-InstanceGroup.md)
Retrieve InstanceGroups by the ID(s).

### [Get-Inventory](Get-Inventory.md)
Retrieve Inventories by the ID(s).

### [Get-InventoryFile](Get-InventoryFile.md)
Retrieve inventory files.

### [Get-InventorySource](Get-InventorySource.md)
Retrieve InventorySources by the ID(s).

### [Get-InventoryUpdateJob](Get-InventoryUpdateJob.md)
Retrieve InventoryUpdate job details by ID(s).

### [Get-Job](Get-Job.md)
Retrieve JobTemplate's job details by ID(s).

### [Get-JobHostSummary](Get-JobHostSummary.md)
Retrieve JobHostSummaries by ID(s).

### [Get-JobLog](Get-JobLog.md)
Retrieve job logs.

### [Get-JobStatistics](Get-JobStatistics.md)
Retrieve statistics for job runs.

### [Get-JobTemplate](Get-JobTemplate.md)
Retrieve JobTemplates by the ID(s).

### [Get-Label](Get-Label.md)
Retrieve Labels by the ID(s).

### [Get-Me](Get-Me.md)
Retrieve the current user.

### [Get-Metric](Get-Metric.md)
Retrieve Metrics.

### [Get-Notification](Get-Notification.md)
Retrieve Notifications by the ID(s).

### [Get-NotificationTemplate](Get-NotificationTemplate.md)
Retrieve NotificationTemplates by the ID(s).

### [Get-Organization](Get-Organization.md)
Retrieve Organizations by the ID(s).

### [Get-Ping](Get-Ping.md)
Retrieve some basic information about the instance.

### [Get-Playbook](Get-Playbook.md)
Retrieve playbooks.

### [Get-Project](Get-Project.md)
Retrieve Projects by the ID(s).

### [Get-ProjectUpdateJob](Get-ProjectUpdateJob.md)
Retrieve ProjectUpdate job details by ID(s).

### [Get-Role](Get-Role.md)
Retrieve Roles by the ID(s).

### [Get-Schedule](Get-Schedule.md)
Retrieve Schedules by the ID(s).

### [Get-Setting](Get-Setting.md)
Retrieve Settings.

### [Get-SurveySpec](Get-SurveySpec.md)
Retrieve Survey specifications.

### [Get-SystemJob](Get-SystemJob.md)
Retrieve SystemJob details by ID(s).

### [Get-SystemJobTemplate](Get-SystemJobTemplate.md)
Retrieve SystemJobTemplates by the ID(s).

### [Get-Team](Get-Team.md)
Retrieve Teams by the ID(s).

### [Get-Token](Get-Token.md)
Retrieve (OAuth2) AccessTokens by the ID(s).

### [Get-User](Get-User.md)
Retrieve Users by the ID(s).

### [Get-VariableData](Get-VariableData.md)
Retrieve Variable Data

### [Get-WorkflowApprovalRequest](Get-WorkflowApprovalRequest.md)
Retrieve request jobs for WorkflowApproval by ID(s).

### [Get-WorkflowApprovalTemplate](Get-WorkflowApprovalTemplate.md)
Retrieve WorkflowApprovalTemplates by the ID(s).

### [Get-WorkflowJob](Get-WorkflowJob.md)
Retrieve WorkflowJob details by ID(s).

### [Get-WorkflowJobNode](Get-WorkflowJobNode.md)
Retrieve nodes for WorkflowJob by ID(s).

### [Get-WorkflowJobTemplate](Get-WorkflowJobTemplate.md)
Retrieve WorkflowJobTemplates by the ID(s).

### [Get-WorkflowJobTemplateNode](Get-WorkflowJobTemplateNode.md)
Retrieve nodes for WorkflowJobTemplate by ID(s).

### [Invoke-AdHocCommand](Invoke-AdHocCommand.md)
Invoke (launch) an AdHocCommand and wait until the job is finished.

### [Invoke-API](Invoke-API.md)
Execute Ansible's (low-level) Rest API.

### [Invoke-InventoryUpdate](Invoke-InventoryUpdate.md)
Invoke (update) an InventorySource and wait until the job is finished.

### [Invoke-JobTemplate](Invoke-JobTemplate.md)
Invoke (launch) a JobTemplate and wait unti the job is finished.

### [Invoke-ProjectUpdate](Invoke-ProjectUpdate.md)
Invoke (update) a Project and wait until the job is finished.

### [Invoke-SystemJobTemplate](Invoke-SystemJobTemplate.md)
Invoke (launch) a SystemJobTemplate and wait unti the job is finished.

### [Invoke-WorkflowJobTemplate](Invoke-WorkflowJobTemplate.md)
Invoke (update) a WorkflowJobTemplate and wait until the job is finished.

### [New-ApiConfig](New-ApiConfig.md)
Create config file that should be used by this module.

### [New-Label](New-Label.md)
Create a Label.

### [New-Team](New-Team.md)
Create a Team.

### [Remove-Label](Remove-Label.md)
Remove a Label.

### [Remove-Team](Remove-Team.md)
Remove a Team

### [Start-AdHocCommand](Start-AdHocCommand.md)
Invoke (launch) an AdHocCommand.

### [Start-InventoryUpdate](Start-InventoryUpdate.md)
Invoke (update) InventorySources.

### [Start-JobTemplate](Start-JobTemplate.md)
Invoke (launch) a JobTemplate.

### [Start-ProjectUpdate](Start-ProjectUpdate.md)
Invoke (update) Project.

### [Start-SystemJobTemplate](Start-SystemJobTemplate.md)
Invoke (launch) a SystemJobTemplate.

### [Start-WorkflowJobTemplate](Start-WorkflowJobTemplate.md)
Invoke (update) a WorkflowJobTemplate.

### [Stop-UnifiedJob](Stop-UnifiedJob.md)
Stop (cancel) a running job.

### [Switch-ApiConfig](Switch-ApiConfig.md)
Switch to anothor config.

### [Update-Label](Update-Label.md)
Update a Label.

### [Update-Team](Update-Team.md)
Update a Team.

### [Wait-UnifiedJob](Wait-UnifiedJob.md)
Wait until jobs are finished.
