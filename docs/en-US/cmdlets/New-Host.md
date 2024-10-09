---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# New-Host

## SYNOPSIS
Create a Host.

## SYNTAX

```
New-Host [-Inventory] <UInt64> [-Name] <String> [-Description <String>] [-InstanceId <String>]
 [-Variables <String>] [-Disabled] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION
Create a Host.

Implements following Rest API:  
- `/api/v2/hosts/` (POST)

## EXAMPLES

### Example 1
```powershell
PS C:\> New-Host -Inventory 1 -Name host_name
```

Create a new Host into the Inventory of ID 1.

## PARAMETERS

### -Description
Optional description of the host.

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

### -Disabled
Create a Host as disabled state.

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

### -InstanceId
Used by the remote inventory source to uniquely identify the host.

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

### -Inventory
Inventory ID.

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of the host.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Variables
Specify in JSON or YAML format.
You can also specify an object of type `IDictionary` as a parameter value.

Example: `-Variables @{ ansible_host = "192.168.0.10"; ansible_connection = "ssh"; }`

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

### None
## OUTPUTS

### AWX.Resources.Host
New created Host object.

## NOTES

## RELATED LINKS

[Get-Host](Get-Host.md)

[Find-Host](Find-Host.md)

[Update-Host](Update-Host.md)

[Register-Host](Register-Host.md)

[Unregister-Host](Unregister-Host.md)

[Remove-Host](Remove-Host.md)
