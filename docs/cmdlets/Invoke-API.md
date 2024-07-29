---
external help file: AWX.psm.dll-Help.xml
Module Name: AWX.psm
online version:
schema: 2.0.0
---

# Invoke-API

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### NonSendData (Default)
```
Invoke-API [-Method] <Method> [-Path] <String> [[-QueryString] <String>] [-AsRawString]
 [<CommonParameters>]
```

### SendData
```
Invoke-API [-Method] <Method> [-Path] <String> [[-QueryString] <String>] -SenData <Object> [-AsRawString]
 [<CommonParameters>]
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

### -Method
{{ Fill Method Description }}

```yaml
Type: Method
Parameter Sets: (All)
Aliases:
Accepted values: GET, POST, PUT, PATCH, DELETE, OPTIONS

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
{{ Fill Path Description }}

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

### -QueryString
{{ Fill QueryString Description }}

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

### -SenData
{{ Fill SenData Description }}

```yaml
Type: Object
Parameter Sets: SendData
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -AsRawString
{{ Fill AsRawString Description }}

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

### System.Object
## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
