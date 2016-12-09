// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    using System;
    using Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class EnvironmentExtensionsTests
    {
        private readonly ICakeContext cakeContext;
        private readonly ICakeEnvironment environment;

        private const string TestVariableName = "Test";
        private const string TestVariableValue = "Value";

        public EnvironmentExtensionsTests()
        {
            environment = A.Fake<ICakeEnvironment>();
            A.CallTo(() => environment.GetEnvironmentVariable(TestVariableName)).Returns(TestVariableValue);

            cakeContext = A.Fake<ICakeContext>();
            A.CallTo(() => cakeContext.Environment).Returns(environment);
        }

        [Fact]
        public void EnvironmentVariable_ReturnsValue_IfFound()
        {
            var result = cakeContext.EnvironmentVariable<string>(TestVariableName);

            result.Should().Be(TestVariableValue);
        }

        [Fact]
        public void EnvironmentVariable_Throws_IfNotFound()
        {
            Action action = () => cakeContext.EnvironmentVariable<string>($"__{TestVariableName}");
            action.ShouldThrow<CakeException>().WithMessage(
                $"Environment variable '__{TestVariableName}' was not found.");
        }

        [Fact]
        public void EnvironmentVariable_WithDefault_ReturnsValue_IfFound()
        {
            var result = cakeContext.EnvironmentVariable(TestVariableName, "foo");

            result.Should().Be(TestVariableValue);
        }

        [Fact]
        public void EnvironmentVariable_WithDefault_ReturnsDefault_IfNotFound()
        {
            const string defaultValue = "foo";
            var result = cakeContext.EnvironmentVariable($"__{TestVariableName}", defaultValue);

            result.Should().Be(defaultValue);
        }
    }
}
