﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Service.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Common.Service.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekening is beëindigd..
        /// </summary>
        public static string Calculation_ended {
            get {
                return ResourceManager.GetString("Calculation_ended", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Berekening is gestart..
        /// </summary>
        public static string Calculation_started {
            get {
                return ResourceManager.GetString("Calculation_started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een onverwachte fout opgetreden tijdens het uitvoeren van de berekening..
        /// </summary>
        public static string CalculationService_Calculate_unexpected_error {
            get {
                return ResourceManager.GetString("CalculationService_Calculate_unexpected_error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fout bij het uitlezen van de illustratiepunten voor berekening {0}: {1} Het uitlezen van illustratiepunten wordt overgeslagen..
        /// </summary>
        public static string CalculationService_Error_in_reading_illustrationPoints_for_CalculationName_0_with_ErrorMessage_1 {
            get {
                return ResourceManager.GetString("CalculationService_Error_in_reading_illustrationPoints_for_CalculationName_0_with" +
                        "_ErrorMessage_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan de waterstand niet afleiden op basis van de invoer..
        /// </summary>
        public static string CalculationService_ValidateInput_Cannot_determine_AssessmentLevel {
            get {
                return ResourceManager.GetString("CalculationService_ValidateInput_Cannot_determine_AssessmentLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen dijkprofiel geselecteerd..
        /// </summary>
        public static string CalculationService_ValidateInput_No_dike_profile_selected {
            get {
                return ResourceManager.GetString("CalculationService_ValidateInput_No_dike_profile_selected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen hydraulische belastingenlocatie geselecteerd..
        /// </summary>
        public static string CalculationService_ValidateInput_No_hydraulic_boundary_location_selected {
            get {
                return ResourceManager.GetString("CalculationService_ValidateInput_No_hydraulic_boundary_location_selected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstand berekening voor locatie &apos;{0}&apos; ({1}) is niet geconvergeerd..
        /// </summary>
        public static string DesignWaterLevelCalculationActivity_DesignWaterLevelCalculation_for_HydraulicBoundaryLocation_0_CalculationIdentifier_1_not_converged {
            get {
                return ResourceManager.GetString("DesignWaterLevelCalculationActivity_DesignWaterLevelCalculation_for_HydraulicBoun" +
                        "daryLocation_0_CalculationIdentifier_1_not_converged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstand berekening is uitgevoerd op de tijdelijke locatie &apos;{0}&apos;. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden..
        /// </summary>
        public static string DesignWaterLevelCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0 {
            get {
                return ResourceManager.GetString("DesignWaterLevelCalculationService_Calculate_Calculation_temporary_directory_can_" +
                        "be_found_on_location_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden tijdens de waterstand berekening voor locatie &apos;{0}&apos; ({1}). Bekijk het foutrapport door op details te klikken.
        ///{2}.
        /// </summary>
        public static string DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_CalculationIdentifier_1_click_details_for_last_error_report_2 {
            get {
                return ResourceManager.GetString("DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation" +
                        "_0_CalculationIdentifier_1_click_details_for_last_error_report_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden tijdens de waterstand berekening voor locatie &apos;{0}&apos; ({1}). Er is geen foutrapport beschikbaar..
        /// </summary>
        public static string DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation_0_CalculationIdentifier_1_no_error_report {
            get {
                return ResourceManager.GetString("DesignWaterLevelCalculationService_Calculate_Error_in_DesignWaterLevelCalculation" +
                        "_0_CalculationIdentifier_1_no_error_report", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstand berekenen voor locatie &apos;{0}&apos; ({1}).
        /// </summary>
        public static string DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_HydraulicBoundaryLocation_0_CalculationIdentifier_1_ {
            get {
                return ResourceManager.GetString("DesignWaterLevelCalculationService_Name_Calculate_assessment_level_for_HydraulicB" +
                        "oundaryLocation_0_CalculationIdentifier_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. {0}.
        /// </summary>
        public static string Hydraulic_boundary_database_connection_failed_0_ {
            get {
                return ResourceManager.GetString("Hydraulic_boundary_database_connection_failed_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De versie van de corresponderende hydraulische belastingendatabase wijkt af van de versie zoals gevonden in het bestand &apos;{0}&apos;..
        /// </summary>
        public static string Hydraulic_boundary_database_mismatching_version_in_file_0_ {
            get {
                return ResourceManager.GetString("Hydraulic_boundary_database_mismatching_version_in_file_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De waarde voor &apos;{0}&apos; moet een concreet getal zijn..
        /// </summary>
        public static string NumericInputRule_Value_of_0_must_be_a_valid_number {
            get {
                return ResourceManager.GetString("NumericInputRule_Value_of_0_must_be_a_valid_number", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uitvoeren van berekening &apos;{0}&apos;.
        /// </summary>
        public static string Perform_calculation_with_name_0_ {
            get {
                return ResourceManager.GetString("Perform_calculation_with_name_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfcondities berekenen voor &apos;{0}&apos;.
        /// </summary>
        public static string Perform_wave_conditions_calculation_with_name_0_ {
            get {
                return ResourceManager.GetString("Perform_wave_conditions_calculation_with_name_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De verwachtingswaarde voor &apos;{0}&apos; moet een concreet getal zijn..
        /// </summary>
        public static string ProbabilisticDistributionValidationRule_Mean_of_0_must_be_a_valid_number {
            get {
                return ResourceManager.GetString("ProbabilisticDistributionValidationRule_Mean_of_0_must_be_a_valid_number", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De verwachtingswaarde voor &apos;{0}&apos; moet een positief getal zijn..
        /// </summary>
        public static string ProbabilisticDistributionValidationRule_Mean_of_0_must_be_positive_value {
            get {
                return ResourceManager.GetString("ProbabilisticDistributionValidationRule_Mean_of_0_must_be_positive_value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De standaardafwijking voor &apos;{0}&apos; moet groter zijn dan of gelijk zijn aan 0..
        /// </summary>
        public static string ProbabilisticDistributionValidationRule_StandardDeviation_of_ParameterName_0_must_be_larger_or_equal_to_zero {
            get {
                return ResourceManager.GetString("ProbabilisticDistributionValidationRule_StandardDeviation_of_ParameterName_0_must" +
                        "_be_larger_or_equal_to_zero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De variatiecoëfficiënt voor &apos;{0}&apos; moet groter zijn dan of gelijk zijn aan 0..
        /// </summary>
        public static string ProbabilistiDistributionValidationRule_CoefficientOfVariation_of_ParameterName_0_must_be_larger_or_equal_to_zero {
            get {
                return ResourceManager.GetString("ProbabilistiDistributionValidationRule_CoefficientOfVariation_of_ParameterName_0_" +
                        "must_be_larger_or_equal_to_zero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het uitlezen van illustratiepunten is mislukt..
        /// </summary>
        public static string SetGeneralResult_Converting_IllustrationPointResult_Failed {
            get {
                return ResourceManager.GetString("SetGeneralResult_Converting_IllustrationPointResult_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen kunstwerk geselecteerd..
        /// </summary>
        public static string StructuresCalculationService_ValidateInput_No_Structure_selected {
            get {
                return ResourceManager.GetString("StructuresCalculationService_ValidateInput_No_Structure_selected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validatie is beëindigd..
        /// </summary>
        public static string Validation_ended {
            get {
                return ResourceManager.GetString("Validation_ended", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De waarde voor &apos;hoogte&apos; van de dam moet een concreet getal zijn..
        /// </summary>
        public static string Validation_Invalid_BreakWaterHeight_value {
            get {
                return ResourceManager.GetString("Validation_Invalid_BreakWaterHeight_value", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validatie is gestart..
        /// </summary>
        public static string Validation_started {
            get {
                return ResourceManager.GetString("Validation_started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogte berekening voor locatie &apos;{0}&apos; ({1}) is niet geconvergeerd..
        /// </summary>
        public static string WaveHeightCalculationActivity_WaveHeightCalculation_for_HydraulicBoundaryLocation_0_CalculationIdentifier_1_not_converged {
            get {
                return ResourceManager.GetString("WaveHeightCalculationActivity_WaveHeightCalculation_for_HydraulicBoundaryLocation" +
                        "_0_CalculationIdentifier_1_not_converged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogte berekening is uitgevoerd op de tijdelijke locatie &apos;{0}&apos;. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden..
        /// </summary>
        public static string WaveHeightCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0 {
            get {
                return ResourceManager.GetString("WaveHeightCalculationService_Calculate_Calculation_temporary_directory_can_be_fou" +
                        "nd_on_location_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie &apos;{0}&apos; ({1}). Bekijk het foutrapport door op details te klikken.
        ///{2}.
        /// </summary>
        public static string WaveHeightCalculationService_Calculate_Error_in_WaveHeightCalculation_0_CalculationIdentifier_1_click_details_for_last_error_report_2 {
            get {
                return ResourceManager.GetString("WaveHeightCalculationService_Calculate_Error_in_WaveHeightCalculation_0_Calculati" +
                        "onIdentifier_1_click_details_for_last_error_report_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie &apos;{0}&apos; ({1}). Er is geen foutrapport beschikbaar..
        /// </summary>
        public static string WaveHeightCalculationService_Calculate_Error_in_WaveHeightCalculation_0_CalculationIdentifier_1_no_error_report {
            get {
                return ResourceManager.GetString("WaveHeightCalculationService_Calculate_Error_in_WaveHeightCalculation_0_Calculati" +
                        "onIdentifier_1_no_error_report", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogte berekenen voor locatie &apos;{0}&apos; ({1}).
        /// </summary>
        public static string WaveHeightCalculationService_Name_Calculate_wave_height_for_HydraulicBoundaryLocation_0_CalculationIdentifier_1_ {
            get {
                return ResourceManager.GetString("WaveHeightCalculationService_Name_Calculate_wave_height_for_HydraulicBoundaryLoca" +
                        "tion_0_CalculationIdentifier_1_", resourceCulture);
            }
        }
    }
}
