# Copyright (c) Thomas Nieto - All Rights Reserved
# You may use, distribute and modify this code under the
# terms of the MIT license.

$framework = if ($PSVersionTable.PSVersion -ge '7.4') {
    'net8.0'
} else {
    'netstandard2.0'
}

$path = Join-Path -Path $PSScriptRoot -ChildPath "lib/$framework/AnyPackage.dll"
Import-Module $path
