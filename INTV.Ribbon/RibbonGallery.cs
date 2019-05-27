// <copyright file="RibbonGallery.cs" company="INTV Funhouse">
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

using System.Windows;

#if WIN_XP
using RibbonGalleryBase = Microsoft.Windows.Controls.Ribbon.RibbonGallery;
#else
using RibbonGalleryBase = System.Windows.Controls.Ribbon.RibbonGallery;
#endif // WIN_XP

namespace INTV.Ribbon
{
    public class RibbonGallery : RibbonGalleryBase
    {
        /// <summary>
        /// Setting this attached property to <c>true</c> enables the observers.
        /// </summary>
        public static readonly DependencyProperty CoerceToItemIndexProperty = DependencyProperty.RegisterAttached("CoerceToItemIndex", typeof(int), typeof(RibbonGallery), new PropertyMetadata(-1, CoerceToItemIndexChangedCallBack));

        /// <summary>
        /// Get the value of the CoerceToItemIndex attached property.
        /// </summary>
        /// <param name="ribbonGallery">The visual from which to read the CoerceToItemIndex property.</param>
        /// <returns>A value greater than or equal to zero if the coerce behavior has been enabled.</returns>
        public static int GetCoerceToItemIndex(RibbonGallery ribbonGallery)
        {
            return (int)ribbonGallery.GetValue(CoerceToItemIndexProperty);
        }

        /// <summary>
        /// Sets the value of the CoerceToItemIndex attached property.
        /// </summary>
        /// <param name="itemsControl">The visual upon which to set the CoerceToItemIndex property.</param>
        /// <param name="coerceToItemAtIndex">The index to which selection must be coerced.</param>
        public static void SetCoerceToItemIndex(RibbonGallery itemsControl, int coerceToItemAtIndex)
        {
            itemsControl.SetValue(CoerceToItemIndexProperty, coerceToItemAtIndex);
        }

        private static void CoerceToItemIndexChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var ribbonGallery = o as RibbonGallery;
            if (ribbonGallery != null)
            {
                if ((int)e.NewValue >= 0)
                {
                    ribbonGallery.SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    ribbonGallery.SelectionChanged -= OnSelectionChanged;
                }
            }
        }

        private static void OnSelectionChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var ribbonGallery = sender as RibbonGallery;
            if (ribbonGallery != null)
            {
                var selectedItem = ribbonGallery.SelectedItem;
                var selectedValue = ribbonGallery.SelectedValue;
                var category = ribbonGallery.Items[0] as RibbonGalleryCategory;
                var items = category.Items;
                var coerceIndex = GetCoerceToItemIndex(ribbonGallery);
                if (coerceIndex >= 0 && coerceIndex < items.Count)
                {
                    var newItem = e.NewValue as RibbonGalleryItem;
                    var coerceItem = items[coerceIndex] as RibbonGalleryItem;
                    var newItemIndex = items.IndexOf(e.NewValue);
                    System.Diagnostics.Debug.WriteLine("");
                }
            }
        }
    }
}
