// <copyright file="AsyncTaskData.cs" company="INTV Funhouse">
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

using System.ComponentModel;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// This class provides a means to communicate data between an AsyncTaskWithProgress, the driver of the task, and the progress UI.
    /// </summary>
    public abstract class AsyncTaskData
    {
        private DoWorkEventArgs _doWorkArgs;
        private RunWorkerCompletedEventArgs _completedArgs;

        /// <summary>
        /// Initializes a new instance of AsyncTaskData.
        /// </summary>
        /// <param name="task">The task the data will work with. Must be non-null prior to starting the task.</param>
        protected AsyncTaskData(AsyncTaskWithProgress task)
        {
            Task = task;
        }

        /// <summary>
        /// Gets or sets the arguments for the underlying BackgroundWorker. Implementation detail.
        /// </summary>
        internal DoWorkEventArgs DoWorkArgs
        {
            get
            {
                return _doWorkArgs;
            }

            set
            {
                if (_doWorkArgs != value)
                {
                    _doWorkArgs = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the task complete arguments for the underlying BackgroundWorker. Implementation detail.
        /// </summary>
        internal RunWorkerCompletedEventArgs WorkerCompletedArgs
        {
            get
            {
                return _completedArgs;
            }

            set
            {
                if (_completedArgs != value)
                {
                    _completedArgs = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the task has been asked to cancel.
        /// </summary>
        public bool CancelRequsted
        {
            get { return Task.CancelRequested; }
        }

        /// <summary>
        /// Gets the task associated with the task data.
        /// </summary>
        /// <remarks>This is settable because task data may be constructed separate from the task with which it will be associated.</remarks>
        public AsyncTaskWithProgress Task { get; internal set; }

        /// <summary>
        /// Gets a value indicating which error occurred during an asynchronous operation.
        /// </summary>
        public System.Exception Error { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the operation was cancelled by the user.
        /// </summary>
        public bool Cancelled
        {
            get { return (WorkerCompletedArgs != null) && WorkerCompletedArgs.Cancelled; }
        }

        /// <summary>
        /// Gets a value indicating whether the progress indicator was shown during the task.
        /// </summary>
        public bool DidShowProgress
        {
            get { return Task.DidShowProgress; }
        }

        /// <summary>
        /// Update the underlying task's progress indicator text, which appears below the progress bar.
        /// </summary>
        /// <param name="percentDone">The completion percentage to display in the progress indicator.</param>
        /// <param name="updateText">Text to display below the indicator. If this value is <c>null</c>, it is ignored.</param>
        /// <remarks>If the operation has been cancelled, a the update text is ignored and a cancellation message is displayed.</remarks>
        public void UpdateTaskProgress(double percentDone, string updateText)
        {
            Task.UpdateTaskProgress(percentDone, updateText);
        }

        /// <summary>
        /// Updates the underlying task's progress indicator's title text, which appears above the indicator.
        /// </summary>
        /// <param name="newTitle">The new title to display. This value is ignored if <c>null</c>.</param>
        public void UpdateTaskTitle(string newTitle)
        {
            if (Task != null)
            {
                Task.UpdateTaskTitle(newTitle);
            }
        }

        /// <summary>
        /// If the underlying task has been asked to cancel, accept the cancellation.
        /// </summary>
        /// <returns><c>true</c> if the cancellation was accepted.</returns>
        public bool AcceptCancelIfRequested()
        {
            return Task.AcceptCancellation(DoWorkArgs);
        }
    }
}
