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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Properties;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO.Configurations
{
    public abstract class StructuresCalculationStochastAssigner<T> where T: StructuresCalculationConfiguration
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StructuresCalculationStochastAssigner<T>));

        protected readonly T configuration;
        protected readonly StructuresCalculation<HeightStructuresInput> calculation;
        protected readonly TrySetStandardDeviationStochast standardDeviationStochastSetter;
        protected readonly TrySetVariationCoefficientStochast variationCoefficientStochastSetter;

        public delegate bool TrySetStandardDeviationStochast(StandardDeviationDefinition definition);

        public delegate bool TrySetVariationCoefficientStochast(VariationCoefficientDefinition definition);

        public StructuresCalculationStochastAssigner(
            T configuration,
            StructuresCalculation<HeightStructuresInput> calculation,
            TrySetStandardDeviationStochast standardDeviationStochastSetter,
            TrySetVariationCoefficientStochast variationCoefficientStochastSetter)
        {
            this.configuration = configuration;
            this.calculation = calculation;
            this.standardDeviationStochastSetter = standardDeviationStochastSetter;
            this.variationCoefficientStochastSetter = variationCoefficientStochastSetter;
        }

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

            if (configuration.StructureName != null)
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

        public bool SetAllStochasts()
        {
            foreach (StandardDeviationDefinition stochastDefinition in GetStandardDeviationStochasts())
            {
                if (!standardDeviationStochastSetter(stochastDefinition))
                {
                    return false;
                }
            }

            foreach (VariationCoefficientDefinition stochastDefinition in GetVariationCoefficientStochasts())
            {
                if (!variationCoefficientStochastSetter(stochastDefinition))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual bool ValidateSpecificStochasts()
        {
            return true;
        }

        protected abstract IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false);

        protected abstract IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false);

        private bool ValidateNoParametersDefined(StochastConfiguration stochastConfiguration, string stochastName)
        {
            if (stochastName == null)
            {
                throw new ArgumentNullException(nameof(stochastName));
            }

            string calculationName = calculation.Name;

            bool parameterDefined = stochastConfiguration != null && (stochastConfiguration.Mean.HasValue || stochastConfiguration.StandardDeviation.HasValue || stochastConfiguration.VariationCoefficient.HasValue);
            if (parameterDefined)
            {
                log.LogCalculationConversionError($"Er is geen kunstwerk opgegeven om de stochast '{stochastName}' aan toe te voegen.", calculationName);
            }

            return !parameterDefined;
        }

        private bool ValidateBaseStochasts()
        {
            if (configuration.StormDuration?.StandardDeviation != null
                || configuration.StormDuration?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_StormDuration,
                                                  configuration.Name);
                return false;
            }
            if (configuration.ModelFactorSuperCriticalFlow?.StandardDeviation != null
                || configuration.ModelFactorSuperCriticalFlow?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_ModelFactorSuperCriticalFlow,
                                                  configuration.Name);
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

        public struct StandardDeviationDefinition
        {
            public string StochastName;
            public StochastConfiguration Configuration;
            public Func<HeightStructuresInput, IDistribution> Getter;
            public Action<HeightStructuresInput, IDistribution> Setter;

            public static StandardDeviationDefinition Create(
                string stochastName,
                StochastConfiguration configuration,
                Func<HeightStructuresInput, IDistribution> getter,
                Action<HeightStructuresInput, IDistribution> setter)
            {
                return new StandardDeviationDefinition
                {
                    StochastName = stochastName,
                    Configuration = configuration,
                    Getter = getter,
                    Setter = setter
                };
            }
        }

        public struct VariationCoefficientDefinition
        {
            public string StochastName;
            public StochastConfiguration Configuration;
            public Func<HeightStructuresInput, IVariationCoefficientDistribution> Getter;
            public Action<HeightStructuresInput, IVariationCoefficientDistribution> Setter;

            public static VariationCoefficientDefinition Create(
                string stochastName,
                StochastConfiguration configuration,
                Func<HeightStructuresInput, IVariationCoefficientDistribution> getter,
                Action<HeightStructuresInput, IVariationCoefficientDistribution> setter)
            {
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