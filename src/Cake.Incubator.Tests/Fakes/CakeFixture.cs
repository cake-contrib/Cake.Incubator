// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

using Cake.Testing;

namespace Cake.Incubator.Tests
{
    using System;
    using System.Collections.Generic;
    using Cake.Core;
    using Cake.Core.Configuration;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using Cake.Core.Tooling;

    public class CakeFixture
    {
        public FakeFileSystem FileSystem { get; }

        public CakeContext Context { get; }

        public CakeFixture()
        {
            var env = FakeEnvironment.CreateUnixEnvironment();
            FileSystem = new FakeFileSystem(env);
            var globber = new Globber(FileSystem, env);
            var log = new NullLog();
            var reg = new WindowsRegistry();
            var config = new CakeConfiguration(new Dictionary<string, string>());
            var strategy = new ToolResolutionStrategy(FileSystem, env, globber, config, log);
            var toolLocator = new ToolLocator(env, new ToolRepository(env), strategy);
            var cakeDataService = new FakeDataService();
            var runner = new ProcessRunner(FileSystem, env, log, toolLocator, config);
            var args = new FakeArguments();
            Context = new CakeContext(FileSystem, env, globber, log, args, runner, reg, toolLocator, cakeDataService, config);
        }
        
        public class FakeDataService : ICakeDataService
        {
            public TData Get<TData>() where TData : class
            {
                throw new NotImplementedException();
            }

            public void Add<TData>(TData value) where TData : class
            {
                throw new NotImplementedException();
            }
        }

        public class FakeArguments : ICakeArguments
        {
            public bool HasArgument(string name)
            {
                throw new NotImplementedException();
            }

            public ICollection<string> GetArguments(string name)
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, ICollection<string>> GetArguments()
            {
                throw new NotImplementedException();
            }
        }
        
    }
}