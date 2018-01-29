namespace Cake.Incubator.Test
{
    public class DotNetCoreXUnitFrameworkSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to not use app domains to run test code.
        /// </summary>
        /// <value>
        ///   <c>true</c> to not use app domains to run test code; otherwise, <c>false</c>.
        /// </value>
        public bool NoAppDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tests should be run as a shadow copy.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tests should be run as a shadow copy; otherwise, <c>false</c>.
        /// </value>
        public bool ShadowCopy { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to run force tests to run in 32 bit mode.
        /// </summary>
        /// <value>
        /// <c>true</c> to run tests in 32 bit mode; otherwise, <c>false</c>.
        /// </value>
        public bool UseX86 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an xUnit.net v1 style XML report should be generated.
        /// [net4x only]
        /// </summary>
        public bool XmlReportV1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an HTML report should be generated.
        /// [net4x only]
        /// </summary>
        /// <value>
        ///   <c>true</c> if an HTML report should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool HtmlReport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an NUnit style XML report should be generated.
        /// [net4x only]
        /// </summary>
        /// <value>
        ///   <c>true</c> if an NUnit Style XML report should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool NUnitReport { get; set; }
    }
}