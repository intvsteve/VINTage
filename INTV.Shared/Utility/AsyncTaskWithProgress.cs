// <copyright file="AsyncTaskWithProgress.cs" company="INTV Funhouse">
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

#define USE_ASYNC

using System;
using System.ComponentModel;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// This class provides integration between the expected application-level progress bar ViewModel and the execution
    /// of an asynchronous task. The task will be executed via a BackgroundWorker, and progress will be updated. It is
    /// the duty of the task to periodically check to see if a request to cancel the operation has been made, if the
    /// task supports such functionality.
    /// </summary>
    public class AsyncTaskWithProgress
    {
#if USE_ASYNC
        private BackgroundWorker _backgroundWorker;
#endif // USE_ASYNC
        private ProgressIndicatorViewModel _progressIndicator;
        private Action<AsyncTaskData> _doWork;
        private Action<AsyncTaskData> _workComplete;
        private AsyncTaskData _taskData;
        private bool _showsProgress;
        private bool _didShowProgresss;

        #region Constructors

        /// <summary>
        /// Creates a new instance of AsyncTaskWithProgress.
        /// </summary>
        /// <param name="taskName">The name of the task, which will be shown above the progress indicator.</param>
        public AsyncTaskWithProgress(string taskName)
            : this(taskName, false, true, false, ProgressIndicatorViewModel.DefaultDisplayDelay)
        {
        }

        /// <summary>
        /// Creates a new instance of AsyncTaskWithProgress.
        /// </summary>
        /// <param name="taskName">The name of the task, which will be shown above the progress indicator.</param>
        /// <param name="allowsCancel">If <c>true</c>, indicates that a 'Cancel' button should be available, allowing the user to cancel the operation.</param>
        public AsyncTaskWithProgress(string taskName, bool allowsCancel)
            : this(taskName, allowsCancel, true, false, ProgressIndicatorViewModel.DefaultDisplayDelay)
        {
        }

        /// <summary>
        /// Creates a new instance of AsyncTaskWithProgress.
        /// </summary>
        /// <param name="taskName">The name of the task, which will be shown above the progress indicator.</param>
        /// <param name="allowsCancel">If <c>true</c>, indicates that a 'Cancel' button should be available, allowing the user to cancel the operation.</param>
        /// <param name="isIndeterminate">If <c>true</c>, indicates the task will not indicate a percentage done, but rather that the task is 'alive'.</param>
        public AsyncTaskWithProgress(string taskName, bool allowsCancel, bool isIndeterminate)
            : this(taskName, allowsCancel, isIndeterminate, true, ProgressIndicatorViewModel.DefaultDisplayDelay)
        {
        }

        /// <summary>
        /// Creates a new instance of AsyncTaskWithProgress.
        /// </summary>
        /// <param name="taskName">The name of the task, which will be shown above the progress indicator.</param>
        /// <param name="allowsCancel">If <c>true</c>, indicates that a 'Cancel' button should be available, allowing the user to cancel the operation.</param>
        /// <param name="isIndeterminate">If <c>true</c>, indicates the task will not indicate a percentage done, but rather that the task is 'alive'.</param>
        /// <param name="progressDisplayDelay">How long to wait (in seconds) before showing the progress bar.</param>
        public AsyncTaskWithProgress(string taskName, bool allowsCancel, bool isIndeterminate, double progressDisplayDelay)
            : this(taskName, allowsCancel, isIndeterminate, true, progressDisplayDelay)
        {
        }

        /// <summary>
        /// Creates a new instance of AsyncTaskWithProgress.
        /// </summary>
        /// <param name="taskName">The name of the task, which will be shown above the progress indicator.</param>
        /// <param name="allowsCancel">If <c>true</c>, indicates that a 'Cancel' button should be available, allowing the user to cancel the operation.</param>
        /// <param name="isIndeterminate">If <c>true</c>, indicates the task will not indicate a percentage done, but rather that the task is 'alive'.</param>
        /// <param name="showsProgress">If <c>true</c>, indicates the task will show progress bar after appropriate time passes, otherwise no progress bar will be shown.</param>
        /// <param name="progressDisplayDelay">How long to wait (in seconds) before showing the progress bar.</param>
        public AsyncTaskWithProgress(string taskName, bool allowsCancel, bool isIndeterminate, bool showsProgress, double progressDisplayDelay)
        {
            _showsProgress = showsProgress;
            if (_showsProgress)
            {
                _progressIndicator = ProgressIndicatorViewModel.ApplicationProgressIndicator;
                if (_progressIndicator != null)
                {
                    _progressIndicator.DisplayDelay = progressDisplayDelay;
                    _progressIndicator.Title = taskName;
                    _progressIndicator.IsIndeterminate = isIndeterminate;
                    _progressIndicator.AllowsCancel = allowsCancel;
                    _progressIndicator.UpdateText = string.Empty;
                }
            }
#if USE_ASYNC
            _backgroundWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = allowsCancel };
            _backgroundWorker.DoWork += AsyncTaskDoWork;
            _backgroundWorker.ProgressChanged += AsyncTaskProgressChanged;
            _backgroundWorker.RunWorkerCompleted += AsynTaskRunWorkerCompleted;
#endif // USE_ASYNC
        }

        #endregion // Constructors

        #region Properties

