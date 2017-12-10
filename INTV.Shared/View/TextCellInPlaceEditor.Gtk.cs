// <copyright file="TextCellInPlaceEditor.Gtk.cs" company="INTV Funhouse">
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

////#define ENABLE_INPLACEEDIT_TRACE

using System;
using INTV.Shared.Behavior;

namespace INTV.Shared.View
{
    /// <summary>
    /// Text cell in place editor implementation for use with Gtk.TreeView and Gtk.Entry.
    /// </summary>
    /// <remarks>This offers a mechanism to restrict the length of the text being entered, as well as a
    /// filtering mechanism to restrict the set of allowed characters.</remarks>
    public sealed class TextCellInPlaceEditor : IInPlaceEditor, IDisposable
    {
        private TextCellInPlaceEditor(Gtk.Widget owner, int maxLength, Gtk.Widget editedElement)
        {
            ElementOwner = owner;
            MaxLength = maxLength;
            EditedElement = editedElement;
            IsValidCharacter = _ => true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.TextCellInPlaceEditor"/> class.
        /// </summary>
        /// <param name="owner">Main visual the editor is associated with, e.g. a Gtk.TreeView or Gtk.Entry.</param>
        /// <param name="maxLength">Maximum length of the text being edited. If this value is <c>-1</c>, text length is not limited.</param>
        public TextCellInPlaceEditor(Gtk.Entry owner, int maxLength)
            : this(owner, maxLength, owner)
        {
            Editor.MaxLength = MaxLength;
            Editor.TextInserted += HandleTextInserted;
            owner.KeyPressEvent += HandleEditorKeyPressEvent;
            owner.Activated += HandleEditorValueCommit;
        }

        /// <summary>
        /// Initializes a new instance of the in-place editor for a text cell.
        /// </summary>
        /// <param name="owner">The visual owning the element being edited.</param>
        /// <param name="column">The column in the Gtk.TreeView being edited.</param>
        /// <param name="initialValue">The initial value of the edited item.</param>
        /// <param name="editingObject">The entity being edited.</param>
        public TextCellInPlaceEditor(Gtk.TreeView owner, Gtk.TreeViewColumn column, Gtk.CellRendererText cell, int maxLength)
            : this(owner, maxLength, null)
        {
            cell.Editable = true;
            cell.EditingStarted += CellEditingStarted;
            EditingObject = new TextCellInPlaceEditorObjectData(null, column);
            Renderer = cell;
        }

        #region Properties

        #region IInPlaceEditor Properties

        /// <inheritdoc/>
        public Gtk.Widget EditedElement { get; set; }

        /// <inheritdoc/>
        public Gtk.Widget ElementOwner { get; set; }

        #endregion // IInPlaceEditor Properties

        /// <summary>
        /// Gets or sets the maximum length allowed for the string being edited.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a function to call to verify if a character is allowed.
        /// </summary>
        public System.Predicate<char> IsValidCharacter { get; set; }

        /// <summary>
        /// Gets or sets the object being edited.
        /// </summary>
        public TextCellInPlaceEditorObjectData EditingObject { get; set; }

        private Gtk.CellRendererText Renderer { get; set; }

        private Gtk.Entry Editor { get { return EditedElement as Gtk.Entry; } }

        #endregion // Properties

        #region IInPlaceEditor

        /// <inheritdoc/>
        public event System.EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        /// <inheritdoc/>
        public void BeginEdit()
        {
            DebugOutput("!$!$!$!$ BEGIN EDIT");
            var treeView = ElementOwner as Gtk.TreeView;
            treeView.SetCursorOnCell(EditingObject.Path, EditingObject.Column, Renderer, true);
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            DebugOutput("!$!$!$!$ CANCEL EDIT");
            Renderer.StopEditing(true); // UNTESTED
        }

        /// <inheritdoc/>
        public void CommitEdit()
        {
            DebugOutput("!$!$!$!$ COMMIT EDIT");
            Renderer.StopEditing(false); // UNTESTED
        }

        #endregion // IInPlaceEditor

        #region IDisposable

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable

        [System.Diagnostics.Conditional("ENABLE_INPLACEEDIT_TRACE")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ElementOwner is Gtk.Entry)
                {
                    var owner = (Gtk.Entry)ElementOwner;
                    Editor.TextInserted -= HandleTextInserted;
                    owner.KeyPressEvent -= HandleEditorKeyPressEvent;
                    owner.Activated -= HandleEditorValueCommit;
                }
                if (Renderer != null)
                {
                    Renderer.EditingStarted -= CellEditingStarted;
                }
                ElementOwner = null;
                EditedElement = null;
                EditingObject = null;
                Renderer = null;
            }
        }

