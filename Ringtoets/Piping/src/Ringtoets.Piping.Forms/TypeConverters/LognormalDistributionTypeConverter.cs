using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="LognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class LognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<LognormalDistribution>
    {
        private readonly ParameterDefinition<LognormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistributionTypeConverter"/> class.
        /// </summary>
        public LognormalDistributionTypeConverter()
        {
            parameters = new[]
            {
                new ParameterDefinition<LognormalDistribution>(d => d.Mean)
                {
                    Symbol = "\u03BC", Description = "De gemiddelde waarde van de lognormale verdeling."
                },
                new ParameterDefinition<LognormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = "\u03C3", Description = "De standaardafwijking van de lognormale verdeling."
                }
            };
        }

        protected override string DistributionName
        {
            get
            {
                return "Lognormale verdeling";
            }
        }

        protected override ParameterDefinition<LognormalDistribution>[] Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}