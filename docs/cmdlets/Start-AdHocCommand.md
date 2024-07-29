---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Start-AdHocCommand

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Host
```
Start-AdHocCommand [-Host] <Host> [-ModuleName] <String> [[-ModuleArgs] <String>] [-Credential] <UInt64>
 [-Limit <String>] [-Check] [<CommonParameters>]
```

### Group
```
Start-AdHocCommand [-Group] <Group> [-ModuleName] <String> [[-ModuleArgs] <String>] [-Credential] <UInt64>
 [-Limit <String>] [-Check] [<CommonParameters>]
```

### Inventory
```
Start-AdHocCommand [-Inventory] <Inventory> [-ModuleName] <String> [[-ModuleArgs] <String>]
 [-Credential] <UInt64> [-Limit <String>] [-Check] [<CommonParameters>]
```

### InventoryId
```
Start-AdHocCommand [-InventoryId] <UInt64> [-ModuleName] <String> [[-ModuleArgs] <String>]
 [-Credential] <UInt64> [-Limit <String>] [-Check] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Check
{{ Fill Check Description }}

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

### -Credential
{{ Fill Credential Description }}

```yaml
Type: UInt64
Parameter Sets: (All)
Aliases:

Required: True
Position: 3
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Group
{{ Fill Group Description }}

```yaml
Type: Group
Parameter Sets: Group
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Host
{{ Fill Host Description }}

```yaml
Type: Host
Parameter Sets: Host
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Inventory
{{ Fill Inventory Description }}

```yaml
Type: Inventory
Parameter Sets: Inventory
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -InventoryId
{{ Fill InventoryId Description }}

```yaml
Type: UInt64
Parameter Sets: InventoryId
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Limit
{{ Fill Limit Description }}

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

### -ModuleArgs
{{ Fill ModuleArgs Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ModuleName
{{ Fill ModuleName Description }}

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AWX.Resources.Host
### AWX.Resources.Group
### AWX.Resources.Inventory
### System.UInt64
## OUTPUTS

### AWX.Resources.AdHocCommand
## NOTES

## RELATED LINKS
