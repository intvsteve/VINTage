// <copyright file="ErrorReporting.cs" company="INTV Funhouse">
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
using INTV.Shared.View;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mechanism to use to report an error.
    /// </summary>
    public enum ReportMechanism
    {
        /// <summary>
        /// Send message to console output.
        /// </summary>
        Console,

        /// <summary>
        /// Send message to debug stream.
        /// </summary>
        Debug,

        /// <summary>
        /// Throw an exception containing a message.
        /// </summary>
        Exception,

        /// <summary>
        /// Display the message in a dialog.
        /// </summary>
        Dialog,

#if MAC
        /// <summary>
        /// The default error reporting mechanism.
        /// </summary>
        Default = Exception, // Console
#else
        /// <summary>
        /// The default error reporting mechanism.
        /// </summary>
        Default = Exception,
#endif
    }

    /// <summary>
    /// Helper class for reporting errors during development.
    /// </summary>
    public static class ErrorReporting
    {
        /// <summary>
        /// Report an error message using the default reporting mechanism.
        /// </summary>
        /// <param name="message">The message to report.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportError(string message)
        {
            ReportError(ReportMechanism.Default, message);
        }

        /// <summary>
        /// Report an error message using the default reporting mechanism if the given condition is true.
        /// </summary>
        /// <param name="condition">The condition to be met.</param>
        /// <param name="message">The message to report.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportErrorIf(bool condition, string message)
        {
            if (condition)
            {
                ReportError(ReportMechanism.Default, message);
            }
        }

        /// <summary>
        /// Report a 'Not Implemented' error via a dialog.
        /// </summary>
        /// <param name="message">The message to report.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportNotImplementedError(string message)
        {
            ReportNotImplemented(ReportMechanism.Dialog, message);
        }

        /// <summary>
        /// Report an error using a specific error reporting mechanism.
        /// </summary>
        /// <param name="reportMechanism">The reporting mechanism to use.</param>
        /// <param name="message">The message to report.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportError(ReportMechanism reportMechanism, string message)
        {
            ReportError<Exception>(reportMechanism, message, "Error");
        }

        /// <summary>
        /// Report a 'Not Implemented' error using a specific error reporting mechanism.
        /// </summary>
        /// <param name="reportMechanism">The reporting mechanism to use.</param>
        /// <param name="message">The message to report.</param>
        /// <remarks>If the reporting mechanism is ReportMechanism.Exception, then a NotImplementedException will be thrown.</remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportNotImplemented(ReportMechanism reportMechanism, string message)
        {
            ReportError<NotImplementedException>(reportMechanism, message, "Not Implemented");
        }

        /// <summary>
        /// Report an error message using the given reporting mechanism if the given condition is true.
        /// </summary>
        /// <param name="condition">The condition to be met.</param>
        /// <param name="reportMechanism">The reporting mechanism to use.</param>
        /// <param name="message">The message to report.</param>
        /// <param name="title">Title for a dialog, if the reporting mechanism is ReportMechanism.Dialog.</param>
        /// <typeparam name="T">The type of exception to throw.</typeparam>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportErrorIf<T>(bool condition, ReportMechanism reportMechanism, string message, string title) where T : Exception
        {
            if (condition)
            {
                ReportError<T>(reportMechanism, message, title);
            }
        }

        /// <summary>
        /// Report an error using a specific error reporting mechanism.
        /// </summary>
        /// <typeparam name="T">The type of exception to throw if the reporting mechanism is ReportMechanism.Exception.</typeparam>
        /// <param name="reportMechanism">The reporting mechanism to use.</param>
        /// <param name="message">The message to report.</param>
        /// <param name="title">Title for a dialog, if the reporting mechanism is ReportMechanism.Dialog.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ReportError<T>(ReportMechanism reportMechanism, string message, string title) where T : Exception
        {
            switch (reportMechanism)
            {
                case ReportMechanism.Console:
                    System.Console.WriteLine(title + ": " + message);
                    break;
                case ReportMechanism.Debug:
                    System.Diagnostics.Debug.WriteLine(title + ": " + message);
                    break;
                case ReportMechanism.Dialog:
                    OSMessageBox.Show(message, title, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                    break;
                case ReportMechanism.Exception:
                    throw (Exception)Activator.CreateInstance(typeof(T), message);
            }
        }
    }
}
