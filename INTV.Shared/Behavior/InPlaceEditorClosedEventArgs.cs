// <copyright file="InPlaceEditorClosedEventArgs.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
    /// Event arguments passed to the IInPlaceEditor.EditorClosed event handler.
    /// </summary>
    public class InPlaceEditorClosedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the InPlaceEditorClosedEventArgs type.
        /// </summary>
        /// <param name="committedChanges">If <c>true</c>, indicates that when the IInPlaceEditor closed, it committed any changes to the item it was editing.</param>
        public InPlaceEditorClosedEventArgs(bool committedChanges)
            : this(committedChanges, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Behavior.InPlaceEditorClosedEventArgs"/> class.
        /// </summary>
        /// <param name="committedChanges">If <c>true</c>, indicates that when the IInPlaceEditor closed, it committed any changes to the item it was editing.</param>
        /// <param name="state">State that may be meaningful to recipient.</param>
        public InPlaceEditorClosedEventArgs(bool committedChanges, object state)
        {
            CommitedChanges = committedChanges;
            State = state;
        }

        /// <summary>
        /// Gets a value indicating whether or not changes made by the IInPlaceEditor were committed.
        /// </summary>
        public bool CommitedChanges { get; private set; }

        /// <summary>
        /// Gets implementation-defined state data that may be passed through the event.
        /// </summary>
        public object State { get; private set; }
    }
}
