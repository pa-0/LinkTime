#region MIT License
// The MIT License (MIT)
//
// Copyright © 2017-2024 Tobias Koch <t.koch@tk-software.de>
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
using System.Text;
#endregion

namespace LinkTime.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="FileInfo"/> class.
    /// </summary>
    public static class FileInfoExtension
    {
        #region Methods

        /// <summary>
        /// Gets a <see cref="DateTime"/> (UTC) that indicates when the given application has been linked.
        /// </summary>
        /// <param name="fileInfo">The application that shall be analyzed.</param>
        /// <returns>A <see cref="DateTime"/> (UTC) that indicates when the given application has been linked.</returns>
        /// <exception cref="FileNotFoundException">File does not exist.</exception>
        /// <exception cref="InvalidOperationException">File is not in the portable executable format.</exception>
        /// <exception cref="ApplicationException">Error while analyzing the file.</exception>
        public static DateTime GetLinkTime(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("The file does not exist", fileInfo.FullName);
            }

            try
            {
                byte[] buffer = new byte[2048];

                using (Stream fileStream = File.OpenRead(fileInfo.FullName))
                {
                    fileStream.Read(buffer, 0, buffer.Length);
                }

                // Check for ms-dos stub - see https://en.wikipedia.org/wiki/DOS_MZ_executable
                string mzMarker = Encoding.Default.GetString(buffer, 0, 2);
                if (mzMarker != "MZ")
                {
                    throw new InvalidOperationException("File not in portable executable format: MZ marker missing");
                }

                int peHeaderPosition = BitConverter.ToInt32(buffer, 60);
                int linkerTimestamp = BitConverter.ToInt32(buffer, peHeaderPosition + 8);

                // Check for portable executable header - see https://en.wikipedia.org/wiki/Portable_Executable
                string peMarker = Encoding.Default.GetString(buffer, peHeaderPosition, 2);
                if (peMarker != "PE")
                {
                    throw new InvalidOperationException("File not in portable executable format: PE marker missing");
                }

                DateTime theEpoch = new DateTime(1970, 1, 1);
                DateTime.SpecifyKind(theEpoch, DateTimeKind.Utc);

                DateTime linkerTimeUtc = theEpoch.AddSeconds(linkerTimestamp);

                return linkerTimeUtc;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while analyzing the file", ex);
            }
        }

        #endregion
    }
}
