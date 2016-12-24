// <copyright file="CrashLog.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Data structure that stores a raw crash log retrieved from a Locutus device.
    /// </summary>
    public class CrashLog : INTV.Core.Utility.ByteSerializer
    {
        #region Constants

        /// <summary>
        /// Size of the raw crash data buffer.
        /// </summary>
        public const int FlatSizeInBytes = 0x00020000; // 128KB

        /// <summary>
        /// Default file name to use for saving a crash log.
        /// </summary>
        public const string CrashLogFileName = "LTOFlashDiagnostics.lfd";

        #endregion // Constants

        #region Properties

        /// <summary>
        /// Gets the raw crash buffer.
        /// </summary>
        private IEnumerable<byte> CrashBuffer
        {
            get { return _crashBuffer; }
        }
        private byte[] _crashBuffer;

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

        #region ByteSerializer

        /// <summary>
        /// Creates a new instance of a CrashLog by inflating it from a Stream.
        /// </summary>
        /// <param name="stream">The stream containing the data to deserialize to create the object.</param>
        /// <returns>A new instance of a CrashLog.</returns>
        public static CrashLog Inflate(System.IO.Stream stream)
        {
            return Inflate<CrashLog>(stream);
        }

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            writer.Write(_crashBuffer);
            return SerializeByteCount;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            _crashBuffer = reader.ReadBytes(DeserializeByteCount);
            return DeserializeByteCount;
        }

        #endregion // ByteSerializer
    }
}
