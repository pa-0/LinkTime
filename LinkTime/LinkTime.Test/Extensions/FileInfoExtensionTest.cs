#region MIT License
// The MIT License (MIT)
//
// Copyright © 2017 Tobias Koch <t.koch@tk-software.de>
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the “Software”), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#region Namespaces
using System;
using System.IO;
using System.Reflection;
using LinkTime.Extensions;
using NUnit.Framework;
#endregion

namespace LinkTime.Test.Extensions
{
    /// <summary>
    /// Contains the unit tests for the <see cref="FileInfoExtension"/> class.
    /// </summary>
    [TestFixture]
    public class FileInfoExtensionTest
    {
        #region Methods

        /// <summary>
        /// Checks if the timestamp of the test library is read correctly.
        /// </summary>
        [Test]
        public void GetLinkTimeTest()
        {
            string directoryName = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            string testFileName = Path.Combine(directoryName, "Pass.test");

            DateTime expected = new DateTime(2017, 06, 05, 20, 07, 55);
            DateTime.SpecifyKind(expected, DateTimeKind.Utc);

            FileInfo file = new FileInfo(testFileName);
            TimeSpan diff = file.GetLinkTime() - expected;

            Console.WriteLine("Test file: {0}", testFileName);
            Console.WriteLine("Difference in seconds: {0}", diff.TotalSeconds);

            Assert.That(diff.TotalSeconds < 1);
        }

        #endregion
    }
}
