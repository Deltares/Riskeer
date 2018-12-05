// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Calculator
{
    [TestFixture]
    public class HydraRingCalculatorBaseTest
    {
        [Test]
        public void Constructor_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraRingCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Calculate_NoPreprocessorDirectoryWhenRequired_ThrowsInvalidOperationException()
        {
            // Setup
            var parser = new TestParser();
            HydraRingCalculationSettings settings = CreateHydraRingCalculationSettings();
            var calculator = new TestHydraRingCalculator(settings, parser);
            var hydraRingCalculationInput = new TestHydraRingCalculationInput();

            hydraRingCalculationInput.PreprocessorSetting = new PreprocessorSetting(1, 2, new NumericsSetting(1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000, 0.1, -6, 6));

            // Call
            TestDelegate test = () => calculator.PublicCalculate(hydraRingCalculationInput);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual("Preprocessor directory required but not specified.", message);
        }

        [Test]
        public void Calculate_WithCustomParser_ParsersExecutedAndOutputSet()
        {
            // Setup
            var parser = new TestParser();
            var calculator = new TestHydraRingCalculator(CreateHydraRingCalculationSettings(), parser);

            // Call
            calculator.PublicCalculate(new TestHydraRingCalculationInput());

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(calculator.OutputDirectory));
            Assert.IsTrue(parser.Parsed);
        }

        [Test]
        public void Calculate_WithCustomParserThrowingHydraRingFileParserException_HydraRingCalculationExceptionThrown()
        {
            // Setup
            var parseException = new HydraRingFileParserException("message", new Exception());
            var parser = new TestParser(parseException);
            var calculator = new TestHydraRingCalculator(CreateHydraRingCalculationSettings(), parser);

            // Call
            TestDelegate test = () => calculator.PublicCalculate(new TestHydraRingCalculationInput());

            // Assert
            var exception = Assert.Throws<HydraRingCalculationException>(test);
            Assert.AreEqual(parseException.Message, exception.Message);
            Assert.AreSame(parseException.InnerException, exception.InnerException);
        }

        [Test]
        [TestCase(typeof(SecurityException))]
        [TestCase(typeof(IOException))]
        [TestCase(typeof(UnauthorizedAccessException))]
        [TestCase(typeof(ArgumentException))]
        [TestCase(typeof(NotSupportedException))]
        [TestCase(typeof(Win32Exception))]
        public void Calculate_WithCustomParserThrowingSupportedCalculatedException_HydraRingCalculationExceptionThrown(Type exceptionType)
        {
            // Setup
            var supportedException = (Exception) Activator.CreateInstance(exceptionType,
                                                                          "Exception message",
                                                                          new Exception("InnerException"));
            var parser = new TestParser(supportedException);
            var calculator = new TestHydraRingCalculator(CreateHydraRingCalculationSettings(), parser);

            // Call
            TestDelegate test = () => calculator.PublicCalculate(new TestHydraRingCalculationInput());

            // Assert
            var exception = Assert.Throws<HydraRingCalculationException>(test);
            string expectedMessage = "Het besturingssysteem geeft de volgende melding:"
                                     + Environment.NewLine
                                     + $"{supportedException.Message}";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreSame(supportedException.InnerException, exception.InnerException);
        }

        [Test]
        public void Calculate_LastErrorFilePresent_LastErrorFileContentSet()
        {
            // Setup
            var settings = new HydraRingCalculationSettings("D:\\HLCD.sqlite", string.Empty);
            var calculator = new TestHydraRingCalculator(settings, new TestParser());

            // Call
            calculator.PublicCalculate(new TestHydraRingCalculationInput());

            // Assert
            string expectedContent = $"Hydraulic database {settings.HlcdFilePath} not found.\r\n";
            Assert.AreEqual(expectedContent, calculator.LastErrorFileContent);
        }

        [Test]
        public void Calculate_IllustrationPointsParserThrowsException_SetsIllustrationPointsParserError()
        {
            // Setup
            var calculator = new TestHydraRingCalculator(CreateHydraRingCalculationSettings(), new TestParser());

            // Call
            calculator.PublicCalculate(new TestHydraRingCalculationInput());

            // Assert
            const string expectedMessage = "Er konden geen illustratiepunten worden uitgelezen.";
            Assert.AreEqual(expectedMessage, calculator.IllustrationPointsParserErrorMessage);
            Assert.IsNull(calculator.IllustrationPointsResult);
        }

        private static HydraRingCalculationSettings CreateHydraRingCalculationSettings()
        {
            var settings = new HydraRingCalculationSettings(string.Empty,
                                                            string.Empty);
            return settings;
        }
    }

    internal class TestHydraRingCalculator : HydraRingCalculatorBase
    {
        private readonly IHydraRingFileParser parser;

        public TestHydraRingCalculator(HydraRingCalculationSettings calculationSettings) : base(calculationSettings) {}

        public TestHydraRingCalculator(HydraRingCalculationSettings calculationSettings, IHydraRingFileParser parser) : base(calculationSettings)
        {
            this.parser = parser;
        }

        public void PublicCalculate(HydraRingCalculationInput hydraRingCalculationInput)
        {
            Calculate(HydraRingUncertaintiesType.All, hydraRingCalculationInput);
        }

        protected override void SetOutputs() {}

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return parser;
        }
    }

    internal class TestHydraRingCalculationInput : HydraRingCalculationInput
    {
        public TestHydraRingCalculationInput() : base(12)
        {
            PreprocessorSetting = new PreprocessorSetting();
            DesignTablesSetting = new DesignTablesSetting(0, 0);
            NumericsSettings = new Dictionary<int, NumericsSetting>
            {
                {
                    1, new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25)
                }
            };
            TimeIntegrationSetting = new TimeIntegrationSetting(1);
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.AssessmentLevel;
            }
        }

        public override int CalculationTypeId
        {
            get
            {
                return 0;
            }
        }

        public override int VariableId
        {
            get
            {
                return 0;
            }
        }

        public override HydraRingSection Section { get; } = new HydraRingSection(12, 12, 12);
    }

    internal class TestParser : IHydraRingFileParser
    {
        private readonly Exception exception;

        public TestParser(Exception exception = null)
        {
            this.exception = exception;
        }

        public bool Parsed { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            if (exception != null)
            {
                throw exception;
            }

            Parsed = true;
        }
    }
}