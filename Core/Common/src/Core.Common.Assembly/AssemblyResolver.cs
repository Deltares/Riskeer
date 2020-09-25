using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Common.Assembly
{
    /// <summary>
    /// Static class that tries to resolve any missing assemblies from the directory that
    /// is provided during initialization. The contents of the assembly directory will be
    /// iterated recursively up until the point where one or more assemblies are found.
    /// </summary>
    /// <remarks>When multiple assemblies are found with the same name, only the last one
    /// will actually be resolved at runtime.</remarks>
    /// <example>
    /// Given the following assembly directory structure:
    /// <list type="bullet">
    ///   <item>
    ///     <list type="bullet">
    ///       <listheader>Directory</listheader>
    ///       <item>
    ///         <list type="bullet">
    ///           <listheader>Directory</listheader>
    ///           <item>
    ///             <description>Assembly_1.dll</description>
    ///           </item>
    ///         </list>b
    ///       </item>
    ///       <item>
    ///         <list type="bullet">
    ///           <listheader>Directory</listheader>
    ///           <item>
    ///             <description>Assembly_2.dll</description>
    ///           </item>
    ///           <item>
    ///             <description>Assembly_3.dll *</description>
    ///           </item>
    ///         </list>
    ///       </item>
    ///       <item>
    ///         <list type="bullet">
    ///           <listheader>Directory</listheader>
    ///           <item>
    ///             <description>Assembly_4.dll</description>
    ///           </item>
    ///           <item>
    ///             <list type="bullet">
    ///               <listheader>Directory</listheader>
    ///               <item>
    ///                 <description>Assembly_5.dll</description>
    ///               </item>
    ///             </list>
    ///           </item>
    ///         </list>
    ///       </item>
    ///       <item>
    ///         <list type="bullet">
    ///           <listheader>Directory</listheader>
    ///           <item>
    ///             <description>Assembly_3.dll **</description>
    ///           </item>
    ///           <item>
    ///             <description>Assembly_6.dll</description>
    ///           </item>
    ///         </list>
    ///       </item>
    ///     </list>
    ///   </item>
    /// </list>
    /// Then the following assemblies will be resolved:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Assembly_1.dll</description>
    ///   </item>
    ///   <item>
    ///     <description>Assembly_2.dll</description>
    ///   </item>
    ///   <item>
    ///     <description>Assembly_3.dll **</description>
    ///   </item>
    ///   <item>
    ///     <description>Assembly_4.dll</description>
    ///   </item>
    ///   <item>
    ///     <description>Assembly_6.dll</description>
    ///   </item>
    /// </list>
    /// </example>
    public static class AssemblyResolver
    {
        private const string assemblySearchPattern = "*.dll";

        private static Dictionary<string, string> assemblyLookup = new Dictionary<string, string>();

        private static bool initialized;

        /// <summary>
        /// Gets whether or not <see cref="AssemblyResolver"/> requires initialization.
        /// </summary>
        public static bool RequiresInitialization => !initialized;

        /// <summary>
        /// Initializes the <see cref="AssemblyResolver"/>.
        /// </summary>
        /// <param name="assemblyDirectory">The directory to recursively resolve any missing assemblies from.</param>
        /// <exception cref="InvalidOperationException">Thrown when the <see cref="AssemblyResolver"/>
        /// is already initialized.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when <paramref name="assemblyDirectory"/> cannot
        /// be found.</exception>
        /// <remarks>
        /// After successfully performing this method, <see cref="RequiresInitialization"/> equals <c>false</c>.
        /// </remarks>
        public static void Initialize(string assemblyDirectory)
        {
            if (initialized)
            {
                throw new InvalidOperationException("Cannot initialize the assembly resolver more than once.");
            }

            if (!Directory.Exists(assemblyDirectory))
            {
                throw new DirectoryNotFoundException($"Cannot find the directory '{assemblyDirectory}'.");
            }

            initialized = true;

            InitializeAssemblyLookup(assemblyDirectory);

            AppDomain.CurrentDomain.AssemblyResolve += LoadFileFromAssemblyLookup;
        }

        /// <summary>
        /// Resets the <see cref="AssemblyResolver"/>.
        /// </summary>
        /// <remarks>
        /// After performing this method, <see cref="RequiresInitialization"/> equals <c>true</c>.
        /// </remarks>
        public static void Reset()
        {
            initialized = false;

            assemblyLookup = new Dictionary<string, string>();

            AppDomain.CurrentDomain.AssemblyResolve -= LoadFileFromAssemblyLookup;
        }

        private static void InitializeAssemblyLookup(string assemblyDirectory)
        {
            string[] assemblies = Directory.GetFiles(assemblyDirectory, assemblySearchPattern);

            foreach (string directory in Directory.GetDirectories(assemblyDirectory))
            {
                InitializeAssemblyLookup(directory);
            }

            foreach (string assembly in assemblies)
            {
                assemblyLookup[Path.GetFileNameWithoutExtension(assembly)] = assembly;
            }
        }

        private static System.Reflection.Assembly LoadFileFromAssemblyLookup(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(','));

            if (assemblyLookup.TryGetValue(assemblyName, out string assemblyFile))
            {
                return System.Reflection.Assembly.LoadFile(assemblyFile);
            }

            return null;
        }
    }
}