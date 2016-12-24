// <copyright file="CanonicalRomComparerStrict.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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

using INTV.Core.Model;
using INTV.Core.Model.Program;

namespace INTV.Shared.Model
{
    // UNDONE This comparer isn't in use. It has performance issues when used in
    // large ROM sets because of circumstances that can cause repeated regeneration
    // of LUIGI files to do the canonical compare. Also, with the base class being
    // in the INTV.Core assembly, this has the disadvantage of needing features not
    // available in INTV.Core. It could be plugged into INTV.Core at run-time, but
    // the performance concerns have precluded its adoption.

    /// <summary>
    /// Implements a comparer based on the canonical ROM format. This requires that the two ROM binaries are equal
    /// AND that there are no differences in their features (e.g. compatibility with other hardware may NOT differ).
    /// If the original ROM(s) are of the .bin+.cfg form, the .cfg files for both ROMs *MUST BE IDENTICAL* for equality.
    /// Thus, even one extra space in one of the .cfg files would result in an inequality, even if the two .bin files
    /// were identical.
    /// </summary>
    public class CanonicalRomComparerStrict : CanonicalRomComparer
    {
        /// <summary>
        /// The default comparer instance.
        /// </summary>
        public static readonly new CanonicalRomComparerStrict Default = new CanonicalRomComparerStrict();

        private static readonly RomComparer BaseComparer = new RomComparerStrict();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of CanonicalRomComparerStrict.
        /// </summary>
        public CanonicalRomComparerStrict()
            : base()
        {
        }

        #endregion // Constructor

        #region RomComparer

        /// <inheritdoc />
        public override int Compare(IRom x, IProgramInformation programInformationX, IRom y, IProgramInformation programInformationY)
        {
            var result = BaseComparer.Compare(x, programInformationX, y, programInformationY);
            if (result != 0)
            {
                result = CanonicalCompare(x, y, true); // require features to match as well
            }
            return result;
        }

        #endregion // RomComparer
    }
}
