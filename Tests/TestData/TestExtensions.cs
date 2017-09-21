using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bio.Tests
{
    public static class UtilityExtensions
    {
        const string ProjectName = "TestData";

        public static string TestDir(this string path)
        {
            // Fix up for non-Windows.
            path = path.Replace('\\', Path.DirectorySeparatorChar);

            string testPath = path;

            // Try local first.
            if (File.Exists(testPath) || Directory.Exists(testPath))
                return testPath;

            // Then test context
            if (TestContext.CurrentContext != null)
            {
                testPath = Path.Combine(TestContext.CurrentContext.TestDirectory, path);
                if (File.Exists(testPath) || Directory.Exists(testPath))
                    return testPath;
            }

            // Begin walking backwards from current folder to find this file/folder.
            string cdir = Directory.GetCurrentDirectory();
            path = Path.Combine(ProjectName, path);
            testPath = Path.Combine(cdir, path);
            do
            {
                if (File.Exists(testPath) || Directory.Exists(testPath))
                    return testPath;
                cdir = Directory.GetParent(cdir).FullName;
                testPath = Path.Combine(cdir, path);
            } while (new DirectoryInfo(cdir).Parent != null);

            throw new Exception($"Failed to locate {path}.");
        }
    }
}

