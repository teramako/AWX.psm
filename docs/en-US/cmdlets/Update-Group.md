---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Update-Group

## SYNOPSIS
Update a Group.

## SYNTAX

```
Update-Group [-Id] <UInt64> [-Name <String>] [-Description <String>] [-Variables <String>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Update a Group.

Implements following Rest API:  
- `/api/v2/groups/{id}/` (PATCH)

## EXAMPLES

### Example 1
```powershell
PS C:\> Update-Host -Id 3 -Variables @{ ansible_user = "new_user" }
```

Update variable for the Group of ID 3.

## PARAMETERS

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Description
Optional description of the group.

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
Group ID.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the group.

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

### -Variables
Specify in JSON or YAML format.
You can also specify an object of type `IDictionary` as a parameter value.

Example: `-Variables @{ ansible_connection = "ssh"; ansible_user = "ssh_user" }`

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

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

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
Group Id.

## OUTPUTS

### AWX.Resources.Group
Updated Group object.

## NOTES

## RELATED LINKS
