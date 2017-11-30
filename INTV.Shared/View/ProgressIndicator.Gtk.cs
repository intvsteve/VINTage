// <copyright file="ProgressIndicator.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

// Haven't figured out a nice way to "gray out" the main window while progress bar shows...
// but the hacky way it's done has issues, too...
#define ENABLE_OVERLAY

using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [Gtk.Binding(Gdk.Key.Escape, "HandleEscapeKey")]
    public partial class ProgressIndicator : Gtk.Window, IFakeDependencyObject
    {
        private const int TimerTickMilliseconds = 100;
        private const int WidthPadding = 88;
        private const int MinimumWindowWidth = 300;

        private static readonly string ProgressIndicatorVisualPropertyName = "ProgressIndicatorVisual";

        private OSDispatcherTimer _pulseTimer;
#if ENABLE_OVERLAY
        private ProgressIndicatorOverlay _overlay;
#endif // ENABLE_OVERLAY

        private ProgressIndicator(ProgressIndicatorViewModel viewModel, Gtk.Window owner)
            : base(Gtk.WindowType.Toplevel)
        {
#if ENABLE_OVERLAY
            _overlay = new ProgressIndicatorOverlay(owner);
#endif // ENABLE_OVERLAY
            _pulseTimer = new OSDispatcherTimer();
            _pulseTimer.Interval = System.TimeSpan.FromMilliseconds(TimerTickMilliseconds);
            _pulseTimer.Tick += Pulse;
            DataContext = viewModel;
            viewModel.PropertyChanged += HandlePropertyChanged;
            this.Build();
            this.DefaultHeight = -1;
            _cancel.Label = ProgressIndicatorViewModel.Cancel;
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <summary>
        /// Initialize the progress indicator visual using the specified viewModel.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        public static void Initialize(ProgressIndicatorViewModel viewModel)
        {
            var owner = SingleInstanceApplication.Current.MainWindow;
            var progressIndicator = new ProgressIndicator(viewModel, owner);
            owner.SetValue(ProgressIndicatorVisualPropertyName, progressIndicator);
        }

        /// <summary>
        /// Executed when the user clicks the 'Cancel' button on the progress indicator.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">Event argument - which is nothing.</param>
        protected void HandleCancelClicked(object sender, System.EventArgs e)
        {
            var viewModel = ((ProgressIndicatorViewModel)DataContext);
            if (viewModel.CancelCommand.CanExecute(viewModel))
            {
                viewModel.CancelCommand.Execute(viewModel);
            }
        }

        private void HandleEscapeKey()
        {
            HandleCancelClicked(_cancel, System.EventArgs.Empty);
        }

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var viewModel = ((ProgressIndicatorViewModel)DataContext);
            switch (e.PropertyName)
            {
                case ProgressIndicatorViewModel.AllowsCancelPropertyName:
                    _cancel.Sensitive = viewModel.AllowsCancel;
                    break;
                case ProgressIndicatorViewModel.TitlePropertyName:
                    _progressTitle.Text = viewModel.Title;
                    break;
                case ProgressIndicatorViewModel.UpdateTextPropertyName:
                    _updateText.Text = viewModel.UpdateText;
                    break;
                case ProgressIndicatorViewModel.IsIndeterminatePropertyName:
                    if (viewModel.IsVisible && viewModel.IsIndeterminate)
                    {
                        _pulseTimer.Start();
                    }
                    else
                    {
                        _pulseTimer.Stop();
                    }
                    break;
                case ProgressIndicatorViewModel.PercentFinishedPropertyName:
                    if (!viewModel.IsIndeterminate)
                    {
                        _progressBar.Fraction = viewModel.PercentFinished;
                    }
                    break;
                case ProgressIndicatorViewModel.IsVisiblePropertyName:
                    var owner = SingleInstanceApplication.Current.MainWindow;
                    if (viewModel.IsVisible)
                    {
                        this.Modal = true;
                        int w, h, ownerWidth, ownerHeight;
                        owner.GetSize(out ownerWidth, out ownerHeight);
#if ENABLE_OVERLAY
                        _overlay.Resize(ownerWidth, ownerHeight);
#endif // ENABLE_OVERLAY
                        this.GetSize(out w, out h);
                        this.Resize(System.Math.Max(ownerWidth - WidthPadding, MinimumWindowWidth), System.Math.Min(h, ownerHeight));
                        if (viewModel.IsIndeterminate)
                        {
                            _pulseTimer.Start();
                        }
                    }
                    else
                    {
                        _pulseTimer.Stop();
                        _progressBar.Fraction = 0;
                        INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                    }
                    this.Visible = viewModel.IsVisible;
#if ENABLE_OVERLAY
                    _overlay.Visible = viewModel.IsVisible;
                    this.TransientFor = _overlay;
#else
                    this.TransientFor = owner;
#endif // ENABLE_OVERLAY
                    this.Modal = viewModel.IsVisible;
                    break;
            }
        }

        /// <summary>
        /// Used to indicate activity for an indeterminate progress bar. By your command.
        /// </summary>
        /// <param name="s">Don't care.</param>
        /// <param name="e">A little slice of nothing.</param>
        private void Pulse(object s, System.EventArgs e)
        {
            _progressBar.Pulse();
        }
    }
}
