﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core.Common.Utils.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Core.Common.Utils.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Het bestandspad verwijst naar een map die niet bestaat..
        /// </summary>
        public static string Error_Directory_missing {
            get {
                return ResourceManager.GetString("Error_Directory_missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand bestaat niet..
        /// </summary>
        public static string Error_File_does_not_exist {
            get {
                return ResourceManager.GetString("Error_File_does_not_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand is leeg..
        /// </summary>
        public static string Error_File_empty {
            get {
                return ResourceManager.GetString("Error_File_empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een onverwachte fout opgetreden tijdens het inlezen van het bestand: {0}.
        /// </summary>
        public static string Error_General_IO_ErrorMessage_0_ {
            get {
                return ResourceManager.GetString("Error_General_IO_ErrorMessage_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet de volgende tekens bevatten: {0}.
        /// </summary>
        public static string Error_Path_cannot_contain_Characters_0_ {
            get {
                return ResourceManager.GetString("Error_Path_cannot_contain_Characters_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet leeg of ongedefinieerd zijn..
        /// </summary>
        public static string Error_Path_must_be_specified {
            get {
                return ResourceManager.GetString("Error_Path_must_be_specified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet naar een map verwijzen..
        /// </summary>
        public static string Error_Path_must_not_point_to_folder {
            get {
                return ResourceManager.GetString("Error_Path_must_not_point_to_folder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fout bij het schrijven naar bestand &apos;{0}&apos;: {1}.
        /// </summary>
        public static string Error_Writing_To_File_0_1 {
            get {
                return ResourceManager.GetString("Error_Writing_To_File_0_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is geen geldige expressie voor deze methode..
        /// </summary>
        public static string TypeUtils_GetMemberName_0_is_not_a_valid_expression_for_this_method {
            get {
                return ResourceManager.GetString("TypeUtils_GetMemberName_0_is_not_a_valid_expression_for_this_method", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;member&apos; is geen geldige expressie voor deze methode..
        /// </summary>
        public static string TypeUtils_GetMemberNameFromMemberExpression_member_not_a_valid_expression_for_this_method {
            get {
                return ResourceManager.GetString("TypeUtils_GetMemberNameFromMemberExpression_member_not_a_valid_expression_for_thi" +
                        "s_method", resourceCulture);
            }
        }
    }
}
