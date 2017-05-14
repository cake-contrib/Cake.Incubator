// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    /// <summary>
    /// Optional runtime options to override the default settings
    /// </summary>
    public class RuntimeOptions
    {
        /// <summary>
        /// The runtime option for server GC
        /// </summary>
        public bool ServerGarbageCollection { get; set; }

        /// <summary>
        /// The runtime option for retaining vm GC
        /// </summary>
        public bool RetainVMGarbageCollection { get; set; }

        /// <summary>
        /// The runtime option for min thread pool threads
        /// </summary>
        public string ThreadPoolMinThreads { get; set; }

        /// <summary>
        /// The runtime option for max thread pool threads
        /// </summary>
        public string ThreadPoolMaxThreads { get; set; }

        /// <summary>
        /// The runtime option for concurrent GC
        /// </summary>
        public bool ConcurrentGarbageCollection { get; set; }
    }
}