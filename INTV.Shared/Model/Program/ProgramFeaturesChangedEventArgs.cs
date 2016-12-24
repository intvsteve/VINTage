// <copyright file="ProgramFeaturesChangedEventArgs.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Event argument passed when one or more program ROM's features have been changed
    /// in the user interface.
    /// </summary>
    public class ProgramFeaturesChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Model.Program.ProgramFeaturesChangedEventArgs"/> class.
        /// </summary>
        /// <param name="updatedRoms">An enumerable of the ROMs whose features have been updated.</param>
        /// <param name="resetToDefault">If <c>true</c>, features are being reset to default values.</param>
        public ProgramFeaturesChangedEventArgs(IEnumerable<INTV.Core.Model.IRom> updatedRoms, bool resetToDefault)
        {
            UpdatedRoms = updatedRoms;
            ResetToDefault = resetToDefault;
        }

        /// <summary>
        /// Gets the enumerable of the ROMs whose features have changed.
        /// </summary>
        public IEnumerable<INTV.Core.Model.IRom> UpdatedRoms { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the ROMs features were reset to default values.
        /// </summary>
        public bool ResetToDefault { get; private set; }
    }
}
