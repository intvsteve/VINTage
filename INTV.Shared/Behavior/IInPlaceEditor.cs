// <copyright file="IInPlaceEditor.cs" company="INTV Funhouse">
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

using System;

#if WIN
using OSVisual = System.Windows.UIElement;
#elif MAC
using OSVisual = MonoMac.AppKit.NSView;
#endif

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Defines an interface for editing the contents of a visual, typically used in conjunction with an Adorner or
    /// other visual type that provides a means to edit content that is otherwise not editable.
    /// </summary>
    public interface IInPlaceEditor
    {
        /// <summary>
        /// This event is raised when the in-place editor has finished, either cancelling the edit operation or committing it.
        /// </summary>
        event EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        /// <summary>
        /// Gets the element that contains the data to be edited.
        /// </summary>
        OSVisual EditedElement { get; }

        /// <summary>
        /// Gets or sets the owner of the element being edited.
        /// </summary>
        OSVisual ElementOwner { get; set; }

        /// <summary>
        /// Call this method to indicate to the editor that it should begin editing operations.
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Call this method to cancel any edits and close the editor.
        /// </summary>
        /// <remarks>Calling this method causes the EditorClosed event to be raised.</remarks>
        void CancelEdit();

        /// <summary>
        /// Call this method to commit any edits and close the editor.
        /// </summary>
        /// <remarks>Calling this method causes the EditorClosed event to be raised.</remarks>
        void CommitEdit();
    }
}
