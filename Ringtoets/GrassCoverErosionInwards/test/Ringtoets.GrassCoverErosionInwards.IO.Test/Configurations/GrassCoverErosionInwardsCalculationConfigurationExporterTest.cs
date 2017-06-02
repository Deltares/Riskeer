﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Configurations;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            GrassCoverErosionInwardsCalculationConfigurationExporter,
            GrassCoverErosionInwardsCalculationConfigurationWriter,
            GrassCoverErosionInwardsCalculation,
            GrassCoverErosionInwardsCalculationConfiguration>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 1"
            };
            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                Name = "Berekening 2"
            };

            var calculationGroup2 = new CalculationGroup("Nested", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("Testmap", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(
                TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                Path.Combine(nameof(GrassCoverErosionInwardsCalculationConfigurationExporter), "folderWithSubfolderAndCalculation.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override GrassCoverErosionInwardsCalculation CreateCalculation()
        {
            return new GrassCoverErosionInwardsCalculation();
        }

        protected override GrassCoverErosionInwardsCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new GrassCoverErosionInwardsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}