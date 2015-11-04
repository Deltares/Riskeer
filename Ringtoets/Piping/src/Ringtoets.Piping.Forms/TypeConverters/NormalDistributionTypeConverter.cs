using System;
using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="NormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public sealed class NormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<NormalDistribution>
    {
        private readonly ParameterDefinition<NormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalDistributionTypeConverter"/> class.
        /// </summary>
        public NormalDistributionTypeConverter()
        {
            var lowerCaseDistributionName = DistributionName.ToLower();
            parameters = new[]
            {
                new ParameterDefinition<NormalDistribution>(d => d.Mean)
                {
                    Symbol = Resources.Probabilistics_Mean_Symbol, 
                    Description = String.Format(Resources.Probabilistics_Mean_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<NormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = Resources.Probabilistics_StandardDeviation_Symbol,
                    Description = String.Format(Resources.Probabilistics_StandardDeviation_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                }
            };
        }

        protected override string DistributionName
        {
            get
            {
                return Resources.NormalDistribution_DisplayName;
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