// <copyright file="OneShotLaunchTask.cs" company="INTV Funhouse">
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
using System.IO;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// A task that should execute one time, at application launch. Typically this is part of an update / upgrade / mutation action. Hopefully rarely used or needed.
    /// </summary>
    public abstract class OneShotLaunchTask
    {
        /// <summary>
        /// Initializes a new instance of a one-shot task.
        /// </summary>
        /// <param name="name">The name for the task.</param>
        protected OneShotLaunchTask(string name)
        {
            Name = name;
        }

        private OneShotLaunchTask()
        {
        }

        /// <summary>
        /// Gets the name for the task.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Registers the task with the application. The application will run the task at an appropriate time.
        /// </summary>
        /// <param name="priority">Priority of the one-shot task. Usually, these are >= StartupTaskPriority.LowestSyncTaskPriority</param>
        public void Register(StartupTaskPriority priority)
        {
            // This is null in the Visual Studio designer.
            if (SingleInstanceApplication.Instance != null)
            {
                SingleInstanceApplication.Instance.AddStartupAction(Name, Task, priority);
            }
        }

        /// <summary>
        /// Executes the actual task.
        /// </summary>
        protected abstract void Run();

        private void Task()
        {
            try
            {
                var oneShotTaskFile = Path.Combine(PathUtils.GetDocumentsDirectory(), SingleInstanceApplication.AppInfo.DocumentFolderName, Name + ".updatefinished");
                if (!File.Exists(oneShotTaskFile))
                {
                    using (File.Create(oneShotTaskFile))
                    {
                        Run();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
