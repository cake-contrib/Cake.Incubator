// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Incubator.EnumerableExtensions;
    using FluentAssertions;
    using Xunit;

    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_Array_True_IfNull()
        {
            string[] arr = null;
            arr.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Array_True_IfEmpty() => new string[0].IsNullOrEmpty().Should().BeTrue();

        [Fact]
        public void IsNullOrEmpty_Array_False_IfPopulated() => new[] { "foo" }.IsNullOrEmpty().Should().BeFalse();

        [Fact]
        public void IsNullOrEmpty_List_True_IfNull()
        {
            IList<int> arr = null;
            arr.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_List_True_IfEmpty() => new List<string>().IsNullOrEmpty().Should().BeTrue();

        [Fact]
        public void IsNullOrEmpty_List_False_IfPopulated()
            => new List<string> { "foo" }.IsNullOrEmpty().Should().BeFalse();

        [Fact]
        public void IsNullOrEmpty_IEnumerable_True_IfNull()
        {
            IEnumerable<int> arr = null;
            arr.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_IEnumerable_True_IfEmpty()
        {
            IEnumerable<string> arr = new List<string>().AsEnumerable();
            arr.IsNullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_IEnumerable_False_IfPopulated()
        {
            IEnumerable<string> arr = new List<string> { "foo" }.AsEnumerable();
            arr.IsNullOrEmpty().Should().BeFalse();
        }

        [Fact]
        public void Using_Extension_Test()
        {
            "Two".IsIn("One", "Two", "Three").Should().BeTrue();
        }
    }
}
