// <copyright file="TextBlockEditorAdorner.cs" company="INTV Funhouse">
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using INTV.Shared.Behavior;

using Key = System.Windows.Input.Key;

namespace INTV.Shared.View
{
    /// <summary>
    /// Implements an adorner used to modify the contents of a TextBlock control.
    /// </summary>
    public class TextBlockEditorAdorner : Adorner, IInPlaceEditor
    {
        #region MaxLength

        /// <summary>
        /// This attached property is used to enable the in-place editing feature on a control.
        /// </summary>
        /// <remarks>The default value of this property is -1, indicating no limit is set.</remarks>
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(TextBlockEditorAdorner), new FrameworkPropertyMetadata(-1));

        /// <summary>
        /// Gets the value of the MaxLengthProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>The maximum allowed length of the string that a TextBlockEditorAdorner should allow.</returns>
        public static int GetMaxLength(DependencyObject control)
        {
            return (int)control.GetValue(MaxLengthProperty);
        }

        /// <summary>
        /// Sets the value of the MaxLengthProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="maximumLength">The maximum allowed length of the string that a TextBlockEditorAdorner should allow.</param>
        public static void SetMaxLength(DependencyObject control, int maximumLength)
        {
            control.SetValue(MaxLengthProperty, maximumLength);
        }

        #endregion // MaxLength

        #region RestrictToGromCharacters

        /// <summary>
        /// This attached property is used to restrict the in-place editing feature on a control to allow only characters supported by the Intellivision GROM character set.
        /// </summary>
        /// <remarks>The default value of this property is false, indicating no limit is set.</remarks>
        public static readonly DependencyProperty RestrictToGromCharactersProperty = DependencyProperty.RegisterAttached("RestrictToGromCharacters", typeof(bool), typeof(TextBlockEditorAdorner));

        /// <summary>
        /// Gets the value of the RestrictToGromCharactersProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a TextBlockEditorAdorner should restrict characters to those available on the Intellivision GROM font.</returns>
        public static bool GetRestrictToGromCharacters(DependencyObject control)
        {
            return (bool)control.GetValue(RestrictToGromCharactersProperty);
        }

        /// <summary>
        /// Sets the value of the RestrictToGromCharactersProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="restrictToGromCharacters">If <c>true</c>, a TextBlockEditorAdorner should restrict characters to those available on the Intellivision GROM font.</param>
        public static void SetRestrictToGromCharacters(DependencyObject control, bool restrictToGromCharacters)
        {
            control.SetValue(RestrictToGromCharactersProperty, restrictToGromCharacters);
        }

        #endregion // RestrictToGromCharacters

        #region RestrictToYearCharacters

        /// <summary>
        /// This attached property is used to restrict the in-place editing feature on a control to allow only digits and question marks.
        /// </summary>
        /// <remarks>The default value of this property is false, indicating no limit is set.</remarks>
        public static readonly DependencyProperty RestrictToYearCharactersProperty = DependencyProperty.RegisterAttached("RestrictToYearCharacters", typeof(bool), typeof(TextBlockEditorAdorner));

        /// <summary>
        /// Gets the value of the RestrictToYearCharactersProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns><c>true</c> if a TextBlockEditorAdorner should restrict characters to digits and question mark.</returns>
        public static bool GetRestrictToYearCharacters(DependencyObject control)
        {
            return (bool)control.GetValue(RestrictToYearCharactersProperty);
        }

        /// <summary>
        /// Sets the value of the RestrictToYearCharactersProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="restrictToYearCharacters">If <c>true</c>, a TextBlockEditorAdorner should restrict characters to allow only digits and question marks.</param>
        public static void SetRestrictToYearCharacters(DependencyObject control, bool restrictToYearCharacters)
        {
            control.SetValue(RestrictToGromCharactersProperty, restrictToYearCharacters);
        }

        #endregion // RestrictToYearCharacters

        #region AllowEmpty

        /// <summary>
        /// This attached property is used to allow the in-place editing feature to assign an empty (or whitespace) string.
        /// </summary>
        /// <remarks>The default value of this property is false, indicating that at least one non-whitespace character must be in the string.</remarks>
        public static readonly DependencyProperty AllowEmptyProperty = DependencyProperty.RegisterAttached("AllowEmpty", typeof(bool), typeof(TextBlockEditorAdorner));

