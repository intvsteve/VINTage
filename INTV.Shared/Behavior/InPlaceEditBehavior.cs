// <copyright file="InPlaceEditBehavior.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using INTV.Shared.Utility;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements an attached behavior to achieve an 'in-place' editing operation in a TreeView or ListView control via an IInPlaceEditor.
    /// </summary>
    /// <remarks>Started as an adaptation from a useful article on Codeplex.
    /// NOTE NOTE NOTE! Consider also adding an InPlaceEditCommand attached property for a 'CanExecute' -- or have some other CanExecute update the IsEditable property.</remarks>
    /// <see cref="http://treeviewinplaceedit.codeplex.com/SourceControl/latest#TreeViewMVVMInPlaceEditing/TreeViewMVVMInPlaceEditingDemo/Behaviors/TreeViewInPlaceEditBehavior.cs"/>
    /// <example>In order to use the InPlaceEditBehavior attached behavior, several actions must be taken, either in XAML or code-behind:
    /// The top-level control must set the IsEditableProperty attached property to <c>true</c>
    /// There must exist a suitable implementation of the IInPlaceEditor interface to act as the editor.
    /// The visual that wishes to allow editing must set the value of the InPlaceEditorType attached property, which is
    /// used to create an instance of the editor visual and perform the editing operations.
    /// </example>
    public static class InPlaceEditBehavior
    {
        private static readonly Dictionary<string, Func<FrameworkElement, IInPlaceEditor>> InplaceEditorFactory = new Dictionary<string, Func<FrameworkElement, IInPlaceEditor>>();

        #region Attached Properties

        #region IsEditable

        /// <summary>
        /// This attached property is used to enable the in-place editing feature on a control.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.RegisterAttached("IsEditable", typeof(bool), typeof(InPlaceEditBehavior), new PropertyMetadata(OnIsEditableChanged));

        /// <summary>
        /// Gets the value of the IsEditableProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a control wishes to use the in-place edit attached behavior.</returns>
        public static bool GetIsEditable(DependencyObject control)
        {
            return (bool)control.GetValue(IsEditableProperty);
        }

        /// <summary>
        /// Sets the value of the IsEditableProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="isEditable">If <c>true</c>, indicates that the control wishes to use the in-place edit attached behavior.</param>
        public static void SetIsEditable(DependencyObject control, bool isEditable)
        {
            control.SetValue(IsEditableProperty, isEditable);
        }

        #endregion // IsEditable

        #region CommitWhenDismissed

        /// <summary>
        /// This attached property is used to define whether to commit (true) or cancel (false) when the editor is implicitly closed (e.g. clicking away from the adorner).
        /// </summary>
        public static readonly DependencyProperty CommitWhenDismissedProperty = DependencyProperty.RegisterAttached("CommitWhenDismissed", typeof(bool), typeof(InPlaceEditBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Gets the value of the CommitWhenDismissedProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a control wishes to commit changes when dismissed in a non-specific way (e.g. clicking elsewhere).</returns>
        public static bool GetCommitWhenDismissed(DependencyObject control)
        {
            return (bool)control.GetValue(CommitWhenDismissedProperty);
        }

        /// <summary>
        /// Sets the value of the CommitWhenDismissedProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="commitWhenDismissed">If <c>true</c>, indicates that the control wishes to commit changes when dismissed by loss of focus, clicking outside editor, etc.</param>
        public static void SetCommitWhenDismissed(DependencyObject control, bool commitWhenDismissed)
        {
            control.SetValue(CommitWhenDismissedProperty, commitWhenDismissed);
        }

        #endregion // CommitWhenDismissed

        #region InPlaceEditorType

        /// <summary>
        /// This attached property is used in conjunction with the control that performs the actual edit to communicate whether an editing operation should be committed.
        /// </summary>
        public static readonly DependencyProperty InPlaceEditorTypeProperty = DependencyProperty.RegisterAttached("InPlaceEditorType", typeof(string), typeof(InPlaceEditBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the InPlaceEditorTypeProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>The unique type of the in place editor to use.</returns>
        public static string GetInPlaceEditorType(DependencyObject control)
        {
            return control.GetValue(InPlaceEditorTypeProperty) as string;
        }

        /// <summary>
        /// Sets the value of the InPlaceEditorTypeProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="inPlaceEditorType">The unique type of the in place editor to use.</param>
        public static void SetInPlaceEditorType(DependencyObject control, string inPlaceEditorType)
        {
            control.SetValue(InPlaceEditorTypeProperty, inPlaceEditorType);
        }

        #endregion // InPlaceEditorType

        #region IsEditing

        /// <summary>
        /// This attached property is used in conjunction with the control to track if an in-place editing operation is underway.
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.RegisterAttached("IsEditing", typeof(bool), typeof(InPlaceEditBehavior), new PropertyMetadata(OnIsEditingChanged));

        /// <summary>
        /// Gets the value of the IsEditingProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if editing is underway, otherwise <c>false</c>.</returns>
        public static bool GetIsEditing(DependencyObject control)
        {
            return (bool)control.GetValue(IsEditingProperty);
        }

        /// <summary>
        /// Sets the value of the IsEditingProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="isEditing">If <c>true</c>, initiates the in-place edit operation, creating the appropriate edit control. If <c>false</c>, the edit operation is ended.</param>
        public static void SetIsEditing(DependencyObject control, bool isEditing)
        {
            control.SetValue(IsEditingProperty, isEditing);
        }

        #endregion // IsEditing

        #region LastSelectedItem

        private static readonly DependencyProperty LastSelectedItemProperty = DependencyProperty.RegisterAttached("LastSelectedItem", typeof(object), typeof(InPlaceEditBehavior));

        private static object GetLastSelectedItem(DependencyObject control)
        {
            return control.GetValue(LastSelectedItemProperty);
        }

        private static void SetLastSelectedItem(DependencyObject control, object item)
        {
            control.SetValue(LastSelectedItemProperty, item);
        }

        #endregion // LastSelectedItem

        #region LastSelectedTime

        private static readonly DependencyProperty LastSelectedTimestampProperty = DependencyProperty.RegisterAttached("LastSelectedTimestamp", typeof(int), typeof(InPlaceEditBehavior));

        private static int GetLastSelectedTimestamp(DependencyObject control)
        {
            return (int)control.GetValue(LastSelectedTimestampProperty);
        }

        private static void SetLastSelectedTimestamp(DependencyObject control, int time)
        {
            control.SetValue(LastSelectedTimestampProperty, time);
        }

        #endregion // LastSelectedTime

        #region LastClickedElement

        /// <summary>
        /// This attached property is used in conjunction with a control supporting in-place editing to determine a specific entity within the control to act as the target
        /// of an in-place edit operation. It should be used with great caution. This value is maintained automatically by this behavior via mouse event handlers. When
        /// manipulated directly, it is a wait steer in-place edit operations invoked programmatically to a specific visual element.
        /// </summary>
        public static readonly DependencyProperty LastClickedElementProperty = DependencyProperty.RegisterAttached("LastClickedElement", typeof(object), typeof(InPlaceEditBehavior), new PropertyMetadata(OnClickedElementChanged));

        /// <summary>
        /// Gets the value of the LastClickedElementProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>The last clicked element on the control</returns>
        public static object GetLastClickedElement(DependencyObject control)
        {
            return control.GetValue(LastClickedElementProperty);
        }

        /// <summary>
        /// Sets the value of the LastClickedElementProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="element">The control that was last clicked.</param>
        public static void SetLastClickedElement(DependencyObject control, object element)
        {
            control.SetValue(LastClickedElementProperty, element);
        }

        #endregion // LastClickedElement

        #region ClickedElementOwnerRoot

        private static readonly DependencyProperty ClickedElementOwnerRootProperty = DependencyProperty.RegisterAttached("ClickedElementOwnerRoot", typeof(DependencyObject), typeof(InPlaceEditBehavior));

        private static DependencyObject GetClickedElementOwnerRoot(DependencyObject control)
        {
            return control.GetValue(ClickedElementOwnerRootProperty) as DependencyObject;
        }

        private static void SetClickedElementOwnerRoot(DependencyObject control, DependencyObject ownerRoot)
        {
            control.SetValue(ClickedElementOwnerRootProperty, ownerRoot);
        }

        #endregion // ClickedElementOwnerRoot

        #region InPlaceEditor

        private static readonly DependencyProperty InPlaceEditorProperty = DependencyProperty.RegisterAttached("InPlaceEditor", typeof(IInPlaceEditor), typeof(InPlaceEditBehavior));

        private static IInPlaceEditor GetInPlaceEditor(DependencyObject control)
        {
            return control.GetValue(InPlaceEditorProperty) as IInPlaceEditor;
        }

        private static void SetInPlaceEditor(DependencyObject control, IInPlaceEditor inPlaceEditor)
        {
            control.SetValue(InPlaceEditorProperty, inPlaceEditor);
        }

        #endregion // InPlaceEditor

        #region ItemContainerGeneratorStatusChangedHandler

        private static readonly DependencyProperty ItemContainerGeneratorStatusChangedHandlerProperty = DependencyProperty.RegisterAttached("ItemContainerGeneratorStatusChangedHandler", typeof(ItemContainerGeneratorStatusChangedHandler), typeof(InPlaceEditBehavior));

        private static ItemContainerGeneratorStatusChangedHandler GetItemContainerGeneratorStatusChangedHandler(DependencyObject control)
        {
            return control.GetValue(ItemContainerGeneratorStatusChangedHandlerProperty) as ItemContainerGeneratorStatusChangedHandler;
        }

        private static void SetItemContainerGeneratorStatusChangedHandler(DependencyObject control, ItemContainerGeneratorStatusChangedHandler statusChangedHandler)
        {
            control.SetValue(ItemContainerGeneratorStatusChangedHandlerProperty, statusChangedHandler);
        }

        #endregion // GetInPlaceEditVisual

        #endregion // Attached Properties

        /// <summary>
        /// Register the factory function for an in-place editor.
        /// </summary>
        /// <param name="inPlaceEditorType">Type editor identifier.</param>
        /// <param name="factory">The factory function.</param>
        /// <returns><c>true</c> if the in-place editor factory was registered.</returns>
        /// <remarks>Make this discoverable via MEF?</remarks>
        public static bool RegisterInPlaceEditorFactory(string inPlaceEditorType, Func<FrameworkElement, IInPlaceEditor> factory)
        {
            bool registered = !InplaceEditorFactory.ContainsKey(inPlaceEditorType);
            InplaceEditorFactory[inPlaceEditorType] = factory;
            return registered;
        }

        /// <summary>
        /// Programmatically initiate an in-place edit.
        /// </summary>
        /// <param name="inPlaceEditableControl">The root control that supports in-place editing.</param>
        /// <param name="containerOfEditableElement">The container of the specific part of the item to be edited (ItemsControl).</param>
        /// <param name="itemToEdit">The ViewModel backing containerOfEditableElement.</param>
        /// <param name="getEditableElement">Function to locate the specific visual for the item being edited.</param>
        public static void BeginInPlaceEdit(this DependencyObject inPlaceEditableControl, DependencyObject containerOfEditableElement, object itemToEdit, Func<DependencyObject, object> getEditableElement)
        {
            if (GetIsEditable(inPlaceEditableControl))
            {
                var itemsContainer = containerOfEditableElement as ItemsControl;
                if (itemsContainer != null)
                {
                    var containerVisual = itemsContainer.ItemContainerGenerator.ContainerFromItem(itemToEdit);
                    if ((containerVisual == null) || !StartInPlaceEdit(inPlaceEditableControl, containerVisual, itemToEdit, getEditableElement))
                    {
                        var statusChangeHandler = new ItemContainerGeneratorStatusChangedHandler(inPlaceEditableControl, itemToEdit, getEditableElement);
                        SetItemContainerGeneratorStatusChangedHandler(inPlaceEditableControl, statusChangeHandler);
                        itemsContainer.ItemContainerGenerator.StatusChanged += statusChangeHandler.OnItemContainerGeneratorStatusChanged;
                    }
                }
            }
        }

        private static bool StartInPlaceEdit(this DependencyObject inPlaceEditableControl, DependencyObject containerOfEditableElement, object itemToEdit, Func<DependencyObject, object> getEditableElement)
        {
            var editableElement = getEditableElement(containerOfEditableElement);
            var startEdit = editableElement != null;
            if (startEdit)
            {
                ((UIElement)containerOfEditableElement).Focus();
                SetLastClickedElement(inPlaceEditableControl, editableElement);
                SetIsEditing(inPlaceEditableControl, true);
            }
            return startEdit;
        }

        private class ItemContainerGeneratorStatusChangedHandler
        {
            public ItemContainerGeneratorStatusChangedHandler(DependencyObject editableItemRoot, object itemToFind, Func<DependencyObject, object> getEditableElement)
            {
                EditableItemRoot = editableItemRoot;
                EditableItem = itemToFind;
                GetEditableElement = getEditableElement;
            }

            private DependencyObject EditableItemRoot { get; set; }

            private object EditableItem { get; set; }

            private Func<DependencyObject, object> GetEditableElement { get; set; }

            public void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e)
            {
                var itemsContainerGenerator = (ItemContainerGenerator)sender;
                var containerVisual = itemsContainerGenerator.ContainerFromItem(EditableItem);
                if ((containerVisual != null) && StartInPlaceEdit(EditableItemRoot, containerVisual, EditableItem, GetEditableElement))
                {
                    itemsContainerGenerator.StatusChanged -= OnItemContainerGeneratorStatusChanged;
                    SetItemContainerGeneratorStatusChangedHandler(EditableItemRoot, null);
                }
            }
        }

        #region Event Handlers

        private static void OnIsEditableChanged(DependencyObject control, DependencyPropertyChangedEventArgs args)
        {
            var itemsControl = control as ItemsControl;
            if (itemsControl == null)
            {
                throw new ArgumentException("control is not an ItemsControl");
            }

            itemsControl.PreviewKeyDown -= PreviewKeyDown;
            itemsControl.PreviewMouseLeftButtonUp -= PreviewMouseLeftButtonUp;
            var treeView = itemsControl as TreeView;
            var listView = itemsControl as ListView;
            if (treeView != null)
            {
                treeView.SelectedItemChanged -= TreeViewSelectedItemChanged;
                treeView.ContextMenuOpening -= HandleContextMenuOpening;
            }
            else if (listView != null)
            {
                listView.SelectionChanged -= ListViewSelectionChanged;
                listView.ContextMenuOpening -= HandleContextMenuOpening;
            }
            if ((bool)args.NewValue)
            {
                itemsControl.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
                itemsControl.PreviewKeyDown += PreviewKeyDown;
                if (treeView != null)
                {
                    treeView.SelectedItemChanged += TreeViewSelectedItemChanged;
                    treeView.ContextMenuOpening += HandleContextMenuOpening;
                }
                else if (listView != null)
                {
                    listView.SelectionChanged += ListViewSelectionChanged;
                    listView.ContextMenuOpening += HandleContextMenuOpening;
                }
            }
        }

        private static void HandleContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // We do this so edit operations that are programmatically invoked via context menu handlers by setting
            // IsEditingProperty indirectly (i.e. if they don't operate directly on click item) can work.
            var control = sender as ItemsControl;
            var clickedElement = e.OriginalSource as FrameworkElement;
            if ((control != null) && (clickedElement != null))
            {
                SetLastClickedElement(control, clickedElement);
            }
        }

        private static void OnIsEditingChanged(DependencyObject control, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                var lastClickedElement = GetLastClickedElement(control) as FrameworkElement;
                if (lastClickedElement != null)
                {
                    var inPlaceEditor = GetInPlaceEditor(lastClickedElement);
                    if (inPlaceEditor == null)
                    {
                        var inPlaceEditorType = GetInPlaceEditorType(lastClickedElement);
                        if (!string.IsNullOrEmpty(inPlaceEditorType))
                        {
                            Func<FrameworkElement, IInPlaceEditor> factory;
                            if (InplaceEditorFactory.TryGetValue(inPlaceEditorType, out factory))
                            {
                                inPlaceEditor = factory(lastClickedElement);
                            }
                        }
                    }
                    SetInPlaceEditor(lastClickedElement, inPlaceEditor);
                    if (inPlaceEditor != null)
                    {
                        inPlaceEditor.EditorClosed += OnInPlaceEditorClosed;
                        inPlaceEditor.ElementOwner = control as UIElement;
                        inPlaceEditor.BeginEdit();
                    }
                    else
                    {
                        // We don't directly reset the IsEditing property here because we're already inside the 'OnChanged' callback,
                        // and there's protection from infinite recursion. BeginInvoke to reset to false, just in case there are bindings, etc.
                        control.Dispatcher.BeginInvoke(new Action(() => SetIsEditing(control, false)));
                    }
                }
            }
        }

        private static void OnClickedElementChanged(DependencyObject control, DependencyPropertyChangedEventArgs args)
        {
            var previousClickedElement = args.OldValue as FrameworkElement;
            if (previousClickedElement != null)
            {
                previousClickedElement.Unloaded -= ClickedElementUnloaded;
                SetClickedElementOwnerRoot(previousClickedElement, null);
            }
            var newClickedElement = args.NewValue as FrameworkElement;
            if (newClickedElement != null)
            {
                SetClickedElementOwnerRoot(newClickedElement, control);
                newClickedElement.Unloaded += ClickedElementUnloaded;
            }
        }

        private static void ClickedElementUnloaded(object sender, RoutedEventArgs e)
        {
            var clickedElement = (FrameworkElement)sender;
            var ownerRoot = GetClickedElementOwnerRoot(clickedElement);
            if (ownerRoot != null)
            {
                SetLastClickedElement(ownerRoot, null);
            }
        }

        private static void OnInPlaceEditorClosed(object sender, InPlaceEditorClosedEventArgs e)
        {
            var inPlaceEditor = sender as IInPlaceEditor;
            inPlaceEditor.EditorClosed -= OnInPlaceEditorClosed;
            var editedElement = inPlaceEditor.EditedElement;
            SetInPlaceEditor(editedElement, null);
            var control = inPlaceEditor.ElementOwner;
            SetIsEditing(control, false);
        }

        private static void PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var control = sender as ItemsControl;
            switch (e.Key)
            {
                case System.Windows.Input.Key.F2:
                    if (!GetIsEditing(control))
                    {
                        var lastClickedElement = GetLastClickedElement(control);
                        if (lastClickedElement != null)
                        {
                            SetIsEditing(control, true);
                        }
                    }
                    break;
            }
        }

        private static void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectionChanged(sender, SelectedItemObserver.GetSelectedItem(sender));
        }

        private static void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged(sender, SelectedItemObserver.GetSelectedItem(sender));
        }

        private static void SelectionChanged(object sender, object newSelectedItem)
        {
            var itemsControl = sender as ItemsControl;
            var lastSelectedItem = GetLastSelectedItem(itemsControl);
            if (lastSelectedItem != newSelectedItem)
            {
                // Selection changed; update the selected item and the selected time.
                SetLastSelectedItem(itemsControl, newSelectedItem);
                if (GetIsEditing(itemsControl))
                {
                    var inPlaceEditor = GetInPlaceEditor(itemsControl);
                    if (inPlaceEditor != null)
                    {
                        if (GetCommitWhenDismissed(inPlaceEditor.EditedElement))
                        {
                            inPlaceEditor.CommitEdit();
                        }
                        else
                        {
                            inPlaceEditor.CancelEdit();
                        }
                    }
                    else
                    {
                        SetIsEditing(itemsControl, false);
                    }
                }

                // Update last clicked control.
                var lastClickedControl = GetLastClickedElement(itemsControl) as FrameworkElement;
                if (lastClickedControl != null)
                {
                    SetLastClickedElement(itemsControl, null); // clear so F2 doesn't edit something completely different
                }
            }
        }

        private static void PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                // If we get 2+ clicks, just ignore everything. In practice, this hasn't been hit.
                return;
            }
            var control = sender as ItemsControl;
            var captured = System.Windows.Input.Mouse.Captured as UIElement;
            bool isEditing = GetIsEditing(control);
            var inPlaceEditor = isEditing ? GetInPlaceEditor(control) : null;
            var hitElement = control.InputHitTest(e.GetPosition(control)) as UIElement;
            var adorner = (hitElement != null) ? hitElement.GetParent<System.Windows.Documents.Adorner>() : null;
            if (((captured != null) && (control != null) && !ReferenceEquals(captured, control)) || ((adorner != null) && (adorner == inPlaceEditor)))
            {
                return;
            }

            var clickedElement = e.OriginalSource as FrameworkElement;
            var listView = clickedElement as ListView;
            if (listView != null)
            {
                clickedElement = listView.InputHitTest(e.GetPosition(listView)) as FrameworkElement;
            }
            bool clickedElementIsInPlaceEditor = (inPlaceEditor != null) && (clickedElement.GetParent<FrameworkElement>((v) => v is IInPlaceEditor) == inPlaceEditor);

            bool commit = false;
            bool checkClickTime = true;

            Control clickedItem = clickedElement.GetParent<TreeViewItem>();
            if (clickedItem == null)
            {
                clickedItem = clickedElement.GetParent<ListViewItem>();
            }
            if (clickedItem == null && !clickedElementIsInPlaceEditor)
            {
                // Did not click on an item. Commit any in-progress edit, otherwise cancel.
                commit = isEditing && (inPlaceEditor != null);
                checkClickTime = false;
            }

            var lastSelectedItem = GetLastSelectedItem(control);
            if (checkClickTime && ((lastSelectedItem == null) || (lastSelectedItem != SelectedItemObserver.GetSelectedItem(control))))
            {
                // Clicked on a different item. Commit the current edit if applicable.
                commit = isEditing && (inPlaceEditor != null);
                checkClickTime = false;
                SetLastSelectedItem(control, null);
            }

            var lastClickedElement = GetLastClickedElement(control);
            if (checkClickTime && !clickedElementIsInPlaceEditor && ((lastClickedElement == null) || (lastClickedElement != clickedElement)))
            {
                SetLastClickedElement(control, clickedElement);
                SetLastSelectedTimestamp(control, e.Timestamp);
                checkClickTime = false;
            }

            if (checkClickTime)
            {
                var lastSelectedTime = GetLastSelectedTimestamp(control);
                var interval = Math.Abs(e.Timestamp - lastSelectedTime);
                const int MinInterval = 400;
                const int MaxInterval = 1200;
                if (interval >= MinInterval && interval <= MaxInterval)
                {
                    SetIsEditing(control, true);
                }
                else if (interval <= System.Windows.Forms.SystemInformation.DoubleClickTime)
                {
                    SetLastSelectedTimestamp(control, 0); // ignore double+ clicks -- force restart of the check
                }
                else if (interval > MaxInterval)
                {
                    SetLastSelectedTimestamp(control, e.Timestamp);
                }
            }
            else if (commit)
            {
                inPlaceEditor.CommitEdit();
            }
        }

        private static IInPlaceEditor GetInPlaceEditor(ItemsControl control)
        {
            IInPlaceEditor inPlaceEditor = null;
            var lastClickedElement = GetLastClickedElement(control) as DependencyObject;
            if (lastClickedElement != null)
            {
                inPlaceEditor = GetInPlaceEditor(lastClickedElement);
            }
            return inPlaceEditor;
        }

        #endregion // Event Handlers
    }
}
