// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using System.Collections.Generic;
    using Cake.Incubator.LoggingExtensions;
    using Cake.Incubator.Project;
    using Core.IO;
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

#if NETFRAMEWORK
            var expected = $"\tStringProp:\t{test.StringProp}\r\n\tIntProp:\t{test.IntProp}\r\n\tDateTimeProp:\t{test.DateTimeProp}\r\n";
#else
            var expected = $"\tStringProp:\t{test.StringProp}\r\n\tIntProp:\t{test.IntProp}\r\n\tDateTimeProp:\t{test.DateTimeProp}\r\n\tUnreadable:\t180\r\n";
#endif
            test.Dump().Should().Be(expected);
        }

        [Fact]
        public void Dump_OutputsCorrectString_IEnumerableProps()
        {
            var test = new ListTest { ListProp = new List<string> { "foo", "bar" }, IntListProp = new[] { 1, 2, 3, 5 } };

            const string expected = "\tListProp:\t[ \"foo\", \"bar\" ]\r\n\tIntListProp:\t[ \"1\", \"2\", \"3\", \"5\" ]\r\n";

            var dump = test.Dump();
            dump.Should().Be(expected);
        }

        [Fact]
        public void Dump_OutputsCorrectString_IEnumerablePropsForFilePath()
        {
            var test = new[] {new FilePath("/Test1.a"), new FilePath("/Test2.a"),};

            var dump = test.Dump();
            dump.Should().Be("{\r\n\tHasExtension:\tTrue\r\n\tFullPath:\t/Test1.a\r\n\tIsRelative:\tFalse\r\n\tSegments:\t[ \"/Test1.a\" ]\r\n},\r\n{\r\n\tHasExtension:\tTrue\r\n\tFullPath:\t/Test2.a\r\n\tIsRelative:\tFalse\r\n\tSegments:\t[ \"/Test2.a\" ]\r\n}");
        }

        [Fact]
        public void Dump_ToStringsCustomType()
        {
            var test = new Family
            {
                Parent = new Person("Older person", 42),
                Children = new List<Person> { new Person("Toddler", 2), new Person("Teenager", 13) }
            };

            const string expected = "\tParent:\tOlder person is 42\r\n\tChildren:\t[ \tName:\tToddler\r\n\t\tAge:\t2\r\n\t, \tName:\tTeenager\r\n\t\tAge:\t13\r\n\t ]\r\n";

            var dump = test.Dump();
            dump.Should().Be(expected);
        }

        [Fact(Skip = "Testing complex object output")]
        public void Dump_ProjectParserResult()
        {
            var file = new FakeFile("CsProj_ValidFile".SafeLoad());
            file.ParseProjectFile("test").Dump().Should().Be("");
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
