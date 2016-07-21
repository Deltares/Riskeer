﻿//------------------------------------------------------------------------------
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
        ////*  Created On : 21-jul-2016 11:30:42 				*/
        ////*  DBMS       : SQLite 								*/
        ////* ---------------------------------------------------- */
        ///
        ////* Drop Tables */
        ///
        ///DROP TABLE IF EXISTS &apos;VersionEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;PipingFailureMechanismMetaEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;ProjectEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;AssessmentSectionEntity&apos;
        ///;
        ///
        ///DROP TABLE IF EXISTS &apos;FailureMechanismSectionEnti [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DatabaseStructure {
            get {
                return ResourceManager.GetString("DatabaseStructure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden in de verbinding met het Ringtoets bestand..
        /// </summary>
        internal static string Error_During_Connection {
            get {
                return ResourceManager.GetString("Error_During_Connection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het object &apos;{0}&apos; met id &apos;{1}&apos; is niet gevonden..
        /// </summary>
        internal static string Error_Entity_Not_Found_0_1 {
            get {
                return ResourceManager.GetString("Error_Entity_Not_Found_0_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een fout is opgetreden met het updaten van het Ringtoets bestand..
        /// </summary>
        internal static string Error_Update_Database {
            get {
                return ResourceManager.GetString("Error_Update_Database", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een fout is opgetreden met schrijven naar het nieuwe Ringtoets bestand..
        /// </summary>
        internal static string Error_Write_Structure_to_Database {
            get {
                return ResourceManager.GetString("Error_Write_Structure_to_Database", resourceCulture);
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
        ///   Looks up a localized string similar to Er bestaat al een tijdelijk bestand ({0}) dat niet verwijderd kan worden. Het bestaande tijdelijke bestand dient handmatig verwijderd te worden..
        /// </summary>
        internal static string SafeOverwriteFileHelper_RemoveAlreadyExistingTemporaryFile_Already_existing_temporary_file_at_FilePath_0_could_not_be_removed {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_RemoveAlreadyExistingTemporaryFile_Already_existing_tempo" +
                        "rary_file_at_FilePath_0_could_not_be_removed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld worden..
        /// </summary>
        internal static string SafeOverwriteFileHelper_RestoreOriginalFile_Cannot_revert_to_original_FilePath_0_Try_reverting_manually {
            get {
                return ResourceManager.GetString("SafeOverwriteFileHelper_RestoreOriginalFile_Cannot_revert_to_original_FilePath_0_" +
                        "Try_reverting_manually", resourceCulture);
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
    }
}
