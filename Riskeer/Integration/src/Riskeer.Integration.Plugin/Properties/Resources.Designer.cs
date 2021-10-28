﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
        ///   Looks up a localized string similar to gml.
        /// </summary>
        public static string AssemblyResult_file_filter_Extension {
            get {
                return ResourceManager.GetString("AssemblyResult_file_filter_Extension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap AssemblyResultPerSection {
            get {
                object obj = ResourceManager.GetObject("AssemblyResultPerSection", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assemblagekaart.
        /// </summary>
        public static string AssemblyResultPerSection_Map_DisplayName {
            get {
                return ResourceManager.GetString("AssemblyResultPerSection_Map_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        public static System.Drawing.Bitmap AssemblyResultPerSectionMap {
            get {
                object obj = ResourceManager.GetObject("AssemblyResultPerSectionMap", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assemblage.
        /// </summary>
        public static string AssemblyResultsCategoryTreeFolder_DisplayName {
            get {
                return ResourceManager.GetString("AssemblyResultsCategoryTreeFolder_DisplayName", resourceCulture);
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
        ///   Looks up a localized string similar to Er zijn geen instellingen gevonden voor het geselecteerde traject. Standaardinstellingen zullen gebruikt worden..
        /// </summary>
        public static string AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_No_settings_found_for_AssessmentSection {
            get {
                return ResourceManager.GetString("AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_No_settings_found" +
                        "_for_AssessmentSection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het traject kan niet aangemaakt worden met een ondergrens van {0} en een signaleringswaarde van {1}. De waarde van de ondergrens en signaleringswaarde moet in het bereik {2} liggen en de ondergrens moet gelijk zijn aan of groter zijn dan de signaleringswaarde..
        /// </summary>
        public static string AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_LowerLimitNorm_0_and_SignalingNorm_1_Norms_should_be_in_Range_2_ {
            get {
                return ResourceManager.GetString("AssessmentSectionFromFileCommandHandler_Unable_to_create_assessmentSection_with_L" +
                        "owerLimitNorm_0_and_SignalingNorm_1_Norms_should_be_in_Range_2_", resourceCulture);
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
        ///   Looks up a localized string similar to Gegevens van toetsspoor &apos;{0}&apos; zijn vervangen..
        /// </summary>
        public static string AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replaced {
            get {
                return ResourceManager.GetString("AssessmentSectionMergeHandler_TryMergeFailureMechanism_FailureMechanism_0_replace" +
                        "d", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er zijn geen trajecten gevonden die samengevoegd kunnen worden..
        /// </summary>
        public static string AssessmentSectionMerger_No_matching_AssessmentSections {
            get {
                return ResourceManager.GetString("AssessmentSectionMerger_No_matching_AssessmentSections", resourceCulture);
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
        ///   Looks up a localized string similar to Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm en semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd.
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
        ///   Looks up a localized string similar to Als u de norm aanpast, dan worden de rekenresultaten van semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd.
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
        ///   Looks up a localized string similar to Generieke faalpaden.
        /// </summary>
        public static string GenericFailurePaths_DisplayName {
            get {
                return ResourceManager.GetString("GenericFailurePaths_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische belastingendatabase.
        /// </summary>
        public static string HydraulicBoundaryDatabase_file_filter_Description {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabase_file_filter_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sqlite.
        /// </summary>
        public static string HydraulicBoundaryDatabase_FilePath_Extension {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabase_FilePath_Extension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to U heeft een ander hydraulische belastingendatabase bestand geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden.
        ///
        ///Wilt u doorgaan?.
        /// </summary>
        public static string HydraulicBoundaryDatabaseUpdateHandler_Confirm_clear_hydraulicBoundaryDatabase_dependent_data {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseUpdateHandler_Confirm_clear_hydraulicBoundaryDatabase_de" +
                        "pendent_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Importeren van het HLCD bestand.
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImportHandler_ImportHydraulicLocationConfigurationSettings_Description {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImportHandler_ImportHydraulicLocationConfig" +
                        "urationSettings_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Als u het gekoppelde HLCD bestand wijzigt, zal de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden.
        ///
        ///Wilt u doorgaan?.
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseUpdateHandler_Confirm_clear_hydraulicLocationConfigurationDatabase_dependent_data {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseUpdateHandler_Confirm_clear_hydraulicLocati" +
                        "onConfigurationDatabase_dependent_data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; in het HLCD bestand is niet aanwezig. Er worden standaardwaarden conform WBI2017 gebruikt voor de HLCD bestandsinformatie..
        /// </summary>
        public static string HydraulicLocationConfigurationSettingsUpdateHelper_ReadHydraulicLocationConfigurationDatabaseSettings_No_ScenarioInformation_entries_present {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationSettingsUpdateHelper_ReadHydraulicLocationConfigura" +
                        "tionDatabaseSettings_No_ScenarioInformation_entries_present", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Na het importeren van een aangepaste ligging van de referentielijn zullen alle geïmporteerde en berekende gegevens van alle toetssporen worden gewist.
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
        ///   Looks up a localized string similar to Faalpad toevoegen.
        /// </summary>
        public static string RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath {
            get {
                return ResourceManager.GetString("RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voeg een nieuw faalpad toe aan deze map..
        /// </summary>
        public static string RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath_Tooltip {
            get {
                return ResourceManager.GetString("RiskeerPlugin_ContextMenuStrip_Add_SpecificFailurePath_Tooltip", resourceCulture);
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
        ///   Looks up a localized string similar to Faalpaden /
        ///assemblage.
        /// </summary>
        public static string RiskeerPlugin_GetStateInfos_FailurePaths {
            get {
                return ResourceManager.GetString("RiskeerPlugin_GetStateInfos_FailurePaths", resourceCulture);
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
        ///   Looks up a localized string similar to Als u voorlandprofielen importeert, dan worden de  resultaten van alle berekeningen in dit toetsspoor die voorlandprofielen gebruiken verwijderd.
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
        ///   Looks up a localized string similar to Specifieke faalpaden.
        /// </summary>
        public static string SpecificFailurePaths_DisplayName {
            get {
                return ResourceManager.GetString("SpecificFailurePaths_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to zip archief.
        /// </summary>
        public static string Zip_file_filter_Description {
            get {
                return ResourceManager.GetString("Zip_file_filter_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to zip.
        /// </summary>
        public static string Zip_file_filter_Extension {
            get {
                return ResourceManager.GetString("Zip_file_filter_Extension", resourceCulture);
            }
        }
    }
}
