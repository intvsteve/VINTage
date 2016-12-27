// <copyright file="SampleRate.cs" company="INTV Funhouse">
// Copyright (c) 2012-2016 All Rights Reserved
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Audio sample rates for jzIntv sound output.
    /// </summary>
    public enum SampleRate
    {
        /// <summary>
        /// The default audio output rate (44.1 kHz)
        /// </summary>
        Default = FortyFourK,

        /// <summary>
        /// 8 kHz.
        /// </summary>
        EightK = 8000,

        /// <summary>
        /// 11 kHz.
        /// </summary>
        ElevenK = 11025,

        /// <summary>
        /// 16 kHz.
        /// </summary>
        SixteenK = 16000,

        /// <summary>
        /// One half of the standard CD audio rate of 44.1 kHz.
        /// </summary>
        TwentyTwoK = 22050,

        /// <summary>
        /// 32 kHz.
        /// </summary>
        ThirtyTwoK = 32000,

        /// <summary>
        /// 44.1 kHz.
        /// </summary>
        FortyFourK = 44100,

        /// <summary>
        /// 48 kHz.
        /// </summary>
        FortyEightK = 48000
    }
/*
    /// <summary>
    /// Extension methods for the SampleRate enumeration.
    /// </summary>
    public static class SampleRateHelpers
    {
        private static readonly INTVFunhouse.JzIntv.Converter.SampleRateToStringConverter Converter = new INTVFunhouse.JzIntv.Converter.SampleRateToStringConverter();

        /// <summary>
        /// Converts a SampleRate value to a display string.
        /// </summary>
        /// <param name="sampleRate">The SampleRate to convert to a string.</param>
        /// <returns>The SampleRate as a display string.</returns>
        public static string ToDisplayString(this SampleRate sampleRate)
        {
            return Converter.Convert(sampleRate, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture) as string;
        }

        /// <summary>
        /// Creates a SampleRate value from a string.
        /// </summary>
        /// <param name="sampleRateDisplayString">The string to convert to a SampleRate value.</param>
        /// <returns>A SampleRate value from the given string.</returns>
        public static SampleRate FromDisplayString(this string sampleRateDisplayString)
        {
            return (SampleRate)Converter.ConvertBack(sampleRateDisplayString, typeof(SampleRate), null, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
    */
}
