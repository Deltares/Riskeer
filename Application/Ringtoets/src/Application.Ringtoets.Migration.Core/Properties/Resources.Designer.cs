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

namespace Application.Ringtoets.Migration.Core.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Ringtoets.Migration.Core.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to /* ---------------------------------------------------- */
        ////*  Generated by Enterprise Architect Version 12.0 		*/
        ////*  Created On : 15-feb-2017 11:11:55 				*/
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
        public static string DatabaseStructure17_1 {
            get {
                return ResourceManager.GetString("DatabaseStructure17_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*
        ///Migration script for migrating Ringtoets databases.
        ///SourceProject version: 5
        ///TargetProject version: 17.1
        ///*/
        ///PRAGMA foreign_keys = OFF;
        ///
        ///ATTACH DATABASE [{0}] AS SOURCEPROJECT;
        ///
        ///INSERT INTO AssessmentSectionEntity SELECT * FROM [SOURCEPROJECT].AssessmentSectionEntity;
        ///INSERT INTO CalculationGroupEntity SELECT * FROM [SOURCEPROJECT].CalculationGroupEntity;
        ///INSERT INTO CharacteristicPointEntity SELECT * FROM [SOURCEPROJECT].CharacteristicPointEntity;
        ///INSERT INTO ClosingStructureEntity SELECT * F [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Migration_5_171 {
            get {
                return ResourceManager.GetString("Migration_5_171", resourceCulture);
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
