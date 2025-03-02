using System;
using System.IO;
using System.Xml;

namespace TSAB.ModManager.Core.Helpers

{
    public static class XmlValidator
    {
        /// <summary>
        /// Validates an XML file and returns true if it is correctly formatted.
        /// </summary>
        public static bool ValidateXml(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!File.Exists(filePath))
            {
                errorMessage = "[ERROR] XML file not found.";
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                return true;
            }
            catch (XmlException ex)
            {
                errorMessage = $"[ERROR] XML error in line {ex.LineNumber}: {ex.Message}";
                return false;
            }
        }
    }
}
