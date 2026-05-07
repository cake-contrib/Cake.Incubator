using System;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using Cake.Incubator.StringExtensions;

namespace Build.Tasks
{
    [TaskName("String-Extensions")]
    [IsDependentOn(typeof(ParseProjectTask))]
    public sealed class StringExtensionsTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            AssertThat("Hello".EqualsIgnoreCase("HELLO"), "EqualsIgnoreCase: case mismatch");
            AssertThat("HelloWorld".StartsWithIgnoreCase("hello"), "StartsWithIgnoreCase: case mismatch");
            AssertThat("HelloWorld".EndsWithIgnoreCase("WORLD"), "EndsWithIgnoreCase: case mismatch");
            AssertThat(!"Hello".EqualsIgnoreCase("Goodbye"), "EqualsIgnoreCase: false-positive on different strings");
            context.Information("String extensions OK (EqualsIgnoreCase + StartsWithIgnoreCase + EndsWithIgnoreCase)");
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
