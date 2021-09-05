using System;
using System.Text;
using Cake.Core.IO;
using Cake.Testing;

namespace Cake.Incubator.Tests
{
    public static class TestingExtensions
    {
        public static FakeFile CreateFakeFile(this FakeFileSystem fs, string content, string path = null)
        {
            var contentBytes = Array.Empty<byte>();

            if (!string.IsNullOrEmpty(content))
            {
                contentBytes = Encoding.Default.GetBytes(content);
            }

            if (string.IsNullOrEmpty(path))
            {
                path = $"./{Guid.NewGuid():N}.csproj";
            }
            
            var f = fs.CreateFile(path, contentBytes);

            if (f.Path.IsRelative)
            {
                // make a copy using the absolute path.
                // this is not really a good idea, it would probably be better
                // if Cake.Testing FakeFileSystem would allow for accessing a file
                // via absolute path that was created using a relative path.
                // Also, having "/Working/" hard-coded here is probably not a good replacement for "current working directory"
                fs.CreateFile("/Working/" + f.Path.FullPath, contentBytes);
            }
            
            return f;
        }
    }
}