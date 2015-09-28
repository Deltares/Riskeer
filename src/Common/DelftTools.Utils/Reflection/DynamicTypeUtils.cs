using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DelftTools.Utils.Reflection
{
    public static class DynamicTypeUtils
    {
        public static Type CreateDynamicEnum(string enumName, int[] values, string[] descriptions, string[] displayNames=null)
        {
            if (values.Length != descriptions.Length || (displayNames != null && values.Length != displayNames.Length))
                throw new ArgumentException("Number of values / descriptions / displayNames does not match");

            if (displayNames == null)
                displayNames = values.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray();
            
            var dynamicAssembly = CreateOrGetDynamicAssembly();

            var existingType = dynamicAssembly.GetType(enumName);
            if (existingType != null)
            {
                if (existingType.IsEnum && Enum.GetValues(existingType).Length == values.Length)
                    return existingType;

                throw new InvalidOperationException("An enum with that name, but with different fields, is already defined");
            }

            var enumBuilder = dynamicAssembly.DefineEnum(enumName, TypeAttributes.Public, typeof(int));

            //add attribute on enum: [TypeConverter(typeof(EnumDescriptionAttributeTypeConverter))]
            enumBuilder.SetCustomAttribute(CreateAttribute<TypeConverterAttribute, Type>(typeof (EnumDescriptionAttributeTypeConverter)));

            // add all enum values
            for (var i = 0; i < values.Length; i++)
            {
                var fb = enumBuilder.DefineLiteral("val" + i, values[i]);
                
                //add [Description("<description>")] attribute
                fb.SetCustomAttribute(CreateAttribute<DisplayNameAttribute, string>(displayNames[i]));
                fb.SetCustomAttribute(CreateAttribute<DescriptionAttribute, string>(descriptions[i]));
            }
            
            return enumBuilder.CreateType();
            // for testing purposes, save to disk to be inspected with ILSpy:
            //dynamicAssembly.Save("enumdll_debug.dll");
        }
        
        private static CustomAttributeBuilder CreateAttribute<TAttrib, TArg>(TArg arg)
        {
            var ci = typeof(TAttrib).GetConstructor(new[] { typeof(TArg) });
            return new CustomAttributeBuilder(ci, new object[] { arg });
        }

        private static ModuleBuilder existingDynamicAssembly;
        private static ModuleBuilder CreateOrGetDynamicAssembly()
        {
            if (existingDynamicAssembly != null)
                return existingDynamicAssembly;

            var assemblyName = new AssemblyName("DynamicTypeUtilsAssembly");
            var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            existingDynamicAssembly = dynamicAssembly.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            return existingDynamicAssembly;
        }
    }
}