// <copyright file="ErrorLog.ErrorDatabase.Yaml.cs" company="INTV Funhouse">
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

// Enable this for YAML error database support. Requires the YAML libraries!
////#define ENABLE_YAML_ERROR_DATABASE

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// YAML-specific error database code.
    /// </summary>
    public partial class ErrorLog
    {
        private static bool IsYamlSupported()
        {
#if ENABLE_YAML_ERROR_DATABASE
            return true;
#else
            return false;
#endif // ENABLE_YAML_ERROR_DATABASE
        }

        /// <summary>
        /// Provides YAML error database parsing.
        /// </summary>
        private partial class ErrorDatabase
        {
            internal const string YamlFileExtension = ".yaml";
            private const string YamlSchemaFileExtension = ".ysd"; // utterly un-creative, but there is no official one; didn't use .yschema or .yaml.schema or .yaml because!

            private void PopulateFromYaml(System.IO.TextReader textReader, System.IO.TextReader schemaReader)
            {
                PopulateFromYamlCore(textReader, schemaReader);
            }

            [System.Diagnostics.Conditional("ENABLE_YAML_ERROR_DATABASE")]
            private void PopulateFromYamlCore(System.IO.TextReader textReader, System.IO.TextReader schemaReader)
            {
                // Unfortunately, the #if is still needed so the build is not broken when the YAML projects are not available.
#if ENABLE_YAML_ERROR_DATABASE
                try
                {
                    var rawErrorDatabaseData = YaTools.Yaml.YamlLanguage.StringTo(textReader.ReadToEnd()) as IDictionary;
                    if ((rawErrorDatabaseData != null) && rawErrorDatabaseData.Contains(StringsKey) && rawErrorDatabaseData.Contains(ErrorMapKey))
                    {
                        var rawErrorMap = rawErrorDatabaseData[ErrorMapKey] as IList;
                        for (int errorIdType = 0; errorIdType < rawErrorMap.Count; ++errorIdType)
                        {
                            var firmwareVersionToLineNumberToStringIndicies = new Dictionary<int, IDictionary<int, int>>();
                            var rawFirmwareVersionToLineNumberToStringIndicies = rawErrorMap[errorIdType] as IDictionary;
                            if (rawFirmwareVersionToLineNumberToStringIndicies == null)
                            {
                                // Ran into an empty table, so just fake it.
                                rawFirmwareVersionToLineNumberToStringIndicies = new Hashtable();
                            }
                            foreach (DictionaryEntry entry in rawFirmwareVersionToLineNumberToStringIndicies)
                            {
                                int firmwareVersion;
                                if (int.TryParse(entry.Key as string, out firmwareVersion))
                                {
                                    var lineNumberToStringIndexMap = new Dictionary<int, int>();
                                    var rawLineNumberToStringIndexMap = entry.Value as IDictionary;
                                    foreach (DictionaryEntry lineToStringIndexEntry in rawLineNumberToStringIndexMap)
                                    {
                                        int lineNumber;
                                        int stringIndex;
                                        if (int.TryParse(lineToStringIndexEntry.Key as string, out lineNumber) && int.TryParse(lineToStringIndexEntry.Value as string, out stringIndex))
                                        {
                                            lineNumberToStringIndexMap[lineNumber] = stringIndex;
                                        }
                                    }
                                    firmwareVersionToLineNumberToStringIndicies[firmwareVersion] = lineNumberToStringIndexMap;
                                }
                            }
                            ErrorMaps[(ErrorLogId)errorIdType] = firmwareVersionToLineNumberToStringIndicies;
                        }
                        var strings = rawErrorDatabaseData[StringsKey] as IList;
                        Strings = strings.Cast<string>().Select(s => RemoveNewline(s)).ToList();
                    }
                }
                catch (YaTools.Yaml.YamlSyntaxErrorException)
                {
                }
#endif // ENABLE_YAML_ERROR_DATABASE
            }
        }
    }
}
