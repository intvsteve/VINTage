// <copyright file="RomDiscoveryData.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

using System.Collections.Generic;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Specialize AsyncTaskData for use with ROM searches / discovery.
    /// </summary>
    internal class RomDiscoveryData : AsyncTaskData
    {
        /// <summary>
        /// Initialize a new instance of RomDiscoveryData.
        /// </summary>
        /// <param name="potentialRoms">An enumerable of potential ROM file paths.</param>
        /// <param name="currentRoms">The known list of ROMs.</param>
        /// <param name="insertLocation">The insertion point for the newly added ROMs. A value if <c>-1</c> indicates to append to the existing ROMs</param>
        /// <param name="title">The title to show for the progress indicator.</param>
        /// <param name="options">ROM discovery behaviors.</param>
        public RomDiscoveryData(IEnumerable<string> potentialRoms, IEnumerable<ProgramDescription> currentRoms, int insertLocation, string title, RomDiscoveryOptions options)
            : base(null)
        {
            Options = options;
            Title = title;
            CurrentRoms = currentRoms;
            PotentialRoms = potentialRoms;
            NewRoms = new List<ProgramDescription>();
            RejectedRoms = new List<IRom>();
            DuplicateRomPaths = new List<string>();
            InsertLocation = insertLocation;
        }

        /// <summary>
        /// Gets the ROM discovery options.
        /// </summary>
        public RomDiscoveryOptions Options { get; private set; }

        /// <summary>
        /// Gets the title to show for reporting progress during the discovery process.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the current ROMs.
        /// </summary>
        public IEnumerable<ProgramDescription> CurrentRoms { get; private set; }

        /// <summary>
        /// Gets the potential ROMs.
        /// </summary>
        public IEnumerable<string> PotentialRoms { get; private set; }

        /// <summary>
        /// Gets or sets the newly found ROMs.
        /// </summary>
        public IList<ProgramDescription> NewRoms { get; set; }

        /// <summary>
        /// Gets the list of modified ROMs.
        /// </summary>
        public IList<IRom> ModifiedRoms { get; private set; }

        /// <summary>
        /// Gets the list of rejected ROMs.
        /// </summary>
        public IList<IRom> RejectedRoms { get; private set; }

        /// <summary>
        /// Gets the list of ROMs that were not found during the search, but which were in the CurrentRoms list.
        /// </summary>
        public IList<IRom> MissingRoms { get; private set; }

        /// <summary>
        /// Gets a list of duplicate ROM paths. These ROMs were not added because they were already in the list.
        /// </summary>
        public IList<string> DuplicateRomPaths { get; private set; }

        /// <summary>
        /// Gets the insert location for the ROMs to add.
        /// </summary>
        /// <remarks>A value of <c>-1</c> indicates to append to the end of the list.</remarks>
        public int InsertLocation { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this operation is adding the "starter" ROMs.
        /// </summary>
        public bool AddingStarterRoms { get; set; }
    }
}
