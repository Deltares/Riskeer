// // Copyright (C) Stichting Deltares 2019. All rights reserved.
// //
// // This file is part of Riskeer.
// //
// // Riskeer is free software: you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation, either version 3 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU General Public License for more details.
// //
// // You should have received a copy of the GNU General Public License
// // along with this program. If not, see <http://www.gnu.org/licenses/>.
// //
// // All names, logos, and references to "Deltares" are registered trademarks of
// // Stichting Deltares and remain full property of Stichting Deltares at all times.
// // All rights reserved.
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using NUnit.Framework;
// using Riskeer.Common.Data.TestUtil.IllustrationPoints;
// using Riskeer.Piping.Data.Probabilistic;
//
// namespace Riskeer.Piping.Util.Test
// {
//     [TestFixture]
//     public class ProbabilisticPipingIllustrationPointsHelperTest
//     {
//         [Test]
//         public void HasIllustrationPoints_CalculationsNull_ThrowsArgumentNullException()
//         {
//             // Call
//             TestDelegate call = () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints((IEnumerable<ProbabilisticPipingCalculationScenario>) null);
//
//             // Assert
//             var exception = Assert.Throws<ArgumentNullException>(call);
//             Assert.AreEqual("calculations", exception.ParamName);
//         }
//
//         [Test]
//         [TestCaseSource(nameof(GetCalculationCollectionConfigurations))]
//         public void HasIllustrationPoints_WithVariousCalculationCollectionConfigurations_ReturnsExpectedResults(
//             IEnumerable<ProbabilisticPipingCalculationScenario> calculations,
//             bool expectedHasIllustrationPoints)
//         {
//             // Call
//             bool hasIllustrationPoints = ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculations);
//
//             // Assert
//             Assert.AreEqual(expectedHasIllustrationPoints, hasIllustrationPoints);
//         }
//
//         [Test]
//         public void HasIllustrationPoints_CalculationNull_ThrowsArgumentNullException()
//         {
//             // Call
//             TestDelegate call = () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints((ProbabilisticPipingCalculationScenario) null);
//
//             // Assert
//             var exception = Assert.Throws<ArgumentNullException>(call);
//             Assert.AreEqual("calculation", exception.ParamName);
//         }
//
//         [Test]
//         [TestCaseSource(nameof(GetCalculationConfigurations))]
//         public void HasIllustrationPoints_WithVariousCalculationConfigurations_ReturnsExpectedResult(
//             ProbabilisticPipingCalculationScenario calculation,
//             bool expectedHasIllustrationPoints)
//         {
//             // Call
//             bool hasIllustrationPoints = ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculation);
//
//             // Assert
//             Assert.AreEqual(expectedHasIllustrationPoints, hasIllustrationPoints);
//         }
//
//         private static IEnumerable<TestCaseData> GetCalculationCollectionConfigurations()
//         {
//             foreach (TestCaseData configuration in GetCalculationConfigurations())
//             {
//                 yield return new TestCaseData(new[]
//                 {
//                     new ProbabilisticPipingCalculationScenario(),
//                     new ProbabilisticPipingCalculationScenario
//                     {
//                         Output = new 
//                     },
//                     (ProbabilisticPipingCalculationScenario) configuration.Arguments[0]
//                 }, configuration.Arguments[1]);
//             }
//
//             yield return new TestCaseData(Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), false);
//         }
//
//         private static IEnumerable<TestCaseData> GetCalculationConfigurations()
//         {
//             var random = new Random(21);
//             var calculationWithOverToppingOutputWithIllustrationPoints = new ProbabilisticPipingCalculationScenario
//             {
//                 Output = new ProbabilisticPipingOutput(new PartialProbabilisticPipingOutput(0.7, new TestGeneralResultFaultTreeIllustrationPoint()),
//                                                        new PartialProbabilisticPipingOutput(0.6, new TestGeneralResultFaultTreeIllustrationPoint()))};
//
//             var calculationWithDikeHeightRateWithIllustrationPoints = new ProbabilisticPipingCalculationScenario
//             {
//                 Output = new ProbabilisticPipingOutput(new TestOvertoppingOutput(random.NextDouble()),
//                                                             new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
//                                                             null)
//             };
//
//             var calculationWithOvertoppingRateWithIllustrationPoints = new ProbabilisticPipingCalculationScenario
//             {
//                 Output = new ProbabilisticPipingOutput(new TestOvertoppingOutput(random.NextDouble()),
//                                                             null,
//                                                             new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint()))
//             };
//
//             var calculationWitNoIllustrationPoints = new ProbabilisticPipingCalculationScenario
//             {
//                 Output = new ProbabilisticPipingOutput(new TestOvertoppingOutput(random.NextDouble()),
//                                                             null,
//                                                             null)
//             };
//
//             yield return new TestCaseData(calculationWithOverToppingOutputWithIllustrationPoints, true);
//             yield return new TestCaseData(calculationWithDikeHeightRateWithIllustrationPoints, true);
//             yield return new TestCaseData(calculationWithOvertoppingRateWithIllustrationPoints, true);
//             yield return new TestCaseData(calculationWitNoIllustrationPoints, false);
//             yield return new TestCaseData(new ProbabilisticPipingCalculationScenario(), false);
//         }
//     }
// }