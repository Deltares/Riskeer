// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Security;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Defaults;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Settings;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.Providers;

namespace Riskeer.HydraRing.Calculation.Services
{
    /// <summary>
    /// Service for generating the database creation script that is necessary for performing Hydra-Ring calculations.
    /// The following Hydra-Ring features are not exposed (yet):
    /// <list type="bullet">
    /// <item>
    /// Combination of multiple sections
    /// </item>
    /// <item>
    /// Coupling two hydraulic boundary locations
    /// </item>
    /// <item>
    /// Performing revetment calculations (DesignTables > LayerId)
    /// </item>
    /// <item>
    /// Performing piping calculations (DesignTables > AlternativeId)
    /// </item>
    /// <item>
    /// Type III calculations (DesignTables > Method)
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
        private readonly List<HydraRingCalculationInput> hydraRingInputs = new List<HydraRingCalculationInput>();
        private readonly FailureMechanismDefaultsProvider failureMechanismDefaultsProvider = new FailureMechanismDefaultsProvider();
        private readonly VariableDefaultsProvider variableDefaultsProvider = new VariableDefaultsProvider();

        /// <summary>
        /// Creates a new instance of the <see cref="HydraRingConfigurationService"/> class.
        /// </summary>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while performing Hydra-Ring calculations.</param>
        public HydraRingConfigurationService(HydraRingUncertaintiesType uncertaintiesType)
        {
            UncertaintiesType = uncertaintiesType;
        }

        /// <summary>
        /// Gets the <see cref="HydraRingUncertaintiesType"/> to use while performing Hydra-Ring calculations.
        /// </summary>
        public HydraRingUncertaintiesType UncertaintiesType { get; }

        /// <summary>
        /// Adds Hydra-Ring calculation input to the configuration.
        /// </summary>
        /// <param name="input">The calculation input to add to the configuration.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="input"/> with 
        /// the same <see cref="HydraRingSection.SectionId"/> has already been added.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="input"/> is not unique.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="HydraRingCalculationInput.FailureMechanismType"/>
        /// is not the same with already added input.</exception>
        public void AddHydraRingCalculationInput(HydraRingCalculationInput input)
        {
            if (hydraRingInputs.Any(h => h.Section.SectionId == input.Section.SectionId))
            {
                throw new ArgumentException(@"Section id is not unique", nameof(input));
            }

            if (hydraRingInputs.Count > 0 && hydraRingInputs.First().FailureMechanismType != input.FailureMechanismType)
            {
                throw new NotSupportedException("Running calculations for multiple failure mechanism types is not supported.");
            }

            hydraRingInputs.Add(input);
        }

        /// <summary>
        /// Writes the database creation script necessary for performing Hydra-Ring calculations.
        /// </summary>
        /// <param name="databaseFilePath">The file path to write the database creation script to.</param>
        /// <exception cref="IOException">Thrown when an I/O error occurred while opening the file.</exception>
        /// <exception cref="SecurityException">Thrown when the path can't be accessed due to missing permissions.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the path can't be accessed due to missing permissions.</exception>
        public void WriteDatabaseCreationScript(string databaseFilePath)
        {
            var configurationDictionary = new Dictionary<string, IEnumerable<OrderedDictionary>>
            {
                ["HydraulicModels"] = GetHydraulicModelsConfiguration(),
                ["Sections"] = GetSectionsConfiguration(),
                ["SectionCalculationSchemes"] = GetSectionCalculationSchemesConfiguration(),
                ["DesignTables"] = GetDesignTablesConfiguration(),
                ["PreprocessorSettings"] = GetPreprocessorSettingsConfiguration(),
                ["Numerics"] = GetNumericsConfiguration(),
                ["VariableDatas"] = GetVariableDatasConfiguration(),
                ["CalculationProfiles"] = GetCalculationProfilesConfiguration(),
                ["SectionFaultTreeModels"] = GetSectionFaultTreeModelsConfiguration(),
                ["SectionSubMechanismModels"] = GetSectionSubMechanismModelsConfiguration(),
                ["Fetches"] = new List<OrderedDictionary>(),
                ["AreaPoints"] = new List<OrderedDictionary>(),
                ["PresentationSections"] = new List<OrderedDictionary>(),
                ["Profiles"] = GetProfilesConfiguration(),
                ["ForelandModels"] = GetForlandModelsConfiguration(),
                ["Forelands"] = GetForelandsConfiguration(),
                ["ProbabilityAlternatives"] = new List<OrderedDictionary>(),
                ["SetUpHeights"] = new List<OrderedDictionary>(),
                ["CalcWindDirections"] = new List<OrderedDictionary>(),
                ["Swells"] = new List<OrderedDictionary>(),
                ["WaveReductions"] = new List<OrderedDictionary>(),
                ["Areas"] = GetAreasConfiguration(),
                ["Projects"] = GetProjectsConfiguration(),
                ["Breakwaters"] = GetBreakWatersConfiguration()
            };

            File.WriteAllText(databaseFilePath, GenerateDatabaseCreationScript(configurationDictionary));
        }