        private void CellEditingCanceled (object sender, EventArgs e)
        {
            DebugOutput("!$!$!$!$ CellEditingCanceled");
            CellEditingEnded(null, null, false);
        }

        private void CellEdited(object o, Gtk.EditedArgs args)
        {
            DebugOutput("!$!$!$!$ CellEdited");
            CellEditingEnded(args.NewText, new Gtk.TreePath(args.Path), true);
        }

        private void CellEditingStarted(object o, Gtk.EditingStartedArgs args)
        {
            DebugOutput("!$!$!$!$ CellEditingStarted");
            Renderer.Edited += CellEdited;
            Renderer.EditingCanceled += CellEditingCanceled;
            EditedElement = args.Editable as Gtk.Entry;
            Editor.MaxLength = MaxLength;
            Editor.TextInserted += HandleTextInserted;
            if (EditingObject != null)
            {
                EditingObject = new TextCellInPlaceEditorObjectData(new Gtk.TreePath(args.Path), EditingObject.Column, Editor.Text);
            }
        }

        private void CellEditingEnded(string newValue, Gtk.TreePath path, bool commit)
        {
            if (commit)
            {
                EditingObject = new TextCellInPlaceEditorObjectData(path, EditingObject.Column, newValue);
                CommitEditCore();
            }
            else
            {
                CancelEditCore();
            }
            Renderer.EditingCanceled -= CellEditingCanceled;
            Renderer.Edited -= CellEdited;
            Editor.TextInserted -= HandleTextInserted;
            EditedElement = null;
            EditingObject = new TextCellInPlaceEditorObjectData(EditingObject.Path, EditingObject.Column);
        }

        private void HandleTextInserted(object o, Gtk.TextInsertedArgs args)
        {
            var entry = o as Gtk.Entry;
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(entry, Editor));
            entry.TextInserted -= HandleTextInserted; // disconnect so we don't reenter if restricting characters
            var position = args.Position;
            var currentText = entry.Text;
            foreach (var character in args.Text)
            {
                if (!IsValidCharacter(character))
                {
                    var index = currentText.IndexOf(character);
                    while (index >= 0)
                    {
                        entry.DeleteText(index,index + 1);
                        args.Position = args.Position - 1;
                        currentText = entry.Text;
                        index = currentText.IndexOf(character);
                    }
                }
            }

            entry.TextInserted += HandleTextInserted;
        }

        private void HandleEditorKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Escape)
            {
                // Pushes focus out to default and cancels editing.
                var entry = (Gtk.Entry)o;
                entry.GetParent<Gtk.Window>().Focus = null;
                args.RetVal = true;
                CancelEditCore();
            }
        }

        private void HandleEditorValueCommit(object sender, EventArgs e)
        {
            CommitEditCore();
            var entry = (Gtk.Entry)sender;
            entry.Parent.ChildFocus(Gtk.DirectionType.TabForward);
        }

        private void CancelEditCore()
        {
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        private void CommitEditCore()
        {
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(true, EditingObject));
            }
        }
    }

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
        public Gtk.TreePath Path { get { return Item1; } }

        /// <summary>
        /// Gets the column of the edited item.
        /// </summary>
        public Gtk.TreeViewColumn Column { get { return Item2; } }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public object Data { get { return Item3; } }
    }
}
