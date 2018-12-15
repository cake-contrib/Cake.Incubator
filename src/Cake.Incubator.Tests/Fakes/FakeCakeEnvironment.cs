// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Collections.Generic;
    using System.Runtime.Versioning;
    using Cake.Core;
    using Cake.Core.IO;

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
}