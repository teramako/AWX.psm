---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-HostFactsCache

## SYNOPSIS
Retrieve Ansible Facts for a Host.

## SYNTAX

```
Get-HostFactsCache [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve Ansible Facts for a Host.

Implements following Rest API:  
- `/api/v2/hosts/{id}/ansible_facts/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-HostFactsCache -Id 1

Key                                  Value
---                                  -----
ansible_dns                          {[search, System.Object[]], [options, System.Collections.Generic.Dictionary`2[System.String,System.Object]], [nameservers, System.Object[]]}
ansible_env                          {[_, /usr/bin/python3], [PWD, /runner/project], [HOME, /root], [PATH, /usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin]…}
ansible_lsb                          {}
ansible_lvm                          N/A
ansible_fips                         False
ansible_fqdn                         awx_1
module_setup                         True
ansible_local                        {}
gather_subset                        {all}
ansible_domain
ansible_kernel                       5.15.153.1-microsoft-standard-WSL2

(snip)
```

## PARAMETERS

### -Id
List of Host ID(s).

```yaml
Type: UInt64[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.UInt64[]
One or more Host ID(s).

## OUTPUTS

### System.Collections.Generic.Dictionary`2[[System.String],[System.Object]]
## NOTES

## RELATED LINKS

[Find-Host](Find-Host.md)

[Get-Host](Get-Host.md)
