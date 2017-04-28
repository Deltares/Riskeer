﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Application.Ringtoets.Migration.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Ringtoets.Migration.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to rtd.
        /// </summary>
        internal static string RingtoetsProject_FileExtension {
            get {
                return ResourceManager.GetString("RingtoetsProject_FileExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ringtoets project.
        /// </summary>
        internal static string RingtoetsProject_TypeDescription {
            get {
                return ResourceManager.GetString("RingtoetsProject_TypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het is niet mogelijk om het Ringtoets logbestand &apos;{0}&apos; aan te maken..
        /// </summary>
        internal static string RingtoetsProjectMigrator_CreateInitializedDatabaseLogFile_Unable_to_create_migration_log_file_0 {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_CreateInitializedDatabaseLogFile_Unable_to_create_migrat" +
                        "ion_log_file_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het opruimen van het Ringtoets logbestand &apos;{0}&apos; is mislukt..
        /// </summary>
        internal static string RingtoetsProjectMigrator_Deleting_migration_log_file_0_failed {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_Deleting_migration_log_file_0_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het migreren van het projectbestand &apos;{0}&apos; is geannuleerd..
        /// </summary>
        internal static string RingtoetsProjectMigrator_GenerateMigrationCancelledLogMessage_Updating_projectfile_0_was_cancelled {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_GenerateMigrationCancelledLogMessage_Updating_projectfil" +
                        "e_0_was_cancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het project dat u wilt openen is opgeslagen in het formaat van een eerdere versie van Ringtoets.
        ///
        ///Weet u zeker dat u het bestand wilt migreren naar het formaat van uw huidige Ringtoetsversie ({0})?.
        /// </summary>
        internal static string RingtoetsProjectMigrator_Migrate_Outdated_project_file_update_to_current_version_0_inquire {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_Migrate_Outdated_project_file_update_to_current_version_" +
                        "0_inquire", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het projectbestand &apos;{0}&apos; is succesvol gemigreerd naar &apos;{1}&apos; (versie {2})..
        /// </summary>
        internal static string RingtoetsProjectMigrator_MigrateToTargetLocation_Outdated_projectfile_0_succesfully_updated_to_target_filepath_1_version_2_ {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_MigrateToTargetLocation_Outdated_projectfile_0_succesful" +
                        "ly_updated_to_target_filepath_1_version_2_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het migreren van het projectbestand &apos;{0}&apos; is mislukt: {1}.
        /// </summary>
        internal static string RingtoetsProjectMigrator_MigrateToTargetLocation_Updating_outdated_projectfile_0_failed_with_exception_1_ {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_MigrateToTargetLocation_Updating_outdated_projectfile_0_" +
                        "failed_with_exception_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Door de migratie is het project aangepast. Bekijk het migratierapport door op details te klikken.
        ///Gevolgen van de migratie:
        ///.
        /// </summary>
        internal static string RingtoetsProjectMigrator_Project_file_modified_click_details_for_migration_report {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_Project_file_modified_click_details_for_migration_report" +
                        "", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bron.
        /// </summary>
        internal static string RingtoetsProjectMigrator_Source_Descriptor {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_Source_Descriptor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Doel.
        /// </summary>
        internal static string RingtoetsProjectMigrator_Target_Descriptor {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_Target_Descriptor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}projectpad moet een geldig projectpad zijn..
        /// </summary>
        internal static string RingtoetsProjectMigrator_ValidateProjectPath_TypeDescriptor_0_filepath_must_be_a_valid_path {
            get {
                return ResourceManager.GetString("RingtoetsProjectMigrator_ValidateProjectPath_TypeDescriptor_0_filepath_must_be_a_" +
                        "valid_path", resourceCulture);
            }
        }
    }
}
