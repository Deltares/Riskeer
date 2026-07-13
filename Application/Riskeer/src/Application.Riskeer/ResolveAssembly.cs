using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Application.Riskeer
{
    internal static class AssemblyResolver
    {
        static string root = Path.Combine(
            AppContext.BaseDirectory,
            @"Application\Built-in\Managed");
        
        internal static Assembly ResolveAssembly(
            object sender,
            ResolveEventArgs args)
        {
            Console.WriteLine($"looking for assembly {args.Name}");
            var name = new AssemblyName(args.Name);

            string file = Directory
                          .EnumerateFiles(
                              root,
                              $"{name.Name}.dll",
                              SearchOption.AllDirectories)
                          .FirstOrDefault();

            if (file != null)
            {
                Console.WriteLine($"Loading {name.Name} from {file}");
                return Assembly.LoadFrom(file);
            }

            return null;
        }
        
    }
}