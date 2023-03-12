namespace AnyPackage.Provider
{
    /// <summary>
    /// Supported package version schemes.
    /// </summary>
    public enum PackageVersionScheme
    {
        /// <summary>
        /// Alpha-numeric version.
        /// </summary>
        AlphaNumeric,

        /// <summary>
        /// Single integer value.
        /// </summary>
        Integer,

        /// <summary>
        /// Multiple part numeric.
        /// </summary>
        MultiPartNumeric,

        /// <summary>
        /// Multiple part numeric with alpha-numeric suffix.
        /// </summary>
        MultiPartNumericSuffix,

        /// <summary>
        /// Version adheres to the semver 2.0 spec.
        /// </summary>
        SemanticVersion
    }
}