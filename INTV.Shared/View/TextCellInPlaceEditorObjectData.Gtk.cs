// <copyright file="TextCellInPlaceEditorObjectData.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
//

namespace INTV.Shared.View
{
    /// <summary>
    /// Text cell in place editor object data.
    /// </summary>
    public class TextCellInPlaceEditorObjectData : System.Tuple<Gtk.TreePath, Gtk.TreeViewColumn, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.TextCellInPlaceEditorObjectData"/> class.
        /// </summary>
        /// <param name="itemPath">The path to the item being edited.</param>
        /// <param name="column">The column being edited.</param>
        public TextCellInPlaceEditorObjectData(Gtk.TreePath itemPath, Gtk.TreeViewColumn column)
            : this(itemPath, column, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.TextCellInPlaceEditorObjectData"/> class.
        /// </summary>
        /// <param name="itemPath">The path to the item being edited.</param>
        /// <param name="column">The column being edited.</param>
        /// <param name="data">The data in the column being edited.</param>
        public TextCellInPlaceEditorObjectData(Gtk.TreePath itemPath, Gtk.TreeViewColumn column, object data)
            : base(itemPath, column, data)
        {
        }

        /// <summary>
        /// Gets the path to the item being edited.
        /// </summary>
        public Gtk.TreePath Path
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the column of the edited item.
        /// </summary>
        public Gtk.TreeViewColumn Column
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public object Data
        {
            get { return Item3; }
        }
    }
}
