﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wti.IO.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Wti.IO.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Het bestandspad &apos;{0}&apos; verwijst naar een map die niet bestaat..
        /// </summary>
        public static string Error_Directory_in_path_0_missing {
            get {
                return ResourceManager.GetString("Error_Directory_in_path_0_missing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand op &apos;{0}&apos; heeft op regel {1} teveel tekst om in het RAM geheugen opgeslagen te worden..
        /// </summary>
        public static string Error_File_0_contains_Line_1_too_big {
            get {
                return ResourceManager.GetString("Error_File_0_contains_Line_1_too_big", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand op &apos;{0}&apos; bestaat niet..
        /// </summary>
        public static string Error_File_0_does_not_exist {
            get {
                return ResourceManager.GetString("Error_File_0_does_not_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand op &apos;{0}&apos; is leeg..
        /// </summary>
        public static string Error_File_0_empty {
            get {
                return ResourceManager.GetString("Error_File_0_empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een onverwachte inleesfout opgetreden tijdens het lezen van het bestand &apos;{0}&apos;: {1}.
        /// </summary>
        public static string Error_General_IO_File_0_ErrorMessage_1_ {
            get {
                return ResourceManager.GetString("Error_General_IO_File_0_ErrorMessage_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet de volgende tekens bevatten: {0}.
        /// </summary>
        public static string Error_PathCannotContainCharacters_0_ {
            get {
                return ResourceManager.GetString("Error_PathCannotContainCharacters_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet leeg of ongedefinieerd zijn..
        /// </summary>
        public static string Error_PathMustBeSpecified {
            get {
                return ResourceManager.GetString("Error_PathMustBeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bestandspad mag niet naar een map verwijzen..
        /// </summary>
        public static string Error_PathMustNotPointToFolder {
            get {
                return ResourceManager.GetString("Error_PathMustNotPointToFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand op &apos;{0}&apos; heeft op regel {1} teveel tekst om in het RAM geheugen opgeslagen te worden..
        /// </summary>
        public static string Error_Unexpected_IOError_File_0_Line_1_ {
            get {
                return ResourceManager.GetString("Error_Unexpected_IOError_File_0_Line_1_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bestand op &apos;{0}&apos; is niet geschikt om dwarsdoorsneden uit te lezen (Verwachte header: locationid;X1;Y1;Z1)..
        /// </summary>
        public static string PipingSurfaceLinesCsvReader_File_0_invalid_header {
            get {
                return ResourceManager.GetString("PipingSurfaceLinesCsvReader_File_0_invalid_header", resourceCulture);
            }
        }
    }
}
