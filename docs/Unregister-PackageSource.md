---
external help file: AnyPackage.dll-Help.xml
Module Name: AnyPackage
online version:
schema: 2.0.0
---

# Unregister-PackageSource

## Synopsis

Unregister a package source.

## Syntax

```powershell
Unregister-PackageSource [-Name] <String[]> [-Provider <String>] [-PassThru] [-WhatIf] [-Confirm]
 [<CommonParameters>]
```

## Description

Unregister a package source.

## Examples

### Example 1

```powershell
PS C:\> Unregister-PackageSource -Name PSGallery
```

This command removes the `PSGallery` as a package source.

## Parameters

### -Name

Specifies the name of the package source.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -PassThru

Returns a PackageSourceInfo object that represents the source that was unregistered.
By default, `Unregister-PackageSource` doesn't generate any output.

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

### AnyPackage.Provider.PackageSourceInfo

By default, this cmdlet doesn't return any objects. Use the `PassThru` parameter to a return objects that represent a package source.

## Notes

## Related Links

* [Get-PackageSource](Get-PackageSource.md)
* [Register-PackageSource](Register-PackageSource.md)
* [Set-PackageSource](Set-PackageSource.md)
