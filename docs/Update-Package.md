---
external help file: AnyPackage.dll-Help.xml
Module Name: AnyPackage
online version:
schema: 2.0.0
---

# Update-Package

## Synopsis

Updates a package.

## Syntax

### Name

```powershell
Update-Package [[-Name] <String[]>] [[-Version] <VersionRange>] [-Source <String>] [-Prerelease] [-PassThru] [-TrustSource] [-Provider <String>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

### InputObject

```powershell
Update-Package [-InputObject] <PackageInfo[]> [-PassThru] [-TrustSource] [-Provider <String>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## Description

Updates a package.

## Examples

### Example 1

```powershell
PS C:\> Update-Package -Name PowerShellGet
```

This command updates `PowerShellGet` package.

## Parameters

### -InputObject

Specifies a PackageInfo object that represents the package to update.
Enter a variable that contains the object, or type a command or expression that gets the object, such as a `Get-Package` command.
You can use the pipeline to send a package object to `Update-Package`.

```yaml
Type: PackageInfo[]
Parameter Sets: InputObject
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name

Specifies the package name.

```yaml
Type: String[]
Parameter Sets: Name
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -PassThru

If the package information should be output to the pipeline.

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

### -Prerelease

If a prerelease version should be allowed.

```yaml
Type: SwitchParameter
Parameter Sets: Name
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

Specifies the package source name.

```yaml
Type: String
Parameter Sets: Name
Aliases: Repository

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -TrustSource

If an untrusted source should be trusted for this execution suppressing prompts.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: TrustRepository

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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
Parameter Sets: Name
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### System.String, NuGet.Versioning.VersionRange, AnyPackage.Provider.PackageInfo

You can pipe a package name, version range, and package info to this cmdlet.

## Outputs

### AnyPackage.Provider.PackageInfo

By default, this cmdlet doesn't return any objects. Use the `PassThru` parameter to a return objects that represent a package.

## Notes

## Related Links

* [Get-Package](Get-Package.md)
* [Find-Package](Find-Package.md)
* [Install-Package](Install-Package.md)
* [Publish-Package](Publish-Package.md)
* [Save-Package](Save-Package.md)
* [Uninstall-Package](Uninstall-Package.md)
