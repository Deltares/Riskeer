using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="ShiftedLognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class ShiftedLognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<ShiftedLognormalDistribution>
    {
        private readonly ParameterDefinition<ShiftedLognormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLognormalDistributionTypeConverter"/> class.
        /// </summary>
        public ShiftedLognormalDistributionTypeConverter()
        {
            parameters = new[]
            {
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Mean)
                {
                    Symbol = "\u03BC", Description = "De gemiddelde waarde van de verschoven lognormale verdeling."
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = "\u03C3", Description = "De standaardafwijking van de verschoven lognormale verdeling."
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Shift)
                {
                    Symbol = "Verschuiving", Description = "De verschuiving van de lognormale verdeling."
                },
            };
        }

        protected override string DistributionName
        {
            get
            {
                return "Verschoven lognormale verdeling";
            }
        }

        protected override ParameterDefinition<ShiftedLognormalDistribution>[] Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}