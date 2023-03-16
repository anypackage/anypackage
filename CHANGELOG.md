# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/AnyPackage/AnyPackage/compare/v0.4.2...HEAD
[0.4.2]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.4.2
[0.4.1]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.4.1
[0.4.0]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.4.0
[0.3.0]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.3.0
[0.2.0]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.2.0
[0.1.2]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.1.2
[0.1.1]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.1.1
[0.1.0]: https://github.com/AnyPackage/AnyPackage/releases/tag/v0.1.0
