// <copyright file="ActualSizeObserver.cs" company="INTV Funhouse">
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
    /// Uses the attached behavior pattern to observe read-only ActualWidth and ActualHeight values on FrameworkElements.
    /// </summary>
    /// <remarks>Unfortunate to have to do this. You'd think a OneWayToSource binding would handle this kind of thing...</remarks>
    public static class ActualSizeObserver
    {
        /// <summary>
        /// Setting this attached property to true enables the observers.
        /// </summary>
        public static readonly DependencyProperty ActualSizeObserverProperty = DependencyProperty.RegisterAttached("ActualSizeObserver", typeof(bool), typeof(ActualSizeObserver), new PropertyMetadata(ActualSizeObserverChangedCallBack));

        /// <summary>
        /// Bind to this property observe both ActualWidth and ActualSize.
        /// </summary>
        public static readonly DependencyProperty ObservedSizeProperty = DependencyProperty.RegisterAttached("ObservedSize", typeof(Size), typeof(ActualSizeObserver));

        /// <summary>
        /// Bind to this property to observe ActualWidth.
        /// </summary>
        public static readonly DependencyProperty ObservedWidthProperty = DependencyProperty.RegisterAttached("ObservedWidth", typeof(double), typeof(ActualSizeObserver));

        /// <summary>
        /// Bind to this property observe ActualHeight.
        /// </summary>
        public static readonly DependencyProperty ObservedHeightProperty = DependencyProperty.RegisterAttached("ObservedHeight", typeof(double), typeof(ActualSizeObserver));

        /// <summary>
        /// Property setter for the ActualSizeObserver attached property.
        /// </summary>
        /// <param name="element">The visual upon which to set the ActualSizeObserver property.</param>
        /// <param name="observe"><c>true</c> if the observer behavior should be enabled.</param>
        public static void SetActualSizeObserver(this FrameworkElement element, bool observe)
        {
            element.SetValue(ActualSizeObserverProperty, observe);
        }

        /// <summary>
        /// Property getter for the ActualSizeObserver attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the ActualSizeObserver property.</param>
        /// <returns><c>true</c> if the observer behavior has been enabled.</returns>
        public static bool GetActualSizeObserver(this FrameworkElement element)
        {
            return (bool)element.GetValue(ActualSizeObserverProperty);
        }

        /// <summary>
        /// Property setter for the ObservedSize attached property.
        /// </summary>
        /// <param name="element">The visual upon which to set the value of the ObservedSize attached property.</param>
        /// <param name="observedSize">The new value for the ObservedSize property.</param>
        public static void SetObservedSize(this FrameworkElement element, Size observedSize)
        {
            element.SetValue(ObservedSizeProperty, observedSize);
        }

        /// <summary>
        /// Property getter for the ObservedSize attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the value of the ObservedSize attached property.</param>
        /// <returns>The most recent size of the visual as reported by its ActualWidth and ActualHeight.</returns>
        public static Size GetObservedSize(this FrameworkElement element)
        {
            return (Size)element.GetValue(ObservedSizeProperty);
        }

        /// <summary>
        /// Property setter for the ObservedWidth attached property.
        /// </summary>
        /// <param name="element">The visual upon which to set the value of the ObservedWidth attached property.</param>
        /// <param name="observedWidth">The new value for the ObservedWidth property.</param>
        public static void SetObservedWidth(this FrameworkElement element, double observedWidth)
        {
            element.SetValue(ObservedWidthProperty, observedWidth);
        }

        /// <summary>
        /// Property getter for the ObservedWidth attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the value of the ObservedWidth attached property.</param>
        /// <returns>The most recent size of the visual as reported by its ActualWidth.</returns>
        public static double GetObservedWidth(this FrameworkElement element)
        {
            return (double)element.GetValue(ObservedWidthProperty);
        }

        /// <summary>
        /// Property setter for the ObservedHeight attached property.
        /// </summary>
        /// <param name="element">The visual upon which to set the value of the ObservedHeight attached property.</param>
        /// <param name="observedHeight">The new value for the ObservedHeight property.</param>
        public static void SetObservedHeight(FrameworkElement element, double observedHeight)
        {
            element.SetValue(ObservedHeightProperty, observedHeight);
        }

        /// <summary>
        /// Property getter for the ObservedHeight attached property.
        /// </summary>
        /// <param name="element">The visual from which to read the value of the ObservedHeight attached property.</param>
        /// <returns>The most recent height of the visual as reported by its ActualHeight.</returns>
        public static double GetObservedHeight(this FrameworkElement element)
        {
            return (double)element.GetValue(ObservedHeightProperty);
        }

        private static void ActualSizeObserverChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)o;
            if ((bool)e.NewValue)
            {
                element.SizeChanged += OnSizeChanged;
                UpdateObservedSize(element, new Size(element.ActualWidth, element.ActualHeight), true, true);
            }
            else
            {
                element.SizeChanged -= OnSizeChanged;
            }
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateObservedSize(sender as FrameworkElement, e.NewSize, e.WidthChanged, e.HeightChanged);
        }

        private static void UpdateObservedSize(FrameworkElement element, Size newSize, bool widthChanged, bool heightChanged)
        {
            if (widthChanged)
            {
                element.SetCurrentValue(ObservedWidthProperty, newSize.Width);
            }
            if (heightChanged)
            {
                element.SetCurrentValue(ObservedHeightProperty, newSize.Height);
            }
            element.SetCurrentValue(ObservedSizeProperty, newSize);
        }
    }
}
