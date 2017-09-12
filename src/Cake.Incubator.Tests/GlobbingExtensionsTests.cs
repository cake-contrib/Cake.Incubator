// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
    using Cake.Core;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using Cake.Core.Tooling;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GlobbingExtensionsTests
    {
        private IFileSystem fileSystem;
        private ICakeEnvironment environment;
        private IGlobber globber;
        private CakeContext context;

        public GlobbingExtensionsTests()
        {
            fileSystem = new FakeFileSystem("c:/a.txt", "c:/a/c.txt", "c:/c/a.txt");
            environment = new FakeCakeEnvironment();
            globber = new Globber(fileSystem, environment);
            context = new CakeContext(fileSystem, environment, globber, new NullLog(), A.Dummy<ICakeArguments>(),
                A.Dummy<IProcessRunner>(), A.Dummy<IRegistry>(), A.Dummy<IToolLocator>());
        }

        [Fact(Skip = "FakeEnvironment needs fixed")]
        public void GetMatchingFiles_ReturnsEmpty_IfNoMatches()
        {
            var patternA = "a.*";
            var patternB = "b.*";

            var files = context.GetFiles(patternA, patternB);
            files.Should().NotBeEmpty();
        }
    }

    public class FakeCakeEnvironment : ICakeEnvironment
    {
        public FakeCakeEnvironment()
        {
            WorkingDirectory = "c:\\";
            ApplicationRoot = "c:\\";
        }
        public DirectoryPath GetSpecialPath(SpecialPath path)
        {
            throw new System.NotImplementedException();
        }

        public string GetEnvironmentVariable(string variable)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<string, string> GetEnvironmentVariables()
        {
            throw new System.NotImplementedException();
        }

        public bool Is64BitOperativeSystem()
        {
            return false;
        }

        public bool IsUnix()
        {
            return false;
        }

        public DirectoryPath GetApplicationRoot()
        {
            return "c:/";
        }

        public FrameworkName GetTargetFramework()
        {
            return new FrameworkName("net45");
        }

        public DirectoryPath WorkingDirectory { get; set; }
        public DirectoryPath ApplicationRoot { get; }
        public ICakePlatform Platform { get; } = new CakePlatform();
        public ICakeRuntime Runtime { get; }
    }

    public class FakeFileSystem : IFileSystem
    {
        private IEnumerable<FilePath> files;

        public FakeFileSystem(params string[] files)
        {
            this.files = files.Select(x => new FilePath(x));
        }

        public IFile GetFile(FilePath path)
        {
            return files.Where(x => x == path).Select(x => new FakeFile("", x.ToString())).SingleOrDefault();
        }

        public IDirectory GetDirectory(DirectoryPath path)
        {
            return new FakeDirectory(files.Where(x => x.GetDirectory() == path), path);
        }
    }

    public class FakeDirectory : IDirectory
    {
        private readonly IEnumerable<FilePath> files;

        public FakeDirectory(IEnumerable<FilePath> files, DirectoryPath path)
        {
            this.files = files;
            Path = path;
        }

        public void Create()
        {
            throw new System.NotImplementedException();
        }

        public void Move(DirectoryPath destination)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(bool recursive)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDirectory> GetDirectories(string filter, SearchScope scope)
        {
            var directories = new List<IDirectory>();
            directories.Add(new FakeDirectory(files, Path));
            return directories;
        }

        public IEnumerable<IFile> GetFiles(string filter, SearchScope scope)
        {
            return files.Select(x => new FakeFile("", x.ToString()));
        }

        public DirectoryPath Path { get; }

        Path IFileSystemInfo.Path => Path;

        public bool Exists { get; } = true;
        public bool Hidden { get; }
    }
}