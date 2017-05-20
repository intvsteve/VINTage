// <copyright file="ErrorLog.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

////#define ENABLE_YAML_ERROR_DATABASE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Generates a user-readable error log retrieved from a Locutus device.
    /// </summary>
    public class ErrorLog : INTV.Core.Utility.ByteSerializer, IComparable<ErrorLog>, IComparable
    {
        #region Constants

        /// <summary>
        /// The flat size in bytes.
        /// </summary>
        public const int FlatSizeInBytes = (BufferSize * sizeof(ushort)) + sizeof(ushort);

        /// <summary>
        /// The name of the error database file.
        /// </summary>
        internal const string ErrorDatabaseFileName = "error_db.yaml";

        private const int BufferSize = 128;

        private static readonly Dictionary<ErrorLogId, string> ErrorLogIdStrings = new Dictionary<ErrorLogId, string>()
        {
            { ErrorLogId.Ftl, "FTLv2" },
            { ErrorLogId.Lfs, "LFSv2" },
            { ErrorLogId.ExtFlash, "Ext Flash" },
            { ErrorLogId.Luigi, "Decode Luigi" },
            { ErrorLogId.Unknown, "??" }
        };

        #endregion // Constants

        private ushort[] _errorBuffer = new ushort[BufferSize];

        #region Properties

        /// <summary>
        /// Gets the error IDs in the log.
        /// </summary>
        public IEnumerable<ErrorLogId> ErrorIds { get; private set; }

        /// <summary>
        /// Gets the error line numbers.
        /// </summary>
        public IEnumerable<int> ErrorLineNumbers { get; private set; }

        /// <summary>
        /// Gets the error data.
        /// </summary>
        public IEnumerable<string> ErrorData { get; private set; }

        private IEnumerable<ErrorLogEntry> ErrorLogEntries { get; set; }

        private ErrorDatabase ErrorsDatabase
        {
            get
            {
                if (_errorsDatabase == null)
                {
                    _errorsDatabase = ErrorDatabase.Create();
                }
                return _errorsDatabase;
            }
        }

        private ErrorDatabase _errorsDatabase;

        /// <summary>
        /// Gets a value indicating whether the error log is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (ErrorLogEntries == null) || !ErrorLogEntries.Any(); }
        }

        /// <summary>
        /// Gets the index of the error in the log.
        /// </summary>
        public ushort ErrorIndex { get; private set; }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return FlatSizeInBytes; }
        }

        #endregion // ByteSerializer Properties

        #endregion Properties

        /// <summary>
        /// Creates a new instance of a ErrorLogData by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a ErrorLogData.</returns>
        public static ErrorLog Inflate(System.IO.Stream stream)
        {
            return Inflate<ErrorLog>(stream);
        }

        #region IComparable<ErrorLog>

        /// <inheritdoc />
        public int CompareTo(ErrorLog other)
        {
            var result = 0;
            for (var i = 0; (result == 0) && (i < BufferSize); ++i)
            {
                result = _errorBuffer[i] - other._errorBuffer[i];
            }
            return result;
        }

        #endregion // IComparable<ErrorLog>

        #region IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            var result = 1;
            if (object.ReferenceEquals(this, obj))
            {
                result = 0;
            }
            else if (obj != null)
            {
                var otherLog = obj as ErrorLog;
                if (otherLog != null)
                {
                    result = this.CompareTo(otherLog);
                }
            }
            return result;
        }

        #endregion // IComparable

        #region ByteSerializer

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            writer.Write(GetDetailedErrorReport(FirmwareRevisions.UnavailableFirmwareVersion));
            return SerializeByteCount;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            ErrorIndex = reader.ReadUInt16();
            for (int i = 0; i < BufferSize; ++i)
            {
                _errorBuffer[i] = reader.ReadUInt16();
            }
            DecodeRawResults();
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer

        /// <summary>
        /// Serializes the error log in a fashion suitable for a text file.
        /// </summary>
        /// <param name="currentFirmwareVersion">Version of firmware at time the error log was collected.</param>
        /// <param name="writer">The binary writer to use to serialize the data.</param>
        public void SerializeToTextFile(int currentFirmwareVersion, INTV.Core.Utility.BinaryWriter writer)
        {
            var reportText = GetDetailedErrorReport(currentFirmwareVersion);
            writer.Write(reportText.ToCharArray());
        }

        /// <summary>
        /// Gets a detailed error report.
        /// </summary>
        /// <param name="currentFirmwareVersion">Current firmware version of the device whose error log is being presented as a string.</param>
        /// <returns>The detailed error report as a string.</returns>
        public string GetDetailedErrorReport(int currentFirmwareVersion)
        {
            var report = string.Empty;
            if ((ErrorLogEntries != null) && ErrorLogEntries.Any())
            {
                var reportBuilder = new StringBuilder();
                reportBuilder.AppendLine(Resources.Strings.Device_ErrorLog_Header);
                reportBuilder.AppendLine("---------------------------------------------------------------------");

                // Append friendly output.
                if ((currentFirmwareVersion != FirmwareRevisions.UnavailableFirmwareVersion) && (ErrorsDatabase != null))
                {
                    reportBuilder.AppendLine("Details");
                    reportBuilder.AppendLine("--------------------------------------------------------------");
                    foreach (var entry in ErrorLogEntries)
                    {
                        var errorDetail = ErrorsDatabase.GetErrorString(currentFirmwareVersion, entry);
                        if (string.IsNullOrEmpty(errorDetail))
                        {
                            errorDetail = "<Error Detail Unavailable>";
                        }
                        reportBuilder.AppendFormat("{0}: {1}", entry.LogIndex, errorDetail).AppendLine();
                    }
                    reportBuilder.AppendLine();
                }

                // Append raw output.
                reportBuilder.AppendLine("Raw Data");
                reportBuilder.AppendLine("--------------------------------------------------------------");
                foreach (var entry in ErrorLogEntries)
                {
                    reportBuilder.AppendLine(entry.ToString());
                }
                reportBuilder.AppendLine("--------------------------------------------------------------");
                report = reportBuilder.ToString();
            }
            return report;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return GetDetailedErrorReport(FirmwareRevisions.UnavailableFirmwareVersion);
        }

        private void DecodeRawResults()
        {
            /*  Interpret error log.  Error log needs to be processed in reverse.   */
            /*      15:14   size of the entry (0, 1, 2, or 3 extra words)           */
            /*      13:12   file ID (0 = FTL, 1 = LFS, 2 = ExtFlash, 3 = Luigi)     */
            /*      11:0    line number of the error.                               */
            var errorReport = new StringBuilder();
            var errorIds = new List<ErrorLogId>();
            ErrorIds = errorIds;
            var errorLineNumbers = new List<int>();
            ErrorLineNumbers = errorLineNumbers;
            var errorStrings = new List<string>();
            ErrorData = errorStrings;
            var errorLogEntries = new List<ErrorLogEntry>();

            var started = false;
            var i = 1;
            do
            {
                var index = (ErrorIndex - i) & 0x7F;
                var word = _errorBuffer[index];
                if (word == 0)
                {
                    ++i;
                    continue;
                }

                var extra = (word >> 14) & 3;
                var errorLogId = (ErrorLogId)((word >> 12) & 3);
                errorIds.Add(errorLogId);
                var lineNumber = word & 0xFFF;
                errorLineNumbers.Add(lineNumber);

                if (!started)
                {
                    started = true;
                    errorReport.AppendLine(Resources.Strings.Device_ErrorLog_Header);
                }

                var entryIndex = (ushort)index;
                errorReport.AppendFormat("{0}: {1}:{2}", index.ToString().PadLeft(3), ErrorLogIdStrings[errorLogId], lineNumber);

                var errorString = string.Empty;
                var errorMessageArgs = new List<int>();
                for (int j = 0; j < extra; ++j)
                {
                    index = (ErrorIndex - i - extra + j) & 0x7F;
                    errorString += string.Format(" {0}", _errorBuffer[index].ToString("X4"));
                    errorMessageArgs.Add(_errorBuffer[index]);
                }
                var errorLogEntry = new ErrorLogEntry(entryIndex, errorLogId, lineNumber, errorMessageArgs);
                errorLogEntries.Add(errorLogEntry);
                errorReport.Append(errorString);
                errorStrings.Add(errorString);

                errorReport.AppendLine();

                i += 1 + extra;
            }
            while (i <= BufferSize);

            ErrorLogEntries = errorLogEntries;
        }

        private class ErrorLogEntry : Tuple<ushort, ErrorLogId, int, IEnumerable<int>>
        {
            public ErrorLogEntry(ushort logIndex, ErrorLogId logId, int lineNumber, IEnumerable<int> errorDetails)
                : base(logIndex, logId, lineNumber, errorDetails)
            {
            }

            public ushort LogIndex
            {
                get { return Item1; }
            }

            public ErrorLogId LogId
            {
                get { return Item2; }
            }

            public int LineNumber
            {
                get { return Item3; }
            }

            public IEnumerable<int> ErrorDetails
            {
                get { return Item4; }
            }

            public override string ToString()
            {
                var entryStringBuilder = new StringBuilder();
                entryStringBuilder.AppendFormat("{0}: {1}:{2}", LogIndex.ToString().PadLeft(3), ErrorLogIdStrings[LogId], LineNumber);
                foreach (var errorDetail in ErrorDetails)
                {
                    entryStringBuilder.AppendFormat(" {0}", errorDetail.ToString("X4"));
                }
                return entryStringBuilder.ToString();
            }
        }

        private class ErrorDatabase
        {
            private const string StringsKey = "strings";
            private const string ErrorMapKey = "err_map";

            private ErrorDatabase(string errorDatabasePath)
            {
#if ENABLE_YAML_ERROR_DATABASE
                var rawErrorDatabaseData = YaTools.Yaml.YamlLanguage.FileTo(errorDatabasePath) as IDictionary;
                if ((rawErrorDatabaseData != null) && rawErrorDatabaseData.Contains(StringsKey) && rawErrorDatabaseData.Contains(ErrorMapKey))
                {
                    var errorMaps = new Dictionary<ErrorLogId, IDictionary<int, IDictionary<int, int>>>();
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
                        errorMaps[(ErrorLogId)errorIdType] = firmwareVersionToLineNumberToStringIndicies;
                    }
                    ErrorMaps = errorMaps;
                    var strings = rawErrorDatabaseData[StringsKey] as IList;
                    Strings = strings.Cast<string>().Select(s => s.Replace(@"\n", string.Empty)).ToList();
                }
#endif // ENABLE_YAML_ERROR_DATABASE
            }

            /// <summary>
            /// Gets the error maps.
            /// </summary>
            /// <remarks>This dictionary's keys correspond to the general category (ID) of the error. The entries for
            /// these keys are, in turn, dictionaries whose keys are firmware revision numbers. The values for those
            /// keys are each a dictionary whose entries map the error's line number to the index of a specific error
            /// string in the Strings list. The string may contain format specifiers for more detail in the error string.</remarks>
            internal IDictionary<ErrorLogId, IDictionary<int, IDictionary<int, int>>> ErrorMaps { get; private set; }

            internal IList<string> Strings { get; private set; }

            internal static ErrorDatabase Create()
            {
                ErrorDatabase errorDatabase = null;
                try
                {
                    var errorDatabasePath = System.IO.Path.Combine(Configuration.Instance.FirmwareUpdatesDirectory, ErrorDatabaseFileName);
                    errorDatabase = new ErrorDatabase(errorDatabasePath);
                }
                catch (Exception)
                {
                    // If a problem arises, we'll just use the raw error log data to report any problems.
                }
                return errorDatabase;
            }

            internal string GetErrorString(int firmwareVersion, ErrorLogEntry errorLogEntry)
            {
                var errorString = string.Empty;
                IDictionary<int, IDictionary<int, int>> firmwareVersionEntries;
                if (ErrorMaps.TryGetValue(errorLogEntry.LogId, out firmwareVersionEntries))
                {
                    var isUnreleased = firmwareVersion & FirmwareRevisions.UnofficialReleaseMask;
                    var firmwareVersionKey = ((firmwareVersion & FirmwareRevisions.BaseVersionMask) >> 1) + isUnreleased;
                    IDictionary<int, int> lineNumberToErrorString;
                    if (firmwareVersionEntries.TryGetValue(firmwareVersionKey, out lineNumberToErrorString))
                    {
                        int errorStringIndex;
                        if (lineNumberToErrorString.TryGetValue(errorLogEntry.LineNumber, out errorStringIndex) && (errorStringIndex >= 0) && (errorStringIndex < Strings.Count))
                        {
                            try
                            {
                                errorString = INTV.Core.Utility.StringUtilities.SPrintf(Strings[errorStringIndex], errorLogEntry.ErrorDetails);
                            }
                            catch (Exception e)
                            {
                                // Don't care if this fails.
                                ReportErrorStringLookupFailure(firmwareVersion, errorLogEntry, "Failed to format output" + "; " + e.Message);
                            }
                        }
                        else
                        {
                            ReportErrorStringLookupFailure(firmwareVersion, errorLogEntry, "Failed to look up line number");
                        }
                    }
                    else
                    {
                        ReportErrorStringLookupFailure(firmwareVersion, errorLogEntry, "Failed to locate data for firmware revision");
                    }
                }
                else
                {
                    ReportErrorStringLookupFailure(firmwareVersion, errorLogEntry, "Failed to locate database for log kind");
                }
                return errorString;
            }

            private static void ReportErrorStringLookupFailure(int firmwareVersion, ErrorLogEntry errorLogEntry, string detail)
            {
                System.Diagnostics.Debug.WriteLine("Failed to locate error string.");
                System.Diagnostics.Debug.WriteLine("  FW version: " + firmwareVersion);
                System.Diagnostics.Debug.WriteLine("  LogId: " + errorLogEntry.LogId);
                System.Diagnostics.Debug.WriteLine("  Line Number: " + errorLogEntry.LineNumber);
                System.Diagnostics.Debug.WriteLine("  Reason: " + detail);
            }
        }
    }
}
