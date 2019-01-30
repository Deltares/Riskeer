// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Migration.Core.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Riskeer.Migration.Core.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Bronprojectpad moet een geldig bestandspad zijn..
        /// </summary>
        public static string CommandSupported_Source_Not_Valid_Path {
            get {
                return ResourceManager.GetString("CommandSupported_Source_Not_Valid_Path", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kritieke fout opgetreden bij het uitlezen van het Ringtoets logbestand van de migratie..
        /// </summary>
        public static string MigrationLogDatabaseReader_GetMigrationLogMessages_failed {
            get {
                return ResourceManager.GetString("MigrationLogDatabaseReader_GetMigrationLogMessages_failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het aanmaken van het Ringtoets projectbestand met versie &apos;{0}&apos; is mislukt..
        /// </summary>
        public static string RingtoetsCreateScript_Creating_Version_0_Failed {
            get {
                return ResourceManager.GetString("RingtoetsCreateScript_Creating_Version_0_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand &apos;{0}&apos; moet een geldig Ringtoets projectbestand zijn..
        /// </summary>
        public static string RingtoetsDatabaseSourceFile_Invalid_Ringtoets_File_Path_0 {
            get {
                return ResourceManager.GetString("RingtoetsDatabaseSourceFile_Invalid_Ringtoets_File_Path_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het migreren van het Ringtoets projectbestand van versie &apos;{0}&apos; naar &apos;{1}&apos; is mislukt..
        /// </summary>
        public static string RingtoetsUpgradeScript_Upgrading_Version_0_To_Version_1_Failed {
            get {
                return ResourceManager.GetString("RingtoetsUpgradeScript_Upgrading_Version_0_To_Version_1_Failed", resourceCulture);
            }
        }
    }
}
