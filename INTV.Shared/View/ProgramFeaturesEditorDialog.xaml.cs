// <copyright file="ProgramFeaturesEditorDialog.xaml.cs" company="INTV Funhouse">
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
using System.Windows;
using INTV.Shared.Behavior;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for ProgramFeaturesEditorAdornerDialog.xaml
    /// </summary>
    /// <remarks>Actually, this is almost identical to the RomsFeaturesConfiguration class cluster on Mac, in that it provides access to the
    /// raw RomFeaturesConfiguration visual via a dialog. On Mac, however, rather than implementing the page wrapper as a basic NSView, and then
    /// writing a separate dialog as in Windows, a shortcut was taken. I.e. on Mac, no <see cref=">System.Windows.DOcuments.Adorner"/> analog was implemented.</remarks>
    public partial class ProgramFeaturesEditorDialog : Window, IInPlaceEditor
    {
        /// <summary>
        /// Initialize a new instance of the ProgramFeaturesEditorDialog type.
        /// </summary>
        public ProgramFeaturesEditorDialog()
        {
            InitializeComponent();
        }

        private RomFeaturesConfigurationViewModel ViewModel
        {
            get { return _romFeaturesConfiguration.DataContext as RomFeaturesConfigurationViewModel; }
        }

        #region IInPlaceEditor

        /// <inheritdoc />
        public event EventHandler<Behavior.InPlaceEditorClosedEventArgs> EditorClosed;

        /// <inheritdoc />
        public UIElement EditedElement
        {
            get { return null; }
        }

        /// <inheritdoc />
        public UIElement ElementOwner { get; set; }

        #endregion // IInPlaceEditor

        /// <summary>
        /// Create the dialog from configuring ROM features.
        /// </summary>
        /// <param name="program">The program whose features are being edited.</param>
        /// <returns>THe dialog for editing ROM features.</returns>
        public static ProgramFeaturesEditorDialog Create(ProgramDescriptionViewModel program)
        {
            var editor = new ProgramFeaturesEditorDialog();
            var viewModel = editor.ViewModel;
            viewModel.Initialize(program.Model, editor);
            editor.Owner = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow;
            return editor;
        }

        #region IInPlaceEditor

        /// <inheritdoc />
        public void BeginEdit()
        {
            this.ShowDialog();
        }

        /// <inheritdoc />
        public void CancelEdit()
        {
            this.DialogResult = false;
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        /// <inheritdoc />
        public void CommitEdit()
        {
            bool commitedChanges = ViewModel.CommitChangesToProgramDescription();
            this.DialogResult = commitedChanges;
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(commitedChanges));
            }
        }

        #endregion // IInPlaceEditor
    }
}
