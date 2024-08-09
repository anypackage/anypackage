# Copyright (c) Thomas Nieto - All Rights Reserved
# You may use, distribute and modify this code under the
# terms of the MIT license.

if ($PSVersionTable.PSVersion -ge '7.4') {
    Import-Module $PSScriptRoot\lib\net8.0\AnyPackage.dll
}
else {
    Import-Module $PSScriptRoot\lib\netstandard2.0\AnyPackage.dll
}

