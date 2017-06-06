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
#endregion

namespace LinkTime
{
    /// <summary>
    /// Contains the main entry point of the application.
    /// </summary>
    public static class Program
    {
        #region Methods

        /// <summary>
        /// Represents the main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments given to the application.</param>
        public static void Main(string[] args)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine($"LinkTime {version.Major}.{version.Minor}.{version.Revision}");
            Console.WriteLine("(C) 2017  Tobias Koch <t.koch@tk-software.de>");
            Console.WriteLine();

            if (args.Length != 0)
            {
                if (args[0] == "/?" || args[0] == "-?" || args[0] == "--?" || args[0].ToLower() == "help" || args[0].ToLower() == "-help" || args[0].ToLower() == "--help")
                {
                    Console.WriteLine("USAGE:");
                    Console.WriteLine(" LinkTime [filename]");
                }
                else
                {
                    FileInfo peFile = new FileInfo(args[0]);

                    try
                    {
                        DateTime linkTimeUtc = peFile.GetLinkTime();
                        DateTime linkTimeLocal = linkTimeUtc.ToLocalTime();

                        string currentTimeZoneName = string.Empty;

                        if (linkTimeLocal.IsDaylightSavingTime())
                        {
                            currentTimeZoneName = TimeZone.CurrentTimeZone.DaylightName;
                        }
                        else
                        {
                            currentTimeZoneName = TimeZone.CurrentTimeZone.StandardName;
                        }

                        Console.WriteLine("File: {0}", peFile.FullName);
                        Console.WriteLine();
                        Console.WriteLine("{0} {1} UTC", linkTimeUtc.ToShortDateString(), linkTimeUtc.ToLongTimeString());
                        Console.WriteLine("{0} {1} LOCAL ({2})", linkTimeLocal.ToShortDateString(), linkTimeLocal.ToLongTimeString(), currentTimeZoneName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("No command line arguments - use LinkTime /? for help");
            }
        }

        #endregion
    }
}