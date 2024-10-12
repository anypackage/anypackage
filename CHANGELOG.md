# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][keep-a-changelog],
and this project adheres to [Semantic Versioning][semver].

[keep-a-changelog]: https://keepachangelog.com/en/1.0.0/
[semver]: https://semver.org/spec/v2.0.0.html

## [Unreleased]

## [0.8.0] - 2024-10-11

### Changed

- Change code signing to SignPath (#176)

## [0.7.1] - 2024-08-25

### Fixed

- Fix piping null package version (#215)
- Fix file extension error message (#216)
- Fix source error message (#217)
- Fix Find-Package parameter set (#222)
- Fix Update-Package name providers (#223)
- Fix untrusted source caption (#228)
- Fix package provider priority (#232)

## [0.7.0] - 2024-08-25

### Added

- Add package and source PSObject constructor (#205)

## [0.6.1] - 2024-08-15

### Fixed

- Fix runspaces not being started async (#192)

## [0.6.0] - 2024-08-12

### Added

- Add IsMatch overload with version comparer (#139)
- Add PackageRequest IsVersionFiltered property (#140)
- Add WriteError method (#166)
- Add symbols package (#175)
- Add localization (#181)
- Add command not found feedback provider (#187)

### Fixed

- Fix provider argument completer not quoting spaces (#163)
- Fix source completer not quoting spaces (#164)
- Fix provider completer to return full name on duplicate short names (#165)
- Fix provider completer list item and tooltip (#169)

### Removed

- Remove wrapping exception (#160)

## [0.5.1] - 2023-04-05

### Changed

- Change request provider info to public (#109)

## [0.5.0] - 2023-04-05

### Added

- Add IsSource provider method (#107)
- Add Uri parameter (#105)
- Add Path and LiteralPath parameter (#101)
- Add implicit cast from string to PackageVersion (#89)

### Changed

- Change Id to be passed during registration (#96)
- Change metadata to IReadOnlyDictionary (#95)
- Change package version to nullable (#91)

### Fixed

- Fix provider completion results order (#106)
- Fix save path to use provider path (#103)
- Fix directory validation for PSPath (#102)
- Fix provider parameter set (#97)

### Removed

- Remove WriteSource overload (#94)
- Remove WritePackage overload (#90)

## [0.4.2] - 2023-03-16

### Fixed

- Fix pipeline provider operation validation (#81)

## [0.4.1] - 2023-03-15

### Fixed

- Fix PackageVersionRange ToString method (#78)

## [0.4.0] - 2023-03-15

### Removed

- Remove NuGet.Versioning (#75)

## [0.3.0] - 2023-03-09

### Added

- Add Provider property to PackageDependency (#65)
- Add IsMatch method to SourceRequest (#66)
- Add Source argument completer (#67)

### Changed

- Update formatting (#64)

### Fixed

- Fix multi-threaded (#57)
- Fix runspace creation (#59)
- Fix schema error (#63)
- Fix prompt untrusted sources (#69)

## [0.2.0] - 2023-03-03

### Added

- Add ValidateNoWildcards to parameters (#21)
- Add xml code documentation (#48)
- Add validate no wildcards to source parameter (#49)
- Add Get-PackageProvider -ListAvailable parameter (#52)

### Fixed

- Fix analyzer issues (#28)
- Fix Update-Package error with wildcards (#42)
- Fix Update-Package not calling all providers (#43)
- Fix module property (#50)
- Fix Get-PackageProvider error when using wildcards (#53)

## [0.1.2] - 2023-02-04

### Fixed

- Revert "Update System.Collections.Immutable to 7.0.0" (#17)

## [0.1.1] - 2023-01-31

### Fixed

- Fix inherited provider info (#4)
- Update NuGet.Versioning to 6.4.0 (#5)
- Fix pipeline parameter binding (#6)
- Add error when there is no provider present (#7)
- Update System.Collections.Immutable to 7.0.0 (#8)
- Add Register-PackageSource parameter set (#9)
- Fix IsMatch method (#12)
- Add ValidateNoWildcards attribute (#13)
- Fix version range transformation (#14)

## [0.1.0] - 2022-10-27

### Added

- Initial release

[Unreleased]: https://github.com/anypackage/anypackage/compare/v0.8.0...HEAD
[0.8.0]: https://github.com/anypackage/anypackage/releases/tag/v0.8.0
[0.7.1]: https://github.com/anypackage/anypackage/releases/tag/v0.7.1
[0.7.0]: https://github.com/anypackage/anypackage/releases/tag/v0.7.0
[0.6.1]: https://github.com/anypackage/anypackage/releases/tag/v0.6.1
[0.6.0]: https://github.com/anypackage/anypackage/releases/tag/v0.6.0
[0.5.1]: https://github.com/anypackage/anypackage/releases/tag/v0.5.1
[0.5.0]: https://github.com/anypackage/anypackage/releases/tag/v0.5.0
[0.4.2]: https://github.com/anypackage/anypackage/releases/tag/v0.4.2
[0.4.1]: https://github.com/anypackage/anypackage/releases/tag/v0.4.1
[0.4.0]: https://github.com/anypackage/anypackage/releases/tag/v0.4.0
[0.3.0]: https://github.com/anypackage/anypackage/releases/tag/v0.3.0
[0.2.0]: https://github.com/anypackage/anypackage/releases/tag/v0.2.0
[0.1.2]: https://github.com/anypackage/anypackage/releases/tag/v0.1.2
[0.1.1]: https://github.com/anypackage/anypackage/releases/tag/v0.1.1
[0.1.0]: https://github.com/anypackage/anypackage/releases/tag/v0.1.0
