// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Cake.Core.IO;
    using Path = Cake.Core.IO.Path;

    public class FakeFile : IFile
    {
        private readonly string content;

        public FakeFile(string content, string path = "./project.csproj")
        {
            this.content = content;
            Path = path;
        }

        public void Copy(FilePath destination, bool overwrite)
        {
            throw new System.NotImplementedException();
        }

        public void Move(FilePath destination)
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public FilePath Path { get; }

        public long Length => throw new NotImplementedException();

        public FileAttributes Attributes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        Path IFileSystemInfo.Path => Path;

        public bool Exists => true;
        public bool Hidden => throw new NotImplementedException();
    }
}