#if USE_ASYNC
        /// <summary>
        /// Gets a value indicating whether or not the task is still executing.
        /// </summary>
        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the task as been asked to be cancelled.
        /// </summary>
        public bool CancelRequested
        {
            get { return _backgroundWorker.CancellationPending; }
        }
#else
        public bool IsBusy
        {
            get;
            private set;
        }

        public bool CancelRequested
        {
            get { return false; }
        }
#endif // USE_ASYNC

        /// <summary>
        /// Gets a value indicating whether or not the progress indicator was made visible during the task.
        /// </summary>
        public bool DidShowProgress
        {
            get { return _showsProgress && _didShowProgresss; }
        }

        #endregion // Properties

        /// <summary>
        /// Starts running the task asynchronously on another thread.
        /// </summary>
        /// <param name="taskData">Task-specific data to be supplied to the task.</param>
        /// <param name="doWork">The function that executes the asynchronous task.</param>
        /// <param name="onWorkComplete">The function to call when the task ends.</param>
        /// <remarks>This function will cause the application-wide progress indicator to appear after a small delay.</remarks>
        public void RunTask(AsyncTaskData taskData, Action<AsyncTaskData> doWork, Action<AsyncTaskData> onWorkComplete)
        {
#if MAC
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
#endif // MAC
            if (_showsProgress && (_progressIndicator != null))
            {
                _didShowProgresss = _progressIndicator.IsVisible;
                _progressIndicator.PropertyChanged += OnProgressIndicatorPropertyChanged;
                _progressIndicator.Show(this);
            }
            _taskData = taskData;
            _taskData.Task = this;
            _doWork = doWork;
            _workComplete = onWorkComplete;
#if USE_ASYNC
            _backgroundWorker.RunWorkerAsync(taskData);
#else
            IsBusy = true;
            doWork(taskData);
            onWorkComplete(taskData);
            IsBusy = false;
#endif // USE_ASYNC
        }

        /// <summary>
        /// Updates the progress indicator with the given text and completion percentage.
        /// </summary>
        /// <param name="percentDone">The completion percentage to display in the progress indicator.</param>
        /// <param name="updateText">Text to display below the indicator. If this value is <c>null</c>, it is ignored.</param>
        /// <remarks>If the operation has been cancelled, a the update text is ignored and a cancellation message is displayed.</remarks>
        public void UpdateTaskProgress(double percentDone, string updateText)
        {
#if USE_ASYNC
            var message = _backgroundWorker.CancellationPending ? Resources.Strings.ProgressIndicator_CancelRequested : updateText;
            _backgroundWorker.ReportProgress(Convert.ToInt32(Math.Round(percentDone)), message);
#endif // USE_ASYNC
        }

        /// <summary>
        /// Updates the progress indicator's title text, which appears above the indicator.
        /// </summary>
        /// <param name="newTitle">The new title to display. This value is ignored if <c>null</c>.</param>
        public void UpdateTaskTitle(string newTitle)
        {
            if ((newTitle != null) && _showsProgress && (_progressIndicator != null))
            {
                SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() => _progressIndicator.Title = newTitle));
            }
        }

        /// <summary>
        /// Instructs the task that it should cancel its operation. The cancellation will not occur until Is the worker method indicates
        /// it has acknowledged the cancellation request.
        /// </summary>
        public void CancelTask()
        {
#if USE_ASYNC
            _backgroundWorker.CancelAsync();
#endif // USE_ASYNC
        }

        /// <summary>
        /// Called during the task to acknowledge and accept the cancellation of the task.
        /// </summary>
        /// <param name="e">The arguments sent to the task.</param>
        /// <returns><c>true</c> if a cancellation was requested and accepted.</returns>
        public bool AcceptCancellation(DoWorkEventArgs e)
        {
            bool cancelAccepted = CancelRequested;
            if (CancelRequested && (e != null))
            {
                e.Cancel = true;
            }
            return cancelAccepted;
        }

        private void AsyncTaskDoWork(object sender, DoWorkEventArgs e)
        {
            // UNDONE Should Mac set up an autorelease bool here?
            // Based on this post: http://forums.xamarin.com/discussion/6404/memory-leaks-and-nsautoreleasepool
            // it is unclear whether it's necessary. The thread's about iOS, but is likely
            // applicable to Mac OS X as well.
            // This: https://stackoverflow.com/questions/7659843/system-componentmodel-backgroundworker-in-monotouch-correct-usage
            // using (var pool = new NSAutoreleasePool())
            _taskData.DoWorkArgs = e;
            _doWork(_taskData);
        }

        private void AsyncTaskProgressChanged(object sender, ProgressChangedEventArgs e)
        {
#if MAC
            // BUG reported here: https://bugzilla.xamarin.com/show_bug.cgi?id=57544
            INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
#endif // MAC
            if (_showsProgress && (_progressIndicator != null))
            {
                if (e.UserState is string)
                {
                    _progressIndicator.PercentFinished = e.ProgressPercentage;
                    if (e.UserState != null)
                    {
                        _progressIndicator.UpdateText = e.UserState as string;
                    }
                }
                else
                {
                    _progressIndicator.PercentFinished = 100;
                    _progressIndicator.UpdateText = Resources.Strings.ProgressIndicator_Cancelled;
                }
            }
#if MAC
                });
