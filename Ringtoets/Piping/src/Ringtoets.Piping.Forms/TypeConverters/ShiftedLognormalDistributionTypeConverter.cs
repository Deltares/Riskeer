using System;
using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="ShiftedLognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public sealed class ShiftedLognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<ShiftedLognormalDistribution>
    {
        private readonly ParameterDefinition<ShiftedLognormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLognormalDistributionTypeConverter"/> class.
        /// </summary>
        public ShiftedLognormalDistributionTypeConverter()
        {
            var lowerCaseDistributionName = DistributionName.ToLower();
            parameters = new[]
            {
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Mean)
                {
                    Symbol = Resources.Probabilistics_Mean_Symbol,
                    Description = String.Format(Resources.Probabilistics_Mean_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = Resources.Probabilistics_StandardDeviation_Symbol,
                    Description = String.Format(Resources.Probabilistics_StandardDeviation_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Shift)
                {
                    Symbol = Resources.Probabilistics_Shift_Symbol, 
                    Description = Resources.Probabilistics_Shift_Description
                },
            };
        }

        protected override string DistributionName
        {
            get
            {
                return Resources.ShiftedLognormalDistribution_DisplayName;
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