using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DelftTools.Utils.Xml.Serialization
{
    /// <summary>
    /// Serializes objects into a xml file using custom xml read and write functions. <para/>
    /// It serializes using more generic elements than standard xml serializer. It also serializes object type into xml. 
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public class CustomXmlSerializer<ItemType> : IXmlSerializable where ItemType : class
    {
        #region Private Members

        /// <summary>
        /// Holds the object that this serializer operates on.
        /// </summary>
        private ItemType obj;

        private static Dictionary<Type, XmlSerializer> serializers; // optimization step, preload all type serializers

        #endregion

        #region Static Methods

        /// <summary>
        /// Implicit operators the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static implicit operator CustomXmlSerializer<ItemType>(ItemType p)
        {
            //create custom serializer based on ItemType=BaseClass
            return p == null ? null : new CustomXmlSerializer<ItemType>(p);
        }

        /// <summary>
        /// Implicit operators the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static implicit operator
            ItemType(CustomXmlSerializer<ItemType> p)
        {
            return p.Equals(default(ItemType)) ? default(ItemType) : p.Parameters;
        }

        #endregion Static

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXmlSerializer{ItemType}"/> class.
        /// </summary>
        public CustomXmlSerializer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXmlSerializer{ItemType}"/> class.
        /// </summary>
        /// <param name="obj">The parameters.</param>
        public CustomXmlSerializer(ItemType obj) : this()
        {
            this.obj = obj;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public ItemType Parameters
        {
            get { return obj; }
        }

        #endregion Properties

        #region IXmlSerializable Implementation

        /// <summary>
        /// Returns schema of the XML document representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            // Get type from xml attribute
            String typeName = reader.GetAttribute("type");
            Type type = FindType(typeName);

            if (type == null)
            {
                throw new ArgumentOutOfRangeException("Can not create type for: " + reader.GetAttribute("type") + 
                    ". Make sure that plugin containing that type is loaded (check log file)");
            }

            // Deserialize
            reader.ReadStartElement();

            XmlSerializer serializer = CreateXmlSerializer(type, typeof (ItemType));
            obj = (ItemType) serializer.Deserialize(reader);

            reader.ReadEndElement();
        }

        private static Type FindType(string typeName)
        {
            Type type = null;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (typeName.Contains("`"))
                {
                    if(!typeName.Contains("`1"))
                    {
                        throw new NotSupportedException("Only generic types with one argument are supported now");
                    }

                    string genericTypeName = typeName.Split('[')[0];
                    string genericTypeArgumentName = typeName.Split('[')[1].TrimEnd(']');

                    if ((type = a.GetType(genericTypeName)) != null)
                    {
                        Type argumentType = Type.GetType(genericTypeArgumentName);
                        if(argumentType == null)
                        {
                            throw new Exception("Can't instantiate generic type argument: " + genericTypeArgumentName);
                        }

                        type = type.MakeGenericType(argumentType);
                        
                        break;
                    }
                }

                else
                {
                    if ((type = a.GetType(typeName)) != null)
                    {
                        break;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            // Write type as xml attribute
            writer.WriteAttributeString("type", obj.GetType().ToString());

            XmlSerializer serializer = CreateXmlSerializer(obj.GetType(), typeof (ItemType));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); 
            serializer.Serialize(writer, obj, ns);
        }

        public static XmlSerializer CreateXmlSerializer(string typeName)
        {
            Type type = FindType(typeName);
            return CreateXmlSerializer(type, typeof (ItemType));
        }

        public static XmlSerializer CreateXmlSerializer(Type type, Type baseType)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            AddAttributeOverrides(baseType, type, overrides);

            Type derivedType = type;

            while (!derivedType.Equals(baseType))
            {
                //AddAttributeOverrides(baseType, derivedType, overrides);
                AddXmlIgnoreForPropertiesWithoutXmlAttribute(derivedType, overrides);
                derivedType = derivedType.BaseType;
            }

            AddXmlIgnoreForPropertiesWithoutXmlAttribute(derivedType, overrides);

            // create serializers if they still do not exist
            XmlSerializer serializer;
            if (serializers == null)
            {
                serializers = new Dictionary<Type, XmlSerializer>();
            }
            if (serializers.ContainsKey(type))
            {
                serializer = serializers[type];
            }
            else
            {
                if (type.IsGenericTypeDefinition)
                {
                    return null; // no support for xml serialization of generic types yet
                }

                serializer = new XmlSerializer(type, overrides);
                serializers[type] = serializer;
            }

            return serializer;
        }

        /// <summary>
        /// Loops to the inheritance chain of an object, takes xml attributes from base
        /// objects and applies the overrides to the derived object
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="derivedType"></param>
        /// <param name="overrides"></param>
        public static void AddAttributeOverrides(Type baseType, Type derivedType, XmlAttributeOverrides overrides)
        {
            //loop, starting with derived type
            Type type = derivedType;

            do
            {
                //override attributes detected the next basetype
                type = type.BaseType;
                if (type == null)
                {
                    break;
                }
                PropertyInfo[] props = baseType.GetProperties();
                foreach (PropertyInfo pi in props)
                {
                    //do not override attribute again if it already has override
                    XmlAttributes overrideAttributes = overrides[derivedType, pi.Name];
                    if (overrideAttributes == null)
                    {
                        Object[] attributes = pi.GetCustomAttributes(true);

                        foreach (Object a in attributes)
                        {
                            XmlAttributes attrs = new XmlAttributes();
                            if (a is XmlAttributeAttribute)
                            {
                                attrs.XmlAttribute = a as XmlAttributeAttribute;
                            }
                            else if (a is XmlElementAttribute)
                            {
                                attrs.XmlElements.Add(a as XmlElementAttribute);
                            }
                            else if (a is XmlIgnoreAttribute)
                            {
                                attrs.XmlIgnore = true;
                            }
                            else if (a is XmlArrayAttribute)
                            {
                                attrs.XmlArray = a as XmlArrayAttribute;
                            }
                            else if (a is XmlArrayItemAttribute)
                            {
                                attrs.XmlArrayItems.Add(a as XmlArrayItemAttribute);
                            }
                            else
                            {
                                continue;
                            }


                            bool exist = false; // first check if attribute does not exist

                            // iterate over all properties because there can be property which hides type of the base class
                            PropertyInfo[] properties = derivedType.GetProperties();
                            foreach (PropertyInfo pi2 in properties)
                            {
                                if (pi2.Name == pi.Name && pi2.Name != "Item")
                                {
                                    Object[] attrs2 = derivedType.GetProperty(pi2.Name, pi2.PropertyType).GetCustomAttributes(false);
                                    foreach (Object a2 in attrs2)
                                    {
                                        if (a2.Equals(a))
                                        {
                                            exist = true;
                                        }
                                    }
                                }
                            }

                            if (!exist) // override
                            {
                                overrides.Add(derivedType, pi.Name, attrs);
                            }
                        }
                    }
                }
            } while (!type.Equals(baseType));
        }

        private static void AddXmlIgnoreForPropertiesWithoutXmlAttribute(Type derivedType,
                                                                         XmlAttributeOverrides overrides)
        {
            PropertyInfo[] props = derivedType.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                //if (pi.Name.Equals("ValueXml")) {
                //    string iets=pi.Name; }

                XmlAttributes overrideAttributes = overrides[derivedType, pi.Name];
                if (overrideAttributes == null)
                {
                    Object[] attributes = pi.GetCustomAttributes(true);
                    bool addIgnoreAttribute;
                    if (attributes.Length == 0)
                    {
                        addIgnoreAttribute = true;
                    }
                    else
                    {
                        addIgnoreAttribute = true;
                        foreach (object a in attributes)
                        {
                            if ((a is XmlAttributeAttribute) |
                                (a is XmlElementAttribute) |
                                (a is XmlIgnoreAttribute) |
                                (a is XmlArrayAttribute) |
                                (a is XmlArrayItemAttribute))
                            {
                                addIgnoreAttribute = false;
                                break;
                            }
                        }
                    }
                    if (addIgnoreAttribute)
                    {
                        XmlAttributes attrs = new XmlAttributes();
                        attrs.XmlIgnore = true;
                        overrides.Add(derivedType, pi.Name, attrs);
                    }
                }
            }
        }

        #endregion IXmlSerializable Implementation
    }
}