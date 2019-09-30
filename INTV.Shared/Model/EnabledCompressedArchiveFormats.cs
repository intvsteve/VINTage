// <copyright file="EnabledCompressedArchiveFormats.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INTV.Shared.Utility;

namespace INTV.Shared.Model
{
    /// <summary>
    /// These flags enable which compressed archive formats should be enabled, if any.
    /// </summary>
    [Flags]
    public enum EnabledCompressedArchiveFormats
    {
        /// <summary>Indicates none of the available archive formats are enabled.</summary>
        None = 0,

        /// <summary>When set, indicates to enable support for all available formats.</summary>
        All = 1 << 0, // 1

        /// <summary>When set, indicates to enable the .ZIP format.</summary>
        Zip = 1 << CompressedArchiveFormat.Zip, // 2

        /// <summary>When set, indicates to enable the GNU gzip format.</summary>
        GZip = 1 << CompressedArchiveFormat.GZip, // 4

        /// <summary>When set, indicates to enable the Tape Archive (TAR) format.</summary>
        Tar = 1 << CompressedArchiveFormat.Tar, // 8

        /// <summary>When set, indicates to enable the bzip2 format.</summary>
        BZip2 = 1 << CompressedArchiveFormat.BZip2, // 16

        /// <summary>
        /// A special value to use to indicate an unknown compressed archive format is present.
        /// </summary>
        Unknown = 1 << 31
    }

    /// <summary>
    /// Extension and helper methods for working with <see cref="EnabledCompressedArchiveFormats"/>.
    /// </summary>
    public static class EnabledCompressedArchiveFormatsExtensions
    {
        private static readonly IDictionary<EnabledCompressedArchiveFormats, CompressedArchiveFormat> FlagToFormatData = new Dictionary<EnabledCompressedArchiveFormats, CompressedArchiveFormat>()
        {
            { EnabledCompressedArchiveFormats.None, CompressedArchiveFormat.None },
            { EnabledCompressedArchiveFormats.All, CompressedArchiveFormat.None },
            { EnabledCompressedArchiveFormats.Zip, CompressedArchiveFormat.Zip },
            { EnabledCompressedArchiveFormats.GZip, CompressedArchiveFormat.GZip },
            { EnabledCompressedArchiveFormats.Tar, CompressedArchiveFormat.Tar },
            { EnabledCompressedArchiveFormats.BZip2, CompressedArchiveFormat.BZip2 },
            { EnabledCompressedArchiveFormats.Unknown, CompressedArchiveFormat.None },
        };

        /// <summary>
        /// Given a set of flags specified in a <see cref="EnabledCompressedArchiveFormats"/>, produce a corresponding enumerable of
        /// <see cref="CompressedArchiveFormat"/> values the bit array represents that are supported by the implementation.
        /// </summary>
        /// <param name="formats">The bit array to check.</param>
        /// <returns>The <see cref="CompressedArchiveFormat"/> that are in the flags and that are supported.</returns>
        public static IEnumerable<CompressedArchiveFormat> ToCompressedArchiveFormats(this EnabledCompressedArchiveFormats formats)
        {
            return formats.ToCompressedArchiveFormats(onlyIncludeAvailableFormats: true);
        }

