// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Core.IO;

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