// <copyright file="ILfsFileInfoHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

using System.Linq;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Extension methods to assist working with the ILfsFileInfo interface.
    /// </summary>
    public static class ILfsFileInfoHelpers
    {
        /// <summary>
        /// Remove all the forks from a file.
        /// </summary>
        /// <param name="file">The file from which all forks are to be removed.</param>
        public static void RemoveForks(this ILfsFileInfo file)
        {
            if (file != null)
            {
                file.Rom = null;
                file.Manual = null;
                file.JlpFlash = null;
                file.Vignette = null;
                file.ReservedFork4 = null;
                file.ReservedFork5 = null;
                file.ReservedFork6 = null;
            }
        }

        /// <summary>
        /// Determines whether a file is using a particular Fork.
        /// </summary>
        /// <param name="file">The file whose Forks are being searched.</param>
        /// <param name="fork">The fork of interest.</param>
        /// <returns><c>true</c> if the file uses the given Fork.</returns>
        public static bool UsesFork(this ILfsFileInfo file, Fork fork)
        {
            bool usesFork = false;
            if (file != null && fork != null)
            {
                Fork[] forks = new Fork[(int)ForkKind.NumberOfForkKinds]
                {
                    file.Rom,
                    file.Manual,
                    file.JlpFlash,
                    file.Vignette,
                    file.ReservedFork4,
                    file.ReservedFork5,
                    file.ReservedFork6,
                };
                usesFork = forks.Contains(fork);
            }
            return usesFork;
        }
    }
}
