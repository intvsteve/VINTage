// <copyright file="ApplicationLogger.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// These values describe the level of severity of an entry in the application log.
    /// </summary>
    public enum ApplicationLogSeverity
    {
        /// <summary>
        /// A debug trace statement, typically used for gathering debug information in the field.
        /// </summary>
        DebugTrace = 0,

        /// <summary>
        /// Basic informational statement.
        /// </summary>
        Information,

        /// <summary>
        /// An internal warning. Such conditions are non-fatal, but of interest for
        /// bug fixing or planned-for, but undesired conditions.
        /// </summary>
        InternalWarning,

        /// <summary>
        /// An error condition that is important enough to be reported to the user
        /// via an error dialog, but not fatal.
        /// </summary>
        VisibleWarning,

        /// <summary>
        /// A fatal error condition that causes the product to exit. This is usually reported
        /// via the crash reporter and retained in a dedicated error log file of its own.
        /// </summary>
        Fatal
    }

    /// <summary>
    /// A general-purpose logger for the application and components to use.
    /// </summary>
    public sealed class ApplicationLogger : Logger
    {
        private static readonly Lazy<ApplicationLogger> AppLogger = new Lazy<ApplicationLogger>(Initialize);

        private ApplicationLogger(string logPath)
            : base(logPath)
        {
        }

        private static ApplicationLogger Logger
        {
            get { return AppLogger.Value; }
        }

        /// <summary>
        /// Records a debug trace message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public static void RecordDebugTraceMessage(string message)
        {
            Log(message, ApplicationLogSeverity.DebugTrace);
        }

        /// <summary>
        /// Records an informational message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public static void RecordInformationalMessage(string message)
        {
            Log(message, ApplicationLogSeverity.Information);
        }

        /// <summary>
        /// Records an internal warning message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public static void RecordInternalWarning(string message)
        {
            Log(message, ApplicationLogSeverity.InternalWarning);
        }

        /// <summary>
        /// Records an error message that is also reported directly to the user.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public static void RecordVisibleWarning(string message)
        {
            Log(message, ApplicationLogSeverity.VisibleWarning);
        }

        /// <summary>
        /// Records a crash message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public static void RecordFatalEvent(string message)
        {
            Log(message, ApplicationLogSeverity.Fatal);
        }

        /// <summary>
        /// Log the specified message with the given severity.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="severity">The <see cref="ApplicationLogSeverity"/> of the message.</param>
        public static void Log(string message, ApplicationLogSeverity severity)
        {
            message = severity.ToString().ToUpperInvariant() + ": " + message;
            Logger.Log(message);
        }

        private static ApplicationLogger Initialize()
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var appName = System.IO.Path.GetFileNameWithoutExtension(entryAssembly.GetName().Name);
            var fileName = appName + "_Log_" + PathUtils.GetTimeString(PathUtils.AllTimeStringFields, includeMilliseconds: false, useUTC: false) + ".txt";
            var documentFolderName = SingleInstanceApplication.Instance.AppInfo.DocumentFolderName;
            var logDirectory = System.IO.Path.Combine(PathUtils.GetDocumentsDirectory(), documentFolderName);
            System.IO.Directory.CreateDirectory(logDirectory);
            var applicationLogger = new ApplicationLogger(System.IO.Path.Combine(logDirectory, fileName));
            return applicationLogger;
        }
    }
}
