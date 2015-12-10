/******************************************************************************
 *
 * Name:     GdalConfiguration.cs.pp
 * Project:  GDAL CSharp Interface
 * Purpose:  A static configuration utility class to enable GDAL/OGR.
 * Author:   Felix Obermaier
 *
 ******************************************************************************
 * Copyright (c) 2012, Felix Obermaier
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using System;
using System.IO;
using System.Reflection;

using Core.GIS.SharpMap.Extensions.Interop;

using OSGeo.GDAL;
using OSGeo.OGR;

namespace Core.GIS.SharpMap.Extensions
{
    public static partial class GdalConfiguration
    {
        private static bool _configuredOgr;
        private static bool _configuredGdal;

        /// <summary>
        /// Construction of Gdal/Ogr
        /// </summary>
        static GdalConfiguration()
        {
            var executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            var executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

            if (string.IsNullOrEmpty(executingDirectory))
            {
                throw new InvalidOperationException("cannot get executing directory");
            }

            var gdalPath = Path.Combine(executingDirectory, "gdal");

            if (!Directory.Exists(gdalPath))
            {
                // some code to handle tests: instead of adding gdal native to all test 
                // projects (+/- 10), I decided to handle those cases here..
                const string ringtoetslTestDir = @"\test\";
                if (gdalPath.Contains(ringtoetslTestDir))
                {
                    var indexOfTestDir = gdalPath.IndexOf(@"\test\", StringComparison.InvariantCultureIgnoreCase);
                    var root = gdalPath.Substring(0, indexOfTestDir);
                    gdalPath = Path.Combine(root, "..", "..", @"packages\GDAL.Native.1.9.2\gdal");
                }
            }

            if (!Directory.Exists(gdalPath))
            {
                throw new DirectoryNotFoundException(String.Format("Directory '{0}' not found!", gdalPath));
            }

            var nativePath = Path.Combine(gdalPath, GetPlatform());

            if (!Directory.Exists(nativePath))
            {
                throw new DirectoryNotFoundException(String.Format("Directory '{0}' not found!", nativePath));
            }

            using (NativeLibrary.SwitchDllSearchDirectory(nativePath))
            {
                // Prepend native path to environment path, to ensure the
                // right libs are being used.
                var path = Environment.GetEnvironmentVariable("PATH");
                path = nativePath + ";" + path;
                Environment.SetEnvironmentVariable("PATH", path);

                // disable plugins directory
                Gdal.SetConfigOption("GDAL_DRIVER_PATH", "disable");

                // Set the additional GDAL environment variables.
                var gdalData = Path.Combine(gdalPath, "data");
                Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
                Gdal.SetConfigOption("GDAL_DATA", gdalData);

                Environment.SetEnvironmentVariable("GEOTIFF_CSV", gdalData);
                Gdal.SetConfigOption("GEOTIFF_CSV", gdalData);

                var projSharePath = Path.Combine(gdalPath, "share");
                Environment.SetEnvironmentVariable("PROJ_LIB", projSharePath);
                Gdal.SetConfigOption("PROJ_LIB", projSharePath);
            }
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureOgr()
        {
            if (_configuredOgr)
            {
                return;
            }

            // Register drivers
            Ogr.RegisterAll();
            _configuredOgr = true;

            PrintDriversOgr();
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureGdal()
        {
            if (_configuredGdal)
            {
                return;
            }

            // Register drivers
            Gdal.AllRegister();
            _configuredGdal = true;

            PrintDriversGdal();
        }

        /// <summary>
        /// Function to determine which platform we're on
        /// </summary>
        private static string GetPlatform()
        {
            return Environment.Is64BitProcess ? "x64" : "x86";
        }

        private static void PrintDriversOgr()
        {
#if DEBUG
            var num = Ogr.GetDriverCount();
            for (var i = 0; i < num; i++)
            {
                var driver = Ogr.GetDriver(i);
                Console.WriteLine(string.Format("OGR {0}: {1}", i, driver.name));
            }
#endif
        }

        private static void PrintDriversGdal()
        {
#if DEBUG
            var num = Gdal.GetDriverCount();
            for (var i = 0; i < num; i++)
            {
                var driver = Gdal.GetDriver(i);
                Console.WriteLine(string.Format("GDAL {0}: {1}-{2}", i, driver.ShortName, driver.LongName));
            }
#endif
        }
    }
}