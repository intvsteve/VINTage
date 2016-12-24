// <copyright file="DynamicContextMenuBehavior.cs" company="INTV Funhouse">
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

using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Commands;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Attached behavior to build a context menu. Inspired by this article:
    /// https://stevemdev.wordpress.com/2010/02/07/wpf-dynamic-context-menu/
    /// </summary>
    public static class DynamicContextMenuBehavior
    {
        #region DynamicContextMenu

        /// <summary>
        /// Setting this property to <c>true</c> will create a behavior to generate a new context menu on the the given
        /// control every time the control attempts to create a context menu.
        /// </summary>
        public static readonly DependencyProperty DynamicContextMenuProperty = DependencyProperty.RegisterAttached("DynamicContextMenu", typeof(bool), typeof(DynamicContextMenuBehavior), new PropertyMetadata(DynamicContextMenuPropertyChangedCallback));

        /// <summary>
        /// Property setter for the DynamicContextMenu attached property.
        /// </summary>
        /// <param name="element">The visual that wants to build a context menu.</param>
        /// <param name="buildContextMenu">The new value.</param>
        /// <remarks>Setting this value to <c>true</c> will cause calls to the <see cref="ICommandProvider"/> infrastructure to
        /// create a context menu for the targeted visual's <see cref=">FrameworkElement.DataContext"/> as a target. The original
        /// <paramref name="element"/>'s DataContext will be used as the context argument when creating the context menu.
        /// Setting this value to <c>false</c> will stop re-creating the context menu, but will not remove it from whichever
        /// visuals the menu has been attached to.</remarks>
        public static void SetDynamicContextMenu(this FrameworkElement element, bool buildContextMenu)
        {
            element.SetValue(DynamicContextMenuProperty, buildContextMenu);
        }

        /// <summary>
        /// Property getter for the DynamicContextMenu attached property.
        /// </summary>
        /// <param name="element">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static bool GetDynamicContextMenu(this FrameworkElement element)
        {
            return (bool)element.GetValue(DynamicContextMenuProperty);
        }

        private static void DynamicContextMenuPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = o as FrameworkElement;
            element.ContextMenuOpening -= HandleContextMenuOpening;
            if ((bool)e.NewValue)
            {
                element.ContextMenuOpening += HandleContextMenuOpening;
            }
        }

        #endregion // DynamicContextMenu

        private static void HandleContextMenuOpening(object sender, ContextMenuEventArgs args)
        {
            var element = sender as FrameworkElement;
            var buildContextMenu = (element != null) && element.GetDynamicContextMenu();
            if (buildContextMenu)
            {
                var target = GetContextMenuTarget(element, args.OriginalSource as DependencyObject);

                // If menu wasn't previously defined, we need to explicitly open after assignment. Weird.
                var needToSetIsOpen = element.ContextMenu == null;
                element.ContextMenu = target.CreateContextMenu(string.Empty, element.DataContext);
                if (needToSetIsOpen)
                {
                    element.ContextMenu.IsOpen = true;
                }
            }
        }

        private static object GetContextMenuTarget(FrameworkElement sender, DependencyObject originalSource)
        {
            object target = null;
            var itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                FrameworkElement hitItem = null;
                if (sender is ListView)
                {
                    hitItem = itemsControl.GetContainerAtPoint<ListViewItem>(originalSource);
                }
                else if (sender is TreeView)
                {
                    hitItem = itemsControl.GetContainerAtPoint<TreeViewItem>(originalSource);
                }
                if (hitItem != null)
                {
                    target = hitItem.DataContext;
                }
            }
            return target;
        }

        private static ItemVisual GetContainerAtPoint<ItemVisual>(this ItemsControl control, DependencyObject hitVisual) where ItemVisual : DependencyObject
        {
            while ((System.Windows.Media.VisualTreeHelper.GetParent(hitVisual) != null) && !(hitVisual is ItemVisual))
            {
                hitVisual = System.Windows.Media.VisualTreeHelper.GetParent(hitVisual);
            }
            return hitVisual as ItemVisual;
        }
    }
}
