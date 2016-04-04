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
    /// Combination of multiple sections
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
        private readonly double? defaultHydraRingNullValue = null;

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
            var configurationDictionary = new Dictionary<string, IList<OrderedDictionary>>();

            configurationDictionary["HydraulicModels"] = GetHydraulicModelsConfiguration();
            configurationDictionary["Sections"] = GetSectionsConfiguration();
            configurationDictionary["DesignTables"] = GetDesignTablesConfiguration();
            configurationDictionary["Numerics"] = GetNumericsConfiguration();
            configurationDictionary["VariableDatas"] = GetVariableDatasConfiguration();
            configurationDictionary["CalculationProfiles"] = GetCalculationProfilesConfiguration();
            configurationDictionary["SectionFaultTreeModels"] = GetSectionFaultTreeModelsConfiguration();
            configurationDictionary["SectionSubMechanismModels"] = GetSectionSubMechanismModelsConfiguration();
            configurationDictionary["Fetches"] = new List<OrderedDictionary>();
            configurationDictionary["AreaPoints"] = new List<OrderedDictionary>();
            configurationDictionary["PresentationSections"] = new List<OrderedDictionary>();
            configurationDictionary["Profiles"] = GetCalculationProfilesConfiguration();
            configurationDictionary["ForelandModels"] = new List<OrderedDictionary>();
            configurationDictionary["Forelands"] = GetForelandsConfiguration();
            configurationDictionary["ProbabilityAlternatives"] = new List<OrderedDictionary>();
            configurationDictionary["SetUpHeights"] = new List<OrderedDictionary>();
            configurationDictionary["CalcWindDirections"] = new List<OrderedDictionary>();
            configurationDictionary["Swells"] = new List<OrderedDictionary>();
            configurationDictionary["WaveReductions"] = new List<OrderedDictionary>();
            configurationDictionary["Areas"] = GetAreasConfiguration();
            configurationDictionary["Projects"] = GetProjectsConfiguration();
            configurationDictionary["Breakwaters"] = GetBreakWatersConfiguration();

            return GenerateDataBaseCreationScript(configurationDictionary);
        }

        private IList<OrderedDictionary> GetHydraulicModelsConfiguration()
        {
            return new List<OrderedDictionary>
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

        private IList<OrderedDictionary> GetSectionsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var hydraRingSection = hydraRingCalculationInput.Section;

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingSection.SectionId
                    },
                    {
                        "PresentationId", 1 // Fixed: no support for combination of multiple sections
                    },
                    {
                        "MainMechanismId", 1 // Fixed: no support for combination of multiple sections
                    },
                    {
                        "Name", hydraRingSection.SectionName
                    },
                    {
                        "Description", hydraRingSection.SectionName // Just use the section name
                    },
                    {
                        "RingCoordinateBegin", GetHydraRingValue(hydraRingSection.SectionBeginCoordinate)
                    },
                    {
                        "RingCoordinateEnd", GetHydraRingValue(hydraRingSection.SectionEndCoordinate)
                    },
                    {
                        "XCoordinate", GetHydraRingValue(hydraRingSection.CrossSectionXCoordinate)
                    },
                    {
                        "YCoordinate", GetHydraRingValue(hydraRingSection.CrossSectionYCoordinate)
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
                        "Normal", GetHydraRingValue(hydraRingSection.CrossSectionNormal)
                    },
                    {
                        "Length", GetHydraRingValue(hydraRingSection.SectionLength)
                    }
                });
            }

            return orderedDictionaries;
        }

        private IList<OrderedDictionary> GetDesignTablesConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(hydraRingCalculationInput.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingCalculationInput.Section.SectionId
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

            return orderedDictionaries;
        }

        private IList<OrderedDictionary> GetNumericsConfiguration()
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
                            "SectionId", hydraRingCalculationInput.Section.SectionId
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

            return orderDictionaries;
        }

        private IList<OrderedDictionary> GetVariableDatasConfiguration()
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
                            "SectionId", hydraRingCalculationInput.Section.SectionId
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
                                              ? GetHydraRingNullableValue(hydraRingVariable.Variability)
                                              : defaultHydraRingNullValue
                        },
                        {
                            "Parameter3", hydraRingVariable.DistributionType == HydraRingDistributionType.LogNormal
                                              ? GetHydraRingNullableValue(hydraRingVariable.Shift)
                                              : defaultHydraRingNullValue
                        },
                        {
                            "Parameter4", defaultHydraRingNullValue // Fixed: Not relevant
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

            return orderDictionaries;
        }

        private IList<OrderedDictionary> GetCalculationProfilesConfiguration()
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
                            "SectionId", hydraRingCalculationInput.Section.SectionId
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

            return orderDictionaries;
        }

        private IList<OrderedDictionary> GetForelandsConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();
            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                for (var i = 0; i < hydraRingCalculationInput.ForelandsPoints.Count(); i++)
                {
                    var forelandPoint = hydraRingCalculationInput.ForelandsPoints.ElementAt(i);

                    orderDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.Section.SectionId
                        },
                        {
                            "SequenceNumber", i + 1
                        },
                        {
                            "XCoordinate", GetHydraRingValue(forelandPoint.X)
                        },
                        {
                            "ZCoordinate", GetHydraRingValue(forelandPoint.Z)
                        }
                    });
                }
            }
            return orderDictionaries;
        }

        private IList<OrderedDictionary> GetBreakWatersConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();
            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                foreach (var breakWater in hydraRingCalculationInput.BreakWaters)
                {
                    orderedDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.Section.SectionId
                        },
                        {
                            "Type", GetHydraRingValue(breakWater.Type)
                        },
                        {
                            "Height", GetHydraRingValue(breakWater.Height)
                        }
                    });
                }
            }
            return orderedDictionaries;
        }

        private IList<OrderedDictionary> GetSectionFaultTreeModelsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (var hydraRingCalculationInput in hydraRingCalculationInputs)
            {
                var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(hydraRingCalculationInput.FailureMechanismType);

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingCalculationInput.Section.SectionId
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

            return orderedDictionaries;
        }

        private IList<OrderedDictionary> GetSectionSubMechanismModelsConfiguration()
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
                                "SectionId", hydraRingCalculationInput.Section.SectionId
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

            return orderedDictionaries;
        }

        private IList<OrderedDictionary> GetAreasConfiguration()
        {
            return new List<OrderedDictionary>
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

        private IList<OrderedDictionary> GetProjectsConfiguration()
        {
            return new List<OrderedDictionary>
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

        private static string GenerateDataBaseCreationScript(Dictionary<string, IList<OrderedDictionary>> configurationDictionary)
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

        private static double GetHydraRingValue(double value)
        {
            return !double.IsNaN(value) ? value : defaultHydraRingValue;
        }

        private double? GetHydraRingNullableValue(double value)
        {
            return !double.IsNaN(value) ? value : defaultHydraRingNullValue;
        }
    }
}