        /// <summary>
        /// Given a set of flags specified in a <see cref="EnabledCompressedArchiveFormats"/>, produce a corresponding enumerable of
        /// <see cref="CompressedArchiveFormat"/> values the bit array represents.
        /// </summary>
        /// <param name="formats">The bit array to check.</param>
        /// <param name="onlyIncludeAvailableFormats">If <c>true</c>, only include formats that have a valid implementation. If <c>false</c>,
        /// the returned enumerable may include formats that are recognized as archive formats, but that do not have a proper implementation.</param>
        /// <returns>The <see cref="CompressedArchiveFormat"/> that are in the flags and that are available, depending on the value of <paramref name="onlyIncludeAvailableFormats"/>.</returns>
        public static IEnumerable<CompressedArchiveFormat> ToCompressedArchiveFormats(this EnabledCompressedArchiveFormats formats, bool onlyIncludeAvailableFormats)
        {
            if (formats == EnabledCompressedArchiveFormats.None)
            {
                yield break;
            }
            var returnAll = formats.HasFlag(EnabledCompressedArchiveFormats.All);
            foreach (EnabledCompressedArchiveFormats value in Enum.GetValues(typeof(EnabledCompressedArchiveFormats)))
            {
                switch (value)
                {
                    case EnabledCompressedArchiveFormats.Unknown:
                    case EnabledCompressedArchiveFormats.None:
                    case EnabledCompressedArchiveFormats.All:
                        break;
                    default:
                        if (returnAll || formats.HasFlag(value))
                        {
                            var compressedArchiveFormat = FlagToFormatData[value];
                            if (onlyIncludeAvailableFormats)
                            {
                                if (compressedArchiveFormat.IsCompressedArchiveFormatSupported())
                                {
                                    yield return compressedArchiveFormat;
                                }
                            }
                            else
                            {
                                yield return compressedArchiveFormat;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Given a collection of <see cref="CompressedArchiveFormat"/> values, produce a <see cref="EnabledCompressedArchiveFormats"/> bit array
        /// that represents the formats that are supported in the implementation.
        /// </summary>
        /// <param name="formats">The compressed archive formats from which the bit array is created.</param>
        /// <returns>A <see cref="EnabledCompressedArchiveFormats"/> bit array representing the supported <see cref="CompressedArchiveFormat"/>s in <paramref name="formats"/>.</returns>
        public static EnabledCompressedArchiveFormats FromCompressedArchiveFormats(this IEnumerable<CompressedArchiveFormat> formats)
        {
            return formats.FromCompressedArchiveFormats(onlyIncludeAvailableFormats: true);
        }

        /// <summary>
        /// Given a collection of <see cref="CompressedArchiveFormat"/> values, produce a <see cref="EnabledCompressedArchiveFormats"/> bit array
        /// that represents the formats.
        /// </summary>
        /// <param name="formats">The compressed archive formats from which the bit array is created.</param>
        /// <param name="onlyIncludeAvailableFormats">If <c>true</c>, only compressed archive formats with a supporting implementation are included in the result.
        /// If <c>false</c>, all recognized compressed archive formats will be represented.</param>
        /// <returns>A <see cref="EnabledCompressedArchiveFormats"/> bit array representing the <see cref="CompressedArchiveFormat"/>s in <paramref name="formats"/>,
        /// depending upon the value of <paramref name="onlyIncludeAvailableFormats"/>.</returns>
        public static EnabledCompressedArchiveFormats FromCompressedArchiveFormats(this IEnumerable<CompressedArchiveFormat> formats, bool onlyIncludeAvailableFormats)
        {
            var formatFlags = EnabledCompressedArchiveFormats.None;
            var values = Enum.GetValues(typeof(CompressedArchiveFormat)).Cast<CompressedArchiveFormat>();
            foreach (var format in formats.Where(f => f != CompressedArchiveFormat.None))
            {
                if (values.Contains(format))
                {
                    var formatFlag = (EnabledCompressedArchiveFormats)(1 << (int)format);
                    if (onlyIncludeAvailableFormats)
                    {
                        if (format.IsCompressedArchiveFormatSupported())
                        {
                            formatFlags |= formatFlag;
                        }
                    }
                    else
                    {
                        formatFlags |= formatFlag;
                    }
                }
                else
                {
                    if (!onlyIncludeAvailableFormats || format.IsCompressedArchiveFormatSupported())
                    {
                        formatFlags |= EnabledCompressedArchiveFormats.Unknown;
                    }
                }
            }
            return formatFlags;
        }

        /// <summary>
        /// Updates the availability of the supported compressed archive formats in the system based on the provided flags.
        /// </summary>
        /// <param name="formats">The compressed archive formats to mark as available.</param>
        /// <remarks>Any archives already opened prior to changing the availability of the archive type will remain usable
        /// in their current state.</remarks>
        public static void UpdateAvailableCompressedArchiveFormats(this EnabledCompressedArchiveFormats formats)
        {
            var supportedFormatsToEnable = formats.ToCompressedArchiveFormats();
            foreach (CompressedArchiveFormat possibleCompressedArchiveFormat in Enum.GetValues(typeof(CompressedArchiveFormat)))
            {
                switch (possibleCompressedArchiveFormat)
                {
                    case CompressedArchiveFormat.None:
                        break;
                    default:
                        if (supportedFormatsToEnable.Contains(possibleCompressedArchiveFormat))
                        {
                            possibleCompressedArchiveFormat.EnableCompressedArchiveFormat();
                        }
                        else
                        {
                            possibleCompressedArchiveFormat.DisableCompressedArchiveFormat();
                        }
                        break;
                }
            }
        }
    }
}
