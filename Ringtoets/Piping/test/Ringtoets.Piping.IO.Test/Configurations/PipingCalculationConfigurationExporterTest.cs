// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.IO;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Integration.TestUtils;
using Ringtoets.Piping.IO.Configurations;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationExporterTest
        : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
            PipingCalculationConfigurationExporter,
            PipingCalculationConfigurationWriter,
            PipingCalculation>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            PipingCalculation calculation = PipingTestDataGenerator.GetPipingCalculation();
            calculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            calculation.InputParameters.ExitPointL = (RoundedDouble) 0.2;

            PipingCalculation calculation2 = PipingTestDataGenerator.GetPipingCalculation();
            calculation2.Name = "PK001_0002 W1-6_4_1D1";
            calculation2.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "PUNT_SCH_17", 0, 0);
            calculation2.InputParameters.SurfaceLine.Name = "PK001_0002";
            calculation2.InputParameters.EntryPointL = (RoundedDouble) 0.3;
            calculation2.InputParameters.ExitPointL = (RoundedDouble) 0.4;
            calculation2.InputParameters.StochasticSoilModel = new StochasticSoilModel(1, "PK001_0002_Piping", string.Empty);
            calculation2.InputParameters.StochasticSoilProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile("W1-6_4_1D1", 0, new[]
                {
                    new PipingSoilLayer(0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            var calculationGroup2 = new CalculationGroup("PK001_0002", false)
            {
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup("PK001_0001", false)
            {
                Children =
                {
                    calculation,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                                    Path.Combine(nameof(PipingCalculationConfigurationExporter),
                                                                                 "folderWithSubfolderAndCalculation.xml"));

            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override PipingCalculation CreateCalculation()
        {
            return PipingTestDataGenerator.GetPipingCalculation();
        }
    }
}