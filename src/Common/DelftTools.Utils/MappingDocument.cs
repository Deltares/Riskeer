using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace DelftTools.Utils
{
    public class MappingDocument
    {
        private static readonly Regex FileVersionRegex =
            new Regex(@"(?<mappingname>.*?)\.(?<version>(\d+\.??)*)?\.?hbm\.xml");

        public MappingDocument(string fileName, XmlDocument xmlDocument)
        {
            FileName = fileName;
            XmlDocument = xmlDocument;
            RootName = ExtractRootNameFromFileName(fileName);

            VersionMask = ExtractVersionFromFileName(fileName); //-1 for undefined versions
            
            if (VersionMask != null) 
                Version = VersionMask.GetFullVersion(); //0 for undefined versions
        }
        
        public string FileName { get; private set; }
        
        public XmlDocument XmlDocument { get; private set; }

        /// <summary>
        /// Root name of mapping. For example weir.1.2.3.hbm.xml -> weir
        /// </summary>
        public string RootName { get; private set; }
        public Version Version { get; private set; }
        public Version VersionMask { get; private set; }

        private static Version ExtractVersionFromFileName(string fileName)
        {
            var match = FileVersionRegex.Match(fileName);
            var versionStr = match.Groups["version"].Value;
            return String.IsNullOrEmpty(versionStr) ? null : new Version(versionStr);
        }
        
        private static string ExtractRootNameFromFileName(string fileName)
        {
            var match = FileVersionRegex.Match(fileName);
            return match.Groups["mappingname"].Value;
        }
    }
}