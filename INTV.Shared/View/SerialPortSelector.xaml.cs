// <copyright file="SerialPortSelector.xaml.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for SerialPortSelector.xaml
    /// </summary>
    public partial class SerialPortSelector : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the type SerialPortSelector.
        /// </summary>
        public SerialPortSelector()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // We need to do this specifically here because setting the parameter in XAML runs too early -- before the
            // property DataContext is actually pushed into the visual by its parent. It's all a bit more convoluted
            // than it should be. This control and its dialog have been pushed around between a couple platforms and
            // components and the result is a little messy.
            INTV.Shared.Behavior.DoubleClickBehavior.SetDoubleClickCommandParameter(_ports, DataContext);
        }
    }
}