#endif // MAC
        }

        private void AsynTaskRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // BUG: In Mono, this is NOT NOT NOT getting called on the same thread as the
            // one that started the worker. Testing on:
            // Mono 5.0.1.1 (2017-02/5077205 Thu May 25 09:19:18 UTC 2017) (64-bit)
            // GTK+ 2.24.30 (Ambiance theme)
            // This bug report sounds much like the problem, too: https://bugzilla.xamarin.com/show_bug.cgi?id=2916
            // NEW bug report here: https://bugzilla.xamarin.com/show_bug.cgi?id=57544
            // Notes over in FileUtilities - which are less than a year old? - indicate the same
            // problem happens on Mac.
#if MAC
            INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
#endif // MAC
            _taskData.WorkerCompletedArgs = e;
            _taskData.Error = e.Error;
            if (_progressIndicator != null)
            {
                _progressIndicator.PropertyChanged -= OnProgressIndicatorPropertyChanged;
                if (_showsProgress)
                {
                    _progressIndicator.Hide();
                }
            }
#if MAC
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
            _workComplete(_taskData);
            });
#else
            _workComplete(_taskData);
#endif // MAC
        }

        private void OnProgressIndicatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((_progressIndicator != null) && (e.PropertyName == "IsVisible"))
            {
                _didShowProgresss |= _progressIndicator.IsVisible;
            }
        }
    }
}
