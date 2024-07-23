﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.Integration.Plugin.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Integration.Plugin.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Assemblage.
        /// </summary>
        public static string AssemblyResults_DisplayName {
            get {
                return ResourceManager.GetString("AssemblyResults_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to gml.
        /// </summary>
        public static string AssemblyResults_file_filter_Extension {
            get {
                return ResourceManager.GetString("AssemblyResults_file_filter_Extension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap AssemblyResultTotal {
            get {
                object obj = ResourceManager.GetObject("AssemblyResultTotal", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voer alle berekeningen binnen dit traject uit..
        /// </summary>
        public static string AssessmentSection_Calculate_All_ToolTip {
            get {
                return ResourceManager.GetString("AssessmentSection_Calculate_All_ToolTip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het importeren van de referentielijn is mislukt..
        /// </summary>
        public static string AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_Importing_ReferenceLineFailed {
            get {
                return ResourceManager.GetString("AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_Importing_Referen" +
                        "ceLineFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het traject kan niet aangemaakt worden met een omgevingswaarde van {0} en een signaleringsparameter van {1}. De waarde van de omgevingswaarde en signaleringsparameter moet in het bereik {2} liggen en de omgevingswaarde moet gelijk zijn aan of groter zijn dan de signaleringsparameter..
        /// </summary>
        public static string AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_MaximumAllowableFloodingProbability_0_and_SignalFloodingProbability_1_Probabilities_should_be_in_Range_2_ {
            get {
                return ResourceManager.GetString("AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_M" +
                        "aximumAllowableFloodingProbability_0_and_SignalFloodingProbability_1_Probabiliti" +
                        "es_should_be_in_Range_2_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er kunnen geen trajecten gelezen worden uit het shapebestand..
        /// </summary>
        public static string AssessmentSectionFromFileCommandHandler_ValidateReferenceLineMetas_No_referenceLines_in_file {
            get {
                return ResourceManager.GetString("AssessmentSectionFromFileCommandHandler_ValidateReferenceLineMetas_No_referenceLi" +
                        "nes_in_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische belastingen zijn samengevoegd..
        /// </summary>
        public static string AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_merged {
            get {
                return ResourceManager.GetString("AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLo" +
                        "cations_merged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische belastingen zijn niet samengevoegd omdat het huidige traject meer gegevens bevat..
        /// </summary>
        public static string AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLocations_not_merged {
            get {
                return ResourceManager.GetString("AssessmentSectionMergeHandler_MergeHydraulicBoundaryLocations_HydraulicBoundaryLo" +
                        "cations_not_merged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Faalmechanisme &apos;{0}&apos; en de bijbehorende gegevens zijn toegevoegd aan de lijst van specifieke faalmechanismen..
        /// </summary>
        public static string AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_added {
            get {
                return ResourceManager.GetString("AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_added", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gegevens van het generieke faalmechanisme &apos;{0}&apos; zijn vervangen..
        /// </summary>
        public static string AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replaced {
            get {
                return ResourceManager.GetString("AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replace" +
                        "d", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is geen traject gevonden dat samengevoegd kan worden..
        /// </summary>
        public static string AssessmentSectionMerger_No_matching_AssessmentSection {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_No_matching_AssessmentSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Samenvoegen van trajectinformatie is mislukt..
        /// </summary>
        public static string AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_failed {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Samenvoegen van trajectinformatie is gestart..
        /// </summary>
        public static string AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_started {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Samenvoegen van trajectinformatie is gelukt..
        /// </summary>
        public static string AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_successful {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_PerformMerge_Merging_AssessmentSections_successful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een onverwachte fout opgetreden tijdens het samenvoegen van de trajecten..
        /// </summary>
        public static string AssessmentSectionMerger_PerformMerge_Unexpected_error_occurred_during_merge {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_PerformMerge_Unexpected_error_occurred_during_merge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &amp;Selecteren....
        /// </summary>
        public static string BackgroundData_SelectMapData {
            get {
                return ResourceManager.GetString("BackgroundData_SelectMapData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Selecteer een achtergrondkaart..
        /// </summary>
        public static string BackgroundData_SelectMapData_Tooltip {
            get {
                return ResourceManager.GetString("BackgroundData_SelectMapData_Tooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Opmerkingen.
        /// </summary>
        public static string Comment_DisplayName {
            get {
                return ResourceManager.GetString("Comment_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HRD bestand &amp;toevoegen....
        /// </summary>
        public static string ContextMenuStrip_Add_HydraulicBoundaryDatabase {
            get {
                return ResourceManager.GetString("ContextMenuStrip_Add_HydraulicBoundaryDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voeg een nieuw HRD bestand toe aan deze map..
        /// </summary>
        public static string ContextMenuStrip_Add_HydraulicBoundaryDatabase_ToolTip {
            get {
                return ResourceManager.GetString("ContextMenuStrip_Add_HydraulicBoundaryDatabase_ToolTip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De resultaten van {0} semi-probabilistische berekeningen zonder handmatige waterstand zijn verwijderd..
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_ClearAllNormDependentSemiProbabilisticCalculationOutput_Results_of_NumberOfCalculations_0_calculations_cleared {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_ClearAllNormDependentSemiProbabilis" +
                        "ticCalculationOutput_Results_of_NumberOfCalculations_0_calculations_cleared", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Alle berekende hydraulische belastingen behorende bij de gewijzigde norm zijn verwijderd..
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_ClearNormDependingHydraulicBoundaryLocationCalculationOutput_Calculation_results_cleared {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_ClearNormDependingHydraulicBoundary" +
                        "LocationCalculationOutput_Calculation_results_cleared", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_hydraulic_calculations_data {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_depen" +
                        "dent_hydraulic_calculations_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm en semi-probabilistische berekeningen zonder handmatige waterstand verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_hydraulic_calculations_data_and_semi_probabilistic_data {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_depen" +
                        "dent_hydraulic_calculations_data_and_semi_probabilistic_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u de norm aanpast, dan worden de rekenresultaten van semi-probabilistische berekeningen zonder handmatige waterstand verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_semi_probabilistic_data {
            get {
                return ResourceManager.GetString("FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_depen" +
                        "dent_semi_probabilistic_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap Foreshore {
            get {
                object obj = ResourceManager.GetObject("Foreshore", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voorlandprofiellocaties.
        /// </summary>
        public static string ForeshoreProfilesImporter_DisplayName {
            get {
                return ResourceManager.GetString("ForeshoreProfilesImporter_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generieke faalmechanismen.
        /// </summary>
        public static string GenericFailureMechanisms_DisplayName {
            get {
                return ResourceManager.GetString("GenericFailureMechanisms_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische databases.
        /// </summary>
        public static string HydraulicBoundaryData_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicBoundaryData_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HRD bestand.
        /// </summary>
        public static string HydraulicBoundaryDatabase_file_filter_Description {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabase_file_filter_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HRD bestanden.
        /// </summary>
        public static string HydraulicBoundaryDatabases_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabases_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sqlite.
        /// </summary>
        public static string HydraulicDatabase_FilePath_Extension {
            get {
                return ResourceManager.GetString("HydraulicDatabase_FilePath_Extension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HLCD bestand.
        /// </summary>
        public static string HydraulicLocationConfigurationDatabase_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabase_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HLCD bestand.
        /// </summary>
        public static string HydraulicLocationConfigurationDatabase_file_filter_Description {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabase_file_filter_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u het geselecteerde HLCD bestand wijzigt, zal de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden.
        ///
        ///Wilt u doorgaan?.
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseUpdateHandler_Confirm_clear_hydraulic_location_configuration_database_dependent_data {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseUpdateHandler_Confirm_clear_hydraulic_locat" +
                        "ion_configuration_database_dependent_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; in het HLCD bestand is niet aanwezig..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseUpdateHelper_ReadHydraulicLocationConfigurationDatabase_No_ScenarioInformation_entries_present {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseUpdateHelper_ReadHydraulicLocationConfigura" +
                        "tionDatabase_No_ScenarioInformation_entries_present", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van alle faalmechanismen worden gewist.
        ///
        ///Wilt u doorgaan?.
        /// </summary>
        public static string ReferenceLineUpdateHandler_Confirm_clear_referenceLine_dependent_data {
            get {
                return ResourceManager.GetString("ReferenceLineUpdateHandler_Confirm_clear_referenceLine_dependent_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Achtergrondkaart.
        /// </summary>
        public static string RiskeerPlugin_BackgroundDataContext_Text {
            get {
                return ResourceManager.GetString("RiskeerPlugin_BackgroundDataContext_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Faalmechanisme &amp;toevoegen.
        /// </summary>
        public static string RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism {
            get {
                return ResourceManager.GetString("RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voeg een nieuw faalmechanisme toe aan deze map..
        /// </summary>
        public static string RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism_Tooltip {
            get {
                return ResourceManager.GetString("RiskeerPlugin_ContextMenuStrip_Add_SpecificFailureMechanism_Tooltip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Traject.
        /// </summary>
        public static string RiskeerPlugin_GetStateInfos_AssessmentSection {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetStateInfos_AssessmentSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sterkte-
        ///berekeningen.
        /// </summary>
        public static string RiskeerPlugin_GetStateInfos_Calculations {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetStateInfos_Calculations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische
        ///belastingen.
        /// </summary>
        public static string RiskeerPlugin_GetStateInfos_HydraulicLoads {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetStateInfos_HydraulicLoads", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registratie
        ///en assemblage.
        /// </summary>
        public static string RiskeerPlugin_GetStateInfos_Registration {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetStateInfos_Registration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u dit HRD bestand verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd. Ook worden alle referenties naar de bijbehorende hydraulische belastingenlocaties verwijderd uit de invoer van de belastingenberekeningen voor bekledingen en de invoer van de sterkteberekeningen.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_HydraulicBoundaryDatabase {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_HydraulicBoundaryDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u deze HRD bestanden verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd. Ook worden alle referenties naar de bijbehorende hydraulische belastingenlocaties verwijderd uit de invoer van de sterkteberekeningen.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_HydraulicBoundaryDatabases {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_HydraulicBoundaryDatabases", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u deze doelkansen verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbabilities {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbabilities", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u deze doelkans verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbability {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetTreeNodeInfos_Confirm_remove_TargetProbability", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u voorlandprofielen importeert, dan worden de  resultaten van alle berekeningen in dit faalmechanisme die voorlandprofielen gebruiken verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_VerifyForeshoreProfileUpdates_When_importing_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm {
            get {
                return ResourceManager.GetString("RiskeerPlugin_VerifyForeshoreProfileUpdates_When_importing_ForeshoreProfile_defin" +
                        "itions_assigned_to_calculations_output_will_be_cleared_confirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als voorlandprofielen wijzigen door het bijwerken, dan worden de resultaten van berekeningen die deze voorlandprofielen gebruiken verwijderd.
        ///
        ///Weet u zeker dat u wilt doorgaan?.
        /// </summary>
        public static string RiskeerPlugin_VerifyForeshoreProfileUpdates_When_updating_ForeshoreProfile_definitions_assigned_to_calculations_output_will_be_cleared_confirm {
            get {
                return ResourceManager.GetString("RiskeerPlugin_VerifyForeshoreProfileUpdates_When_updating_ForeshoreProfile_defini" +
                        "tions_assigned_to_calculations_output_will_be_cleared_confirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Het bestand &apos;{0}&apos; bestaat niet..
        /// </summary>
        public static string RiskeerPlugin_VerifyHydraulicLocationConfigurationDatabaseFilePath_File_0_not_found {
            get {
                return ResourceManager.GetString("RiskeerPlugin_VerifyHydraulicLocationConfigurationDatabaseFilePath_File_0_not_fou" +
                        "nd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifieke faalmechanismen.
        /// </summary>
        public static string SpecificFailureMechanisms_DisplayName {
            get {
                return ResourceManager.GetString("SpecificFailureMechanisms_DisplayName", resourceCulture);
            }
        }
    }
}
