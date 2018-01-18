// <copyright file="NumericTextEditBehavior.cs" company="INTV Funhouse">
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

#define ENABLE_MIN_MAX

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Use this attached behavior to restrict entry in a TextBox to support the entry of numbers.
    /// </summary>
    public static class NumericTextEditBehavior
    {
        #region Attached Properties

        #region IsNumericEditorProperty

        /// <summary>
        /// This attached property is used to enable the numeric text editor feature on a control.
        /// </summary>
        public static readonly DependencyProperty IsNumericEditorProperty = DependencyProperty.RegisterAttached("IsNumericEditor", typeof(bool), typeof(NumericTextEditBehavior), new PropertyMetadata(OnIsNumericEditorChanged));

        /// <summary>
        /// Gets the value of the IsNumericEditorProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a control wishes to use the numeric text editor attached behavior.</returns>
        public static bool GetIsNumericEditor(DependencyObject control)
        {
            return (bool)control.GetValue(IsNumericEditorProperty);
        }

        /// <summary>
        /// Sets the value of the IsNumericEditorProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="isNumericEditor">If <c>true</c>, indicates that the control wishes to use numeric text editor attached behavior.</param>
        public static void SetIsNumericEditor(DependencyObject control, bool isNumericEditor)
        {
            control.SetValue(IsNumericEditorProperty, isNumericEditor);
        }

        private static void OnIsNumericEditorChanged(DependencyObject control, DependencyPropertyChangedEventArgs args)
        {
            var textBox = control as TextBox;
            if (textBox == null)
            {
                throw new ArgumentException(Resources.Strings.ControlMustBeTextBoxErrorMessage);
            }

            textBox.PreviewTextInput -= PreviewTextInput;
            if ((bool)args.NewValue)
            {
                textBox.PreviewTextInput += PreviewTextInput;
            }
        }

        private static void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var control = sender as TextBox;
            var validNumeric = control != null;
            if (validNumeric)
            {
                var regex = new System.Text.RegularExpressions.Regex("^-*[0-9]+$"); // don't care about anything else right now (floats, comma / decimal grouping characters)
                validNumeric = regex.IsMatch(e.Text);
                if (validNumeric)
                {
                    int currentValue = 0;
                    var newText = control.Text + e.Text;
                    if (!string.IsNullOrEmpty(control.SelectedText))
                    {
                        newText = control.Text.Replace(control.SelectedText, e.Text);
                    }
                    validNumeric = int.TryParse(newText, out currentValue);
                    if (validNumeric)
                    {
                        var max = GetMaxIntValue(control);
                        var min = GetMinIntValue(control);
                        validNumeric = (currentValue >= min) && (currentValue <= max);
                    }
                }
                else
                {
                    if (e.Text == "\r")
                    {
                        var binding = System.Windows.Data.BindingOperations.GetBindingExpression(control, TextBox.TextProperty);
                        if (binding != null)
                        {
                            binding.UpdateSource();
                        }
                    }
                }
            }
            e.Handled = !validNumeric; // We mark as handled if not numeric to prevent non-numeric strings from being entered.
        }

        #endregion // IsNumericEditorProperty

#if ENABLE_MIN_MAX

        #region MaxValue Property

        /// <summary>
        /// This attached property is used to set the maximum allowed integer value of a control.
        /// </summary>
        public static readonly DependencyProperty MaxIntValueProperty = DependencyProperty.RegisterAttached("MaxIntValue", typeof(int), typeof(NumericTextEditBehavior), new PropertyMetadata(int.MaxValue));

        /// <summary>
        /// Gets the value of the MaxIntValueProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>Maximum allowed integer value.</returns>
        public static int GetMaxIntValue(DependencyObject control)
        {
            return (int)control.GetValue(MaxIntValueProperty);
        }

        /// <summary>
        /// Sets the value of the MaxIntValueProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="maxValue">Maximum value.</param>
        public static void SetMaxIntValue(DependencyObject control, int maxValue)
        {
            control.SetValue(MaxIntValueProperty, maxValue);
        }

        #endregion // MaxValue Property

        #region MinValue Property

        /// <summary>
        /// This attached property is used to set the minimum allowed integer value of a control.
        /// </summary>
        public static readonly DependencyProperty MinIntValueProperty = DependencyProperty.RegisterAttached("MinIntValue", typeof(int), typeof(NumericTextEditBehavior), new PropertyMetadata(int.MinValue));

        /// <summary>
        /// Gets the value of the MinIntValueProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>Minimum allowed integer value.</returns>
        public static int GetMinIntValue(DependencyObject control)
        {
            return (int)control.GetValue(MinIntValueProperty);
        }

        /// <summary>
        /// Sets the value of the MinIntValueProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="maxValue">Minimum value.</param>
        public static void SetMinIntValue(DependencyObject control, int maxValue)
        {
            control.SetValue(MinIntValueProperty, maxValue);
        }

        #endregion // MinValue Property

#endif // ENABLE_MIN_MAX

        #endregion // Attached Properties
    }
}
