// <copyright file="LuigiScrambleKeyBlock.cs" company="INTV Funhouse">
// Copyright (c) 2016-2019 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Exposes the clear portion of a LUIGI file's scramble key block.
    /// </summary>
    public class LuigiScrambleKeyBlock : LuigiDataBlock
    {
        /// <summary>
        /// The "Any LTO Flash!" identifier.
        /// </summary>
        public const string AnyLTOFlashId = "00000000000000000000000000000000";

        private const int UniqueIdSize = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.LuigiScrambleKeyBlock"/> class.
        /// </summary>
        internal LuigiScrambleKeyBlock()
            : base(LuigiDataBlockType.SetScrambleKey)
        {
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string UniqueId { get; private set; }

        /// <inheritdoc/>
        protected override int DeserializePayload(Core.Utility.BinaryReader reader)
        {
            // Verify we're not going to be truncated.
            if ((reader.BaseStream.Length - reader.BaseStream.Position) < Length)
            {
                throw new System.IO.EndOfStreamException();
            }

            // Get the clear portion of the key - the DRUID.
            var responseBuffer = new byte[UniqueIdSize];
            reader.Read(responseBuffer, 0, UniqueIdSize);
            var lowPart = System.BitConverter.ToUInt64(responseBuffer, 0);
            var highPart = System.BitConverter.ToUInt64(responseBuffer, 8);
            UniqueId = highPart.ToString("X16") + lowPart.ToString("X16");

            // Skip past the rest of it - we've got all we care about.
            reader.BaseStream.Seek(Length - UniqueIdSize, System.IO.SeekOrigin.Current);

            // NOTE: Scrambled blocks *CANNOT* be validated by a CRC check!
            return Length;
        }
    }
}
