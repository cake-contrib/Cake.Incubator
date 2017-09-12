// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class LoggingExtensionsTests
    {
        [Fact]
        public void Dump_ReturnsNull_IfObjectNull()
        {
            object o = null;
            o.Dump().Should().BeNullOrEmpty();
        }

        [Fact]
        public void Dump_OutputsCorrectString_BasicProps()
        {
            var test = new DumpTest
            {
                IntProp = 99,
                StringProp = "foo bar",
                DateTimeProp = DateTime.Today
            };

            var expected = $"StringProp: {test.StringProp}\r\nIntProp: {test.IntProp}\r\nDateTimeProp: {test.DateTimeProp}";

            test.Dump().Should().Be(expected);
        }

        [Fact]
        public void Dump_OutputsCorrectString_IEnumerableProps()
        {
            var test = new ListTest { ListProp = new List<string> { "foo", "bar" }, IntListProp = new[] { 1, 2, 3, 5 } };

            const string expected = "ListProp: foo, bar\r\nIntListProp: 1, 2, 3, 5";

            test.Dump().Should().Be(expected);
        }

        [Fact]
        public void Dump_ToStringsCustomType()
        {
            var test = new Family
            {
                Parent = new Person("Older person", 42),
                Children = new List<Person> { new Person("Toddler", 2), new Person("Teenager", 13) }
            };

            const string expected = "Parent: Older person is 42\r\nChildren: Toddler is 2, Teenager is 13";
            test.Dump().Should().Be(expected);
        }

        private class DumpTest
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            private string PrivateProp { get; set; }
            public int Unreadable { private get; set; }

            public DumpTest()
            {
                Unreadable = 180;
                PrivateProp = "Private Prop";
            }
        }

        private class ListTest
        {
            public IEnumerable<string> ListProp { get; set; }
            public int[] IntListProp { get; set; }
        }

        private class Family
        {
            public Person Parent { get; set; }
            public IEnumerable<Person> Children { get; set; }
        }

        private class Person
        {
            public string Name { get; }
            public int Age { get; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public override string ToString() => $"{Name} is {Age}";
        }
    }
}
