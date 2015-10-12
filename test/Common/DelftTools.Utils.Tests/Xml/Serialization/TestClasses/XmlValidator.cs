using System;
using System.Xml;
using System.Xml.Schema;

namespace DelftTools.Utils.Tests.Xml.Serialization.TestClasses
{
    /// <summary>
    /// Summary description for XmlValidator.
    /// </summary>
    public class XmlValidator
    {
        // variable declarations 
        private readonly string xmlFileName;
        private readonly string schemaFileName;
        private bool failed;

        //public XmlValidator()
        //{

        //}
        public XmlValidator(string xmlFileName, string schemaFileName)
        {
            this.xmlFileName = xmlFileName;
            this.schemaFileName = schemaFileName;
        }

        public bool ValidateXmlFile()
        {
            XmlReader rdr;
            rdr = null;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, schemaFileName);
                settings.ValidationEventHandler += ValidationFailed;
                rdr = XmlReader.Create(xmlFileName, settings);

                while (rdr.Read())
                {
                    //spool text to output device
                }
                return failed;
            }
            catch
            {
                throw new Exception("Validating of xml failed");
            }
            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }
            }
        }

        private void ValidationFailed(object sender, ValidationEventArgs args)
        {
            failed = true;
        }
    }
}