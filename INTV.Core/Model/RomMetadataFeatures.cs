// <copyright file="RomMetadataFeatures.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for features described in .ROM metadata.
    /// </summary>
    public class RomMetadataFeatures : RomMetadataBlock
    {
        private const int FeatureMask = 0x03;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataFeatures"/> class.
        /// </summary>
        /// <param name="length">Length of the payload in bytes.</param>
        public RomMetadataFeatures(uint length)
            : base(length, RomMetadataIdTag.Features)
        {
            Features = ProgramFeatures.EmptyFeatures.Clone();
        }

        #region Properties

        /// <summary>
        /// Gets the program's features.
        /// </summary>
        public ProgramFeatures Features { get; private set; }

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var remainingPayload = Length;
            if (remainingPayload < 3)
            {
                System.Diagnostics.Debug.WriteLine("Too few bytes in feature metadata!");
            }
            if (remainingPayload > 0)
            {
                // The first byte contains compatibility:
                // .   7   6   5   4   3   2   1   0
                // +---+---+---+---+---+---+---+---+
                // |  ECS  | 4CTRL | VOICE | KEYBD |    byte 0
                // +---+---+---+---+---+---+---+---+
                Features = new ProgramFeatures();
                var featureBits = reader.ReadByte();
                --remainingPayload;

                // Bits 0,1 are Keyboard Component compatibility.
                var compatibility = RawFeatureToFeatureCompatibility(featureBits & FeatureMask);
                Features.KeyboardComponent = (KeyboardComponentFeatures)compatibility;

                // Bits 2,3 are Intellivoice compatibility.
                compatibility = RawFeatureToFeatureCompatibility((featureBits >> 2) & FeatureMask);
                Features.Intellivoice = compatibility;

                // Bits 4,5 are 4-controller capability (ignored)
                compatibility = RawFeatureToFeatureCompatibility((featureBits >> 4) & FeatureMask);

                // Bits 6,7 are ECS compatibility.
                compatibility = RawFeatureToFeatureCompatibility((featureBits >> 6) & FeatureMask);
                Features.Ecs = (EcsFeatures)compatibility;
            }
            if (remainingPayload > 0)
            {
                // The second byte contains more compatibility:
                // .   7   6   5   4   3   2   1   0
                // +---+---+---+---+---+---+---+---+
                // | rsvd  | rsvd  | rsvd  | INTY2 |    byte 1
                // +---+---+---+---+---+---+---+---+
                var featureBits = reader.ReadByte();
                --remainingPayload;

                // Bits 0,1 are Intellivision II compatibility.
                var compatibility = RawFeatureToFeatureCompatibility(featureBits & FeatureMask);
                Features.IntellivisionII = compatibility;
            }
            if (remainingPayload > 0)
            {
                // The third byte contains emulator-specific guidance.
                // .    7    6    5    4    3    2    1    0
                // +----+----+----+----+----+----+----+----+
                // |rsvd|rsvd|rsvd|rsvd|rsvd|rsvd|rsvd|rsvd|    byte 2
                // +----+----+----+----+----+----+----+----+

                // We're going to ignore this byte.
                reader.ReadByte();
                --remainingPayload;
            }
            if (remainingPayload > 1)
            {
                // These two bytes contain JLP compatibility.
                // .    7    6    5    4    3    2    1    0
                // +----+----+----+----+----+----+----+----+
                // |JLP Accel|LTOM|   Reserved   |JLPf 9..8|    byte 3
                // +----+----+----+----+----+----+----+----+
                var featureBits = reader.ReadByte();
                --remainingPayload;
                var compatibility = RawFeatureToFeatureCompatibility((featureBits >> 6) & FeatureMask);
                Features.Jlp = (JlpFeatures)compatibility;
                if (((1 << 5) & featureBits) != 0)
                {
                    Features.LtoFlash |= LtoFlashFeatures.LtoFlashMemoryMapped;
                }
                var flashSectors = (ushort)((featureBits & FeatureMask) << 8);

                // .    7    6    5    4    3    2    1    0
                // +----+----+----+----+----+----+----+----+
                // |          JLP Flash bits 7..0          |    byte 4
                // +----+----+----+----+----+----+----+----+
                flashSectors |= reader.ReadByte();
                --remainingPayload;
                Features.JlpFlashMinimumSaveSectors = flashSectors;
                if (Features.Jlp != JlpFeatures.Incompatible)
                {
                    Features.JlpHardwareVersion = JlpHardwareVersion.Jlp03; // Assume minimal hardware version needed
                }
            }
            if (remainingPayload > 0)
            {
                reader.BaseStream.Seek(remainingPayload, System.IO.SeekOrigin.Current);
            }
            return Length;
        }

        #endregion // RomMetadataBlock

        private static FeatureCompatibility RawFeatureToFeatureCompatibility(int featureBits)
        {
            var compatibility = (FeatureCompatibility)((featureBits + 1) & FeatureMask);
            return compatibility;
        }
    }
}
