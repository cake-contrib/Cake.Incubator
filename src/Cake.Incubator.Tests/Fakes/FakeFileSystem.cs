// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Core;
    using Cake.Core.IO;

    public class FakeFileSystem : IFileSystem
    {
        private readonly ICakeEnvironment env;
        private readonly List<IFile> files = new List<IFile>();

        public FakeFileSystem(ICakeEnvironment env, params IFile[] files)
        {
            this.env = env;
            this.files.AddRange(files);
        }

        public IFile GetFile(FilePath path)
        {
            return files.SingleOrDefault(x => x.Path == path || x.Path.MakeAbsolute(env).FullPath == path.FullPath);
        }

        public IDirectory GetDirectory(DirectoryPath path)
        {
            return new FakeDirectory(files.Where(x => x.Path.GetDirectory().FullPath == path.FullPath).Select(x => x.Path), path);
        }

        public void AddFile(IFile file)
        {
            files.Add(file);
        }
    }
}