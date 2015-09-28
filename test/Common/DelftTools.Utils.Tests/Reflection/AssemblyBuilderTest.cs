using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Reflection
{
    public abstract class A
    {
        public abstract string GetToolWindowName();
    }

    [TestFixture]
    public class AssemblyBuilderTest
    {
        [Test]
        public void WriteAssembly()
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "HelloReflectionEmit";
            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, "Generated.dll");

            TypeBuilder typeBuilder =
                moduleBuilder.DefineType("DelftTools.Utils.Reflection.B", TypeAttributes.Public | TypeAttributes.Class, typeof (A));

            typeBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(typeof (GuidAttribute).GetConstructor(new Type[] {typeof (string)}),
                                           new object[] {"527A38E0-3484-465e-AC45-5FC2BE51FB78"}));

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("GetToolWindowName",
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                         typeof (string), new Type[] {});

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldstr, "Hello!");
            ilGenerator.Emit(OpCodes.Ret);

            Type type = typeBuilder.CreateType();

            assemblyBuilder.Save("Generated.dll");

            object obj = Activator.CreateInstance(type);
            MethodInfo getToolWindowName = type.GetMethod("GetToolWindowName");
            object retval = getToolWindowName.Invoke(obj, null);

            Trace.Assert(retval.Equals("Hello!"));
        }
    }
}