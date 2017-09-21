using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bio.Tests
{
    public static class UtilityExtensions
    {
        public static string TestDir(this string path)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, path);
        }
    }
}

