namespace Cake.Incubator.Test
{
    using System;
    using System.Collections.Generic;
    using Common.Tools.DotNetCore;
    using Common.Tools.XUnit;
    using Core.Diagnostics;
    using Core.IO;

    /// <summary>
    /// Contains settings used by <see cref="T:Cake.Incubator.DotNetCoreXUnitTester" />.
    /// </summary>
    public class DotNetCoreXUnitSettings : DotNetCoreSettings
    {
        private int? _maxThreads;

        public DotNetCoreXUnitSettings()
        {
            TraitsToInclude = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
            TraitsToExclude = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);
            MethodsToRun = new List<string>();
            ClassesToRun = new List<string>();
            NamespacesToRun = new List<string>();
            NetFrameworkOptions = new DotNetCoreXUnitFrameworkSettings();
            NetCoreOptions = new DotNetCoreXUnitCoreSettings();
        }
        
        /// <summary>
        /// set the framework (default: all targeted frameworks)
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// Set the build configuration (default: 'Debug')
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Do not build the test assemblies before running
        /// </summary>
        public bool NoBuild { get; set; }

        /// <summary>
        /// Whether to run dotnet restore on the xunit projects to obtain the cli tool reference (dotnet-xunit)
        /// Default is true
        /// </summary>
        public bool RestoreCliTool { get; set; } = true;

        /// <summary>
        /// Do not show the copyright message
        /// </summary>
        public bool NoLogo { get; set; }

        /// <summary>
        /// Do not output results with colors
        /// </summary>
        public bool NoColor { get; set; }

        /// <summary>
        /// Convert skipped tests into failures
        /// </summary>
        public bool FailSkippedTests { get; set; }

        /// <summary>
        /// Stop on first test failure
        /// </summary>
        public bool StopOnFail { get; set; }

        /// <summary>
        /// Wait for input after completion
        /// </summary>
        public bool Wait { get; set; }

        /// <summary>
        /// Enable diagnostics messages for all test assemblies
        /// </summary>
        public bool Diagnostics { get; set; }

        /// <summary>
        /// Enable internal diagnostics messages for all test assemblies
        /// </summary>
        public bool InternalDiagnostics { get; set; }

        /// <summary>
        /// Launch the debugger to debug the tests
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets the parallelism option.
        /// Corresponds to the -parallel command line switch.
        /// </summary>
        /// <value>The parallelism option.</value>
        public ParallelismOption Parallelism { get; set; }

        /// <summary>
        /// Gets or sets the maximum thread count for collection parallelization.
        /// </summary>
        /// <value>
        ///   <c>null</c> (default);
        ///   <c>0</c>: run with unbounded thread count;
        ///   <c>&gt;0</c>: limit task thread pool size to value;
        /// </value>
        /// <exception cref="T:System.ArgumentOutOfRangeException" accessor="set">value &lt; 0</exception>
        public int? MaxThreads
        {
            get => _maxThreads;
            set
            {
                if (value.HasValue)
                {
                    var nullable = value;
                    var num = 0;
                    if ((nullable.GetValueOrDefault() < num ? 1 : 0) != 0)
                        throw new ArgumentOutOfRangeException(nameof (value), value, "Value may not be negative.");
                }
                _maxThreads = value;
            }
        }

        /// <summary>
        /// Serialize all test cases (for diagnostic purposes only)
        /// </summary>
        public bool Serialize { get; set; }

        /// <summary>Gets the traits to include.</summary>
        /// <remarks>
        /// Only run tests with matching name/value traits.
        /// If more than one is specified, it acts as an OR operation.
        /// </remarks>
        /// <value>The traits to include.</value>
        public IDictionary<string, IList<string>> TraitsToInclude { get; private set; }

        /// <summary>Gets the traits to exclude.</summary>
        /// <remarks>
        /// Do not run tests with matching name/value traits.
        /// If more than one is specified, it acts as an AND operation.
        /// </remarks>
        /// <value>The traits to exclude.</value>
        public IDictionary<string, IList<string>> TraitsToExclude { get; private set; }

        /// <summary>
        /// Run given test methods (should be fully specified;
        /// i.e., 'MyNamespace.MyClass.MyTestMethod')
        /// </summary>
        public ICollection<string> MethodsToRun { get; private set; }

        /// <summary>
        /// Run all methods in given test classes (should be fully specified
        /// i.e., 'MyNamespace.MyClass')
        /// </summary>
        public ICollection<string> ClassesToRun { get; private set; }

        /// <summary>
        /// Run all methods in a given namespaces (should be fully specified
        /// i.e., 'MyNamespace.MyClass')
        /// </summary>
        public ICollection<string> NamespacesToRun { get; private set; }

        /// <summary>
        /// Do not allow reporters to be auto-enabled by environment
        /// (for example, auto-detecting TeamCity or AppVeyor)
        /// </summary>
        public bool NoAutoReporters { get; set; }

        /// <summary>
        /// Build with msbuild instead of dotnet
        /// </summary>
        public bool UseMSBuild { get; set; }

        /// <summary>
        /// Sets MSBuild verbosity level (default: 'quiet')
        /// </summary>
        public Verbosity MSBuildVerbosity { get; set; }

        /// <summary>
        /// Valid options for net4x frameworks only
        /// </summary>
        public DotNetCoreXUnitFrameworkSettings NetFrameworkOptions { get; private set; }

        /// <summary>
        /// Valid options for netcoreapp frameworks only:
        /// </summary>
        public DotNetCoreXUnitCoreSettings NetCoreOptions { get; private set; }
        
        /// <summary>
        /// Reporters (optional)
        /// appveyor : forces AppVeyor CI mode (normally auto-detected)
        /// json : show progress messages in JSON format
        /// quiet : do not show progress messages
        /// teamcity : forces TeamCity mode (normally auto-detected)
        /// verbose : show verbose progress messages
        /// </summary>
        public XUnitReporter Reporter { get; set; } 

        /// <summary>Gets or sets the output directory for any results.</summary>
        /// <value>The output directory.</value>
        public DirectoryPath OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an XML report should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an xUnit.net v2 xml report should be generated; otherwise, <c>false</c>.
        /// </value>
        public bool XmlReport { get; set; }

        /// <summary>
        /// Gets or sets the name that should be used for the HTML and XML reports.
        /// NOTE: Do not include the file extension, this is generated automatically
        /// </summary>
        /// <value>The custom report name.</value>
        public string ReportName { get; set; }

    }
}