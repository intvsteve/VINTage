// <copyright file="SortCommandData.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// This class supplies data to a command that sorts a list or table of data.
    /// </summary>
    public class SortCommandData
    {
        /// <summary>
        /// Initializes a new instance of SortCommandData.
        /// </summary>
        /// <param name="sortDirection">The direction to sort the data in.</param>
        public SortCommandData(System.ComponentModel.ListSortDirection sortDirection)
            : this(sortDirection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of SortCommandData.
        /// </summary>
        /// <param name="sortDirection">The direction to sort the data in.</param>
        /// <param name="sortData">Sort-command-specific data.</param>
        /// <remarks>Typically, of the sort is operating on a table, this data indicates which column to sort on.</remarks>
        public SortCommandData(System.ComponentModel.ListSortDirection sortDirection, object sortData)
        {
            SortData = sortData;
            SortDirection = sortDirection;
        }

        /// <summary>
        /// Gets a value the sort command may use to refine a sort operation, e.g. which column to sort a table by.
        /// </summary>
        public object SortData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to perform an ascending or descending sort.
        /// </summary>
        public System.ComponentModel.ListSortDirection SortDirection { get; private set; }
    }
}
