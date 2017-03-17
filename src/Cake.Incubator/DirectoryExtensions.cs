// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator
{
    using System;
    using Cake.Common.IO;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    [CakeAliasCategory("Directory Helpers")]
    public static class DirectoryExtensions
    {
        /// <summary>
        /// Obsolete: Use Cake.Common.IO.CopyDirectory instead
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="source">The source directory</param>
        /// <param name="destination">The destination directory</param>
        /// <exception cref="CakeException">Throws if source directory does not exist</exception>
        /// <exception cref="CakeException">Throws if destination directory does exist</exception>
        [CakeMethodAlias]
        [Obsolete("Use Cake.Common.IO.CopyDirectory instead")]
        public static void TransferDirectory(this ICakeContext context, DirectoryPath source, DirectoryPath destination)
        {
            context.ThrowIfNull(nameof(context));
            source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));

            if(!context.FileSystem.Exist(source)) throw new CakeException($"Source directory {source} does not exist, cannot move");
            if(context.FileSystem.Exist(destination)) throw new CakeException($"Destination directory {destination} already exists, cannot move");

            context.CopyDirectory(source, destination);
            context.DeleteDirectory(source, true);
        }
    }
}