        /// <summary>
        /// Gets the value of the AllowEmptyProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to query the value from.</param>
        /// <returns>The value of the property.</returns>
        public static bool GetAllowEmpty(DependencyObject control)
        {
            return (bool)control.GetValue(AllowEmptyProperty);
        }

        /// <summary>
        /// Sets the value of the AllowEmptyProperty attached property on the given control.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="allowEmpty">If <c>true</c>, a TextBlockEditorAdorner should allow empty or whitespace-only strings.</param>
        public static void SetAllowEmpty(DependencyObject control, bool allowEmpty)
        {
            control.SetValue(AllowEmptyProperty, allowEmpty);
        }

        #endregion // AllowEmpty

        private static readonly List<Key> KeysToIgnore = new List<Key>
        {
            Key.Up, Key.Down, Key.Left, Key.Right, Key.PageDown, Key.PageUp, Key.Home, Key.End
        };

        private bool _closed;
        private readonly Grid _layoutRoot;
        private readonly TextBox _textBox;
        private readonly VisualCollection _visualCollection;
        private AdornerLayer _layer;
        private Point _offset = new Point(double.NaN, double.NaN);
        private DependencyObject _scope;
        private IInputElement _restoreFocusElement;
        private bool _restrictToGromCharacters;
        private bool _restrictToYearCharacters;
        private bool _allowEmpty;

        #region Constructors

