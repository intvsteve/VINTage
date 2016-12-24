// <copyright file="DropItemLocationAdorner.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using INTV.Shared.Behavior;
using INTV.Shared.Converter;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// Adorner used to indicate where an item will be dropped in a list or tree.
    /// </summary>
    internal class DropItemLocationAdorner : Adorner
    {
        private const double Inset = 8;
        private const double LineThickness = 2;
        private const double LineThicknessScale = 1.5;

        private readonly Grid _layoutRoot;
        private readonly Line _line;
        private readonly Grid _itemHighlight;
        private readonly Rectangle _folderHighlight;
        private readonly VisualCollection _visualCollection;

        private DropOnItemLocation _dropLocation;
        private FrameworkElement _dragOverElement;
        private bool _allowsChildren;
        private double _indent;

        private DropItemLocationAdorner(FrameworkElement adornedElement, FrameworkElement draggedOverItem)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            _dragOverElement = draggedOverItem;
            _layoutRoot = new Grid();
            _line = new Line();
            _itemHighlight = new Grid();
            _folderHighlight = new Rectangle();

            InitializeShapes();
            _visualCollection = new VisualCollection(this);
            _visualCollection.Add(_layoutRoot);
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount
        {
            get
            {
                if (_visualCollection != null)
                {
                    return _visualCollection.Count;
                }
                return 0;
            }
        }

        private AdornerLayer Layer { get; set; }

        private ItemsControl AdornedItemsControl
        {
            get { return AdornedElement as ItemsControl; }
        }

        private double Indent
        {
            get
            {
                return _indent;
            }

            set
            {
                if (_indent != value)
                {
                    _indent = value;
                }
            }
        }

        private FrameworkElement DragOverElement
        {
            get
            {
                return _dragOverElement;
            }
            
            set
            {
                if (_dragOverElement != value)
                {
                    _dragOverElement = value;
                    var treeViewItem = value as TreeListViewItem;
                    if (treeViewItem != null)
                    {
                        Indent = treeViewItem.Level * LevelToIndentConverter.IndentSize;
                    }
                    SomethingChanged = true;
                    Update();
                }
            }
        }

        private DropOnItemLocation DropLocation
        {
            get
            {
                return _dropLocation;
            }

            set
            {
                if (_dropLocation != value)
                {
                    _dropLocation = value;
                    SomethingChanged = true;
                    Update();
                }
            }
        }

        private bool AllowsChildren
        {
            get
            {
                return _allowsChildren;
            }

            set
            {
                if (_allowsChildren != value)
                {
                    _allowsChildren = value;
                    SomethingChanged = true;
                    Update();
                }
            }
        }

        private bool ShowFolderHighlight
        {
            get
            {
                var showFolderHighlight = AllowsChildren;
                if (showFolderHighlight)
                {
                    showFolderHighlight = (DropLocation == DropOnItemLocation.TopMiddleQuarter) || (DropLocation == DropOnItemLocation.BottomMiddleQuarter);
                }
                return showFolderHighlight;
            }
        }

        private bool SomethingChanged { get; set; }

        private bool _suspendUpdates;

        private bool SuspendUpdates
        {
            get
            {
                return _suspendUpdates;
            }

            set
            {
                _suspendUpdates = value;
                if (!_suspendUpdates && SomethingChanged)
                {
                    Update();
                }
            }
        }

        /// <summary>
        /// Update the adorner's location, etc.
        /// </summary>
        /// <param name="draggedOverElement">The item being dragged over.</param>
        /// <param name="dragDropFeedback">Feedback interface.</param>
        /// <param name="exitDragDrop">If <c>true</c>, we're doing a drag-drop exit.</param>
        public static void Update(FrameworkElement draggedOverElement, IDragDropFeedback dragDropFeedback, bool exitDragDrop)
        {
            var ending = (dragDropFeedback == null) || (dragDropFeedback.DropLocation == DropOnItemLocation.None);
            var adorner = GetAdorner(dragDropFeedback, draggedOverElement, !ending); // may modify the feedback
            ending = (dragDropFeedback == null) || (dragDropFeedback.DropLocation == DropOnItemLocation.None);
            if (ending || exitDragDrop)
            {
                if (adorner != null)
                {
                    adorner.Remove(dragDropFeedback, exitDragDrop);
                }
            }
            else
            {
                if ((adorner != null) && (draggedOverElement != adorner.AdornedElement))
                {
                    if (adorner.Layer == null)
                    {
                        adorner.Layer = AdornerLayer.GetAdornerLayer(adorner.AdornedElement);
                        adorner.Layer.Add(adorner);
                    }

                    // fix up size / position
                    adorner.SuspendUpdates = true;
                    adorner.DragOverElement = draggedOverElement;
                    adorner.DropLocation = dragDropFeedback.DropLocation;
                    adorner.AllowsChildren = dragDropFeedback.AllowsChildren;
                    adorner.SuspendUpdates = false;
                }
            }
        }

        private static DropItemLocationAdorner GetAdorner(IDragDropFeedback feedback, FrameworkElement draggedOverElement, bool createIfNeeded)
        {
            var dragDropFeedback = (DragDropFeedback)feedback;
            DropItemLocationAdorner adorner = (dragDropFeedback != null) ? dragDropFeedback.Adorner : null;
            if (adorner == null)
            {
                var adornedElement = GetRootItemsControl(draggedOverElement);
                if ((adornedElement != null) && DragDropRearrangeBehavior.GetAllowsDragDropRearrange(adornedElement))
                {
                    if ((adorner == null) && createIfNeeded)
                    {
                        adorner = new DropItemLocationAdorner(adornedElement, draggedOverElement);
                        dragDropFeedback.Adorner = adorner;
                    }
                }
            }
            else if ((draggedOverElement != adorner.AdornedItemsControl) && (draggedOverElement.GetParent<ItemsControl>(i => i == adorner.AdornedItemsControl) == null))
            {
                if (dragDropFeedback != null)
                {
                    dragDropFeedback.DropLocation = DropOnItemLocation.None;
                }
            }
            return adorner;
        }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            if (_visualCollection != null)
            {
                return _visualCollection[index];
            }
            return null;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_visualCollection != null)
            {
                var childLocation = DragOverElement.TranslatePoint(new Point(), AdornedElement);
                var y = 0.0;
                var offsetY = _layoutRoot.DesiredSize.Height / 2;
                var adjustY = 3;
                switch (DropLocation)
                {
                    case DropOnItemLocation.TopQuarter:
                        y = childLocation.Y - offsetY + adjustY;
                        break;
                    case DropOnItemLocation.TopMiddleQuarter:
                        if (ShowFolderHighlight)
                        {
                            y = childLocation.Y - LineThickness;
                        }
                        else
                        {
                            y = childLocation.Y - offsetY + adjustY;
                        }
                        break;
                    case DropOnItemLocation.BottomMiddleQuarter:
                        if (ShowFolderHighlight)
                        {
                            y = childLocation.Y - LineThickness;
                        }
                        else
                        {
                            y = childLocation.Y + DragOverBehavior.GetDragOverVisualHeight(DragOverElement) - offsetY + adjustY;
                        }
                        break;
                    case DropOnItemLocation.BottomQuarter:
                        y = childLocation.Y + DragOverBehavior.GetDragOverVisualHeight(DragOverElement) - offsetY + adjustY;
                        break;
                }
                Rect elementRect = new Rect((2 * Inset) + Indent, y, _layoutRoot.DesiredSize.Width, _layoutRoot.DesiredSize.Height);
                _layoutRoot.Arrange(elementRect);
            }
            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_visualCollection != null)
            {
                if (!(AdornedElement is ItemsControl))
                {
                    // If we're not adorning an ItemsControl, we want to set our size to our adorned element size
                    _layoutRoot.Width = GetAdornerWidth();
                    _layoutRoot.Height = AdornedElement.DesiredSize.Height;
                }
                _layoutRoot.Measure(availableSize);
                return _layoutRoot.DesiredSize;
            }
            return new Size();
        }

        private static ItemsControl GetRootItemsControl(FrameworkElement draggedOverElement)
        {
            ItemsControl rootItemsControl = draggedOverElement as ItemsControl;
            var itemsControl = rootItemsControl;
            while (itemsControl != null)
            {
                itemsControl = ItemsControl.ItemsControlFromItemContainer(itemsControl);
                if (itemsControl != null)
                {
                    rootItemsControl = itemsControl;
                }
            }
            return rootItemsControl;
        }

        private void InitializeShapes()
        {
            _layoutRoot.VerticalAlignment = VerticalAlignment.Center;
            _layoutRoot.Background = null;
            _layoutRoot.IsHitTestVisible = false;

            var treeViewItem = DragOverElement as TreeListViewItem;
            if (treeViewItem != null)
            {
                Indent = treeViewItem.Level * LevelToIndentConverter.IndentSize;
            }

            _line.HorizontalAlignment = HorizontalAlignment.Left;
            _line.X1 = 0;
            _line.Y1 = 0;
            _line.X2 = GetAdornerWidth();
            _line.Y2 = 0;
            _line.StrokeThickness = LineThickness;
            _line.Fill = SystemColors.ControlTextBrush;
            _line.Stroke = SystemColors.ControlTextBrush;

            var _leftArrow = new Polygon();
            _leftArrow.Fill = SystemColors.ControlTextBrush;
            _leftArrow.Stroke = SystemColors.ControlTextBrush;
            _leftArrow.HorizontalAlignment = HorizontalAlignment.Left;
            _leftArrow.Points = new PointCollection(new Point[] { new Point(0, -LineThicknessScale * LineThickness), new Point(0, LineThicknessScale * LineThickness), new Point(LineThicknessScale * LineThickness, 0) });

            var _rightArrow = new Polygon();
            _rightArrow.Fill = SystemColors.ControlTextBrush;
            _rightArrow.Stroke = SystemColors.ControlTextBrush;
            _rightArrow.HorizontalAlignment = HorizontalAlignment.Right;
            _rightArrow.Points = new PointCollection(new Point[] { new Point(0, -LineThicknessScale * LineThickness), new Point(0, LineThicknessScale * LineThickness), new Point(-LineThicknessScale * LineThickness, 0) });

            _itemHighlight.VerticalAlignment = VerticalAlignment.Center;
            _itemHighlight.Children.Add(_line);
            _itemHighlight.Children.Add(_leftArrow);
            _itemHighlight.Children.Add(_rightArrow);

            _folderHighlight.Width = GetAdornerWidth();
            _folderHighlight.Height = DragOverBehavior.GetDragOverVisualHeight(DragOverElement) + (LineThicknessScale * LineThickness);
            _folderHighlight.StrokeThickness = LineThickness;
            _folderHighlight.Stroke = SystemColors.ControlTextBrush;
            Update();
        }

        private void Update()
        {
            if (!SuspendUpdates && SomethingChanged)
            {
                if (ShowFolderHighlight)
                {
                    _folderHighlight.Height = DragOverBehavior.GetDragOverVisualHeight(DragOverElement) + (LineThicknessScale * LineThickness);
                    _folderHighlight.Width = GetAdornerWidth();
                    _layoutRoot.Children.Remove(_itemHighlight);
                    if (!_layoutRoot.Children.Contains(_folderHighlight))
                    {
                        _layoutRoot.Children.Add(_folderHighlight);
                    }
                }
                else
                {
                    _line.X2 = GetAdornerWidth();
                    _layoutRoot.Children.Remove(_folderHighlight);
                    if (!_layoutRoot.Children.Contains(_itemHighlight))
                    {
                        _layoutRoot.Children.Add(_itemHighlight);
                    }
                }
                InvalidateMeasure();
                InvalidateArrange();
                SomethingChanged = false;
            }
        }

        private double GetAdornerWidth()
        {
            // use ActualWidth?
            var isTreeListView = AdornedItemsControl is TreeListView;
            var scrollViewer = AdornedItemsControl.FindChild<ScrollViewer>(v => !isTreeListView || v.Name == "_tv_scrollviewer");
            if (scrollViewer == null)
            {
                scrollViewer = AdornedItemsControl.FindChild<ScrollViewer>(v => !isTreeListView || v.Name == "_tv_scrollviewer_");
            }
            var isScrollbarVisible = (scrollViewer != null) && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible;
            var scrollbarInset = isScrollbarVisible ? SystemParameters.VerticalScrollBarWidth : 0;
            var newWidth = AdornedElement.DesiredSize.Width - Indent - (3 * Inset) - scrollbarInset;
            return newWidth;
        }

        private void Remove(IDragDropFeedback feedback, bool exitDragDrop)
        {
            if (Layer != null)
            {
                Layer.Remove(this);
                Layer = null;
                if (exitDragDrop)
                {
                    ((DragDropFeedback)feedback).Adorner = null;
                }
            }
            SuspendUpdates = true;
            DragOverElement = null;
            DropLocation = DropOnItemLocation.None;
            SomethingChanged = false;
            AllowsChildren = false;
        }
    }
}
