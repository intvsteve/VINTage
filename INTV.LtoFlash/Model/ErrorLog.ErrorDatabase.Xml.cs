// <copyright file="ErrorLog.ErrorDatabase.Xml.cs" company="INTV Funhouse">
// Copyright (c) 2021 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// XML-specific error database code.
    /// </summary>
    public partial class ErrorLog
    {
        private static bool IsXmlSupported()
        {
            return true;
        }

        /// <summary>
        /// Provides XML error database parsing.
        /// </summary>
        private partial class ErrorDatabase
        {
            internal const string XmlFileExtension = ".xml";
            private const string XmlSchemaFileExtension = ".xsd";
            private const string SubsystemNameNodeName = "subsystem_name";
            private const string SubsystemIndexAttributeName = "subsystem_index";

            #region Subsystem Processing

            private static IEnumerable<ErrorLogId> GetSubsystems(IEnumerable<XmlNode> subsystems)
            {
                var errorLogIds = new List<ErrorLogId>();
                foreach (var subsystem in subsystems)
                {
                    AddSubsystem(subsystem, errorLogIds);
                }
                return errorLogIds;
            }

            private static void AddSubsystem(XmlNode subsystemNode, IList<ErrorLogId> errorLogIds)
            {
                ValidateSubsystem(subsystemNode.Name, subsystemNode.InnerText);
                var subsystem = ErrorLogId.Unknown;
                if (Enum.TryParse<ErrorLogId>(subsystemNode.InnerText, ignoreCase: true, result: out subsystem)
                    && !errorLogIds.Contains(subsystem))
                {
                    errorLogIds.Add(subsystem);
                }
            }

            [System.Diagnostics.Conditional("DEBUG")]
            private static void ValidateSubsystem(string nodeName, string subsystemName)
            {
                if (SubsystemNameNodeName != nodeName)
                {
                    throw new InvalidOperationException("Invalid node in subsystem: " + nodeName);
                }
                var subsystem = ErrorLogId.Unknown;
                if (!Enum.TryParse<ErrorLogId>(subsystemName, ignoreCase: true, result: out subsystem))
                {
                    throw new InvalidOperationException("Unknown subsystem: " + subsystemName);
                }
            }

            [System.Diagnostics.Conditional("DEBUG")]
            private static void ValidateSubsystemIndex(string subsystemIndexValue)
            {
                var subsystemIndex = ErrorLogId.Unknown;
                if (!Enum.TryParse<ErrorLogId>(subsystemIndexValue, out subsystemIndex))
                {
                    throw new InvalidOperationException("Invalid subsystem index value: " + subsystemIndexValue);
                }
            }

            #endregion // Subsystem Processing

            #region ErrorMapping Processing

            private static string GetSubsystemIndexValue(XmlNode errorMapping, int subsystemIndex)
            {
                var subsystemIndexValue = subsystemIndex.ToString(CultureInfo.InvariantCulture);
                var subsystemIndexAttribute = errorMapping.Attributes[SubsystemIndexAttributeName];
                if (subsystemIndexAttribute != null)
                {
                    subsystemIndexValue = subsystemIndexAttribute.Value;
                }
                return subsystemIndexValue;
            }

            private void AddErrorMapping(XmlNode errorMapping, string subsystemIndexValue, IEnumerable<ErrorLogId> errorLogIds)
            {
                ValidateSubsystemIndex(subsystemIndexValue);
                var errorLogId = ErrorLogId.Unknown;
                if (!Enum.TryParse<ErrorLogId>(subsystemIndexValue, out errorLogId))
                {
                    // possibly some new kind we're not familiar with - so parse as an int and cast
                    int index;
                    if (int.TryParse(subsystemIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                    {
                        errorLogId = (ErrorLogId)index;
                    }
                }

                IDictionary<int, IDictionary<int, int>> firmwareLineNumberToStringIndexMap;
                if (!ErrorMaps.TryGetValue(errorLogId, out firmwareLineNumberToStringIndexMap))
                {
                    firmwareLineNumberToStringIndexMap = new Dictionary<int, IDictionary<int, int>>();
                    ErrorMaps[errorLogId] = firmwareLineNumberToStringIndexMap;
                }

                foreach (var firmwareVersionNode in errorMapping.ChildNodes.Cast<XmlNode>())
                {
                    const string FirmwareNodeName = "firmware";
                    var firmwareVersionValue = string.Empty;
                    var addIndexData = true;
                    switch (firmwareVersionNode.Name)
                    {
                        case FirmwareNodeName:
                            const string FirmwareVersionAttributeName = "version";
                            var firmwareVersionAttribute = firmwareVersionNode.Attributes[FirmwareVersionAttributeName];
                            firmwareVersionValue = firmwareVersionAttribute == null ? null : firmwareVersionAttribute.Value;
                            break;
                        default:
                            // Not operating w/ a schema so try the other approach.
                            addIndexData = firmwareVersionNode.NodeType == XmlNodeType.Element;
                            if (addIndexData)
                            {
                                var nameParts = firmwareVersionNode.Name.Split('-');
                                addIndexData = nameParts.Length > 1;
                                if (addIndexData)
                                {
                                    firmwareVersionValue = nameParts[1];
                                }
                            }
                            break;
                    }
                    if (addIndexData)
                    {
                        AddFirmwareLineNumberToMessageIndexData(firmwareVersionNode, firmwareVersionValue, firmwareLineNumberToStringIndexMap);
                    }
                }
            }

            #endregion // ErrorMapping Processing

            #region Firmware Line to Message Index Processing

            private void AddFirmwareLineNumberToMessageIndexData(XmlNode firmwareVersionNode, string firmwareVersionValue, IDictionary<int, IDictionary<int, int>> firmwareVersionData)
            {
                int firmwareVersion;
                if (!int.TryParse(firmwareVersionValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out firmwareVersion))
                {
                    firmwareVersion = 0;
                }

                IDictionary<int, int> lineToIndexMap;
                if (!firmwareVersionData.TryGetValue(firmwareVersion, out lineToIndexMap))
                {
                    lineToIndexMap = new Dictionary<int, int>();
                    firmwareVersionData[firmwareVersion] = lineToIndexMap;
                }
                foreach (var messageIndexNode in firmwareVersionNode.ChildNodes.Cast<XmlNode>())
                {
                    const string MessageIndexNodeName = "message_index";
                    var addIndexData = true;
                    var lineNumberValue = string.Empty;
                    switch (messageIndexNode.Name)
                    {
                        case MessageIndexNodeName:
                            const string MessageIndexLineAttributeName = "line";
                            var lineNumberAttribute = firmwareVersionNode.Attributes[MessageIndexLineAttributeName];
                            lineNumberValue = lineNumberAttribute == null ? null : lineNumberAttribute.Value;
                            break;
                        default:
                            // Not operating w/ a schema so try the other approach.
                            addIndexData = firmwareVersionNode.NodeType == XmlNodeType.Element;
                            if (addIndexData)
                            {
                                var nameParts = firmwareVersionNode.Name.Split('-');
                                addIndexData = (nameParts.Length > 1);
                                if (addIndexData)
                                {
                                    lineNumberValue = nameParts[1];
                                }
                            }
                            break;
                    }
                    if (addIndexData)
                    {
                        AddLineNumberToMessageIndexEntry(lineNumberValue, messageIndexNode.InnerText, lineToIndexMap);
                    }
                }
            }

            #endregion // Firmware Line to Message Index Processing

            #region Line Number to Message Index Processing

            private void AddLineNumberToMessageIndexEntry(string lineNumberValue, string indexValue, IDictionary<int, int> lineToIndexMap)
            {
                int lineNumber;
                int index;
                if (int.TryParse(lineNumberValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out lineNumber) &&
                    int.TryParse(indexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                {
                    lineToIndexMap[lineNumber] = index;
                }
            }

            #endregion

            private void PopulateFromXml(System.IO.TextReader textReader, System.IO.TextReader schemaReader)
            {
                try
                {
                    var readerSettings = new XmlReaderSettings();
                    if (schemaReader != null)
                    {
                        var schema = XmlSchema.Read(schemaReader, (s, e) => { });
                        readerSettings.Schemas.Add(schema);
                        readerSettings.ValidationType = ValidationType.Schema;
                    }
                    using (var xmlReader = XmlReader.Create(textReader, readerSettings))
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlReader);
                        if (readerSettings.ValidationType == ValidationType.Schema)
                        {
                            xmlDocument.Validate((s, e) => { });
                        }
                        var errorDatabase = xmlDocument.GetElementsByTagName(ErrorDatabaseRootName).Cast<XmlNode>().FirstOrDefault();
                        if (errorDatabase != null)
                        {
                            PopulateFromXml(errorDatabase);
                        }
                    }
                }
                catch (XmlException)
                {
                }
                catch (XmlSchemaValidationException)
                {
                }
            }

            private void PopulateFromXml(XmlNode errorDatabase)
            {
                const string SubsystemNodeName = "subsystem";
                const string ErrorMappingNodeName = "error_mapping";
                const string MessageNodeName = "message";

                var subsystems = Enum.GetValues(typeof(ErrorLogId)).Cast<ErrorLogId>();
                var currentSubsystemIndex = 0;

                foreach (var node in errorDatabase.ChildNodes.Cast<XmlNode>())
                {
                    switch (node.Name)
                    {
                        case SubsystemNodeName:
                            subsystems = GetSubsystems(node.ChildNodes.Cast<XmlNode>());
                            break;
                        case ErrorMapKey:
                        case ErrorMappingNodeName:
                            var subsystemIndexValue = GetSubsystemIndexValue(node, currentSubsystemIndex);
                            AddErrorMapping(node, subsystemIndexValue, subsystems);
                            ++currentSubsystemIndex;
                            break;
                        case StringsKey:
                        case MessageNodeName:
                            Strings.Add(RemoveNewline(node.InnerText));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
