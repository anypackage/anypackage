@{
    Path = @(
        './src/AnyPackage/bin/Release/netstandard2.0/*',
        './src/AnyPackage.format.ps1xml',
        './src/AnyPackage.psd1'
    )
    Destination = './out/AnyPackage'
    Exclude = 'AnyPackage.xml'
}
