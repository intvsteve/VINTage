// <copyright file="ErrorLog.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Generates a user-readable error log retrieved from a Locutus device.
    /// </summary>
    public partial class ErrorLog : INTV.Core.Utility.ByteSerializer, IComparable<ErrorLog>, IComparable
    {
        #region Constants

        /// <summary>
        /// The flat size in bytes.
        /// </summary>
        public const int FlatSizeInBytes = (BufferSize * sizeof(ushort)) + sizeof(ushort);

        private const int BufferSize = 128;

        private static readonly Dictionary<ErrorLogId, string> ErrorLogIdStrings = new Dictionary<ErrorLogId, string>()
        {
            { ErrorLogId.Ftl, "FTLv2" },
            { ErrorLogId.Lfs, "LFSv2" },
            { ErrorLogId.Spi, "SPI" },
            { ErrorLogId.Luigi, "Decode Luigi" },
            { ErrorLogId.Unknown, "??" }
        };

        #endregion // Constants

        private ushort[] _errorBuffer = new ushort[BufferSize];

        #region Properties

        /// <summary>
        /// Gets the default name of the error database file or resource to use.
        /// </summary>
        public static string DefaultErrorDatabaseName
        {
            get { return _defaultErrorDatabaseName.Value; }
        }
        private static readonly Lazy<string> _defaultErrorDatabaseName = new Lazy<string>(GetDefaultErrorDatabaseName);

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
            if (other == null)
            {
                return 1;
            }
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
        /// <exception cref="System.ArgumentException">Thrown if <param name="obj"/> is not an <see cref="ErrorLog"/>.</exception>
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
                else
                {
                    throw new ArgumentException();
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
        public override int Deserialize(INTV.Core.Utility.BinaryReader reader)
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
                var errorDatabases = GetErrorDatabases(currentFirmwareVersion);
                if ((currentFirmwareVersion != FirmwareRevisions.UnavailableFirmwareVersion) && errorDatabases.Any())
                {
                    reportBuilder.AppendLine("Details");
                    reportBuilder.AppendLine("--------------------------------------------------------------");
                    foreach (var entry in ErrorLogEntries)
                    {
                        ErrorDatabase errorDatabase;
                        string errorDetail = null;
                        if (errorDatabases.TryGetValue(entry.LogId, out errorDatabase))
                        {
                            errorDetail = errorDatabase.GetErrorString(currentFirmwareVersion, entry);
                        }
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

        private static string GetDefaultErrorDatabaseName()
        {
            var defaultErrorDatabaseName = IsYamlSupported() ? ErrorDatabase.ErrorDatabaseYamlFileName : ErrorDatabase.ErrorDatabaseXmlFileName;
            return defaultErrorDatabaseName;
        }

        private IDictionary<ErrorLogId, ErrorDatabase> GetErrorDatabases(int rawFirmwareVersion)
        {
            var errorDatabases = new Dictionary<ErrorLogId, ErrorDatabase>();
            foreach (var errorLogKind in ErrorLogEntries.Select(l => l.LogId).Distinct())
            {
                errorDatabases[errorLogKind] = ErrorDatabase.GetErrorDatabase(rawFirmwareVersion, errorLogKind);
            }
            return errorDatabases;
        }

        private void DecodeRawResults()
        {
            /*  Interpret error log.  Error log needs to be processed in reverse.   */
            /*      15:14   size of the entry (0, 1, 2, or 3 extra words)           */
            /*      13:12   file ID (0 = FTL, 1 = LFS, 2 = SPI, 3 = LUIGI)     */
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

        /// <summary>
        /// This class represents an error database for one or more versions of Locutus firmware.
        /// </summary>
        private partial class ErrorDatabase
        {
            /// <summary>
            /// The base name for a general purpose error database file or resource.
            /// </summary>
            internal const string ErrorDatabaseRootName = "error_db";

            /// <summary>
            /// The name of the YAML error database file.
            /// </summary>
            internal const string ErrorDatabaseYamlFileName = ErrorDatabaseRootName + YamlFileExtension;

            /// <summary>
            /// The name of the XML error database file.
            /// </summary>
            internal const string ErrorDatabaseXmlFileName = ErrorDatabaseRootName + XmlFileExtension;

            /// <summary>
            /// Gets the default error database.
            /// </summary>
            /// <remarks>The default database is the one defined by an embedded resource in this assembly, named either
            /// error_db.yaml or error_db.xml, depending on the supported formats. The YAML format is the canonical form,
            /// with XML being added later to avoid licensing conflicts between the YAML assemblies initially adopted.
            /// Starting around version 5050, the error database format was upgraded in two ways:
            /// The format became inclusive of all prior firmware releases (tools improvement by Joe Zbiciak)
            /// XML format and schema were established for more widely supportable error database in C# (Steve Orth, Joe Zbiciak)</remarks>
            internal static ErrorDatabase Default
            {
                get { return DefaultErrorDatabase.Value; }
            }
            private static readonly Lazy<ErrorDatabase> DefaultErrorDatabase = new Lazy<ErrorDatabase>(() => new ErrorDatabase(DefaultErrorDatabaseResource));

            private static string DefaultErrorDatabaseResource
            {
                get { return DefaultDatabaseResource.Value; }
            }
            private static readonly Lazy<string> DefaultDatabaseResource = new Lazy<string>(GetDefaultErrorDatabaseResource);
            private static readonly Lazy<IEnumerable<string>> ErrorDatabaseResources = new Lazy<IEnumerable<string>>(GetEmbeddedErrorDatabases);
            private static readonly Dictionary<string, string> ErrorDatabaseFileTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { YamlFileExtension, YamlFileExtension },
                { XmlFileExtension, XmlFileExtension },
            };
            private const string StringsKey = "strings";
            private const string ErrorMapKey = "err_map";

            private const string DefaultBaseDatabaseResourceName = FirmwareRevisions.FirmwareUpdateResourcePrefix + ErrorDatabaseRootName;
            private const string DefaultXmlDatabaseResourceName = DefaultBaseDatabaseResourceName + XmlFileExtension;
            private const string DefaultYamlDatabaseResourceName = DefaultBaseDatabaseResourceName + YamlFileExtension;

            /// <summary>
            /// Initialize a new instance of an error database from a file on disk or embedded resource.
            /// </summary>
            /// <param name="errorDatabasePath">Absolute path to an error database file, or name of resource embedded in this assembly.</param>
            /// <remarks>Supported formats are XML and YAML. Note that at this time, YAML support is not shipped in the release product
            /// due to Ms-PL / GPL license incompatibility problems.</remarks>
            private ErrorDatabase(string errorDatabaseSource)
            {
                string databaseType;
                errorDatabaseSource = GetErrorDatabaseSource(errorDatabaseSource, out databaseType);
                if ((errorDatabaseSource != null) && (databaseType != null))
                {
                    if (System.IO.Path.IsPathRooted(errorDatabaseSource))
                    {
                        InitializeDatabaseFromFile(errorDatabaseSource, databaseType);
                    }
                    else
                    {
                        InitializeDatabaseFromResource(errorDatabaseSource, databaseType);
                    }
                }
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

            /// <summary>
            /// Gets the database best capable of providing meaningful error messages given a firmware version and error log identifier.
            /// </summary>
            /// <param name="rawFirmwareVersion">The raw firmware version for which an error log is to be parsed.</param>
            /// <param name="errorLogId">Which section of the error log is requested.</param>
            /// <param name="errorDatabaseFilesDirectory">Where to look for potential updated version of the error log.</param>
            /// <returns>An error database most likely to support error log parsing given the input parameters.</returns>
            internal static ErrorDatabase GetErrorDatabase(int rawFirmwareVersion, ErrorLogId errorLogId, string errorDatabaseFilesDirectory = null)
            {
                // First, check default map for exact match.
                ErrorDatabase matchingDatabase = Default;
                var errorMapFirmwareVersion = GetFirmwareVersionForLookup(rawFirmwareVersion);
                var matchingMap = Default.GetClosestMatchingErrorMap(errorLogId, errorMapFirmwareVersion);
                if (matchingMap.Item1 != errorMapFirmwareVersion)
                {
                    // Did not find exact match in default database. Next, check on disk in default location for a better match.
                    if (string.IsNullOrWhiteSpace(errorDatabaseFilesDirectory))
                    {
                        errorDatabaseFilesDirectory = Configuration.Instance.FirmwareUpdatesDirectory;
                    }
                    Tuple<int, IDictionary<int, int>> onDiskMatchingMap;
                    var onDiskDatabaseMatch = GetClosestMatchingErrorDatabase(rawFirmwareVersion, errorLogId, errorDatabaseFilesDirectory, out onDiskMatchingMap);
                    if (onDiskMatchingMap.Item1 > matchingMap.Item1)
                    {
                        matchingMap = onDiskMatchingMap;
                        matchingDatabase = onDiskDatabaseMatch;
                    }

                    // Finally, check embedded resources for a better match.
                    if (matchingMap.Item1 != errorMapFirmwareVersion)
                    {
                        Tuple<int, IDictionary<int, int>> resourcesMatchingMap;
                        var resourcesDatabaseMatch = GetClosestMatchingErrorDatabase(rawFirmwareVersion, errorLogId, FirmwareRevisions.FirmwareUpdateResourcePrefix, out resourcesMatchingMap);
                        if (resourcesMatchingMap.Item1 > matchingMap.Item1)
                        {
                            matchingMap = resourcesMatchingMap;
                            matchingDatabase = resourcesDatabaseMatch;
                        }
                    }
                }
                return matchingDatabase;
            }

            /// <summary>
            /// Gets an error string given an error log entry and a raw firmware version.
            /// </summary>
            /// <param name="rawFirmwareVersion">The raw current firmware version from a Locutus device.</param>
            /// <param name="errorLogEntry">An error log entry to convert to a more meaningful error message.</param>
            /// <returns>An error string describing the issue described by <paramref name="errorLogEntry"/>.</returns>
            internal string GetErrorString(int rawFirmwareVersion, ErrorLogEntry errorLogEntry)
            {
                var errorString = string.Empty;
                IDictionary<int, IDictionary<int, int>> firmwareVersionEntries;
                if ((ErrorMaps != null) && ErrorMaps.TryGetValue(errorLogEntry.LogId, out firmwareVersionEntries))
                {
                    var firmwareVersionKey = GetFirmwareVersionForLookup(rawFirmwareVersion);
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
                                ReportErrorStringLookupFailure(rawFirmwareVersion, errorLogEntry, "Failed to format output" + "; " + e.Message);
                            }
                        }
                        else
                        {
                            ReportErrorStringLookupFailure(rawFirmwareVersion, errorLogEntry, "Failed to look up line number");
                        }
                    }
                    else
                    {
                        ReportErrorStringLookupFailure(rawFirmwareVersion, errorLogEntry, "Failed to locate data for firmware revision");
                    }
                }
                else
                {
                    ReportErrorStringLookupFailure(rawFirmwareVersion, errorLogEntry, "Failed to locate database for log kind");
                }
                return errorString;
            }

            private static string GetDefaultErrorDatabaseResource()
            {
                var defaultErrorDatabaseResource = IsYamlSupported() ? DefaultYamlDatabaseResourceName : DefaultXmlDatabaseResourceName;
                return defaultErrorDatabaseResource;
            }

            private static string GetErrorDatabaseSource(string errorDatabase, out string databaseType)
            {
                var isValidErrorDatabase = false;
                databaseType = GetErrorDatabaseType(errorDatabase);
                if (databaseType != null)
                {
                    isValidErrorDatabase = IsValidDatabaseSource(errorDatabase);
                    if (isValidErrorDatabase)
                    {
                        if (!IsDatabaseTypeSupported(databaseType))
                        {
                            databaseType = GetOtherDatabaseType(databaseType);
                            errorDatabase = System.IO.Path.ChangeExtension(errorDatabase, databaseType);
                            isValidErrorDatabase = IsValidDatabaseSource(errorDatabase);
                        }
                    }
                }
                if (!isValidErrorDatabase)
                {
                    errorDatabase = null;
                    databaseType = null;
                }
                return errorDatabase;
            }

            private static string MakeVersionedDatabaseSourceName(int userFriendyFirmwareVersion, string databaseType)
            {
                var versionedDatabaseSourceName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}-{1}{2}",
                                                                ErrorDatabaseRootName, userFriendyFirmwareVersion, databaseType);
                return versionedDatabaseSourceName;
            }

            private static string MakeFullyQualifiedVersionedDatabaseSourceName(int userFriendyFirmwareVersion, string databaseLocation, string databaseType)
            {
                var baseDatabaseSourceName = MakeVersionedDatabaseSourceName(userFriendyFirmwareVersion, databaseType);
                string fullyQualifiedDatabaseSourceName;
                if (System.IO.Path.IsPathRooted(databaseLocation))
                {
                    fullyQualifiedDatabaseSourceName = System.IO.Path.Combine(databaseLocation, baseDatabaseSourceName);
                }
                else
                {
                    fullyQualifiedDatabaseSourceName = FirmwareRevisions.FirmwareUpdateResourcePrefix + baseDatabaseSourceName;
                }
                return fullyQualifiedDatabaseSourceName;
            }

            private static string GetErrorDatabaseType(string errorDatabase)
            {
                string databaseType = null;
                if (!string.IsNullOrWhiteSpace(errorDatabase))
                {
                    databaseType = System.IO.Path.GetExtension(errorDatabase);
                }
                if (string.IsNullOrWhiteSpace(databaseType) || !ErrorDatabaseFileTypes.TryGetValue(databaseType, out databaseType))
                {
                    databaseType = null;
                }
                return databaseType;
            }

            private static bool IsDatabaseTypeSupported(string databaseType)
            {
                var isFileTypeSupported = false;
                switch (databaseType)
                {
                    case XmlFileExtension:
                        isFileTypeSupported = IsXmlSupported();
                        break;
                    case YamlFileExtension:
                        isFileTypeSupported = IsYamlSupported();
                        break;
                    default:
                        break;
                }
                return isFileTypeSupported;
            }

            private static string GetOtherDatabaseType(string databaseType)
            {
                string otherFileType = null;
                switch (databaseType)
                {
                    case XmlFileExtension:
                        otherFileType = YamlFileExtension;
                        break;
                    case YamlFileExtension:
                        otherFileType = XmlFileExtension;
                        break;
                    default:
                        break;
                }
                return otherFileType;
            }

            private static bool IsValidDatabaseSource(string errorDatabase)
            {
                var isValidErrorDatabase = false;
                if (System.IO.Path.IsPathRooted(errorDatabase))
                {
                    isValidErrorDatabase = System.IO.File.Exists(errorDatabase);
                }
                else
                {
                    isValidErrorDatabase = ErrorDatabaseResources.Value.Contains(errorDatabase);
                }
                return isValidErrorDatabase;
            }

            private static int GetFirmwareVersionForLookup(int firmwareVersion)
            {
                var isUnreleased = firmwareVersion & FirmwareRevisions.UnofficialReleaseMask;
                var firmwareVersionKey = ((firmwareVersion & FirmwareRevisions.BaseVersionMask) >> 1) + isUnreleased;
                return firmwareVersionKey;
            }

            private static int GetFirmwareVersionForResource(int firmwareVersion)
            {
                var baseFirmwareVersionNumber = ((firmwareVersion & FirmwareRevisions.BaseVersionMask) >> FirmwareRevisions.BaseVersionBitOffset);
                return baseFirmwareVersionNumber;
            }

            private static string GetErrorDatabaseSchemaName(string errorDatabase)
            {
                string schema = null;
                if (!string.IsNullOrWhiteSpace(errorDatabase))
                {
                    var errorDatabaseFormat = GetErrorDatabaseType(errorDatabase);
                    switch (errorDatabaseFormat)
                    {
                        case XmlFileExtension:
                            schema = System.IO.Path.ChangeExtension(errorDatabase, XmlSchemaFileExtension);
                            break;
                        case YamlFileExtension:
                            schema = System.IO.Path.ChangeExtension(errorDatabase, YamlSchemaFileExtension);
                            break;
                        default:
                            break;
                    }

                    var isFileOnDisk = System.IO.Path.IsPathRooted(errorDatabase);
                    var schemaExists = new Predicate<string>(s => isFileOnDisk ? System.IO.File.Exists(s) : ErrorDatabaseResources.Value.Contains(schema));
                    if (!schemaExists(schema))
                    {
                        schema = null;
                    }
                }

                return schema;
            }

            private static ErrorDatabase GetClosestMatchingErrorDatabase(int rawFirmwareVersion, ErrorLogId errorLogId, string databaseRoot, out Tuple<int, IDictionary<int, int>> matchingMap)
            {
                matchingMap = null;
                ErrorDatabase errorDatabase = null;
                var userFriendlyFirmwareVersion = GetFirmwareVersionForResource(rawFirmwareVersion);
                int closestFirmwareVersion = -1;
                string databaseTypeForClosestFirmwareVersion = null;
                foreach (var databaseType in ErrorDatabaseFileTypes.Keys.Where(t => IsDatabaseTypeSupported(t)))
                {
                    var closestFirmwareVersionMatch = GetClosestMatchForErrorDatabase(userFriendlyFirmwareVersion, databaseType, databaseRoot);
                    if (closestFirmwareVersionMatch > closestFirmwareVersion)
                    {
                        closestFirmwareVersion = closestFirmwareVersionMatch;
                        databaseTypeForClosestFirmwareVersion = databaseType;
                    }
                    if (closestFirmwareVersion == userFriendlyFirmwareVersion)
                    {
                        break;
                    }
                }
                if (closestFirmwareVersion > 0)
                {
                    // load this database and find best match
                    var databasePath = MakeFullyQualifiedVersionedDatabaseSourceName(closestFirmwareVersion, databaseRoot, databaseTypeForClosestFirmwareVersion);
                    errorDatabase = new ErrorDatabase(databasePath);
                    var errorMapFirmwareVersion = GetFirmwareVersionForLookup(rawFirmwareVersion);
                    matchingMap = errorDatabase.GetClosestMatchingErrorMap(errorLogId, errorMapFirmwareVersion);
                }
                return errorDatabase;
            }

            private static int GetClosestMatchForErrorDatabase(int userFriendlyFirmwareVersion, string errorDatabaseFormat, string directory)
            {
                var potentialMatchingErrorDatabases = Enumerable.Empty<string>();
                if (!string.IsNullOrEmpty(directory) && System.IO.Directory.Exists(directory))
                {
                    potentialMatchingErrorDatabases = directory.EnumerateFilesWithPattern(errorDatabaseFormat);
                }
                else
                {
                    potentialMatchingErrorDatabases = ErrorDatabaseResources.Value.Where(r => r.EndsWith(errorDatabaseFormat, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var matchingFirmwareVersion = -1;
                var perfectMatchDatabaseName = MakeVersionedDatabaseSourceName(userFriendlyFirmwareVersion, errorDatabaseFormat);
                var match = potentialMatchingErrorDatabases.FirstOrDefault(d => d.EndsWith(perfectMatchDatabaseName, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    matchingFirmwareVersion = userFriendlyFirmwareVersion;
                }
                else
                {
                    // No perfect match found. Get the "Price is Right" match.
                    var versionedDatabaseNames = new List<Tuple<int, string>>();
                    foreach (var databaseName in potentialMatchingErrorDatabases)
                    {
                        var baseName = System.IO.Path.GetFileNameWithoutExtension(databaseName);
                        var nameParts = baseName.Split('-');
                        int version;
                        if (nameParts.Length > 1 && int.TryParse(nameParts.Last(), out version))
                        {
                            versionedDatabaseNames.Add(new Tuple<int, string>(version, databaseName));
                        }
                    }

                    var priceIsRightVersion = -1;
                    foreach (var versionedDatabaseName in versionedDatabaseNames.OrderBy(d => d.Item1))
                    {
                        if (versionedDatabaseName.Item1 <= userFriendlyFirmwareVersion)
                        {
                            priceIsRightVersion = versionedDatabaseName.Item1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    matchingFirmwareVersion = priceIsRightVersion;
                }
                return matchingFirmwareVersion;
            }

            private static string RemoveNewline(string errorLogFormatString)
            {
                var sanitized = string.Empty;
                if (!string.IsNullOrWhiteSpace(errorLogFormatString))
                {
                    sanitized = errorLogFormatString.TrimEnd('\n');
                }
                return sanitized;
            }

            private static void ReportErrorStringLookupFailure(int firmwareVersion, ErrorLogEntry errorLogEntry, string detail)
            {
                System.Diagnostics.Debug.WriteLine("Failed to locate error string.");
                System.Diagnostics.Debug.WriteLine("  FW version: " + firmwareVersion);
                System.Diagnostics.Debug.WriteLine("  LogId: " + errorLogEntry.LogId);
                System.Diagnostics.Debug.WriteLine("  Line Number: " + errorLogEntry.LineNumber);
                System.Diagnostics.Debug.WriteLine("  Reason: " + detail);
            }

            private static IEnumerable<string> GetEmbeddedErrorDatabases()
            {
                var errorDatabases = new HashSet<string>(typeof(ErrorLog).GetResources(DefaultBaseDatabaseResourceName), StringComparer.OrdinalIgnoreCase);
                return errorDatabases;
            }

            private void InitializeDatabaseFromFile(string errorDatabasePath, string databaseType)
            {
                var schema = GetErrorDatabaseSchemaName(errorDatabasePath);
                using (var textReader = new System.IO.StreamReader(errorDatabasePath))
                using (var schemaReader = schema == null ? null : new System.IO.StreamReader(schema))
                {
                    InitializeDatabaseFromStreams(textReader, schemaReader, databaseType);
                }
            }

            private void InitializeDatabaseFromResource(string errorDatabaseResource, string databaseType)
            {
                var schema = GetErrorDatabaseSchemaName(errorDatabaseResource);
                var assembly = typeof(ErrorLog).Assembly;
                using (var resourceStream = assembly.GetManifestResourceStream(errorDatabaseResource))
                using (var textReader = new System.IO.StreamReader(resourceStream))
                using (var schemaStream = schema == null ? null : assembly.GetManifestResourceStream(schema))
                using (var schemaReader = schema == null ? null : new System.IO.StreamReader(schemaStream))
                {
                    InitializeDatabaseFromStreams(textReader, schemaReader, databaseType);
                }
            }

            private void InitializeDatabaseFromStreams(System.IO.TextReader textReader, System.IO.TextReader schemaReader, string databaseType)
            {
                ErrorMaps = new Dictionary<ErrorLogId, IDictionary<int, IDictionary<int, int>>>();
                Strings = new List<string>();

                switch (databaseType)
                {
                    case XmlFileExtension:
                        PopulateFromXml(textReader, schemaReader);
                        break;
                    case YamlFileExtension:
                        PopulateFromYaml(textReader, schemaReader);
                        break;
                    default:
                        break;
                }
            }

            private Tuple<int, IDictionary<int, int>> GetClosestMatchingErrorMap(ErrorLogId logId, int errorMapFirmwareVersion)
            {
                var matchingMap = new Tuple<int, IDictionary<int, int>>(-1, null);
                IDictionary<int, IDictionary<int, int>> firmwareVersionEntries;
                if (ErrorMaps.TryGetValue(logId, out firmwareVersionEntries))
                {
                    IDictionary<int, int> closestMatchErrorMap;
                    if (firmwareVersionEntries.TryGetValue(errorMapFirmwareVersion, out closestMatchErrorMap))
                    {
                        matchingMap = new Tuple<int, IDictionary<int, int>>(errorMapFirmwareVersion, closestMatchErrorMap);
                    }
                    else
                    {
                        var closestMatchFirmwareVersion = -1;
                        foreach (var firmwareVersionEntry in firmwareVersionEntries)
                        {
                            var firmwareVersion = firmwareVersionEntry.Key;
                            if ((firmwareVersion > closestMatchFirmwareVersion) && (firmwareVersion < errorMapFirmwareVersion))
                            {
                                closestMatchFirmwareVersion = firmwareVersion;
                                closestMatchErrorMap = firmwareVersionEntry.Value;
                            }
                        }
                        if ((closestMatchFirmwareVersion > 0) && (closestMatchErrorMap != null))
                        {
                            matchingMap = new Tuple<int, IDictionary<int, int>>(closestMatchFirmwareVersion, closestMatchErrorMap);
                        }
                    }
                }
                return matchingMap;
            }
        }
    }
}
