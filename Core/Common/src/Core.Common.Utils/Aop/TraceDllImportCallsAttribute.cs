using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using PostSharp.Aspects;

namespace Core.Common.Utils.Aop
{
    /// <summary>
    /// Place this attribute on a class that has DllImport methods to enable tracing of those calls 
    /// and the arguments used in the call. Start and stop logging using the TraceDllImportCallsConfig 
    /// class (xml format).
    /// </summary>
    [Serializable]
    public sealed class TraceDllImportCallsAttribute : OnMethodBoundaryAspect
    {
        private string methodName;
        private string[] parameterNames;
        private Type type;

        public override bool CompileTimeValidate(MethodBase method)
        {
            // trace only methods with the DllImport attribute
            return method.GetCustomAttributes(typeof(DllImportAttribute), false).Length > 0;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo
                                                                          aspectInfo)
        {
            base.CompileTimeInitialize(method, aspectInfo);
            // build cache:
            type = method.DeclaringType;
            methodName = method.Name;
            parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            // todo: check the performance overhead of this:
            if (!TraceDllImportCallsConfig.IsLoggingEnabled(type))
            {
                return;
            }

            // todo: consider using OnExit, because sometimes even on-ref parameters are used as by-ref 
            // and aren't filled in till after the call

            int count = args.Arguments.Count;
            var values = new object[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = args.Arguments.GetArgument(i);
            }

            TraceDllImportCallsConfig.Log(type, methodName, parameterNames, values);
        }
    }

    /// <summary>
    /// Configures options for the TraceDllImportCallsAttribute. Seperate class to make sure calling code 
    /// doesn't need a reference to PostSharp.
    /// </summary>
    public static class TraceDllImportCallsConfig
    {
        private static readonly Dictionary<Type, XmlTextWriter> LogWriters =
            new Dictionary<Type, XmlTextWriter>();

        public static bool IsLoggingEnabled(Type type)
        {
            XmlTextWriter writer;
            return LogWriters.TryGetValue(type, out writer);
        }

        /// <summary>
        /// Start logging (to xml) on a type with the TraceDllImportCalls attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        public static void StartLogging(Type type, string path)
        {
            var writer = new XmlTextWriter(path, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartDocument();
            writer.WriteStartElement("Run");
            writer.WriteAttributeString("type", type.Name);
            writer.WriteAttributeString("time", DateTime.Now.ToString(
                CultureInfo.InvariantCulture.DateTimeFormat.FullDateTimePattern,
                CultureInfo.InvariantCulture));
            LogWriters.Add(type, writer);
        }

        public static void StopLogging(Type type)
        {
            XmlWriter writer = LogWriters[type];
            writer.WriteEndElement(); // close <Run>
            writer.WriteEndDocument(); // close document
            writer.Close(); // close file
            LogWriters.Remove(type);
        }

        internal static void Log(Type type, string callName, string[] parameterNames,
                                 object[] parameterValues)
        {
            XmlTextWriter writer;
            if (!LogWriters.TryGetValue(type, out writer))
            {
                return;
            }

            writer.WriteStartElement(callName);
            for (int i = 0; i < parameterNames.Length; i++)
            {
                writer.WriteAttributeString(parameterNames[i],
                                            ToString(parameterValues[i]));
            }
            writer.WriteEndElement();
        }

        private static string ToString(object value)
        {
            var valueAsString = value as string;
            if (valueAsString != null)
            {
                return valueAsString;
            }

            var enuerable = value as IEnumerable;
            if (enuerable != null)
            {
                var sb = new StringBuilder();
                foreach (var item in enuerable)
                {
                    sb.Append(item);
                    sb.Append(" ");
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1); // remove last space
                }
                return sb.ToString();
            }

            return value.ToString();
        }
    }
}