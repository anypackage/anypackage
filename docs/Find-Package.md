---
external help file: AnyPackage.dll-Help.xml
Module Name: AnyPackage
online version:
schema: 2.0.0
---

# Find-Package

## Synopsis

Finds packages in package sources.

## Syntax

```powershell
Find-Package [[-Name] <String[]>] [[-Version] <VersionRange>] [-Source <String> [-Prerelease] [-Provider <String>] [<CommonParameters>]
```

## Description

Finds packages in package sources.

## Examples

### Example 1: Find all packages

```powershell
PS C:\> Find-Package

Name                                     Version         Source               Provider
----                                     -------         ----------           --------
Microsoft.PowerShell.Archive             1.2.5.0         PSGallery            PowerShellGet
Microsoft.PowerShell.ConsoleGuiTools     0.7.2.0         PSGallery            PowerShellGet
```

The command finds packages from registered package providers.

### Example 2: Find all package from a provider

```powershell
PS C:\> Find-Package -Provider PowerShellGet

Name                                     Version         Source               Provider
----                                     -------         ----------           --------
Microsoft.PowerShell.Archive             1.2.5.0         PSGallery            PowerShellGet
Microsoft.PowerShell.ConsoleGuiTools     0.7.2.0         PSGallery            PowerShellGet
```

The command finds packages from `PowerShellGet` package provider.

## Parameters

### -Name

Specifies one or more package name.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -Prerelease

Specifies if prerelease packages should be returned.

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

### -Source

Specifies the package source.

```yaml
Type: String
Parameter Sets: (All)
Aliases: Repository

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Version

Specifies the package version.
The format is NuGet version range syntax with minor changes.
In normal NuGet version range value of `1.0` would be minimum version inclusive but this parameter converts that value to be exact version of `[1.0]`.
If you need to have minimum version inclusive then use this format `[1.0,]`.

| Notation  | Applied rule  | Description                                     |
| --------  | ------------  | -----------                                     |
| [1.0,]    | x ≥ 1.0       | Minimum version, inclusive                      |
| (1.0,)    | x > 1.0       | Minimum version, exclusive                      |
| [1.0]     | x == 1.0      | Exact version match                             |
| (,1.0]    | x ≤ 1.0       | Maximum version, inclusive                      |
| (,1.0)    | x < 1.0       | Maximum version, exclusive                      |
| [1.0,2.0] | 1.0 ≤ x ≤ 2.0 | Exact range, inclusive                          |
| (1.0,2.0) | 1.0 < x < 2.0 | Exact range, exclusive                          |
| [1.0,2.0) | 1.0 ≤ x < 2.0 | Inclusive minimum and exclusive maximum version |

```yaml
Type: VersionRange
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### System.String, NuGet.Versioning.VersionRange

You can pipe a package name and version range to this cmdlet.

## Outputs

### AnyPackage.Provider.PackageInfo

This cmdlet returns objects that represent a package.

## Notes

## Related Links

* [Get-Package](Get-Package.md)
* [Install-Package](Install-Package.md)
* [Publish-Package](Publish-Package.md)
* [Save-Package](Save-Package.md)
* [Update-Package](Update-Package.md)
* [Uninstall-Package](Uninstall-Package.md)
