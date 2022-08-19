﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.Integration.IO.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Integration.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to {0} Er zijn geen assemblageresultaten geëxporteerd..
        /// </summary>
        public static string AssemblyExporter_Error_Exception_0_no_AssemblyResult_exported {
            get {
                return ResourceManager.GetString("AssemblyExporter_Error_Exception_0_no_AssemblyResult_exported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De assemblage kan niet succesvol worden afgerond. Inspecteer de resultaten van de faalmechanismen die onderdeel zijn van de assemblage of het veiligheidsoordeel voor meer details..
        /// </summary>
        public static string AssemblyExporter_No_AssemblyResult_exported_Check_results_for_details {
            get {
                return ResourceManager.GetString("AssemblyExporter_No_AssemblyResult_exported_Check_results_for_details", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Om assemblageresultaten te kunnen exporteren moeten alle specifieke faalmechanismen die onderdeel zijn van de assemblage een unieke naam hebben..
        /// </summary>
        public static string AssemblyExporter_Specific_failure_mechanisms_must_have_a_unique_name {
            get {
                return ResourceManager.GetString("AssemblyExporter_Specific_failure_mechanisms_must_have_a_unique_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to assemblage.
        /// </summary>
        public static string ExportableAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("ExportableAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bp.
        /// </summary>
        public static string ExportableAssessmentProcess_IdPrefix {
            get {
                return ResourceManager.GetString("ExportableAssessmentProcess_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wks.
        /// </summary>
        public static string ExportableAssessmentSection_IdPrefix {
            get {
                return ResourceManager.GetString("ExportableAssessmentSection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vo.
        /// </summary>
        public static string ExportableTotalAssemblyResult_IdPrefix {
            get {
                return ResourceManager.GetString("ExportableTotalAssemblyResult_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon het rekeninstellingen bestand niet openen. {0}.
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file_0_ {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file" +
                        "_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen hydraulische belastingen database gekoppeld..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_Hydrauli" +
                        "cBoundaryDatabase_imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; in het HLCD bestand moet exact 1 rij bevatten..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_HLCD_Invalid_number_of_ScenarioInformation_entries {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_Invalid_number_of_ScenarioInformation_entr" +
                        "ies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bijbehorende preprocessor closure bestand is niet gevonden in dezelfde map als het HLCD bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_PreprocessorClosure_sqlite_Not_Found {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_PreprocessorClosure_sqlite_Not_Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het hydraulische locatie configuratie bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het hydraulische belastingen bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het rekeninstellingen bestand..
        /// </summary>
        public static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h.
        /// </summary>
        public static string HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaterLevel_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaterLevel_" +
                        "DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hs.
        /// </summary>
        public static string HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaveHeight_DisplayName {
            get {
                return ResourceManager.GetString("HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaveHeight_" +
                        "DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen HLCD geïmporteerd..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_" +
                        "0_No_HydraulicLocationConfigurationDatabase_imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het HLCD bestand is niet gevonden in dezelfde map als het HRD bestand..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1 of meerdere locaties komen niet voor in de HLCD..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImporter_Invalid_locationIds {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_Invalid_locationIds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; moet exact 1 rij bevatten..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformation_entries {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformat" +
                        "ion_entries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HLCD bestand importeren afgebroken. Geen gegevens gewijzigd..
        /// </summary>
        public static string HydraulicLocationConfigurationDatabaseImporter_ProgressText_Import_canceled_No_data_changed {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_ProgressText_Import_canceled_No_da" +
                        "ta_changed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assemblage.
        /// </summary>
        public static string SerializableAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bp.
        /// </summary>
        public static string SerializableAssessmentProcess_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssessmentProcess_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wks.
        /// </summary>
        public static string SerializableAssessmentSection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssessmentSection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gf.
        /// </summary>
        public static string SerializableCombinedFailureMechanismSectionAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableCombinedFailureMechanismSectionAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fm.
        /// </summary>
        public static string SerializableFailureMechanismCreator_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismCreator_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bv.
        /// </summary>
        public static string SerializableFailureMechanismSection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fa.
        /// </summary>
        public static string SerializableFailureMechanismSectionAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSectionAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vi.
        /// </summary>
        public static string SerializableFailureMechanismSectionCollection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSectionCollection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vo.
        /// </summary>
        public static string SerializableTotalAssemblyResult_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableTotalAssemblyResult_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waterstanden.
        /// </summary>
        public static string WaterLevels_DisplayName {
            get {
                return ResourceManager.GetString("WaterLevels_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Golfhoogten.
        /// </summary>
        public static string WaveHeights_DisplayName {
            get {
                return ResourceManager.GetString("WaveHeights_DisplayName", resourceCulture);
            }
        }
    }
}
