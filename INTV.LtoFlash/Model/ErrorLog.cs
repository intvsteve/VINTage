// <copyright file="ErrorLog.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System.Collections.Generic;
using System.Text;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Generates a user-readable error log retrieved from a Locutus device.
    /// </summary>
    public class ErrorLog : INTV.Core.Utility.ByteSerializer, System.IComparable<ErrorLog>, System.IComparable
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
            { ErrorLogId.ExtFlash, "Ext Flash" },
            { ErrorLogId.Luigi, "Decode Luigi" },
            { ErrorLogId.Unknown, "??" }
        };

        #endregion // Constants

        private ushort[] _errorBuffer = new ushort[BufferSize];

        #region Properties

        /// <summary>
        /// Gets the error log text.
        /// </summary>
        public string Text { get; private set; }

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

        /// <summary>
        /// Gets a value indicating whether the error log is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrWhiteSpace(Text); }
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
            writer.Write(Text);
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
        /// <param name="writer">The binary writer to use to serialize the data.</param>
        public void SerializeToTextFile(INTV.Core.Utility.BinaryWriter writer)
        {
            writer.Write(Text.ToCharArray());
        }

        private void DecodeRawResults()
        {
            /*  Interpret error log.  Error log needs to be processed in reverse.   */
            /*      15:14   size of the entry (0, 1, 2, or 3 extra words)           */
            /*      13:12   file ID (0 = FTL, 1 = LFS, 2 = ExtFlash, 3 = ??)        */
            /*      11:0    line number of the error.                               */
            var errorReport = new StringBuilder();
            var errorIds = new List<ErrorLogId>();
            ErrorIds = errorIds;
            var errorLineNumbers = new List<int>();
            ErrorLineNumbers = errorLineNumbers;
            var errorStrings = new List<string>();
            ErrorData = errorStrings;

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

                errorReport.AppendFormat("{0}: {1}:{2}", index.ToString().PadLeft(3), ErrorLogIdStrings[errorLogId], lineNumber);

                var errorString = string.Empty;
                for (int j = 0; j < extra; ++j)
                {
                    index = (ErrorIndex - i - extra + j) & 0x7F;
                    errorString += string.Format(" {0}", _errorBuffer[index].ToString("X4"));
                }
                errorReport.Append(errorString);
                errorStrings.Add(errorString);

                errorReport.AppendLine();

                i += 1 + extra;
            }
            while (i <= BufferSize);

            Text = errorReport.ToString();
        }
    }
}
