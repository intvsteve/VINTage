// <copyright file="ProgressIndicatorViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

#if MAC
using INTV.Shared.View;
#endif

#if WIN
using OSVisual = System.Windows.FrameworkElement;
#elif MAC
#if __UNIFIED__
using OSVisual = AppKit.NSWindow;
#else
using OSVisual = MonoMac.AppKit.NSWindow;
#endif
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// This class provides the ViewModel for the ProgressIndicator visual.
    /// </summary>
    public partial class ProgressIndicatorViewModel : ViewModelBase
    {
        /// <summary>
        /// The name of the progress indicator view model data context property.
        /// </summary>
        public const string ProgressIndicatorViewModelDataContextPropertyName = "ProgressIndicatorViewModelDataContext";

        /// <summary>
        /// The name of the AllowsCancel property.
        /// </summary>
        public const string AllowsCancelPropertyName = "AllowsCancel";

        /// <summary>
        /// The name of the Title property.
        /// </summary>
        public const string TitlePropertyName = "Title";

        /// <summary>
        /// The name of the UpdateText property.
        /// </summary>
        public const string UpdateTextPropertyName = "UpdateText";

        /// <summary>
        /// The name of the IsVisible property.
        /// </summary>
        public const string IsVisiblePropertyName = "IsVisible";

        /// <summary>
        /// The name of the PercentFinished property.
        /// </summary>
        public const string PercentFinishedPropertyName = "PercentFinished";

        /// <summary>
        /// The name of the IsIndeterminate property.
        /// </summary>
        public const string IsIndeterminatePropertyName = "IsIndeterminate";

        private const double MinDisplayTime = 1.0;

        /// <summary>
        /// The text to display for a cancel button associated with the task whose progress is being shown.
        /// </summary>
        public static readonly string Cancel = Resources.Strings.ProgressIndicator_CancelButtonText;

        /// <summary>
        /// The default amount of time to wait before showing the progress indicator, in seconds.
        /// </summary>
        public static readonly double DefaultDisplayDelay = 0.75;

        private string _title;
        private AsyncTaskWithProgress _task;
        private double _percentFinished;
        private string _updateText;
        private bool _isIndeterminate;
        private bool _allowsCancel;
        private bool _isVisible;
        private DateTime _launchTime;
        private OSDispatcherTimer _timer;
        private bool _cancelled;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ProgressIndicatorViewModel, which attaches itself to the application's main window.
        /// </summary>
        public ProgressIndicatorViewModel()
            : this((SingleInstanceApplication.Current != null) && (SingleInstanceApplication.Current.MainWindow != null) ? SingleInstanceApplication.Current.MainWindow : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of ProgressIndicatorViewModel, attaching it to the specified visual.
        /// </summary>
        /// <param name="visual">The visual with which to associate the progress indicator.</param>
        /// <remarks>This is an atypical construction pattern, driven partly by pragmatism and partly
        /// by laziness in getting the different platforms invovled. Blame the Mac and me not caring
        /// to revisit every off-the-cuff workaround I created while trying to avoid replicating the
        /// notion of DependencyObject / DependencyPropery and the whole inheritance mechanism.</remarks>
        public ProgressIndicatorViewModel(OSVisual visual)
        {
            DisplayDelay = DefaultDisplayDelay;
            if (visual != null)
            {
                visual.SetValue(ProgressIndicatorViewModelDataContextProperty, this);
            }
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the progress indicator view model associated with the application.
        /// </summary>
        public static ProgressIndicatorViewModel ApplicationProgressIndicator
        {
            get
            {
                var ready = (SingleInstanceApplication.Current != null) && (SingleInstanceApplication.Current.MainWindow != null);
                InitializeProgressIndicatorIfNecessary(ready);
                return ready ? SingleInstanceApplication.Current.MainWindow.GetValue(ProgressIndicatorViewModelDataContextProperty) as ProgressIndicatorViewModel : null;
            }
        }

        /// <summary>
        /// Gets or sets the title text displayed above the progress indicator.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { AssignAndUpdateProperty(TitlePropertyName, value, ref _title); }
        }

        /// <summary>
        /// Gets or sets the percent finished for the progress indicator.
        /// </summary>
        public double PercentFinished
        {
            get { return _percentFinished; }
            set { AssignAndUpdateProperty(PercentFinishedPropertyName, value, ref _percentFinished); }
        }

        /// <summary>
        /// Gets or sets the update text shown below the progress indicator.
        /// </summary>
        public string UpdateText
        {
            get { return _updateText; }
            set { AssignAndUpdateProperty(UpdateTextPropertyName, value, ref _updateText); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress indicator is for a process whose completion percentage cannot be known.
        /// </summary>
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { AssignAndUpdateProperty(IsIndeterminatePropertyName, value, ref _isIndeterminate); }
        }

        /// <summary>
        /// Gets or sets how long to wait (in seconds) before the progress indicator show be shown.
        /// </summary>
        /// <remarks>Setting this value after calling Show() will have no effect.</remarks>
        public double DisplayDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the command to execute to indicate whether the progress indicator allows the user to cancel the operation.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get { return new RelayCommand(OnCancel, OnCanCancel) { BlockWhenAppIsBusy = false }; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Cancel button for the task associated with the progress indicator should be available.
        /// </summary>
        public bool AllowsCancel
        {
            get { return _allowsCancel; }
            set { AssignAndUpdateProperty(AllowsCancelPropertyName, value, ref _allowsCancel); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the progress indicator should be displayed.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { AssignAndUpdateProperty(IsVisiblePropertyName, value, ref _isVisible, (p, v) => CommandManager.InvalidateRequerySuggested()); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether showing progress is cancelled.
        /// </summary>
        public bool IsCancelled
        {
            get { return _cancelled; }
            set { AssignAndUpdateProperty("IsCancelled", value, ref _cancelled); }
        }

        #endregion // Properties

        /// <summary>
        /// Instructs the progress indicator that it should display itself. It will appear after a brief delay.
        /// </summary>
        /// <param name="task">The task associated with the progress indicator.</param>
        /// <remarks>The progress indicator will display after a brief delay, and will display for a minimum amount of time once shown.
        /// If a task completes quickly, this prevents the progress indicator from appearing only to disappear almost instantaneously.
        /// Similarly, once the indicator becomes visible, it will remain visible for a minimum amount of time so as to be less jarring.</remarks>
        public void Show(AsyncTaskWithProgress task)
        {
            var application = SingleInstanceApplication.Instance;
            if (application != null)
            {
                application.IsBusy = true;
                _task = task;
                _cancelled = false;
                if (!IsVisible && (_timer == null) && (DisplayDelay > 0))
                {
                    PlatformOnShow(application);
                    _timer = new OSDispatcherTimer();
                    _timer.Tick += ShowProgressIndicator;
                    _timer.Interval = TimeSpan.FromSeconds(DisplayDelay);
                    _timer.Start();
                }
                else if (!(DisplayDelay < 0))
                {
                    PlatformOnShow(application);
                    ShowProgressIndicator(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Instructs the progress indicator to hide itself.
        /// </summary>
        /// <remarks>If the indicator is visible and has been showing for at least a minimal amount of time, it will be hidden
        /// immediately. If not, it will hide itself once a the minimum display time has elapsed.</remarks>
        public void Hide()
        {
            if (IsVisible && !_cancelled && (DateTime.Now < _launchTime.AddSeconds(MinDisplayTime)))
            {
                if (_timer != null)
                {
                    _timer.Tick -= ShowProgressIndicator;
                    _timer.Tick -= HideProgressIndicator;
                    _timer.Stop();
                }
                _timer = new OSDispatcherTimer();
                _timer.Tick += HideProgressIndicator;
                _timer.Interval = _launchTime.AddSeconds(MinDisplayTime) - DateTime.Now;
                _timer.Start();
            }
            else
            {
                HideProgressIndicator(null, null);
            }
            var application = SingleInstanceApplication.Instance;
            if (application != null)
            {
                application.IsBusy = false;
            }
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Called when the cancel button in the progress reporter is activated.
        /// </summary>
        /// <param name="parameter">Parameter (unused).</param>
        internal void OnCancel(object parameter)
        {
            _cancelled = true;
            if ((_task != null) && _task.IsBusy)
            {
                _task.CancelTask();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Platform-specific initialization for the progress bar.
        /// </summary>
        /// <param name="ready">If set to <c>true</c>, the application is ready to to initialize the progress indicator.</param>
        static partial void InitializeProgressIndicatorIfNecessary(bool ready);

        private bool OnCanCancel(object parameter)
        {
            return AllowsCancel;
        }

        private void ShowProgressIndicator(object sender, EventArgs e)
        {
            _launchTime = DateTime.Now;
            IsVisible = true;
            if (_timer != null)
            {
                _timer.Tick -= ShowProgressIndicator;
                _timer.Tick -= HideProgressIndicator;
                _timer.Stop();
            }
            _timer = null;
        }

        private void HideProgressIndicator(object sender, EventArgs e)
        {
            _cancelled = false;
            _launchTime = DateTime.MinValue;
            IsVisible = false;
            if (_timer != null)
            {
                _timer.Stop();
            }
            _timer = null;
            _task = null;
            var application = SingleInstanceApplication.Instance;
            if ((application != null) && (application.MainWindow != null))
            {
                PlatformOnHide(application);
            }
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
