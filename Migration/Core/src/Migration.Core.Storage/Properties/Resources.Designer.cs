﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
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

namespace Migration.Core.Storage.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Migration.Core.Storage.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to /* ---------------------------------------------------- */
        ////*  Generated by Enterprise Architect Version 16.1 		*/
        ////*  Created On : 24-jun-2024 07:58:24 				*/
        ////*  DBMS       : SQLite 								*/
        ////* ---------------------------------------------------- */
        ///
        ////* Drop Tables */
        ///
        ///DROP TABLE IF EXISTS VersionEntity
        ///;
        ///
        ///DROP TABLE IF EXISTS ProjectEntity
        ///;
        ///
        ///DROP TABLE IF EXISTS AssessmentSectionEntity
        ///;
        ///
        ///DROP TABLE IF EXISTS FailureMechanismEntity
        ///;
        ///
        ///DROP TABLE IF EXISTS FailureMechanismSectionEnt [rest of string was truncated]&quot;;.
        /// </summary>
        public static string DatabaseStructure {
            get {
                return ResourceManager.GetString("DatabaseStructure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het migreren van een projectbestand met versie &apos;{0}&apos; naar versie &apos;{1}&apos; is niet ondersteund..
        /// </summary>
        public static string Migrate_From_Version_0_To_Version_1_Not_Supported {
            get {
                return ResourceManager.GetString("Migrate_From_Version_0_To_Version_1_Not_Supported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het doelprojectpad moet anders zijn dan het bronprojectpad..
        /// </summary>
        public static string Migrate_Target_File_Path_Must_Differ_From_Source_File_Path {
            get {
                return ResourceManager.GetString("Migrate_Target_File_Path_Must_Differ_From_Source_File_Path", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het gemigreerde projectbestand is aangemaakt op &apos;{0}&apos;, maar er is een onverwachte fout opgetreden tijdens het verplaatsen naar &apos;{1}&apos;..
        /// </summary>
        public static string Migrate_Unable_To_Move_From_Location_0_To_Location_1 {
            get {
                return ResourceManager.GetString("Migrate_Unable_To_Move_From_Location_0_To_Location_1", resourceCulture);
            }
        }
    }
}
