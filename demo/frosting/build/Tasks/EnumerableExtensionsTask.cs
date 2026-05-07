using System;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using Cake.Incubator.EnumerableExtensions;

namespace Build.Tasks
{
    [TaskName("Enumerable-Extensions")]
    [IsDependentOn(typeof(StringExtensionsTask))]
    public sealed class EnumerableExtensionsTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            string[] nullArray = null;
            AssertThat(nullArray.IsNullOrEmpty(), "IsNullOrEmpty: null array should be true");
            AssertThat(new string[0].IsNullOrEmpty(), "IsNullOrEmpty: empty array should be true");
            AssertThat(!new[] { "a" }.IsNullOrEmpty(), "IsNullOrEmpty: non-empty array should be false");
            AssertThat("foo".IsIn("foo", "bar", "baz"), "IsIn: 'foo' should be in list");
            AssertThat(!"qux".IsIn("foo", "bar"), "IsIn: 'qux' should NOT be in list");
            context.Information("Enumerable extensions OK (IsNullOrEmpty + IsIn)");
        }

        private static void AssertThat(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception("Assertion failed: " + message);
            }
        }
    }
}
