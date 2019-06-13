﻿// <copyright file="Crc32.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

////#define ENABLE_TABLE_GENERATOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Different 32-bit CRC polynomials supported.
    /// </summary>
    public enum Crc32Polynomial
    {
        /// <summary>
        /// The standard CRC-32 (ZIP) polynomial: 0xEDB88320, right-shifting, inverted; see e.g. https://www.scadacore.com/tools/programming-calculators/online-checksum-calculator/
        /// </summary>
        Zip,

        /// <summary>
        /// The CRC-32C (Castagnoli) polynomial: 0x82F63B78, right-shifting, inverted; see e.g. https://www.scadacore.com/tools/programming-calculators/online-checksum-calculator/
        /// </summary>
        Castagnoli,

        /// <summary>
        /// The default CRC-32 polynomial to use.
        /// </summary>
        Default = Zip
    }

    /// <summary>
    /// Provides methods to compute a 32-bit CRC value as specified by the ZIP compression standard.
    /// </summary>
    public static class Crc32
    {
        private static readonly Crc32Memo Memos = new Crc32Memo();

        /* ======================================================================== */
        /*  CRC-32 routines                                        J. Zbiciak, 2001 */
        /*                                           Adapted to C# by S. Orth, 2013 */
        /* ------------------------------------------------------------------------ */
        /*  This code is compatible with the CRC-32 that is used by the Zip file    */
        /*  compression standard.  To use this code for that purpose, initialize    */
        /*  your CRC to 0xFFFFFFFF, and XOR it with 0xFFFFFFFF afterwards.          */
        /* ------------------------------------------------------------------------ */
        /* ======================================================================== */

        /// <summary>
        /// The initial value to use for a CRC32 (inverting).
        /// </summary>
        public const uint InitialValue = 0xffffffff;

        /// <summary>
        /// The initial value to use for a CRC32 (non-inverting).
        /// </summary>
        public const uint InitialValueZero = 0x00000000;

        /// <summary>
        /// Default lookup table used for the CRC-32 code. Polynomial: 0xEDB88320
        /// </summary>
        private static readonly uint[] Crc32Table =
        {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
            0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
            0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
            0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
            0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
            0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
            0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
            0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
            0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
            0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
            0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
            0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
            0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
            0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
            0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
            0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
            0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
            0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
            0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
            0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
            0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
            0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
            0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
            0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
            0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
            0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
            0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
            0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
            0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
            0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
            0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
            0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
            0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
            0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
            0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
            0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
            0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
            0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
            0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
            0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
            0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
            0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
            0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
            0x2D02EF8D
        };

        /// <summary>
        /// Lookup table for CRC-32C (Castagnoli). Polynomial: 0x82F63B78
        /// </summary>
        private static readonly uint[] Crc32CTable =
        {
            0x00000000, 0xF26B8303, 0xE13B70F7, 0x1350F3F4, 0xC79A971F,
            0x35F1141C, 0x26A1E7E8, 0xD4CA64EB, 0x8AD958CF, 0x78B2DBCC,
            0x6BE22838, 0x9989AB3B, 0x4D43CFD0, 0xBF284CD3, 0xAC78BF27,
            0x5E133C24, 0x105EC76F, 0xE235446C, 0xF165B798, 0x030E349B,
            0xD7C45070, 0x25AFD373, 0x36FF2087, 0xC494A384, 0x9A879FA0,
            0x68EC1CA3, 0x7BBCEF57, 0x89D76C54, 0x5D1D08BF, 0xAF768BBC,
            0xBC267848, 0x4E4DFB4B, 0x20BD8EDE, 0xD2D60DDD, 0xC186FE29,
            0x33ED7D2A, 0xE72719C1, 0x154C9AC2, 0x061C6936, 0xF477EA35,
            0xAA64D611, 0x580F5512, 0x4B5FA6E6, 0xB93425E5, 0x6DFE410E,
            0x9F95C20D, 0x8CC531F9, 0x7EAEB2FA, 0x30E349B1, 0xC288CAB2,
            0xD1D83946, 0x23B3BA45, 0xF779DEAE, 0x05125DAD, 0x1642AE59,
            0xE4292D5A, 0xBA3A117E, 0x4851927D, 0x5B016189, 0xA96AE28A,
            0x7DA08661, 0x8FCB0562, 0x9C9BF696, 0x6EF07595, 0x417B1DBC,
            0xB3109EBF, 0xA0406D4B, 0x522BEE48, 0x86E18AA3, 0x748A09A0,
            0x67DAFA54, 0x95B17957, 0xCBA24573, 0x39C9C670, 0x2A993584,
            0xD8F2B687, 0x0C38D26C, 0xFE53516F, 0xED03A29B, 0x1F682198,
            0x5125DAD3, 0xA34E59D0, 0xB01EAA24, 0x42752927, 0x96BF4DCC,
            0x64D4CECF, 0x77843D3B, 0x85EFBE38, 0xDBFC821C, 0x2997011F,
            0x3AC7F2EB, 0xC8AC71E8, 0x1C661503, 0xEE0D9600, 0xFD5D65F4,
            0x0F36E6F7, 0x61C69362, 0x93AD1061, 0x80FDE395, 0x72966096,
            0xA65C047D, 0x5437877E, 0x4767748A, 0xB50CF789, 0xEB1FCBAD,
            0x197448AE, 0x0A24BB5A, 0xF84F3859, 0x2C855CB2, 0xDEEEDFB1,
            0xCDBE2C45, 0x3FD5AF46, 0x7198540D, 0x83F3D70E, 0x90A324FA,
            0x62C8A7F9, 0xB602C312, 0x44694011, 0x5739B3E5, 0xA55230E6,
            0xFB410CC2, 0x092A8FC1, 0x1A7A7C35, 0xE811FF36, 0x3CDB9BDD,
            0xCEB018DE, 0xDDE0EB2A, 0x2F8B6829, 0x82F63B78, 0x709DB87B,
            0x63CD4B8F, 0x91A6C88C, 0x456CAC67, 0xB7072F64, 0xA457DC90,
            0x563C5F93, 0x082F63B7, 0xFA44E0B4, 0xE9141340, 0x1B7F9043,
            0xCFB5F4A8, 0x3DDE77AB, 0x2E8E845F, 0xDCE5075C, 0x92A8FC17,
            0x60C37F14, 0x73938CE0, 0x81F80FE3, 0x55326B08, 0xA759E80B,
            0xB4091BFF, 0x466298FC, 0x1871A4D8, 0xEA1A27DB, 0xF94AD42F,
            0x0B21572C, 0xDFEB33C7, 0x2D80B0C4, 0x3ED04330, 0xCCBBC033,
            0xA24BB5A6, 0x502036A5, 0x4370C551, 0xB11B4652, 0x65D122B9,
            0x97BAA1BA, 0x84EA524E, 0x7681D14D, 0x2892ED69, 0xDAF96E6A,
            0xC9A99D9E, 0x3BC21E9D, 0xEF087A76, 0x1D63F975, 0x0E330A81,
            0xFC588982, 0xB21572C9, 0x407EF1CA, 0x532E023E, 0xA145813D,
            0x758FE5D6, 0x87E466D5, 0x94B49521, 0x66DF1622, 0x38CC2A06,
            0xCAA7A905, 0xD9F75AF1, 0x2B9CD9F2, 0xFF56BD19, 0x0D3D3E1A,
            0x1E6DCDEE, 0xEC064EED, 0xC38D26C4, 0x31E6A5C7, 0x22B65633,
            0xD0DDD530, 0x0417B1DB, 0xF67C32D8, 0xE52CC12C, 0x1747422F,
            0x49547E0B, 0xBB3FFD08, 0xA86F0EFC, 0x5A048DFF, 0x8ECEE914,
            0x7CA56A17, 0x6FF599E3, 0x9D9E1AE0, 0xD3D3E1AB, 0x21B862A8,
            0x32E8915C, 0xC083125F, 0x144976B4, 0xE622F5B7, 0xF5720643,
            0x07198540, 0x590AB964, 0xAB613A67, 0xB831C993, 0x4A5A4A90,
            0x9E902E7B, 0x6CFBAD78, 0x7FAB5E8C, 0x8DC0DD8F, 0xE330A81A,
            0x115B2B19, 0x020BD8ED, 0xF0605BEE, 0x24AA3F05, 0xD6C1BC06,
            0xC5914FF2, 0x37FACCF1, 0x69E9F0D5, 0x9B8273D6, 0x88D28022,
            0x7AB90321, 0xAE7367CA, 0x5C18E4C9, 0x4F48173D, 0xBD23943E,
            0xF36E6F75, 0x0105EC76, 0x12551F82, 0xE03E9C81, 0x34F4F86A,
            0xC69F7B69, 0xD5CF889D, 0x27A40B9E, 0x79B737BA, 0x8BDCB4B9,
            0x988C474D, 0x6AE7C44E, 0xBE2DA0A5, 0x4C4623A6, 0x5F16D052,
            0xAD7D5351
        };

        /// <summary>
        /// Populates a 256-element lookup table for a right-shifted CRC32 polynomial.
        /// </summary>
        /// <param name="polynomial">The polynomial to use to populate the table.</param>
        /// <param name="table">Receives the table.</param>
        /// <param name="stringTable">Receives the table represented as a formatted string, suitable for use in source code.</param>
        [System.Diagnostics.Conditional("ENABLE_TABLE_GENERATOR")]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void BuildCrc32Table(uint polynomial, uint[] table, ref string stringTable)
        {
            var tableBuilder = new System.Text.StringBuilder();
            uint crc;
            for (int i = 0; i < 256; i++)
            {
                crc = (uint)i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = (crc >> 1) ^ polynomial;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }

                table[i] = crc;
                if (i % 5 == 0)
                {
                    tableBuilder.AppendLine();
                }
                tableBuilder.AppendFormat("0x{0:X8}, ", crc);
            }
            stringTable = tableBuilder.ToString();
        }

        /// <summary>
        /// Compute a CRC value for a file.
        /// </summary>
        /// <param name="file">The absolute path to the file for which to compute a 32-bit CRC.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfFile(string file)
        {
            return OfFile(file, false, 0, null);
        }

        /// <summary>
        /// Compute a CRC value for a file.
        /// </summary>
        /// <param name="file">The absolute path to the file for which to compute a 32-bit CRC.</param>
        /// <param name="ignoreRanges">Ranges of bytes, defined as indexes into the byte data in <paramref name="file"/>, to exclude from the checksum.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfFile(string file, IEnumerable<Range<int>> ignoreRanges)
        {
            return OfFile(file, false, 0, ignoreRanges);
        }

        /// <summary>
        /// Compute a CRC value for a file.
        /// </summary>
        /// <param name="file">The absolute path to the file for which to compute a 32-bit CRC.</param>
        /// <param name="replaceFirstByte">If <c>true</c>, replaces the first byte in the calculation with the value in alternateFirstByte.</param>
        /// <param name="alternateFirstByte">If useAlternateByte is true, replaces the first byte of the stream with this value for the calculation.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfFile(string file, bool replaceFirstByte, byte alternateFirstByte)
        {
            return OfFile(file, replaceFirstByte, alternateFirstByte, null);
        }

        /// <summary>
        /// Compute a CRC value for a file.
        /// </summary>
        /// <param name="file">The absolute path to the file for which to compute a 32-bit CRC.</param>
        /// <param name="replaceFirstByte">If <c>true</c>, replaces the first byte in the calculation with the value in alternateFirstByte.</param>
        /// <param name="alternateFirstByte">If useAlternateByte is true, replaces the first byte of the stream with this value for the calculation.</param>
        /// <param name="ignoreRanges">Ranges of bytes, defined as indexes into the byte data in <paramref name="file"/>, to exclude from the checksum.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfFile(string file, bool replaceFirstByte, byte alternateFirstByte, IEnumerable<Range<int>> ignoreRanges)
        {
            uint crc = CheckMemo(file, replaceFirstByte, alternateFirstByte, ignoreRanges);
            return crc;
        }

        /// <summary>
        /// Compute a CRC value for a stream.
        /// </summary>
        /// <param name="dataStream">The data stream upon which to compute a CRC.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfStream(Stream dataStream)
        {
            return OfStream(dataStream, false, 0);
        }

        /// <summary>
        /// Compute a CRC value for a stream.
        /// </summary>
        /// <param name="dataStream">The data stream upon which to compute a CRC.</param>
        /// <param name="fromStartOfStream">If <c>true</c>, seek to the beginning of <paramref name="dataStream"/> before starting the computation.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        public static uint OfStream(Stream dataStream, bool fromStartOfStream)
        {
            return OfStream(dataStream, false, 0, null, fromStartOfStream);
        }

        /// <summary>
        /// Compute a CRC value for a stream.
        /// </summary>
        /// <param name="dataStream">The data stream upon which to compute a CRC.</param>
        /// <param name="ignoreRanges">Ranges of bytes, defined as indexes into the byte data in <paramref name="dataStream"/>, to exclude from the checksum.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        /// <remarks>This method will seek to the beginning of <paramref name="dataStream"/> before computing the CRC.</remarks>
        public static uint OfStream(Stream dataStream, IEnumerable<Range<int>> ignoreRanges)
        {
            return OfStream(dataStream, false, 0, ignoreRanges);
        }

        /// <summary>
        /// Compute a CRC value for a stream.
        /// </summary>
        /// <param name="dataStream">The data stream upon which to compute a CRC.</param>
        /// <param name="replaceFirstByte">If <c>true</c>, replaces the first byte in the calculation with the value in alternateFirstByte.</param>
        /// <param name="alternateFirstByte">If useAlternateByte is true, replaces the first byte of the stream with this value for the calculation.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        /// <remarks>This method will seek to the beginning of <paramref name="dataStream"/> before computing the CRC.</remarks>
        private static uint OfStream(Stream dataStream, bool replaceFirstByte, byte alternateFirstByte)
        {
            return OfStream(dataStream, replaceFirstByte, alternateFirstByte, null);
        }

        /// <summary>
        /// Compute a CRC value for a stream.
        /// </summary>
        /// <param name="dataStream">The data stream upon which to compute a CRC.</param>
        /// <param name="replaceFirstByte">If <c>true</c>, replaces the first byte in the calculation with the value in alternateFirstByte.</param>
        /// <param name="alternateFirstByte">If useAlternateByte is true, replaces the first byte of the stream with this value for the calculation.</param>
        /// <param name="ignoreRanges">Ranges of bytes, defined as indexes into the byte data in <paramref name="dataStream"/>, to exclude from the checksum.</param>
        /// <param name="fromStartOfStream">If <c>true</c>, seek to the beginning of <paramref name="dataStream"/> before starting the computation.</param>
        /// <returns>The 32-bit CRC of the stream.</returns>
        /// <remarks>This method will seek to the beginning of <paramref name="dataStream"/> before computing the CRC.</remarks>
        private static uint OfStream(Stream dataStream, bool replaceFirstByte, byte alternateFirstByte, IEnumerable<Range<int>> ignoreRanges, bool fromStartOfStream = true)
        {
            if (fromStartOfStream)
            {
                dataStream.Seek(0, SeekOrigin.Begin);
            }
            var crc = InitialValue;
            var data = new byte[1024];
            var numBytesRead = 0;
            var totalRead = 0;
            do
            {
                numBytesRead = dataStream.Read(data, 0, 1024);
                if ((totalRead == 0) && replaceFirstByte)
                {
                    data[0] = alternateFirstByte;
                }
                totalRead += numBytesRead;
                crc = OfBlock(data, numBytesRead, ignoreRanges, crc);
            }
            while (numBytesRead > 0);

            return crc ^ InitialValue;
        }

        /// <summary>
        /// Compute a CRC value for a block of data.
        /// </summary>
        /// <param name="data">The data from which to compute the CRC.</param>
        /// <returns>The CRC of the data block.</returns>
        public static uint OfBlock(byte[] data)
        {
            uint crc = OfBlock(data, InitialValue) ^ InitialValue;
            return crc;
        }

        /// <summary>
        /// Compute a CRC value for a block of data using a specific polynomial.
        /// </summary>
        /// <param name="data">The data from which to compute the CRC.</param>
        /// <param name="polynomial">The polynomial to use.</param>
        /// <returns>The CRC of the data block.</returns>
        public static uint OfBlock(byte[] data, Crc32Polynomial polynomial)
        {
            var initialValue = InitialValue;
            switch (polynomial)
            {
                case Crc32Polynomial.Castagnoli:
                    initialValue = InitialValueZero;
                    break;
                default:
                    break;
            }
            uint crc = OfBlock(data, polynomial, data.Length, initialValue) ^ initialValue;
            return crc;
        }

        /// <summary>
        /// Compute a CRC value for a block of data.
        /// </summary>
        /// <param name="data">The data from which to compute an updated running CRC.</param>
        /// <param name="runningValue">Running value of the CRC.</param>
        /// <returns>Running CRC, updated with the contents of the given data block.</returns>
        public static uint OfBlock(byte[] data, uint runningValue)
        {
            return OfBlock(data, data.Length, null, runningValue);
        }

        /// <summary>
        /// Compute a CRC value for a block of data.
        /// </summary>
        /// <param name="data">The data from which to compute an updated running CRC.</param>
        /// <param name="numBytesToProcess">Number of bytes in the buffer to process (always start at first element).</param>
        /// <param name="ignoreRanges">Ranges of bytes, defined as indexes into the <paramref name="data"/> array, to exclude from the checksum.</param>
        /// <param name="runningValue">Running value of the CRC.</param>
        /// <returns>Running CRC, updated with the contents of the given data block.</returns>
        public static uint OfBlock(byte[] data, int numBytesToProcess, IEnumerable<Range<int>> ignoreRanges, uint runningValue)
        {
            var crc = runningValue;
            var checkIgnoreRange = (ignoreRanges != null) && ignoreRanges.Any(r => r.IsValid);
            for (var i = 0; i < numBytesToProcess; ++i)
            {
                var ignore = checkIgnoreRange;
                if (checkIgnoreRange)
                {
                    ignore = ignoreRanges.Any(r => r.IsValueInRange(i));
                }
                if (!ignore)
                {
                    crc = Update(crc, data[i]);
                }
            }
            return crc;
        }

        /// <summary>
        /// Accumulates a CRC value using the internally specified lookup table.
        /// </summary>
        /// <param name="crc">The current CRC value.</param>
        /// <param name="data">The new data to incorporate into the CRC.</param>
        /// <returns>The new CRC value.</returns>
        /// <remarks>The 32-bit CRC is set up as a right-shifting CRC with no inversions.</remarks>
        public static uint Update(uint crc, byte data)
        {
            return (crc >> 8) ^ Crc32Table[(crc ^ data) & 0xFF];
        }

        /// <summary>
        /// Compute a CRC value for a block of data.
        /// </summary>
        /// <param name="data">The data from which to compute an updated running CRC.</param>
        /// <param name="polynomial">The polynomial to use.</param>
        /// <param name="numBytesToProcess">Number of bytes in the buffer to process (always start at first element).</param>
        /// <param name="runningValue">Running value of the CRC.</param>
        /// <returns>Running CRC, updated with the contents of the given data block.</returns>
        private static uint OfBlock(byte[] data, Crc32Polynomial polynomial, int numBytesToProcess, uint runningValue)
        {
            uint[] lookupTable = null;
            switch (polynomial)
            {
                case Crc32Polynomial.Zip:
                    lookupTable = Crc32Table;
                    break;
                case Crc32Polynomial.Castagnoli:
                    lookupTable = Crc32CTable;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var crc = runningValue;
            for (var i = 0; i < numBytesToProcess; ++i)
            {
                crc = Update(crc, data[i], lookupTable);
            }
            return crc;
        }

        /// <summary>
        /// Accumulates a CRC value using the internally specified lookup table.
        /// </summary>
        /// <param name="crc">The current CRC value.</param>
        /// <param name="data">The new data to incorporate into the CRC.</param>
        /// <param name="lookupTable">The lookup table to use.</param>
        /// <returns>The new CRC value.</returns>
        /// <remarks>The 32-bit CRC is set up as a right-shifting CRC with no inversions.</remarks>
        private static uint Update(uint crc, byte data, uint[] lookupTable)
        {
            return (crc >> 8) ^ lookupTable[(crc ^ data) & 0xFF];
        }

        private static uint CheckMemo(string file, bool replaceFirstByte, byte alternateFirstByte, IEnumerable<Range<int>> ignoreRanges)
        {
            uint crc;
            Memos.CheckAddMemo(file, new Tuple<bool, byte, IEnumerable<Range<int>>>(replaceFirstByte, alternateFirstByte, ignoreRanges), out crc);
            return crc;
        }

        private class Crc32Memo : FileMemo<uint>
        {
            public Crc32Memo()
                : base(StreamUtilities.DefaultStorage)
            {
            }

            /// <inheritdoc />
            protected override uint DefaultMemoValue
            {
                get { return InitialValue; }
            }

            /// <inheritdoc />
            protected override uint GetMemo(string filePath, object data)
            {
                uint crc = InitialValue;
                using (var fileStream = StreamUtilities.OpenFileStream(filePath, StorageAccess))
                {
                    var supportData = (Tuple<bool, byte, IEnumerable<Range<int>>>)data;
                    var replaceFirstByte = supportData.Item1;
                    var alternateFirstByte = supportData.Item2;
                    var ignoreRanges = supportData.Item3;
                    crc = OfStream(fileStream, replaceFirstByte, alternateFirstByte, ignoreRanges);
                }
                return crc;
            }

            /// <inheritdoc />
            protected override bool IsValidMemo(uint memo)
            {
                return memo != DefaultMemoValue;
            }
        }
    }
}
