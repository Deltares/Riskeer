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

namespace Application.Ringtoets.Migration.Console.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Ringtoets.Migration.Console.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to {0} is geen geldige opdracht..
        /// </summary>
        internal static string CommandInvalid_Command_0_Is_not_valid {
            get {
                return ResourceManager.GetString("CommandInvalid_Command_0_Is_not_valid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MIGRATIEHULPPROGRAMMA bronprojectpad doelprojectpad.
        /// </summary>
        internal static string CommandMigrate_Brief {
            get {
                return ResourceManager.GetString("CommandMigrate_Brief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voert de migratie uit van het projectbestand dat te vinden is in het bronprojectpad en slaat het resulterende projectbestand op in het doelprojectpad..
        /// </summary>
        internal static string CommandMigrate_Detailed {
            get {
                return ResourceManager.GetString("CommandMigrate_Detailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bron- en doelprojectpad moeten geldige bestandspaden zijn..
        /// </summary>
        internal static string CommandMigrate_Source_Or_Destination_Not_Valid_Path {
            get {
                return ResourceManager.GetString("CommandMigrate_Source_Or_Destination_Not_Valid_Path", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het projectbestand &apos;{0}&apos; is succesvol gemigreerd naar &apos;{1}&apos; (versie {2})..
        /// </summary>
        internal static string CommandMigrate_Successful_Migration_From_Location_0_To_Location_1_Version_2 {
            get {
                return ResourceManager.GetString("CommandMigrate_Successful_Migration_From_Location_0_To_Location_1_Version_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MIGRATIEHULPPROGRAMMA bronprojectpad.
        /// </summary>
        internal static string CommandSupported_Brief {
            get {
                return ResourceManager.GetString("CommandSupported_Brief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Controleert of het projectbestand dat te vinden is in het bronprojectpad gemigreerd kan worden..
        /// </summary>
        internal static string CommandSupported_Detailed {
            get {
                return ResourceManager.GetString("CommandSupported_Detailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het projectbestand kan gemigreerd worden naar versie &apos;{0}&apos;..
        /// </summary>
        internal static string CommandSupported_File_Able_To_Migrate_To_Version_0 {
            get {
                return ResourceManager.GetString("CommandSupported_File_Able_To_Migrate_To_Version_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dit hulpprogramma kan worden gebruikt om een projectbestand in het formaat van een eerdere versie van Ringtoets te migreren naar het formaat van de huidige versie van Ringtoets ({0})..
        /// </summary>
        internal static string RingtoetsMigrationTool_ApplicationDescription_Version_0 {
            get {
                return ResourceManager.GetString("RingtoetsMigrationTool_ApplicationDescription_Version_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MIGRATIEHULPPROGRAMMA.
        /// </summary>
        internal static string RingtoetsMigrationTool_ApplicationName {
            get {
                return ResourceManager.GetString("RingtoetsMigrationTool_ApplicationName", resourceCulture);
            }
        }
    }
}
