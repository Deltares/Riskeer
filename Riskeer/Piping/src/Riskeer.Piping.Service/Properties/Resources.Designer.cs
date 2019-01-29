﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riskeer.Piping.Service.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Piping.Service.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Meerdere aaneengesloten watervoerende lagen gevonden. Er wordt geprobeerd de d70 en doorlatendheid van de bovenste watervoerende laag af te leiden..
        /// </summary>
        internal static string PipingCalculationService_GetInputWarnings_Multiple_aquifer_layers_Attempt_to_determine_values_for_DiameterD70_and_DarcyPermeability_from_top_layer {
            get {
                return ResourceManager.GetString("PipingCalculationService_GetInputWarnings_Multiple_aquifer_layers_Attempt_to_dete" +
                        "rmine_values_for_DiameterD70_and_DarcyPermeability_from_top_layer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn..
        /// </summary>
        internal static string PipingCalculationService_GetInputWarnings_Multiple_coverage_layers_Attempt_to_determine_value_from_combination {
            get {
                return ResourceManager.GetString("PipingCalculationService_GetInputWarnings_Multiple_coverage_layers_Attempt_to_det" +
                        "ermine_value_from_combination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rekenwaarde voor d70 ({0} m) ligt buiten het geldigheidsbereik van dit model. Geldige waarden liggen tussen 0.000063 m en 0.0005 m..
        /// </summary>
        internal static string PipingCalculationService_GetInputWarnings_Specified_DiameterD70_value_0_not_in_valid_range_of_model {
            get {
                return ResourceManager.GetString("PipingCalculationService_GetInputWarnings_Specified_DiameterD70_value_0_not_in_va" +
                        "lid_range_of_model", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_derive_DarcyPermeability", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_derive_Diameter70 {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_derive_Diameter70", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_derive_SaturatedVolumicWeight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_determine_PiezometricHeadExit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_determine_thickness_aquifer_layer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_Cannot_determine_thickness_coverage_layer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_SurfaceLine {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_aquifer_layer_at_ExitPointL_under_Surfa" +
                        "ceLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_SurfaceLine {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_coverage_layer_at_ExitPointL_under_Surf" +
                        "aceLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen ondergrondschematisatie geselecteerd..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_StochasticSoilProfile_selected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen profielschematisatie geselecteerd..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_SurfaceLine_selected {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_SurfaceLine_selected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De waarde voor &apos;intredepunt&apos; moet een concreet getal zijn..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_value_for_EntryPointL {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_value_for_EntryPointL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De waarde voor &apos;uittredepunt&apos; moet een concreet getal zijn..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_No_value_for_ExitPointL {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_No_value_for_ExitPointL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water..
        /// </summary>
        internal static string PipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_must_be_larger_than_WaterVolumetricWeight {
            get {
                return ResourceManager.GetString("PipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_must_b" +
                        "e_larger_than_WaterVolumetricWeight", resourceCulture);
            }
        }
    }
}
