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
}
