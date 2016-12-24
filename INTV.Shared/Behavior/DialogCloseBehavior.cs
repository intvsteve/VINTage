// <copyright file="DialogCloseBehavior.cs" company="INTV Funhouse">
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

using System.Windows;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Makes a better way to close dialogs via MVVM.
    /// </summary>
    /// <remarks>Inspired by: http://blog.excastle.com/2010/07/25/mvvm-and-dialogresult-with-no-code-behind/
    /// </remarks>
    public static class DialogCloseBehavior
    {
        /// <summary>
        /// Dependency to bind to for setting dialog result. Use this in XAML to bind to a property usually of the same name on a dialog's ViewModel.
        /// </summary>
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(DialogCloseBehavior), new PropertyMetadata(OnDialogResultChanged));

        /// <summary>
        /// Gets the value of the DialogResultProperty attached property.
        /// </summary>
        /// <param name="window">The dialog.</param>
        /// <returns>A nullable Boolean value indicating the result of the dialog. If not set, aborted; if <c>false</c>, cancelled, if <c>true</c>, then "OK".</returns>
        public static bool? GetDialogResult(DependencyObject window)
        {
            return window.GetValue(DialogResultProperty) as bool?;
        }

        /// <summary>
        /// Sets the value of the  DialogResultProperty attached property.
        /// </summary>
        /// <param name="window">The dialog.</param>
        /// <param name="result">The result for the dialog. If <c>false</c>, the dialog is considered cancelled. If <c>true</c>, then approved.</param>
        public static void SetDialogResult(DependencyObject window, bool? result)
        {
            window.SetValue(DialogResultProperty, result);
        }

        private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            window.DialogResult = (bool?)e.NewValue;
        }
    }
}
