using System;
using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="LognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public sealed class LognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<LognormalDistribution>
    {
        private readonly ParameterDefinition<LognormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistributionTypeConverter"/> class.
        /// </summary>
        public LognormalDistributionTypeConverter()
        {
            var lowerCaseDistributionName = DistributionName.ToLower();
            parameters = new[]
            {
                new ParameterDefinition<LognormalDistribution>(d => d.Mean)
                {
                    Symbol = Resources.Probabilistics_Mean_Symbol,
                    Description = String.Format(Resources.Probabilistics_Mean_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<LognormalDistribution>(d => d.StandardDeviation)
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
                return Resources.LognormalDistribution_DisplayName;
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