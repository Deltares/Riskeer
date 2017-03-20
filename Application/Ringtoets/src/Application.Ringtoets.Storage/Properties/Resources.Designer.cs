﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Ringtoets.Storage.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to /* ---------------------------------------------------- */
        ////*  Generated by Enterprise Architect Version 12.0 		*/
        ////*  Created On : 06-Feb-2017 08:19:11 				*/
        ////*  DBMS       : SQLite 								*/
        ////* ---------------------------------------------------- */
        ///
        ////* Drop Tables */
        ///
        ///DROP TABLE IF EXISTS &apos;VersionEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;GrassCoverErosionInwardsDikeHeightOutputEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;ProjectEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;StabilityPointStructuresCalculationEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXI [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DatabaseStructure {
            get {
                return ResourceManager.GetString("DatabaseStructure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden in de verbinding met het Ringtoets bestand..
        /// </summary>
        internal static string Error_during_connection {
            get {
                return ResourceManager.GetString("Error_during_connection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een fout is opgetreden tijdens het opslaan van het Ringtoets bestand..
        /// </summary>
        internal static string Error_saving_database {
            get {
                return ResourceManager.GetString("Error_saving_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een fout is opgetreden met schrijven naar het nieuwe Ringtoets bestand..
        /// </summary>
        internal static string Error_writing_structure_to_database {
            get {
                return ResourceManager.GetString("Error_writing_structure_to_database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een kritieke fout voorkomt dat een vingerafdruk van de projectdata gemaakt kan worden..
        /// </summary>
        internal static string FingerprintHelper_Critical_error_message {
            get {
                return ResourceManager.GetString("FingerprintHelper_Critical_error_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ringtoetsproject (*.rtd)|*.rtd.
        /// </summary>
        internal static string Ringtoets_project_file_filter {
            get {
                return ResourceManager.GetString("Ringtoets_project_file_filter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan geen tijdelijk bestand maken van het originele bestand ({0})..
        /// </summary>
        internal static string SafeOverwriteFileHelper_CreateNewTemporaryFile_Cannot_create_temporary_FilePath_0_Try_change_save_location {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_CreateNewTemporaryFile_Cannot_create_temporary_FilePath_0" +
                        "_Try_change_save_location", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan het tijdelijke bestand ({0}) niet opruimen. Het tijdelijke bestand dient handmatig verwijderd te worden..
        /// </summary>
        internal static string SafeOverwriteFileHelper_DeleteTemporaryFile_Cannot_remove_temporary_FilePath_0_Try_removing_manually {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_DeleteTemporaryFile_Cannot_remove_temporary_FilePath_0_Tr" +
                        "y_removing_manually", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er bestaat al een tijdelijk bestand ({0}) dat niet verwijderd kan worden. Dit bestand dient handmatig verwijderd te worden..
        /// </summary>
        internal static string SafeOverwriteFileHelper_RemoveAlreadyExistingTemporaryFile_Already_existing_temporary_file_at_FilePath_0_could_not_be_removed {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_RemoveAlreadyExistingTemporaryFile_Already_existing_tempo" +
                        "rary_file_at_FilePath_0_could_not_be_removed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld te worden..
        /// </summary>
        internal static string SafeOverwriteFileHelper_RestoreOriginalFile_Cannot_revert_to_original_FilePath_0_Try_reverting_manually {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_RestoreOriginalFile_Cannot_revert_to_original_FilePath_0_" +
                        "Try_reverting_manually", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het project bevat te veel unieke objecten om een digitale vingerafdruk van te genereren..
        /// </summary>
        internal static string StorageSqLite_HasStagedProjectChanges_Project_contains_too_many_objects_to_generate_fingerprint {
            get {
                return ResourceManager.GetString("StorageSqLite_HasStagedProjectChanges_Project_contains_too_many_objects_to_genera" +
                        "te_fingerprint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand is geen geldig Ringtoets bestand..
        /// </summary>
        internal static string StorageSqLite_LoadProject_Invalid_Ringtoets_database_file {
            get {
                return ResourceManager.GetString("StorageSqLite_LoadProject_Invalid_Ringtoets_database_file", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database moet één rij in de VersionEntity tabel hebben..
        /// </summary>
        internal static string StorageSqLite_ValidateDatabaseVersion_Database_must_have_one_VersionEntity_row {
            get {
                return ResourceManager.GetString("StorageSqLite_ValidateDatabaseVersion_Database_must_have_one_VersionEntity_row", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ringtoets bestand versie &apos;{0}&apos; is hoger dan de huidig ondersteunde versie (&apos;{1}&apos;). Update Ringtoets naar een nieuwere versie..
        /// </summary>
        internal static string StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_higher_then_current_DatabaseVersion_1_ {
            get {
                return ResourceManager.GetString("StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_higher_then_current_Datab" +
                        "aseVersion_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ringtoets bestand versie &apos;{0}&apos; is niet valide. De versie van het Ringtoets projectbestand dient &apos;16.4&apos; of hoger te zijn..
        /// </summary>
        internal static string StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_is_invalid {
            get {
                return ResourceManager.GetString("StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_is_invalid", resourceCulture);
            }
        }
    }
}
