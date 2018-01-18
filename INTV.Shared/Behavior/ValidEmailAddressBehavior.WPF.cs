// <copyright file="ValidEmailAddressBehavior.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
using System.Windows.Controls;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// WPF-specific implementation
    /// </summary>
    public static partial class ValidEmailAddressBehavior
    {
        #region IsEmailAddressEditorProperty

        /// <summary>
        /// This attached property is used to enable the numeric text editor feature on a control.
        /// </summary>
        public static readonly DependencyProperty IsEmailAddressEditorProperty = DependencyProperty.RegisterAttached("IsEmailAddressEditor", typeof(bool), typeof(ValidEmailAddressBehavior), new PropertyMetadata(OnIsEmailEditorChanged));

        /// <summary>
        /// Gets the value of the IsEmailAddressEditorProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a control wishes to use the email address editor attached behavior.</returns>
        public static bool GetIsEmailAddressEditor(DependencyObject control)
        {
            return (bool)control.GetValue(IsEmailAddressEditorProperty);
        }

        /// <summary>
        /// Sets the value of the IsEmailAddressEditorProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="isEmailAddressEditor">If <c>true</c>, indicates that the control wishes to use email address text editor attached behavior.</param>
        public static void SetIsEmailAddressEditor(DependencyObject control, bool isEmailAddressEditor)
        {
            control.SetValue(IsEmailAddressEditorProperty, isEmailAddressEditor);
        }

        private static void OnIsEmailEditorChanged(DependencyObject control, DependencyPropertyChangedEventArgs args)
        {
            var textBox = control as TextBox;
            if (textBox == null)
            {
                throw new ArgumentException(Resources.Strings.ControlMustBeTextBoxErrorMessage);
            }

            textBox.TextChanged -= TextChanged;
            if ((bool)args.NewValue)
            {
                textBox.TextChanged += TextChanged;
            }
        }

        private static void TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            var validEmailAddress = control != null;
            if (validEmailAddress)
            {
                validEmailAddress = IsValidEmailAddress(control.Text);
                SetIsValidEmailAddress(control, validEmailAddress);
            }
        }

        #endregion // IsEmailAddressEditorProperty

        #region IsValidEmailAddressProperty
        /// <summary>
        /// This attached property is used to indicate whether a control contains a valid email address.
        /// </summary>
        public static readonly DependencyProperty IsValidEmailAddressProperty = DependencyProperty.RegisterAttached("IsValidEmailAddress", typeof(bool), typeof(ValidEmailAddressBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Gets the value of the IsValidEmailAddressProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a control contains a valid email address string.</returns>
        public static bool GetIsValidEmailAddress(DependencyObject control)
        {
            return (bool)control.GetValue(IsValidEmailAddressProperty);
        }

        /// <summary>
        /// Sets the value of the IsValidEmailAddressProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="isValidEmailAddressString">If <c>true</c>, indicates that the control contains a valid email address string.</param>
        public static void SetIsValidEmailAddress(DependencyObject control, bool isValidEmailAddressString)
        {
            control.SetValue(IsValidEmailAddressProperty, isValidEmailAddressString);
        }

        #endregion // IsValidEmailAddressProperty
    }
}
