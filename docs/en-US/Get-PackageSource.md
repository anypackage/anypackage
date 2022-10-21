---
external help file: UniversalPackageManager.dll-Help.xml
Module Name: UniversalPackageManager
online version:
schema: 2.0.0
---

# Get-PackageSource

## Synopsis

Gets the package source.

## Syntax

```powershell
Get-PackageSource [[-Name] <String[]>] [-Provider <String> [<CommonParameters>]
```

## Description

Gets the package source.

## Examples

### Example 1

```powershell

PS C:\> Get-PackageSource

Name                           Location                                           Trusted
----                           --------                                           -------
PSGallery                      https://www.powershellgallery.com/api/v2           True
```

This command gets all package repositories.

## Parameters

### -Name

Specifies the package source name.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
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
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### System.String

You can pipe a package source to this cmdlet.

## Outputs

### UniversalPackageManager.Provider.PackageSourceInfo

This cmdlet returns objects that represent a package source.

## Notes

## Related Links

* [Register-PackageSource](Register-PackageSource.md)
* [Set-PackageSource](Set-PackageSource.md)
* [Unregister-PackageSource](Unregister-PackageSource.md)
