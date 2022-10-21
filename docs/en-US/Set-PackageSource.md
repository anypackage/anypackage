---
external help file: UniversalPackageManager.dll-Help.xml
Module Name: UniversalPackageManager
online version:
schema: 2.0.0
---

# Set-PackageSource

## Synopsis

Sets package source configuration.

## Syntax

```powershell
Set-PackageSource [-Name] <String> [-Location <String>] [-Provider <String>] [-Trusted] [-PassThru]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## Description

Sets package source configuration.

## Examples

### Example 1

```powershell
PS C:\> Set-PackageSource -Name PSGallery -Trusted
```

This command sets the `PSGallery` package source to trusted.

## Parameters

### -Location

Specifies the package source location.

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

### -Name

Specifies the package source name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -PassThru

Returns a PackageSourceInfo object that represents the source that was changed.
By default, `Set-PackageSource` doesn't generate any output.

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

### -Provider

Specifies the package provider.

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

### -Trusted

Specifies the package source is trusted.

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

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### System.String

You can use the pipeline to send a string for the source name.

## Outputs

### UniversalPackageManager.Provider.PackageSourceInfo

By default, this cmdlet doesn't return any objects. Use the `PassThru` parameter to a return objects that represent a package source.

## Notes

## Related Links

* [Get-PackageSource](Get-PackageSource.md)
* [Register-PackageSource](Register-PackageSource.md)
* [Unregister-PackageSource](Unregister-PackageSource.md)
