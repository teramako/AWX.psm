# AWX.psm
PowerShell module to operate AWX/AnsibleTower using Rest API.

![demo1](docs/img/AWX.psm-demo-1.gif)

## Build

See [Build](./docs/build.md) document.

## Settings

See [Settings](./docs/settings.md) document.

## Available Commands

See [Cmdlet documents directory(en-US)](./docs/en-US/cmdlets/) for more details

| Name                                 | Description  
|:-------------------------------------|:---------------------------------------------------------------------------|  
| `New-ApiConfig`                        | Create a configuration file, which should be the first step in using this module.  
| `Get-ApiConfig`                        | Get loaded config data currently.  
| `Switch-ApiConfig`                     | Switch to anothor config.  
| `Invoke-API`                           | Execute Ansible's (low-level) Rest API.  
| `Get-ApiHelp`                          | Get and show Ansible's API Help.  
| `Find-UnifiedJob`                      | Retrieve Jobs for Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand and WorkflowJob.  
| `Find-UnifiedJobTemplate`              | Retrieve Templates for JobTemplate, Project, InventorySource, SystemJobTemplate and WorkflowJobTemplate.  
| `Get-JobLog`                           | Retrieve a Job Log for Job, ProjectUpdate, InventoryUpdate, SystemJob, AdHocCommand and WorkflowJob.  
| `Find-JobEvent`                        | Retrieve Events for Job, ProjectUpdate, InventoryUpdate, SystemJob and AdHocCommand.  
| `Wait-UnifiedJob`                      | Wait until jobs are finished.  
| `Stop-UnifiedJob`                      | Stop (cancel) a running job.  
| `Get-Job`                              | Retrieve a job detail for JobTemplate.  
| `Find-Job`                             | Retrieve jobs for JobTemplate.  
| `Get-JobTemplate`                      | Retreive a JobTemplate.  
| `Find-JobTemplate`                     | Retrieve JobTemplates.  
| `Invoke-JobTemplate`                   | Invoke (launch) a JobTemplate and wait unti the job is finished.  
| `Start-JobTemplate`                    | Invoke (launch) a JobTemplate.  
| `Get-ProjectUpdateJob`                 | Retrieve a job detail for ProjectUpdate.  
| `Find-ProjectUpdateJob`                | Retrieve jobs for ProjectUpdate.  
| `Get-Project`                          | Retrieve a Project.  
| `Find-Project`                         | Retrieve Projects.  
| `Invoke-ProjectUpdate`                 | Invoke (update) a Project and wait until the job is finished.  
| `Start-ProjectUpdate`                  | Invoke (update) a Project.  
| `Get-InventoryUpdateJob`               | Retrieve a job detail for InventoryUpdate.  
| `Find-InventoryUpdateJob`              | Retrieve jobs for InventoryUpdate.  
| `Get-InventorySource`                  | Retrieve an InventorySource.  
| `Find-InventorySource`                 | Retrieve InventorySources.  
| `Invoke-InventoryUpdate`               | Invoke (update) an InventorySource and wait until the job is finished.  
| `Start-InventoryUpdate`                | Invoke (update) an InventorySource.  
| `Get-WorkflowJob`                      | Retrieve a job detail for WorkflowJobTemplate.  
| `Find-WorkflowJob`                     | Retrieve jobs for WorkflowJobTemplate.  
| `Get-WorkflowJobNode`                  | Retrieve a node for WorkflowJob.  
| `Find-WorkflowJobNode`                 | Retrieve nodes for WorkflowJob.  
| `Find-WorkflowJobNodeFor`              | Retrieve WorkflowJobNodes linked to the target WorkflowJobNode.  
| `Get-WorkflowJobTemplate`              | Retrieve a WorkflowJobTemplate.  
| `Find-WorkflowJobTemplate`             | Retrieve WorkflowJobTemplates.  
| `Get-WorkflowJobTemplateNode`          | Retrieve a WorkflowJobTemplateNode.  
| `Find-WorkflowJobTemplateNode`         | Retrieve WorkflowJobTemplateNodes.  
| `Find-WorkflowJobTemplateNodeFor`      | Retrieve WorkflowJobTemplateNodes linked to the target WorkflowJobTemplateNode.  
| `Invoke-WorkflowJobTemplate`           | Invoke (update) a WorkflowJobTemplate and wait until the job is finished.  
| `Start-WorkflowJobTemplate`            | Invoke (update) a WorkflowJobTemplate.  
| `Get-WorkflowApprovalRequest`          | Retrieve a request job for WorkflowApproval.  
| `Find-WorkflowApprovalRequest`         | Retrieve request jobs for WorkflowApproval.  
| `Get-WorkflowApprovalTemplate`         | Retrieve a WorkflowApprovalTemplate.  
| `Approve-WorkflowApprovalRequest`      | Approve a request for WorkflowApproval.  
| `Deny-WorkflowApprovalRequest`         | Deny a request for WorkflowApproval.  
| `Get-SystemJob`                        | Retrieve a job for SystemJobTemplate.  
| `Find-SystemJob`                       | Retrieve jobs for SystemJobTemplate.  
| `Get-SystemJobTemplate`                | Retrieve a SystemJobTemplate.  
| `Find-SystemJobTemplate`               | Retrieve SystemJobTemplates.  
| `Invoke-SystemJobTemplate`             | Invoke (launch) a SystemJobTemplate and wait until the job is finished.  
| `Start-SystemJobTemplate`              | Invoke (launch) a SystemJobTemplate.  
| `Get-AdHocCommandJob`                  | Retrieve a job for AdHocCommand.  
| `Find-AdHocCommandJob`                 | Retrieve jobs for AdHocCommand.  
| `Invoke-AdHocCommand`                  | Invoke (launch) an AdHocCommand and wait until the job is finished.  
| `Start-AdHocCommand`                   | Invoke (launch) an AdHocCommand.  
| `Get-Application`                      | Retrieve an (OAuth2) Application.  
| `Find-Application`                     | Retrieve (OAuth2) Applications.  
| `Get-Token`                            | Retrieve an (OAuth2) AccessToken.  
| `Find-Token`                           | Retrieve (OAuth2) AccessTokens.  
| `Get-Instance`                         | Retrieve an Instance.  
| `Find-Instance`                        | Retrieve Instances.  
| `Get-InstanceGroup`                    | Retrieve an InstanceGroup.  
| `Find-InstanceGroup`                   | Retrieve InstanceGroups.  
| `Get-Config`                           | Retrieve various sitewide configuration settings.  
| `Get-Ping`                             | Retrieve some basic information about the instance.  
| `Get-Setting`                          | Retrieve settings.  
| `Get-Organization`                     | Retrieve an Organization.  
| `Find-Organization`                    | Retrieve Organizations.  
| `Get-Me`                               | Retrieve the current user.  
| `Get-User`                             | Retrieve a User.  
| `Find-User`                            | Retrieve Users.  
| `Get-Team`                             | Retrieve a Team.  
| `Find-Team`                            | Retrieve Teams.  
| `Get-Credential`                       | Retrieve a Credential.  
| `Find-Credential`                      | Retrieve Credentials.  
| `Get-CredentialType`                   | Retrieve a CredentialType.  
| `Find-CredentialType`                  | Retrieve CredentialTypes.  
| `Get-CredentialInputSource`            | Retrieve a CredentialInputSource.  
| `Find-CredentialInputSource`           | Retrieve CredentialsInputSources.  
| `Get-Inventory`                        | Retrieve an Inventory.  
| `Find-Inventory`                       | Retrieve Inventories.  
| `Get-Group`                            | Retrieve a Group.  
| `Find-Group`                           | Retrieve Groups.  
| `Get-Host`                             | Retrieve a Host.  
| `Find-Host`                            | Retrieve Hosts.  
| `Get-JobHostSummary`                   | Retrieve a JobHostSummary.  
| `Find-JobHostSummary`                  | Retrieve JobHostSummaries for Job, Host or Group.  
| `Get-Schedule`                         | Retrieve a Schedule.  
| `Find-Schedule`                        | Retrieve Schedules.  
| `Get-Role`                             | Retrieve a Role.  
| `Find-Role`                            | Retrieve Roles all or granted to the target resource.  
| `Find-ObjectRole`                      | Retrieve Roles for the target resource.  
| `Get-NotificationTemplate`             | Retrieve a NotificationTemplate.  
| `Find-NotificationTemplate`            | Retrieve NotificationTemplates.  
| `Find-NotificationTemplateForApproval` | Retrieve Approval NotificationTemplates.  
| `Find-NotificationTemplateForStarted`  | Retrieve Started NotificationTemplates.  
| `Find-NotificationTemplateForSuccess`  | Retrieve Success NotificationTemplates.  
| `Find-NotificationTemplateForError`    | Retrieve Error NotificationTemplates.  
| `Get-Notification`                     | Retrieve a Notification.  
| `Find-Notification`                    | Retrieve Notifications.  
| `Get-Label`                            | Retrieve a Label.  
| `Find-Label`                           | Retrieve Labels.  
| `Get-ActivityStream`                   | Retrieve an ActivityStream.  
| `Find-ActivityStream`                  | Retrieve ActivityStreams.  
| `Get-ExecutionEnvironment`             | Retrieve an ExecutionEnvironment.  
| `Find-ExecutionEnvironment`            | Retrieve ExecutionEnvironments.  
| `Get-HostMetric`                       | Retrieve a HostMetric.  
| `Find-HostMetric`                      | Retrieve HostMerics.  
| `Get-Metric`                           | Retrieve Metrics.  

