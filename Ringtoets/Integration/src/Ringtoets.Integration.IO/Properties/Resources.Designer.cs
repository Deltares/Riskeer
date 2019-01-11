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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ringtoets.Integration.IO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ringtoets.Integration.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Veiligheidsoordeel is (deels) gebaseerd op handmatig ingevoerde toetsoordelen. Tijdens het exporteren worden handmatig ingevoerde toetsoordelen genegeerd..
        /// </summary>
        internal static string AssemblyExporter_CheckManualAssembly_Assembly_result_contains_manual_results_exporter_will_ignore_manual_results {
            get {
                return ResourceManager.GetString("AssemblyExporter_CheckManualAssembly_Assembly_result_contains_manual_results_expo" +
                        "rter_will_ignore_manual_results", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Er is geen toetsoordeel geëxporteerd..
        /// </summary>
        internal static string AssemblyExporter_Error_Exception_0_no_AssemblyResult_exported {
            get {
                return ResourceManager.GetString("AssemblyExporter_Error_Exception_0_no_AssemblyResult_exported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Om een toetsoordeel te kunnen exporteren moet voor alle vakken een resultaat zijn gespecificeerd..
        /// </summary>
        internal static string AssemblyExporter_LogErrorMessage_Only_possible_to_export_a_complete_AssemblyResult {
            get {
                return ResourceManager.GetString("AssemblyExporter_LogErrorMessage_Only_possible_to_export_a_complete_AssemblyResul" +
                        "t", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kon het rekeninstellingen bestand niet openen. {0}.
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file_0_ {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file" +
                        "_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen hydraulische belastingen database gekoppeld..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_Hydrauli" +
                        "cBoundaryDatabase_imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; in het HLCD bestand moet exact 1 rij bevatten..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_HLCD_Invalid_number_of_ScenarioInformation_entries {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_Invalid_number_of_ScenarioInformation_entr" +
                        "ies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; in het HLCD bestand is niet aanwezig, er worden standaard waarden conform WBI2017 voor de HLCD bestand informatie gebruikt..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_HLCD_No_ScenarioInformation_entries_present {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_No_ScenarioInformation_entries_present", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hydraulische belastingen database koppelen afgebroken. Geen gegevens gewijzigd..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het hydraulische locatie configuratie bestand..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het hydraulische belastingen bestand..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inlezen van het rekeninstellingen bestand..
        /// </summary>
        internal static string HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file {
            get {
                return ResourceManager.GetString("HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Er is geen HLCD geïmporteerd..
        /// </summary>
        internal static string HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_" +
                        "0_No_HydraulicLocationConfigurationDatabase_imported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het HLCD bestand is niet gevonden in dezelfde map als het HRD bestand..
        /// </summary>
        internal static string HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De tabel &apos;ScenarioInformation&apos; moet exact 1 rij bevatten..
        /// </summary>
        internal static string HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformation_entries {
            get {
                return ResourceManager.GetString("HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformat" +
                        "ion_entries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assemblage.
        /// </summary>
        internal static string SerializableAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bp.
        /// </summary>
        internal static string SerializableAssessmentProcess_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssessmentProcess_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wks.
        /// </summary>
        internal static string SerializableAssessmentSection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableAssessmentSection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gto.
        /// </summary>
        internal static string SerializableCombinedFailureMechanismSectionAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableCombinedFailureMechanismSectionAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ts.
        /// </summary>
        internal static string SerializableFailureMechanismCreator_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismCreator_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tv.
        /// </summary>
        internal static string SerializableFailureMechanismSection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to T.
        /// </summary>
        internal static string SerializableFailureMechanismSectionAssembly_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSectionAssembly_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vi.
        /// </summary>
        internal static string SerializableFailureMechanismSectionCollection_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableFailureMechanismSectionCollection_IdPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vo.
        /// </summary>
        internal static string SerializableTotalAssemblyResult_IdPrefix {
            get {
                return ResourceManager.GetString("SerializableTotalAssemblyResult_IdPrefix", resourceCulture);
            }
        }
    }
}
