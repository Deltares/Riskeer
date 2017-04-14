// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        where TConfiguration: StructuresCalculationConfiguration
        where TInput : StructuresInputBase<TStructure>, new()
        where TStructure : StructureBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StructuresCalculationStochastAssigner<TConfiguration, TInput, TStructure>));

        /// <summary>
        /// The configuration that is used for stochast parameter source.
        /// </summary>
        protected readonly TConfiguration Configuration;
        private readonly StructuresCalculation<TInput> calculation;
        private readonly TrySetStandardDeviationStochast setStandardDeviationStochast;
        private readonly TrySetVariationCoefficientStochast setVariationCoefficientStochast;

        /// <summary>
        /// Delegate for setting standard deviation stochasts.
        /// </summary>
        /// <param name="definition">The definition of a standard deviation stochast.</param>
        /// <returns><c>true</c> if setting the stochast succeeded, <c>false</c> otherwise.</returns>
        public delegate bool TrySetStandardDeviationStochast(StandardDeviationDefinition definition);

        /// <summary>
        /// Delegate for setting variation coefficient stochasts.
        /// </summary>
        /// <param name="definition">The definition of a variation coefficient stochast.</param>
        /// <returns><c>true</c> if setting the stochast succeeded, <c>false</c> otherwise.</returns>
        public delegate bool TrySetVariationCoefficientStochast(VariationCoefficientDefinition definition);

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCalculationStochastAssigner{TConfiguration,TInput,TStructure}"/>
        /// </summary>
        /// <param name="configuration">The configuration that is used for stochast parameter source.</param>
        /// <param name="calculation">The target calculation.</param>
        /// <param name="setStandardDeviationStochast">The delegate for setting a stochast with standard deviation.</param>
        /// <param name="setVariationCoefficientStochast">The delegate for setting a stochast with variation coefficient.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameters is <c>null</c>.</exception>
        protected StructuresCalculationStochastAssigner(
            TConfiguration configuration,
            StructuresCalculation<TInput> calculation,
            TrySetStandardDeviationStochast setStandardDeviationStochast,
            TrySetVariationCoefficientStochast setVariationCoefficientStochast)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (setStandardDeviationStochast == null)
            {
                throw new ArgumentNullException(nameof(setStandardDeviationStochast));
            }
            if (setVariationCoefficientStochast == null)
            {
                throw new ArgumentNullException(nameof(setVariationCoefficientStochast));
            }
            Configuration = configuration;
            this.calculation = calculation;
            this.setStandardDeviationStochast = setStandardDeviationStochast;
            this.setVariationCoefficientStochast = setVariationCoefficientStochast;
        }

        /// <summary>
        /// Validates the stochasts and their parameters of the configuration.
        /// </summary>
        /// <returns><c>true</c> if all the stochasts are valid, <c>false</c> otherwise.</returns>
        public bool AreStochastsValid()
        {
            if (!ValidateBaseStochasts())
            {
                return false;
            }
            if (!ValidateSpecificStochasts())
            {
                return false;
            }

            if (Configuration.StructureName != null)
            {
                return true;
            }

            foreach (Tuple<string, StochastConfiguration> stochastValidationDefinition in GetStochastsForParameterValidation())
            {
                if (!ValidateNoParametersDefined(stochastValidationDefinition.Item2, stochastValidationDefinition.Item1))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets all the parameters for the stochasts from the configuration to the calculation.
        /// </summary>
        /// <returns><c>true</c> if setting the parameters of all configured stochasts succeeded, <c>false</c>
        /// otherwise.</returns>
        public bool SetAllStochasts()
        {
            foreach (StandardDeviationDefinition stochastDefinition in GetStandardDeviationStochasts())
            {
                if (!setStandardDeviationStochast(stochastDefinition))
                {
                    return false;
                }
            }

            foreach (VariationCoefficientDefinition stochastDefinition in GetVariationCoefficientStochasts())
            {
                if (!setVariationCoefficientStochast(stochastDefinition))
                {
                    return false;
                }
            }
            return true;
        }

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


        /// <summary>bas
        /// Gets the definitions for all stochasts with variation coefficient that are defined for the calculation input.
        /// </summary>
        /// <param name="structureDependent">Optional. If set to <c>true</c>, only definitions for structure dependent
        /// stochasts are returned.</param>
        /// <returns>The variation coefficient stochasts definitions for the calculation.</returns>
        protected abstract IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false);

        private bool ValidateNoParametersDefined(StochastConfiguration stochastConfiguration, string stochastName)
        {
            string calculationName = Configuration.Name;

            bool parameterDefined = stochastConfiguration != null && (stochastConfiguration.Mean.HasValue || stochastConfiguration.StandardDeviation.HasValue || stochastConfiguration.VariationCoefficient.HasValue);
            if (parameterDefined)
            {
                log.LogCalculationConversionError($"Er is geen kunstwerk opgegeven om de stochast '{stochastName}' aan toe te voegen.", calculationName);
            }

            return !parameterDefined;
        }

        private bool ValidateBaseStochasts()
        {
            if (Configuration.StormDuration?.StandardDeviation != null
                || Configuration.StormDuration?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_StormDuration,
                                                  Configuration.Name);
                return false;
            }
            if (Configuration.ModelFactorSuperCriticalFlow?.StandardDeviation != null
                || Configuration.ModelFactorSuperCriticalFlow?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_ModelFactorSuperCriticalFlow,
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
            public string StochastName;
            public StochastConfiguration Configuration;
            public Func<TInput, IDistribution> Getter;
            public Action<TInput, IDistribution> Setter;

            private StandardDeviationDefinition() {}

            /// <summary>
            /// Creates a new instance of <see cref="StandardDeviationDefinition"/>.
            /// </summary>
            /// <param name="configuration">The configuration of the stochast, which can be <c>null</c>.</param>
            /// <param name="stochastName">The name of the stochast.</param>
            /// <param name="getter">Operation of obtaining the stochast from an input.</param>
            /// <param name="setter">Operation of assigning the stuchast to an input.</param>
            /// <returns>The newly created definition.</returns>
            public static StandardDeviationDefinition Create(
                StochastConfiguration configuration,
                string stochastName,
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

                return new StandardDeviationDefinition
                {
                    StochastName = stochastName,
                    Configuration = configuration,
                    Getter = getter,
                    Setter = setter
                };
            }
        }

        /// <summary>
        /// A definition for a stochast with operations defining how to get and set the stochast.
        /// </summary>
        public class VariationCoefficientDefinition
        {
            public string StochastName;
            public StochastConfiguration Configuration;
            public Func<TInput, IVariationCoefficientDistribution> Getter;
            public Action<TInput, IVariationCoefficientDistribution> Setter;

            private VariationCoefficientDefinition() { }

            /// <summary>
            /// Creates a new instance of <see cref="VariationCoefficientDefinition"/>.
            /// </summary>
            /// <param name="configuration">The configuration of the stochast, which can be <c>null</c>.</param>
            /// <param name="stochastName">The name of the stochast.</param>
            /// <param name="getter">Operation of obtaining the stochast from an input.</param>
            /// <param name="setter">Operation of assigning the stuchast to an input.</param>
            /// <returns>The newly created definition.</returns>
            public static VariationCoefficientDefinition Create(
                StochastConfiguration configuration, 
                string stochastName, 
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

                return new VariationCoefficientDefinition
                {
                    StochastName = stochastName,
                    Configuration = configuration,
                    Getter = getter,
                    Setter = setter
                };
            }
        }
    }
}