        private IEnumerable<OrderedDictionary> GetHydraulicModelsConfiguration()
        {
            return new List<OrderedDictionary>
            {
                new OrderedDictionary
                {
                    {
                        "TimeIntegrationSchemeID", 1
                    },
                    {
                        "UncertaintiesID", (int) UncertaintiesType
                    },
                    {
                        "DataSetName", "WTI 2017" // Fixed: use the WTI 2017 set of hydraulic boundary locations
                    }
                }
            };
        }

        private IEnumerable<OrderedDictionary> GetSectionsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                HydraRingSection hydraRingSection = hydraRingCalculationInput.Section;

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
                        "Name", hydraRingSection.SectionId // Just use the section id
                    },
                    {
                        "Description", hydraRingSection.SectionId // Just use the section id
                    },
                    {
                        "RingCoordinateBegin", defaultHydraRingValue // No support for coordinates
                    },
                    {
                        "RingCoordinateEnd", defaultHydraRingValue // No support for coordinates
                    },
                    {
                        "XCoordinate", defaultHydraRingValue // No support for coordinates
                    },
                    {
                        "YCoordinate", defaultHydraRingValue // No support for coordinates
                    },
                    {
                        "StationId1", hydraRingCalculationInput.HydraulicBoundaryLocationId
                    },
                    {
                        "StationId2", hydraRingCalculationInput.HydraulicBoundaryLocationId // Same as "StationId1": no support for coupling two hydraulic boundary locations
                    },
                    {
                        "Relative", 100.0 // Fixed: no support for coupling two hydraulic boundary locations
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

        private IEnumerable<OrderedDictionary> GetSectionCalculationSchemesConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                TimeIntegrationSetting timeIntegrationSetting = hydraRingCalculationInput.TimeIntegrationSetting;

                orderedDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", hydraRingCalculationInput.Section.SectionId
                    },
                    {
                        "MechanismId", failureMechanismDefaults.MechanismId
                    },
                    {
                        "TimeIntegrationSchemeID", timeIntegrationSetting.TimeIntegrationSchemeId
                    }
                });
            }

            return orderedDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetDesignTablesConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);
                DesignTablesSetting designTablesSetting = hydraRingCalculationInput.DesignTablesSetting;

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
                        "TableMin", defaultHydraRingValue // Fixed: no support for type III calculations (see "Method")
                    },
                    {
                        "TableMax", defaultHydraRingValue // Fixed: no support for type III calculations (see "Method")
                    },
                    {
                        "TableStepSize", defaultHydraRingValue // Fixed: no support for type III calculations (see "Method")
                    },
                    {
                        "ValueMin", GetHydraRingValue(designTablesSetting.ValueMin)
                    },
                    {
                        "ValueMax", GetHydraRingValue(designTablesSetting.ValueMax)
                    },
                    {
                        "Beta", GetHydraRingValue(hydraRingCalculationInput.Beta)
                    }
                });
            }

            return orderedDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetPreprocessorSettingsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                PreprocessorSetting preprocessorSetting = hydraRingCalculationInput.PreprocessorSetting;

                if (preprocessorSetting.RunPreprocessor)
                {
                    orderedDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.Section.SectionId
                        },
                        {
                            "MinValueRunPreprocessor", preprocessorSetting.ValueMin
                        },
                        {
                            "MaxValueRunPreprocessor", preprocessorSetting.ValueMax
                        }
                    });
                }
            }

            return orderedDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetNumericsConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (int subMechanismId in failureMechanismDefaults.SubMechanismIds)
                {
                    NumericsSetting numericsSetting = hydraRingCalculationInput.NumericsSettings[subMechanismId];

                    orderDictionaries.Add(CreateNumericsRecord(hydraRingCalculationInput.Section.SectionId,
                                                               failureMechanismDefaults.MechanismId,
                                                               subMechanismId,
                                                               hydraRingCalculationInput.IterationMethodId,
                                                               numericsSetting));
                }

                if (hydraRingCalculationInput.PreprocessorSetting.RunPreprocessor)
                {
                    orderDictionaries.Add(CreateNumericsRecord(hydraRingCalculationInput.Section.SectionId,
                                                               failureMechanismDefaults.PreprocessorMechanismId,
                                                               failureMechanismDefaults.PreprocessorSubMechanismId,
                                                               hydraRingCalculationInput.IterationMethodId,
                                                               hydraRingCalculationInput.PreprocessorSetting.NumericsSetting));
                }
            }

            return orderDictionaries;
        }

        private static OrderedDictionary CreateNumericsRecord(int sectionId, int mechanismId, int subMechanismId, int iterationMethodId, NumericsSetting numericsSetting)
        {
            return new OrderedDictionary
            {
                {
                    "SectionId", sectionId
                },
                {
                    "MechanismId", mechanismId
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
                    "Method", numericsSetting.CalculationTechniqueId
                },
                {
                    "FormStartMethod", numericsSetting.FormStartMethod
                },
                {
                    "FormNumberOfIterations", numericsSetting.FormNumberOfIterations
                },
                {
                    "FormRelaxationFactor", GetHydraRingValue(numericsSetting.FormRelaxationFactor)
                },
                {
                    "FormEpsBeta", GetHydraRingValue(numericsSetting.FormEpsBeta)
                },
                {
                    "FormEpsHOH", GetHydraRingValue(numericsSetting.FormEpsHoh)
                },
                {
                    "FormEpsZFunc", GetHydraRingValue(numericsSetting.FormEpsZFunc)
                },
                {
                    "DsStartMethod", numericsSetting.DsStartMethod
                },
                {
                    "DsIterationmethod", iterationMethodId
                },
                {
                    "DsMinNumberOfIterations", numericsSetting.DsMinNumberOfIterations
                },
                {
                    "DsMaxNumberOfIterations", numericsSetting.DsMaxNumberOfIterations
                },
                {
                    "DsVarCoefficient", GetHydraRingValue(numericsSetting.DsVarCoefficient)
                },
                {
                    "NiUMin", GetHydraRingValue(numericsSetting.NiUMin)
                },
                {
                    "NiUMax", GetHydraRingValue(numericsSetting.NiUMax)
                },
                {
                    "NiNumberSteps", numericsSetting.NiNumberSteps
                }
            };
        }

        private IEnumerable<OrderedDictionary> GetVariableDatasConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (HydraRingVariable hydraRingVariable in hydraRingCalculationInput.Variables)
                {
                    VariableDefaults variableDefaults = variableDefaultsProvider.GetVariableDefaults(hydraRingCalculationInput.FailureMechanismType, hydraRingVariable.VariableId);

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
                            "Value", GetHydraRingValue(hydraRingVariable.Value)
                        },
                        {
                            "DistributionType", (int?) hydraRingVariable.DistributionType
                        },
                        {
                            "Parameter1", GetHydraRingValue(hydraRingVariable.Parameter1)
                        },
                        {
                            "Parameter2", GetHydraRingNullableValue(hydraRingVariable.Parameter2)
                        },
                        {
                            "Parameter3", GetHydraRingNullableValue(hydraRingVariable.Parameter3)
                        },
                        {
                            "Parameter4", GetHydraRingNullableValue(hydraRingVariable.Parameter4)
                        },
                        {
                            "DeviationType", (int?) hydraRingVariable.DeviationType
                        },
                        {
                            "CoefficientOfVariation", GetHydraRingValue(hydraRingVariable.CoefficientOfVariation)
                        },
                        {
                            "CorrelationLength", GetHydraRingValue(variableDefaults.CorrelationLength)
                        }
                    });
                }
            }

            return orderDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetProfilesConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                for (var i = 0; i < hydraRingCalculationInput.ProfilePoints.Count(); i++)
                {
                    HydraRingProfilePoint hydraRingProfilePoint = hydraRingCalculationInput.ProfilePoints.ElementAt(i);

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
                        }
                    });
                }
            }

            return orderDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetCalculationProfilesConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                for (var i = 0; i < hydraRingCalculationInput.ProfilePoints.Count(); i++)
                {
                    HydraRingProfilePoint hydraRingProfilePoint = hydraRingCalculationInput.ProfilePoints.ElementAt(i);

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

        private IEnumerable<OrderedDictionary> GetForlandModelsConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();
            foreach (HydraRingCalculationInput input in hydraRingInputs.Where(i => i.ForelandsPoints.Any() || i.BreakWater != null))
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(input.FailureMechanismType);
                orderDictionaries.Add(new OrderedDictionary
                {
                    {
                        "SectionId", input.Section.SectionId
                    },
                    {
                        "MechanismId", failureMechanismDefaults.MechanismId
                    },
                    {
                        "Model", 3
                    }
                });
            }

            return orderDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetForelandsConfiguration()
        {
            var orderDictionaries = new List<OrderedDictionary>();
            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                for (var i = 0; i < hydraRingCalculationInput.ForelandsPoints.Count(); i++)
                {
                    HydraRingForelandPoint forelandPoint = hydraRingCalculationInput.ForelandsPoints.ElementAt(i);

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

        private IEnumerable<OrderedDictionary> GetBreakWatersConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();
            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                if (hydraRingCalculationInput.BreakWater != null)
                {
                    orderedDictionaries.Add(new OrderedDictionary
                    {
                        {
                            "SectionId", hydraRingCalculationInput.Section.SectionId
                        },
                        {
                            "Type", hydraRingCalculationInput.BreakWater.Type
                        },
                        {
                            "Height", GetHydraRingValue(hydraRingCalculationInput.BreakWater.Height)
                        }
                    });
                }
            }

            return orderedDictionaries;
        }

        private IEnumerable<OrderedDictionary> GetSectionFaultTreeModelsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                orderedDictionaries.Add(CreateFaultTreeModelsRecord(hydraRingCalculationInput.Section.SectionId,
                                                                    failureMechanismDefaults.MechanismId,
                                                                    failureMechanismDefaults.FaultTreeModelId));

                if (hydraRingCalculationInput.PreprocessorSetting.RunPreprocessor)
                {
                    orderedDictionaries.Add(CreateFaultTreeModelsRecord(hydraRingCalculationInput.Section.SectionId,
                                                                        failureMechanismDefaults.PreprocessorMechanismId,
                                                                        failureMechanismDefaults.PreprocessorFaultTreeModelId));
                }
            }

            return orderedDictionaries;
        }

        private static OrderedDictionary CreateFaultTreeModelsRecord(int sectionId, int mechanismId, int faultTreeModelId)
        {
            return new OrderedDictionary
            {
                {
                    "SectionId", sectionId
                },
                {
                    "MechanismId", mechanismId
                },
                {
                    "LayerId", defaultLayerId // Fixed: no support for revetments
                },
                {
                    "AlternativeId", defaultAlternativeId // Fixed: no support for piping
                },
                {
                    "FaultTreeModelId", faultTreeModelId
                }
            };
        }

        private IEnumerable<OrderedDictionary> GetSectionSubMechanismModelsConfiguration()
        {
            var orderedDictionaries = new List<OrderedDictionary>();

            foreach (HydraRingCalculationInput hydraRingCalculationInput in hydraRingInputs)
            {
                FailureMechanismDefaults failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(hydraRingCalculationInput.FailureMechanismType);

                foreach (int subMechanismId in failureMechanismDefaults.SubMechanismIds)
                {
                    int? subMechanismModelId = hydraRingCalculationInput.GetSubMechanismModelId(subMechanismId);

                    if (subMechanismModelId != null)
                    {
                        orderedDictionaries.Add(new OrderedDictionary
                        {
                            {
                                "SectionId", hydraRingCalculationInput.Section.SectionId
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

        private static IEnumerable<OrderedDictionary> GetAreasConfiguration()
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

        private static IEnumerable<OrderedDictionary> GetProjectsConfiguration()
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

        private static string GenerateDatabaseCreationScript(Dictionary<string, IEnumerable<OrderedDictionary>> configurationDictionary)
        {
            var lines = new List<string>();

            foreach (string tableName in configurationDictionary.Keys)
            {
                lines.Add("DELETE FROM [" + tableName + "];");

                if (!configurationDictionary[tableName].Any())
                {
                    lines.Add("");

                    continue;
                }

                foreach (OrderedDictionary orderedDictionary in configurationDictionary[tableName])
                {
                    var valueStrings = new List<string>();

                    foreach (object val in orderedDictionary.Values)
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

                    string valuesString = string.Join(", ", valueStrings);

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