// <copyright file="SerialPortSelectorDialog.xaml.cs" company="INTV Funhouse">
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

using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for SerialPortSelectorDialog.xaml
    /// </summary>
    public partial class SerialPortSelectorDialog : System.Windows.Window
    {
        /// <summary>
        /// Initializes a new instance of the type SerialPortSelectorDialog.
        /// </summary>
        public SerialPortSelectorDialog()
        {
            InitializeComponent();
        }

        private SerialPortSelectorDialogViewModel ViewModel
        {
            get { return DataContext as SerialPortSelectorDialogViewModel; }
        }

        /// <summary>
        /// For compatibility with the Mac version of the dialog.
        /// </summary>
        /// <param name="dontCare">Not used.</param>
        /// <returns>Returns <c>true</c> if the "OK" button was clicked.</returns>
        public bool? ShowDialog(bool dontCare)
        {
            return ShowDialog();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _portSelector.DataContext = ((SerialPortSelectorDialogViewModel)DataContext).PortSelectorViewModel;
        }

        private static SerialPortSelectorDialog Create(SerialPortSelectorDialogViewModel viewModel)
        {
            System.Windows.Window owner = null;
            var dialog = owner.Create<SerialPortSelectorDialog>();
            dialog.DataContext = viewModel;
            return dialog;
        }
    }
}
