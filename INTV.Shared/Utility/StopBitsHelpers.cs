// <copyright file="StopBitsHelpers.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Helper methods for the System.IO.Ports.StopBits enumeration.
    /// </summary>
    public static class StopBitsHelpers
    {
        /// <summary>
        /// Converts the System.IO.Ports.StopBits enumeration to a friendly display string.
        /// </summary>
        /// <param name="stopBits">The stop bits to convert to a display string.</param>
        /// <returns>The display string.</returns>
        public static string ToDisplayString(this System.IO.Ports.StopBits stopBits)
        {
            string stopBitsString = null;
            switch (stopBits)
            {
                case System.IO.Ports.StopBits.None:
                    stopBitsString = "0";
                    break;
                case System.IO.Ports.StopBits.One:
                    stopBitsString = "1";
                    break;
                case System.IO.Ports.StopBits.OnePointFive:
                    stopBitsString = "1.5";
                    break;
                case System.IO.Ports.StopBits.Two:
                    stopBitsString = "2";
                    break;
            }
            return stopBitsString;
        }
    }
}
