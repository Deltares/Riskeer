using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="NormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class NormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<NormalDistribution>
    {
        private readonly ParameterDefinition<NormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalDistributionTypeConverter"/> class.
        /// </summary>
        public NormalDistributionTypeConverter()
        {
            parameters = new[]
            {
                new ParameterDefinition<NormalDistribution>(d => d.Mean)
                {
                    Symbol = "\u03BC", Description = "De gemiddelde waarde van de normale verdeling."
                },
                new ParameterDefinition<NormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = "\u03C3", Description = "De standaardafwijking van de normale verdeling."
                }
            };
        }

        protected override string DistributionName
        {
            get
            {
                return "Normale verdeling";
            }
        }

        protected override ParameterDefinition<NormalDistribution>[] Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}