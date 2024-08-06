---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Get-User

## SYNOPSIS
Retrieve Users by the ID(s).

## SYNTAX

```
Get-User [-Id] <UInt64[]> [<CommonParameters>]
```

## DESCRIPTION
Retrieve Users by the specified ID(s).

Implements following Rest API:  
- `/api/v2/users/{id}/`

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-User -Id 1

Id Type Username Email           FirstName LastName IsSuperuser IsSystemAuditor Created             Modified            LastLogin           LdapDn ExternalAccount
-- ---- -------- -----           --------- -------- ----------- --------------- -------             --------            ---------           ------ ---------------
 1 User admin    admin@localhost                           True           False 2023/11/04 16:20:25 2024/08/02 16:26:10 2024/08/02 16:26:10
```

Retrieve a User for Database ID 1.

## PARAMETERS

### -Id
List of database IDs for one or more Users.

```yaml
Type: UInt64[]
Parameter Sets: (All)
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

### System.UInt64[]
One or more database IDs.

### AWX.Resources.User
## OUTPUTS

## NOTES

## RELATED LINKS

[Find-User](Find-User.md)
