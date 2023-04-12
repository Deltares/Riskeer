// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryDataExtensionsTest
    {
        [Test]
        public void IsLinked_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.IsLinked(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void IsLinked_HlcdFilePathNotSet_ReturnsFalse()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            bool isLinked = hydraulicBoundaryData.IsLinked();

            // Assert
            Assert.IsFalse(isLinked);
        }

        [Test]
        public void IsLinked_HlcdFilePathSet_ReturnsTrue()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = "hlcd.sqlite"
                }
            };

            // Call
            bool isLinked = hydraulicBoundaryData.IsLinked();

            // Assert
            Assert.IsTrue(isLinked);
        }

        [Test]
        public void SetNewFolderPath_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.SetNewFolderPath(null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", paramName);
        }

        [Test]
        public void SetNewFolderPath_NewFolderPathNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            void Call() => hydraulicBoundaryData.SetNewFolderPath(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("newFolderPath", paramName);
        }

        [Test]
        public void SetNewFolderPath_ValidParameters_SetsNewFolderPathAndNotifiesObservers()
        {
            // Setup
            const string hlcdFileName = "hlcd.sqlite";
            const string hrdFileName1 = "hrdFile1.sqlite";
            const string hrdFileName2 = "hrdFile2.sqlite";
            const string currentFolderPath = "some/random/folder";
            const string newFolderPath = "new/folder/to/set";

            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(currentFolderPath, hrdFileName1)
            };

            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(currentFolderPath, hrdFileName2)
            };

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(currentFolderPath, hlcdFileName)
                },
                HydraulicBoundaryDatabases =
                {
                    hydraulicBoundaryDatabase1,
                    hydraulicBoundaryDatabase2
                }
            };

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            hydraulicBoundaryData.Attach(observer);

            mockRepository.ReplayAll();

            // Call
            hydraulicBoundaryData.SetNewFolderPath(newFolderPath);

            // Assert
            Assert.AreEqual(Path.Combine(newFolderPath, hlcdFileName), hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath);
            Assert.AreEqual(Path.Combine(newFolderPath, hrdFileName1), hydraulicBoundaryDatabase1.FilePath);
            Assert.AreEqual(Path.Combine(newFolderPath, hrdFileName2), hydraulicBoundaryDatabase2.FilePath);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetLocations_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.GetLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void GetLocations_HydraulicBoundaryDatabasesEmpty_ReturnsEmptyList()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            IEnumerable<HydraulicBoundaryLocation> locations = hydraulicBoundaryData.GetLocations();

            // Assert
            CollectionAssert.IsEmpty(locations);
        }

        [Test]
        public void GetLocations_MultipleHydraulicBoundaryDatabases_ReturnsAllLocations()
        {
            // Setup
            var location1 = new TestHydraulicBoundaryLocation();
            var location2 = new TestHydraulicBoundaryLocation();
            var location3 = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            location1
                        }
                    },
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            location2,
                            location3
                        }
                    }
                }
            };

            // Call
            IEnumerable<HydraulicBoundaryLocation> locations = hydraulicBoundaryData.GetLocations();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                location1,
                location2,
                location3
            }, locations);
        }

        [Test]
        public void GetHydraulicBoundaryDatabaseForLocation_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataExtensions.GetHydraulicBoundaryDatabaseForLocation(null, new TestHydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void GetHydraulicBoundaryDatabaseForLocation_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            void Call() => hydraulicBoundaryData.GetHydraulicBoundaryDatabaseForLocation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }
    }
}