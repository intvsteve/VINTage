// <copyright file="ProgramFeaturesEditorAdorner.cs" company="INTV Funhouse">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using INTV.Shared.Behavior;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    public class ProgramFeaturesEditorAdorner : Adorner, IInPlaceEditor
    {
        private readonly Grid _layoutRoot;
        private readonly VisualCollection _visualCollection;
        private AdornerLayer _layer;
        private DependencyObject _scope;
        private IInputElement _restoreFocusElement;
        private RomFeaturesConfiguration _configurationVisual;

        #region Constructors

        private ProgramFeaturesEditorAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _scope = System.Windows.Input.FocusManager.GetFocusScope(adornedElement);
            _restoreFocusElement = System.Windows.Input.FocusManager.GetFocusedElement(_scope);
            _visualCollection = new VisualCollection(this);
            _layoutRoot = new Grid();
            _layoutRoot.Background = Brushes.Transparent;
            _configurationVisual = new RomFeaturesConfiguration();
            _layoutRoot.Children.Add(_configurationVisual);
            _visualCollection.Add(_layoutRoot);
            Loaded += HandleAdornerLoaded;
        }

        #endregion Constructors

        #region IInPlaceEditor Events

        /// <inheritdoc />
        public event EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        #endregion // IInPlaceEditor Events

        #region Properties

        #region IInPlaceEditor Properties

        /// <summary>
        /// Gets the unique identifier of this IInPlaceEditor implementation.
        /// </summary>
        public static string InPlaceEditorType
        {
            get { return typeof(ProgramFeaturesEditorAdorner).FullName + "{00DD508B-4796-4EA2-949F-451B9C35A6E7}"; }
        }

        /// <inheritdoc />
        public UIElement EditedElement
        {
            get { return AdornedElement; }
        }

        /// <inheritdoc />
        public UIElement ElementOwner { get; set; }

        #endregion // IInPlaceEditor Properties

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

        private ProgramFeatureImages AdornedFeatures
        {
            get { return AdornedElement as ProgramFeatureImages; }
        }

        #endregion // Properties

        /// <summary>
        /// Function used to register the factory for this editor.
        /// </summary>
        /// <returns><c>true</c> if the factory was registered.</returns>
        public static bool RegisterInPlaceEditor()
        {
            return InPlaceEditBehavior.RegisterInPlaceEditorFactory(InPlaceEditorType, GetEditorAdorner);
        }

        #region IInPlaceEditor Methods

        /// <inheritdoc />
        public void BeginEdit()
        {
            _layer = AdornerLayer.GetAdornerLayer(AdornedElement);
            _layer.Add(this);
            var programDescription = ((ProgramDescriptionViewModel)AdornedFeatures.DataContext).Model;
            var romConfigurationViewModel = (RomFeaturesConfigurationViewModel)_configurationVisual.DataContext;
            romConfigurationViewModel.Initialize(programDescription, this);
        }

        /// <inheritdoc />
        public void CancelEdit()
        {
            Close();
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        /// <inheritdoc />
        public void CommitEdit()
        {
            var romConfigurationViewModel = (RomFeaturesConfigurationViewModel)_configurationVisual.DataContext;
            bool commitedChanges = romConfigurationViewModel.CommitChangesToProgramDescription();
            Close();
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(commitedChanges));
            }
        }

        #endregion // IInplaceEditor Methods

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
                var padding = 2.0;
                var adornedElementHeight = AdornedFeatures.ActualHeight;
                var adornedElementWidth = AdornedFeatures.ActualWidth;
                var x = 0.0;
                var y = adornedElementHeight + padding; // by default, appear below target attributes to edit
                var adornerOrigin = RenderTransform.Transform(new Point(0, 0));
                if ((adornerOrigin.X != 0) || (adornerOrigin.Y != 0))
                {
                    // Ensure we position so we're visible, if possible.
                    var adornerLeftEdge = adornerOrigin.X;
                    var adornerRightEdge = adornerLeftEdge + _layoutRoot.DesiredSize.Width;
                    var adornerTopEdge = adornerOrigin.Y + y;
                    var adornerBottomEdge = adornerTopEdge + _layoutRoot.DesiredSize.Height;

                    if (adornerRightEdge > _layer.ActualWidth)
                    {
                        // Nudge to the left enough to keep visible.
                        x = _layer.ActualWidth - adornerRightEdge - padding;
                    }
                    if (adornerBottomEdge > _layer.ActualHeight)
                    {
                        // Place above the adorned element.
                        y = -_layoutRoot.DesiredSize.Height - padding;
                    }
                }

                Rect elementRect = new Rect(x, y, _layoutRoot.DesiredSize.Width, _layoutRoot.DesiredSize.Height);
                _layoutRoot.Arrange(elementRect);
            }
            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_visualCollection != null)
            {
                ////if (!(AdornedElement is TextBlock))
                ////{
                ////    // If we're not adorning a TextBlock, we want to set our size to our adorned element size
                ////    _layoutRoot.Width = AdornedElement.DesiredSize.Width;
                ////    _layoutRoot.Height = AdornedElement.DesiredSize.Height;
                ////}
                _layoutRoot.Measure(availableSize);
                return _layoutRoot.DesiredSize;
            }
            return new Size();
        }

        /// <summary>
        /// The factory function.
        /// </summary>
        /// <param name="element">The element to edit.</param>
        /// <returns>The in-place editor for the given element.</returns>
        private static ProgramFeaturesEditorAdorner GetEditorAdorner(FrameworkElement element)
        {
            if (!(element is ProgramFeatureImages))
            {
                element = element.GetParent<ProgramFeatureImages>();
            }
            var adorner = new ProgramFeaturesEditorAdorner(element);
            return adorner;
        }

        private void HandleAdornerLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_configurationVisual != null)
                {
                    _configurationVisual.Focus();
                }
            }));
        }

        private void Close()
        {
            if (_layer != null)
            {
                _layer.Remove(this);
            }
            _layer = null;
            if (_restoreFocusElement != null)
            {
                System.Windows.Input.FocusManager.SetFocusedElement(_scope, _restoreFocusElement);
            }
        }
    }
}
