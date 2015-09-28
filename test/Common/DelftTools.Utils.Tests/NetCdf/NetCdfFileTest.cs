using System;
using System.IO;
using System.Linq;
using DelftTools.TestUtils;
using DelftTools.Utils.NetCdf;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.NetCdf
{
    [TestFixture]
    public class NetCdfFileTest
    {
        [Test]
        public void CreateVariableWithFixedSizeDimensionTest()
        {
            var ncFile = NetCdfFile.CreateNew(TestHelper.GetCurrentMethodName() + ".nc");
            var dim = ncFile.AddDimension("vardim", 10);
            var ncVariable = ncFile.AddVariable("var", typeof(double), new [] { dim });
            ncFile.Create();

            Assert.AreEqual(ncVariable, ncFile.GetVariableByName("var"));

            var dimNames = ncFile.GetVariableDimensionNames(ncVariable);
            Assert.AreEqual(1, dimNames.Count());
            Assert.AreEqual(10, ncFile.GetDimensionLength(ncFile.GetDimension(dimNames.First())));
            ncFile.Close();
        }

        [Test]
        [Category(TestCategory.Performance)]
        [Category(TestCategory.DataAccess)]
        public void WritingPerSliceShouldBeBloodyFast()
        {
            int ncId;
            var ncFile = NetCdfFile.CreateNew(TestHelper.GetCurrentMethodName() + ".nc");
            var timeDim = ncFile.AddUnlimitedDimension("time");
            var locationDim = ncFile.AddDimension("location", 5);
            ncFile.AddVariable("time", typeof (double), new[] {timeDim});
            ncFile.AddVariable("location", typeof (int), new[] {locationDim});
            var temperatureVar = ncFile.AddVariable("temperature", typeof (double), new[] {timeDim, locationDim});
            
            ncFile.Create();

            var origin = new[] { 0, 0 };
            var shape = new[] { 1, 5 };
            var values = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };

            TestHelper.AssertIsFasterThan(200, () =>
                {
                    PerformWriting(ncFile, temperatureVar, origin, shape, values);
                    ncFile.Close();
                });
        }

        [Test]
        public void OpeningNonNetCdfFileShouldNotKeepFileLocked()
        {
            var path = "test_file.nc";
            File.WriteAllText(path, ""); //empty file

            var threwException = false;
            try
            {
                NetCdfFile.OpenExisting(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            if (!threwException)
                Assert.Fail("Should throw exception on non-netcdf file");

            // asssert file is not locked (by deleting it):
            File.Delete(path);
        }

        [Test]
		[Ignore("Used to create an example file for DemoApp")]
        [Category(TestCategory.DataAccess)]
        public void WriteExampleFile()
        {
            var ncFile = NetCdfFile.CreateNew(TestHelper.GetCurrentMethodName() + ".nc");

            var numTimes = 24;
            var numStations = 20;

            var timeDim = ncFile.AddDimension("time", numTimes);
            var stationDim = ncFile.AddDimension("station", numStations);

            var lonVar = ncFile.AddVariable("lon", typeof (double), new[] {stationDim});
            var latVar = ncFile.AddVariable("lat", typeof (double), new[] {stationDim});
            var indexVar = ncFile.AddVariable("station_index", typeof(int), new[] { stationDim });
            var timeVar = ncFile.AddVariable("time", typeof(double), new[] { timeDim });
            ncFile.AddAttribute(timeVar, new NetCdfAttribute("units", "milliseconds since 1970-01-01"));
            var temperatureVar = ncFile.AddVariable("temperature", typeof(double), new[] { timeDim, stationDim });

            ncFile.Create();

            var rand = new Random();

            var lons = Enumerable.Range(0, numStations).Select(i => rand.NextDouble() + 4.75).ToArray();
            var lats = Enumerable.Range(0, numStations).Select(i => rand.NextDouble() + 51.75).ToArray();
            var ids = Enumerable.Range(0, numStations).ToArray();
            var times = Enumerable.Range(0, numTimes).Select(i =>
                              new DateTime(2000, 1, 1).AddDays(i)
                                                      .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                                                      .TotalSeconds)
                          .ToArray();

            ncFile.Write(lonVar, new int[] {0}, new int[] {numStations}, lons);
            ncFile.Write(latVar, new int[] {0}, new int[] {numStations}, lats);
            ncFile.Write(indexVar, new int[] {0}, new int[] {numStations}, ids);
            ncFile.Write(timeVar, new int[] { 0 }, new int[] { numTimes }, times);

            var values = Enumerable.Range(0, numStations*numTimes).Select(i => (rand.NextDouble()*10) + 15).ToArray();

            ncFile.Write(temperatureVar, new int[] { 0, 0 }, new int[] { numTimes, numStations }, values);
            ncFile.Close();
        }

        private static void PerformWriting(NetCdfFile ncFile, NetCdfVariable temperatureVar, int[] origin, int[] shape,
                                           double[] values)
        {
            for (int i = 0; i < 62000; ++i)
            {
                ncFile.Write(temperatureVar, origin, shape, values);
                origin[0]++;
            }
        }
    }
}
