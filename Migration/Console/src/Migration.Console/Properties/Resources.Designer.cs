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

namespace Migration.Console.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Migration.Console.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Er is een verkeerd aantal parameters opgegeven voor de opdracht &apos;{0}&apos;.
        /// </summary>
        internal static string Command_0_Incorrect_number_of_parameters {
            get {
                return ResourceManager.GetString("Command_0_Incorrect_number_of_parameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geef het hulp menu weer..
        /// </summary>
        internal static string CommandHelp_Command_0_Detailed {
            get {
                return ResourceManager.GetString("CommandHelp_Command_0_Detailed", resourceCulture);
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
        ///   Looks up a localized string similar to {0} RINGTOETSBESTANDSPAD NIEUWEVERSIE UITVOERPAD.
        /// </summary>
        internal static string CommandMigrate_Command_0_Brief {
            get {
                return ResourceManager.GetString("CommandMigrate_Command_0_Brief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand dat gemigreerd moet worden. NIEUWEVERSIE is de versie naar waar gemigreerd moet worden. UITVOERPAD is het pad waar de het gemigreerde Ringtoetsbestand opgeslagen zal worden..
        /// </summary>
        internal static string CommandMigrate_Detailed {
            get {
                return ResourceManager.GetString("CommandMigrate_Detailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} RINGTOETSBESTANDSPAD.
        /// </summary>
        internal static string CommandSupported_Command_0_Brief {
            get {
                return ResourceManager.GetString("CommandSupported_Command_0_Brief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RINGTOETSBESTANDSPAD is het bestandspad naar het Ringtoetsdatabase bestand waarvan de versie gevalideerd moet worden..
        /// </summary>
        internal static string CommandSupported_Detailed {
            get {
                return ResourceManager.GetString("CommandSupported_Detailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het besturingssysteem geeft de volgende melding: {0}.
        /// </summary>
        internal static string Message_Inner_Exception_0 {
            get {
                return ResourceManager.GetString("Message_Inner_Exception_0", resourceCulture);
            }
        }
    }
}
