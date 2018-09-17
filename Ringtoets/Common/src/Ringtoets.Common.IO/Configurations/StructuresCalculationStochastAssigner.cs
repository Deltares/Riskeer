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

using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations
{
    /// <summary>
    /// Validates and assigns stochast configurations to a structure calculation input.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the configuration that is validated and used in assignments.</typeparam>
    /// <typeparam name="TInput">The type of the input to which to assign stochasts.</typeparam>
    /// <typeparam name="TStructure">The type of structure assigned to the input.</typeparam>
    public abstract class StructuresCalculationStochastAssigner<TConfiguration, TInput, TStructure>
        where TConfiguration : StructuresCalculationConfiguration
        where TInput : StructuresInputBase<TStructure>, new()
        where TStructure : StructureBase
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(StructuresCalculationStochastAssigner<TConfiguration, TInput, TStructure>));

        private readonly StructuresCalculation<TInput> calculation;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCalculationStochastAssigner{TConfiguration,TInput,TStructure}"/>
        /// </summary>
        /// <param name="configuration">The configuration that is used for stochast parameter source.</param>
        /// <param name="calculation">The target calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameters is <c>null</c>.</exception>
        protected StructuresCalculationStochastAssigner(
            TConfiguration configuration,
            StructuresCalculation<TInput> calculation)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            Configuration = configuration;
            this.calculation = calculation;
        }

        /// <summary>
        /// Validates the configuration and sets all the parameters for the stochasts from the configuration
        /// to the calculation.
        /// </summary>
        /// <returns><c>true</c> if setting the parameters of all configured stochasts succeeded, <c>false</c>
        /// otherwise.</returns>
        public bool Assign()
        {
            return Validate()
                   && GetStandardDeviationStochasts().All(SetStandardDeviationStochast)
                   && GetVariationCoefficientStochasts().All(SetVariationCoefficientStochast);
        }

        /// <summary>
        /// Gets the configuration that is used for stochast parameter source.
        /// </summary>
        protected TConfiguration Configuration { get; }

        /// <summary>
        /// Performs additional validations for structure specific stochasts.
        /// </summary>
        /// <returns><c>true</c> if no errors were found, <c>false</c> otherwise.</returns>
        protected virtual bool ValidateSpecificStochasts()
        {
            return true;
        }

        /// <summary>
        /// Gets the definitions for all stochasts with standard deviation that are defined for the calculation input.
        /// </summary>
        /// <param name="structureDependent">Optional. If set to <c>true</c>, only definitions for structure dependent
        /// stochasts are returned.</param>
        /// <returns>The standard deviation stochasts definitions for the calculation.</returns>
        protected abstract IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false);

        /// <summary>
        /// Gets the definitions for all stochasts with variation coefficient that are defined for the calculation input.
        /// </summary>
        /// <param name="structureDependent">Optional. If set to <c>true</c>, only definitions for structure dependent
        /// stochasts are returned.</param>
        /// <returns>The variation coefficient stochasts definitions for the calculation.</returns>
        protected abstract IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false);

        /// <summary>
        /// Validates the stochasts and their parameters of the configuration.
        /// </summary>
        /// <returns><c>true</c> if all the stochasts are valid, <c>false</c> otherwise.</returns>
        private bool Validate()
        {
            return ValidateBaseStochasts()
                   && ValidateSpecificStochasts()
                   && (Configuration.StructureId != null || GetStochastsForParameterValidation().All(ValidateNoParametersDefined));
        }

        private bool SetVariationCoefficientStochast(VariationCoefficientDefinition definition)
        {
            return ConfigurationImportHelper.TrySetVariationCoefficientStochast(
                definition.StochastName,
                calculation.Name,
                calculation.InputParameters,
                definition.Configuration,
                definition.Getter,
                definition.Setter,
                Log);
        }

        private bool SetStandardDeviationStochast(StandardDeviationDefinition definition)
        {
            return ConfigurationImportHelper.TrySetStandardDeviationStochast(
                definition.StochastName,
                calculation.Name,
                calculation.InputParameters,
                definition.Configuration,
                definition.Getter,
                definition.Setter,
                Log);
        }

        private bool ValidateNoParametersDefined(Tuple<string, StochastConfiguration> stochastValidationDefinition)
        {
            string stochastName = stochastValidationDefinition.Item1;
            StochastConfiguration stochastConfiguration = stochastValidationDefinition.Item2;

            string calculationName = Configuration.Name;

            bool parameterDefined = stochastConfiguration != null && (stochastConfiguration.Mean.HasValue || stochastConfiguration.StandardDeviation.HasValue || stochastConfiguration.VariationCoefficient.HasValue);
            if (parameterDefined)
            {
                Log.LogCalculationConversionError(string.Format(Resources.StructuresCalculationStochastAssigner_ValidateNoParametersDefined_No_structure_defined_to_add_Stochast_0_,
                                                                stochastName), calculationName);
            }

            return !parameterDefined;
        }

        private bool ValidateBaseStochasts()
        {
            if (Configuration.StormDuration?.StandardDeviation != null
                || Configuration.StormDuration?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_StormDuration,
                                                  Configuration.Name);
                return false;
            }

            return true;
        }

        private IEnumerable<Tuple<string, StochastConfiguration>> GetStochastsForParameterValidation()
        {
            foreach (StandardDeviationDefinition stochastDefinition in GetStandardDeviationStochasts(true))
            {
                yield return Tuple.Create(stochastDefinition.StochastName, stochastDefinition.Configuration);
            }

            foreach (VariationCoefficientDefinition stochastDefinition in GetVariationCoefficientStochasts(true))
            {
                yield return Tuple.Create(stochastDefinition.StochastName, stochastDefinition.Configuration);
            }
        }

        /// <summary>
        /// A definition for a stochast with operations defining how to get and set the stochast.
        /// </summary>
        public class StandardDeviationDefinition
        {
            /// <summary>
            /// Creates a new instance of <see cref="StandardDeviationDefinition"/>.
            /// </summary>
            /// <param name="stochastName">The name of the stochast.</param>
            /// <param name="configuration">The configuration of the stochast, which can be <c>null</c>.</param>
            /// <param name="getter">Operation of obtaining the stochast from an input.</param>
            /// <param name="setter">Operation of assigning the stochast to an input.</param>
            /// <returns>The newly created definition.</returns>
            /// <exception cref="ArgumentNullException">Thrown when the <paramref name="stochastName"/>,
            /// <paramref name="getter"/> or <paramref name="setter"/> is <c>null</c>.</exception>
            public StandardDeviationDefinition(
                string stochastName,
                StochastConfiguration configuration,
                Func<TInput, IDistribution> getter,
                Action<TInput, IDistribution> setter)
            {
                if (stochastName == null)
                {
                    throw new ArgumentNullException(nameof(stochastName));
                }

                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }

                if (setter == null)
                {
                    throw new ArgumentNullException(nameof(setter));
                }

                StochastName = stochastName;
                Configuration = configuration;
                Getter = getter;
                Setter = setter;
            }

            /// <summary>
            /// Gets the name of the stochast.
            /// </summary>
            public string StochastName { get; }

            /// <summary>
            /// Gets the configuration of the stochast. Can return <c>null</c>
            /// if no configuration was defined.
            /// </summary>
            public StochastConfiguration Configuration { get; }

            /// <summary>
            /// The method for obtaining the distribution matching the stochast
            /// to be configured, from the input.
            /// </summary>
            public Func<TInput, IDistribution> Getter { get; }

            /// <summary>
            /// The method for assigning the distribution matching the stochast
            /// to be configured, to the input.
            /// </summary>
            public Action<TInput, IDistribution> Setter { get; }
        }

        /// <summary>
        /// A definition for a stochast with operations defining how to get and set the stochast.
        /// </summary>
        public class VariationCoefficientDefinition
        {
            /// <summary>
            /// Creates a new instance of <see cref="VariationCoefficientDefinition"/>.
            /// </summary>
            /// <param name="stochastName">The name of the stochast.</param>
            /// <param name="configuration">The configuration of the stochast, which can be <c>null</c>.</param>
            /// <param name="getter">Operation of obtaining the stochast from an input.</param>
            /// <param name="setter">Operation of assigning the stochast to an input.</param>
            /// <returns>The newly created definition.</returns>
            /// <exception cref="ArgumentNullException">Thrown when the <paramref name="stochastName"/>,
            /// <paramref name="getter"/> or <paramref name="setter"/> is <c>null</c>.</exception>
            public VariationCoefficientDefinition(
                string stochastName,
                StochastConfiguration configuration,
                Func<TInput, IVariationCoefficientDistribution> getter,
                Action<TInput, IVariationCoefficientDistribution> setter)
            {
                if (stochastName == null)
                {
                    throw new ArgumentNullException(nameof(stochastName));
                }

                if (getter == null)
                {
                    throw new ArgumentNullException(nameof(getter));
                }

                if (setter == null)
                {
                    throw new ArgumentNullException(nameof(setter));
                }

                StochastName = stochastName;
                Configuration = configuration;
                Getter = getter;
                Setter = setter;
            }

            /// <summary>
            /// Gets the name of the stochast.
            /// </summary>
            public string StochastName { get; }

            /// <summary>
            /// Gets the configuration of the stochast. Can return <c>null</c>
            /// if no configuration was defined.
            /// </summary>
            public StochastConfiguration Configuration { get; }

            /// <summary>
            /// The method for obtaining the distribution matching the stochast
            /// to be configured, from the input.
            /// </summary>
            public Func<TInput, IVariationCoefficientDistribution> Getter { get; }

            /// <summary>
            /// The method for assigning the distribution matching the stochast
            /// to be configured, to the input.
            /// </summary>
            public Action<TInput, IVariationCoefficientDistribution> Setter { get; }
        }
    }
}