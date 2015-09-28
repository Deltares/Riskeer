using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DelftTools.Utils.Xml.Serialization
{

    /// <summary>
    /// Utility class that offers some routines to serialize to/from xml
    /// </summary>
    public static class SerializationUtils
    {
        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <remark>http://www.dotnetjohn.com/articles.aspx?articleid=173http://www.dotnetjohn.com/articles.aspx?articleid=173</remark>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }


        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <remark>http://www.dotnetjohn.com/articles.aspx?articleid=173http://www.dotnetjohn.com/articles.aspx?articleid=173</remark>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }


        /// <summary>
        /// Method to convert a custom Object to XML string
        /// </summary>
        /// <remark>http://www.dotnetjohn.com/articles.aspx?articleid=173http://www.dotnetjohn.com/articles.aspx?articleid=173</remark>
        /// <param name="pObject">Object that is to be serialized to XML</param>
        /// <returns>XML string</returns>
        /// <param name="type">type of object being serialized</param>
        public static String SerializeObject(Object pObject, Type type)
        {
            try
            {
                String XmlizedString;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(type);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xs.Serialize(xmlTextWriter, pObject);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        /// <summary>
        /// Method to reconstruct an Object from XML string
        /// </summary>
        /// <remark>http://www.dotnetjohn.com/articles.aspx?articleid=173http://www.dotnetjohn.com/articles.aspx?articleid=173</remark>
        /// <param name="pXmlizedString"></param>
        /// <returns></returns>
        /// <param name="type">type of object to serialize to</param>
        public static  Object DeserializeObject(String pXmlizedString, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            return xs.Deserialize(memoryStream);
        }

    }
}