        private TextBlockEditorAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _restrictToGromCharacters = GetRestrictToGromCharacters(adornedElement);
            _restrictToYearCharacters = GetRestrictToYearCharacters(adornedElement);
            _allowEmpty = GetAllowEmpty(adornedElement);
            _scope = System.Windows.Input.FocusManager.GetFocusScope(adornedElement);
            _restoreFocusElement = System.Windows.Input.FocusManager.GetFocusedElement(_scope);
            _visualCollection = new VisualCollection(this);
            _layoutRoot = new Grid();
            _layoutRoot.Background = Brushes.Transparent;
            _textBox = new TextBox();
            CustomizeTextBox(_textBox);
            _layoutRoot.Children.Add(_textBox);
            _textBox.KeyDown += HandleKeyDown;
            _textBox.KeyUp += HandleKeyUp;
            _textBox.AddHandler(TextInputEvent, new System.Windows.Input.TextCompositionEventHandler(HandleTextInput), true);
            if (_restrictToGromCharacters || _restrictToYearCharacters)
            {
                _textBox.AddHandler(PreviewTextInputEvent, new System.Windows.Input.TextCompositionEventHandler(PreviewHandleTextInput), true);
            }
            _visualCollection.Add(_layoutRoot);
            Loaded += OnAdornerLoaded;
            LostFocus += OnAdornerLostFocus;
        }

        #endregion Constructors

        #region IInPlaceEditor Events

        /// <inheritdoc />
        public event EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        #endregion // IInPlaceEditor Events

        #region Properties

        /// <summary>
        /// Gets the unique identifier of the IInPlaceEditor implementation.
        /// </summary>
        public static string InPlaceEditorType
        {
            get { return typeof(TextBlockEditorAdorner).FullName + "{B256ED76-BA3B-4E15-8C95-FA98D2690092}"; }
        }

        #region IInPlaceEditor Properties

        /// <inheritdoc />
        public UIElement EditedElement
        {
            get { return AdornedElement; }
        }

        /// <inheritdoc />
        public UIElement ElementOwner { get; set; }

        private TextBlock AdornedTextBlock
        {
            get { return AdornedElement as TextBlock; }
        }

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

        #endregion // Properties

        /// <summary>
        /// Function used to register the factory for this editor.
        /// </summary>
        /// <returns><c>true</c> if the factory was registered.</returns>
        public static bool RegisterInPlaceEditor()
        {
            return InPlaceEditBehavior.RegisterInPlaceEditorFactory(InPlaceEditorType, GetTextBlockEditorAdorner);
        }

        /// <summary>
        /// The factory function.
        /// </summary>
        /// <param name="element">The element to edit.</param>
        /// <returns>The in-place editor for the given element.</returns>
        private static TextBlockEditorAdorner GetTextBlockEditorAdorner(FrameworkElement element)
        {
            var adorner = new TextBlockEditorAdorner(element);
            return adorner;
        }

        #region IInPlaceEditor

        /// <inheritdoc />
        public void BeginEdit()
        {
            _layer = AdornerLayer.GetAdornerLayer(AdornedTextBlock);
            _layer.Add(this);
            AdornedTextBlock.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <inheritdoc />
        public void CancelEdit()
        {
            if (!_closed)
            {
                Close();
            }
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        /// <inheritdoc />
        public void CommitEdit()
        {
            bool commitChanges = false;
            if (!_closed)
            {
                commitChanges = (AdornedTextBlock.Text != _textBox.Text) && (_allowEmpty || !string.IsNullOrWhiteSpace(_textBox.Text));
                if (commitChanges)
                {
                    AdornedTextBlock.Text = _textBox.Text;
                }
                Close();
            }
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(commitChanges));
            }
        }

        #endregion // IInPlaceEditor

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
                // The magic number 2 is there because TextBox has a part within it, TextBoxView, that has a margin of 2. Haven't
                // found documentation about template parts for TextBox
                Rect elementRect = new Rect(-2, 0, _layoutRoot.DesiredSize.Width, _layoutRoot.DesiredSize.Height);
                _layoutRoot.Arrange(elementRect);
            }
            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_visualCollection != null)
            {
                if (!(AdornedElement is TextBlock))
                {
                    // If we're not adorning a TextBlock, we want to set our size to our adorned element size
                    _layoutRoot.Width = AdornedElement.DesiredSize.Width;
                    _layoutRoot.Height = AdornedElement.DesiredSize.Height;
                }
                _layoutRoot.Measure(availableSize);
                return _layoutRoot.DesiredSize;
            }
            return new Size();
        }

        #region Event Handlers

        private void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                CommitEdit();
                e.Handled = true;
            }
            if (KeysToIgnore.Contains(e.Key))
            {
                e.Handled = true;
            }
            UpdateText();
        }

        private void HandleKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelEdit();
                e.Handled = true;
            }
            else
            {
                UpdateText();
            }

            if ((e.Key == Key.Enter) && !string.IsNullOrWhiteSpace(_textBox.Text))
            {
                CommitEdit();
                e.Handled = true;
            }
        }

        private static readonly char[] YearCharacters = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '?' };

        private void PreviewHandleTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool updateText = true;
            if (_restrictToGromCharacters || _restrictToYearCharacters)
            {
                foreach (var c in e.Text)
                {
                    updateText = (_restrictToGromCharacters && INTV.Core.Model.Grom.Characters.Contains(c)) || (_restrictToYearCharacters && YearCharacters.Contains(c));
                    if (!updateText)
                    {
                        break;
                    }
                }
                if (!updateText)
                {
                    e.Handled = true; // do not allow unsupported (non-GROM / non-year) character(s)
                }
            }
        }

        private void HandleTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            UpdateText();
        }

        private void OnAdornerLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_textBox != null)
                {
                    _textBox.Focus();
                    _textBox.SelectAll();
                }
            }));
        }

        private void OnAdornerLostFocus(object sender, RoutedEventArgs e)
        {
            CommitEdit();
        }

        #endregion // Event Handlers

        private void Close()
        {
            _closed = true;
            _layer.Remove(this);
            AdornedElement.Visibility = System.Windows.Visibility.Visible;
            if (_restoreFocusElement != null)
            {
                System.Windows.Input.FocusManager.SetFocusedElement(_scope, _restoreFocusElement);
            }
        }

        private void UpdateText()
        {
            if (!_closed && _textBox.Text != AdornedTextBlock.Text)
            {
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        private void CustomizeTextBox(TextBox textBox)
        {
            textBox.BorderThickness = new Thickness(0);
            textBox.Text = AdornedTextBlock.Text;
            var maxLength = GetMaxLength(AdornedTextBlock);
            if (maxLength > 0)
            {
                textBox.MaxLength = maxLength;
            }
            textBox.AcceptsReturn = false;
            textBox.AcceptsTab = false;
            textBox.IsReadOnly = false;
        }
    }
}
