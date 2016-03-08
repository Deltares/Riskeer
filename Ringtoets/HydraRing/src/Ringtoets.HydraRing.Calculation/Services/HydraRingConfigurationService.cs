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
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Services
{
    /// <summary>
    /// Service for generating the database creation script that is necessary for performing Hydra-Ring calculations.
    /// The following Hydra-Ring features are not exposed (yet):
    /// <list type="bullet">
    /// <item>
    /// Combination of multiple dike sections
    /// </item>
    /// <item>
    /// Coupling two hydraulic boundary stations
    /// </item>
    /// <item>
    /// Performing revetment calculations (DesignTables > LayerId)
    /// </item>
    /// <item>
    /// Performing piping calculations (DesignTables > AlternativeId)
    /// </item>
    /// <item>
    /// Type 3 computations (DesignTables > Method)
    /// </item>
    /// </list>
    /// In the end, the configuration can be used to generate a Hydra-Ring database creation script.
    /// </summary>
    internal class HydraRingConfigurationService
    {
        private const double defaultLayerId = 1;
        private const double defaultAlternativeId = 1;
        private const double defaultHydraRingValue = 0.0;

        private readonly string ringId;
        private readonly IList<HydraRingCalculationInput> hydraRingCalculationInputs;
        private readonly SubMechanismSettingsProvider subMechanismSettingsProvider = new SubMechanismSettingsProvider();
        private readonly FailureMechanismSettingsProvider failureMechanismSettingsProvider = new FailureMechanismSettingsProvider();
        private readonly FailureMechanismDefaultsProvider failureMechanismDefaultsProvider = new FailureMechanismDefaultsProvider();
        private readonly VariableDefaultsProvider variableDefaultsProvider = new VariableDefaultsProvider();
        private readonly HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType;
        private readonly HydraRingUncertaintiesType uncertaintiesType;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingConfigurationService"/> class.
        /// </summary>
        /// <param name="ringId">The id of the ring to perform Hydra-Ring calculations for.</param>
        /// <param name="timeIntegrationSchemeType">The <see cref="HydraRingTimeIntegrationSchemeType"/> to use while performing Hydra-Ring calculations.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while performing Hydra-Ring calculations.</param>
        public HydraRingConfigurationService(string ringId, HydraRingTimeIntegrationSchemeType timeIntegrationSchemeType, HydraRingUncertaintiesType uncertaintiesType)
        {
            hydraRingCalculationInputs = new List<HydraRingCalculationInput>();

            this.ringId = ringId;
            this.timeIntegrationSchemeType = timeIntegrationSchemeType;
            this.uncertaintiesType = uncertaintiesType;
        }

        /// <summary>
        /// Gets the id of the ring to perform Hydra-Ring calculations for.
        /// </summary>
        public string RingId
        {
            get
            {
                return ringId;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraRingTimeIntegrationSchemeType"/> to use while performing Hydra-Ring calculations.
        /// </summary>
        public HydraRingTimeIntegrationSchemeType? TimeIntegrationSchemeType
        {
            get
            {
                return timeIntegrationSchemeType;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraRingUncertaintiesType"/> to use while performing Hydra-Ring calculations.
        /// </summary>
        public HydraRingUncertaintiesType? UncertaintiesType
        {
            get
            {
                return uncertaintiesType;
            }
        }

        /// <summary>
        /// Adds Hydra-Ring calculation input to the configuration.
        /// </summary>
        /// <param name="hydraRingCalculationInput">The calculation input to add to the configuration.</param>
        public void AddHydraRingCalculationInput(HydraRingCalculationInput hydraRingCalculationInput)
        {
            hydraRingCalculationInputs.Add(hydraRingCalculationInput);
        }

        /// <summary>
        /// Generates the database creation script necessary for performing Hydra-Ring calculations.
        /// </summary>
        /// <returns>The database creation script.</returns>
        public string GenerateDataBaseCreationScript()
        {
            var configurationDictionary = new Dictionary<string, List<OrderedDictionary>>();

            InitializeHydraulicModelsConfiguration(configurationDictionary);
            InitializeSectionsConfiguration(configurationDictionary);
            InitializeDesignTablesConfiguration(configurationDictionary);
            InitializeNumericsConfiguration(configurationDictionary);
            InitializeVariableDatasConfiguration(configurationDictionary);
            InitializeCalculationProfilesConfiguration(configurationDictionary);
            InitializeSectionFaultTreeModelsConfiguration(configurationDictionary);
            InitializeSectionSubMechanismModelsConfiguration(configurationDictionary);
            InitializeFetchesConfiguration(configurationDictionary);
            InitializeAreaPointsConfiguration(configurationDictionary);
            InitializePresentationSectionsConfiguration(configurationDictionary);
            InitializeProfilesConfiguration(configurationDictionary);
            InitializeForelandModelsConfiguration(configurationDictionary);
            InitializeForelandsConfiguration(configurationDictionary);
            InitializeProbabilityAlternativesConfiguration(configurationDictionary);
            InitializeSetUpHeightsConfiguration(configurationDictionary);
            InitializeCalcWindDirectionsConfiguration(configurationDictionary);
            InitializeSwellsConfiguration(configurationDictionary);
            InitializeWaveReductionsConfiguration(configurationDictionary);
            InitializeAreasConfiguration(configurationDictionary);
            InitializeProjectsConfiguration(configurationDictionary);

            return GenerateDataBaseCreationScript(configurationDictionary);
        }

        private void InitializeHydraulicModelsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["HydraulicModels"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "TimeIntegrationSchemeID", (int?) TimeIntegrationSchemeType
                    },
                    {
                        "UncertaintiesID", (int?) UncertaintiesType
                    },
                    {
                        "DataSetName", "WTI 2017" // Fixed: use the WTI 2017 set of station locations
                    }
                }
            };
        }

        private void InitializeSectionsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var hydraRingDikeSection = hydraRingCalculationInput.DikeSection;

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingDikeSection.SectionId
                    },
                    {
                        "PresentationId", 1 // Fixed: no support for combination of multiple dike sections
                    },
                    {
                        "MainMechanismId", 1 // Fixed: no support for combination of multiple dike sections
                    },
                    {
                        "Name", hydraRingDikeSection.SectionName
                    },
                    {
                        "Description", hydraRingDikeSection.SectionName // Just use the section name
                    },
                    {
                        "RingCoordinateBegin", GetHydraRingValue(hydraRingDikeSection.SectionBeginCoordinate)
                    },
                    {
                        "RingCoordinateEnd", GetHydraRingValue(hydraRingDikeSection.SectionEndCoordinate)
                    },
                    {
                        "XCoordinate", GetHydraRingValue(hydraRingDikeSection.CrossSectionXCoordinate)
                    },
                    {
                        "YCoordinate", GetHydraRingValue(hydraRingDikeSection.CrossSectionYCoordinate)
                    },
                    {
                        "StationId1", hydraRingCalculationInput.HydraulicBoundaryLocationId
                    },
                    {
                        "StationId2", hydraRingCalculationInput.HydraulicBoundaryLocationId // Same as "StationId1": no support for coupling two stations
                    },
                    {
                        "Relative", 100.0 // Fixed: no support for coupling two stations
                    },
                    {
                        "Normal", GetHydraRingValue(hydraRingDikeSection.CrossSectionNormal)
                    },
                    {
                        "Length", GetHydraRingValue(hydraRingDikeSection.SectionLength)
                    }
                });
            }

            configurationDictionary["Sections"] = orderedDictionaries;
        }

        private void InitializeDesignTablesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(hydraRingCalculationInput.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                    },
                    {
                        "MechanismId", failureMechanismDefaults.MechanismId
                    },
                    {
                        "LayerId", defaultLayerId // Fixed: no support for revetments
                    },
                    {
                        "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                    },
                    {
                        "Method", hydraRingCalculationInput.CalculationTypeId
                    },
                    {
                        "VariableId", hydraRingCalculationInput.VariableId
                    },
                    {
                        "LoadVariableId", defaultHydraRingValue // Fixed: not relevant
                    },
                    {
                        "TableMin", defaultHydraRingValue // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "TableMax", defaultHydraRingValue // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "TableStepSize", defaultHydraRingValue // Fixed: no support for type 3 computations (see "Method")
                    },
                    {
                        "ValueMin", GetHydraRingValue(failureMechanismSettings.ValueMin)
                    },
                    {
                        "ValueMax", GetHydraRingValue(failureMechanismSettings.ValueMax)
                    },
                    {
                        "Beta", GetHydraRingValue(hydraRingCalculationInput.Beta)
                    }
                });
            }

            configurationDictionary["DesignTables"] = orderedDictionaries;
        }

        private void InitializeNumericsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (var subMechanismId in failureMechanismDefaults.SubMechanismIds)
                {
                    var subMechanismSettings = subMechanismSettingsProvider.GetSubMechanismSettings(hydraRingCalculationInput.FailureMechanismType, subMechanismId, ringId);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                        },
                        {
                            "MechanismId", failureMechanismDefaults.MechanismId
                        },
                        {
                            "LayerId", defaultLayerId // Fixed: no support for revetments
                        },
                        {
                            "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                        },
                        {
                            "SubMechanismId", subMechanismId
                        },
                        {
                            "Method", subMechanismSettings.CalculationTechniqueId
                        },
                        {
                            "FormStartMethod", subMechanismSettings.FormStartMethod
                        },
                        {
                            "FormNumberOfIterations", subMechanismSettings.FormNumberOfIterations
                        },
                        {
                            "FormRelaxationFactor", GetHydraRingValue(subMechanismSettings.FormRelaxationFactor)
                        },
                        {
                            "FormEpsBeta", GetHydraRingValue(subMechanismSettings.FormEpsBeta)
                        },
                        {
                            "FormEpsHOH", GetHydraRingValue(subMechanismSettings.FormEpsHoh)
                        },
                        {
                            "FormEpsZFunc", GetHydraRingValue(subMechanismSettings.FormEpsZFunc)
                        },
                        {
                            "DsStartMethod", subMechanismSettings.DsStartMethod
                        },
                        {
                            "DsIterationmethod", 1 // Fixed: not relevant
                        },
                        {
                            "DsMinNumberOfIterations", subMechanismSettings.DsMinNumberOfIterations
                        },
                        {
                            "DsMaxNumberOfIterations", subMechanismSettings.DsMaxNumberOfIterations
                        },
                        {
                            "DsVarCoefficient", GetHydraRingValue(subMechanismSettings.DsVarCoefficient)
                        },
                        {
                            "NiUMin", GetHydraRingValue(subMechanismSettings.NiUMin)
                        },
                        {
                            "NiUMax", GetHydraRingValue(subMechanismSettings.NiUMax)
                        },
                        {
                            "NiNumberSteps", subMechanismSettings.NiNumberSteps
                        }
                    });
                }
            }

            configurationDictionary["Numerics"] = orderDictionaries;
        }

        private void InitializeVariableDatasConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (var hydraRingVariable in hydraRingCalculationInput.Variables)
                {
                    var variableDefaults = variableDefaultsProvider.GetVariableDefaults(hydraRingCalculationInput.FailureMechanismType, hydraRingVariable.VariableId);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                        },
                        {
                            "MechanismId", failureMechanismDefaults.MechanismId
                        },
                        {
                            "LayerId", defaultLayerId // Fixed: no support for revetments
                        },
                        {
                            "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                        },
                        {
                            "VariableId", hydraRingVariable.VariableId
                        },
                        {
                            "Value", hydraRingVariable.DistributionType == HydraRingDistributionType.Deterministic
                                         ? GetHydraRingValue(hydraRingVariable.Value)
                                         : defaultHydraRingValue
                        },
                        {
                            "DistributionType", (int?) hydraRingVariable.DistributionType
                        },
                        {
                            "Parameter1", hydraRingVariable.DistributionType != HydraRingDistributionType.Deterministic
                                              ? GetHydraRingValue(hydraRingVariable.Mean)
                                              : defaultHydraRingValue
                        },
                        {
                            "Parameter2", hydraRingVariable.DistributionType != HydraRingDistributionType.Deterministic
                                          && hydraRingVariable.DeviationType == HydraRingDeviationType.Standard
                                              ? GetHydraRingValue(hydraRingVariable.Variability)
                                              : defaultHydraRingValue
                        },
                        {
                            "Parameter3", hydraRingVariable.DistributionType == HydraRingDistributionType.LogNormal
                                              ? GetHydraRingValue(hydraRingVariable.Shift)
                                              : defaultHydraRingValue
                        },
                        {
                            "Parameter4", defaultHydraRingValue // Fixed: Not relevant
                        },
                        {
                            "DeviationType", (int?) hydraRingVariable.DeviationType
                        },
                        {
                            "CoefficientOfVariation", hydraRingVariable.DistributionType != HydraRingDistributionType.Deterministic
                                                      && hydraRingVariable.DeviationType == HydraRingDeviationType.Variation
                                                          ? GetHydraRingValue(hydraRingVariable.Variability)
                                                          : defaultHydraRingValue
                        },
                        {
                            "CorrelationLength", GetHydraRingValue(variableDefaults.CorrelationLength)
                        }
                    });
                }
            }

            configurationDictionary["VariableDatas"] = orderDictionaries;
        }

        private void InitializeCalculationProfilesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                for (var i = 0; i < hydraRingCalculationInput.ProfilePoints.Count(); i++)
                {
                    var hydraRingProfilePoint = hydraRingCalculationInput.ProfilePoints.ElementAt(i);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                        },
                        {
                            "SequenceNumber", i + 1
                        },
                        {
                            "XCoordinate", GetHydraRingValue(hydraRingProfilePoint.X)
                        },
                        {
                            "ZCoordinate", GetHydraRingValue(hydraRingProfilePoint.Z)
                        },
                        {
                            "Roughness", GetHydraRingValue(hydraRingProfilePoint.Roughness)
                        }
                    });
                }
            }

            configurationDictionary["CalculationProfiles"] = orderDictionaries;
        }

        private void InitializeSectionFaultTreeModelsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(hydraRingCalculationInput.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                    },
                    {
                        "MechanismId", failureMechanismDefaults.MechanismId
                    },
                    {
                        "LayerId", defaultLayerId // Fixed: no support for revetments
                    },
                    {
                        "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                    },
                    {
                        "FaultTreeModelId", failureMechanismSettings.FaultTreeModelId
                    }
                });
            }

            configurationDictionary["SectionFaultTreeModels"] = orderedDictionaries;
        }

        private void InitializeSectionSubMechanismModelsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (var subMechanismId in failureMechanismDefaults.SubMechanismIds)
                {
                    var subMechanismModelId = hydraRingCalculationInput.GetSubMechanismModelId(subMechanismId);

                    if (subMechanismModelId != null)
                    {
                        orderedDictionaries.Add(new OrderedDictionary
                        {
                            {
                                "SectionId", hydraRingCalculationInput.DikeSection.SectionId
                            },
                            {
                                "MechanismId", failureMechanismDefaults.MechanismId
                            },
                            {
                                "LayerId", defaultLayerId // Fixed: no support for revetments
                            },
                            {
                                "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                            },
                            {
                                "SubMechanismId", subMechanismId
                            },
                            {
                                "SubMechanismModelId", subMechanismModelId
                            }
                        });
                    }
                }
            }

            configurationDictionary["SectionSubMechanismModels"] = orderedDictionaries;
        }

        private void InitializeFetchesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Fetches"] = new List<OrderedDictionary>();
        }

        private void InitializeAreaPointsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["AreaPoints"] = new List<OrderedDictionary>();
        }

        private void InitializePresentationSectionsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["PresentationSections"] = new List<OrderedDictionary>();
        }

        private void InitializeProfilesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Profiles"] = new List<OrderedDictionary>();
        }

        private void InitializeForelandModelsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["ForelandModels"] = new List<OrderedDictionary>();
        }

        private void InitializeForelandsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Forelands"] = new List<OrderedDictionary>();
        }

        private void InitializeProbabilityAlternativesConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["ProbabilityAlternatives"] = new List<OrderedDictionary>();
        }

        private void InitializeSetUpHeightsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["SetUpHeights"] = new List<OrderedDictionary>();
        }

        private void InitializeCalcWindDirectionsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["CalcWindDirections"] = new List<OrderedDictionary>();
        }

        private void InitializeSwellsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Swells"] = new List<OrderedDictionary>();
        }

        private void InitializeWaveReductionsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["WaveReductions"] = new List<OrderedDictionary>();
        }

        private void InitializeAreasConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Areas"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "aDefault", 1 // Fixed: not relevant
                    },
                    {
                        "bDefault", "1" // Fixed: not relevant
                    },
                    {
                        "cDefault", "Nederland" // Fixed: not relevant
                    }
                }
            };
        }

        private void InitializeProjectsConfiguration(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            configurationDictionary["Projects"] = new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "aDefault", 1 // Fixed: not relevant
                    },
                    {
                        "bDefault", "WTI 2017" // Fixed: not relevant
                    },
                    {
                        "cDefault", "Ringtoets calculation" // Fixed: not relevant
                    }
                }
            };
        }

        private static string GenerateDataBaseCreationScript(Dictionary<string, List<OrderedDictionary>> configurationDictionary)
        {
            var lines = new List<string>();

            foreach (var tableName in configurationDictionary.Keys)
            {
                lines.Add("DELETE FROM [" + tableName + "];");

                if (configurationDictionary[tableName].Count <= 0)
                {
                    lines.Add("");

                    continue;
                }

                foreach (var orderedDictionary in configurationDictionary[tableName])
                {
                    var valueStrings = new List<string>();

                    foreach (var val in orderedDictionary.Values)
                    {
                        if (val == null)
                        {
                            valueStrings.Add("NULL");
                            continue;
                        }

                        if (val is string)
                        {
                            valueStrings.Add("'" + val + "'");
                            continue;
                        }

                        if (val is double)
                        {
                            valueStrings.Add(((double) val).ToString(CultureInfo.InvariantCulture));
                            continue;
                        }

                        valueStrings.Add(val.ToString());
                    }

                    var valuesString = string.Join(", ", valueStrings);

                    lines.Add("INSERT INTO [" + tableName + "] VALUES (" + valuesString + ");");
                }

                lines.Add("");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static double? GetHydraRingValue(double value)
        {
            return !double.IsNaN(value) ? value : defaultHydraRingValue;
        }
    }
}