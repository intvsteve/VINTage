// <copyright file="OSMessageBox.cs" company="INTV Funhouse">
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

////#define USE_BETA_ERROR_REPORT

using System;
using System.Collections.Generic;

namespace INTV.Shared.View
{
    /// <summary>
    /// Provides access to the platform's general-purpose message box / alert dialog.
    /// </summary>
    public static partial class OSMessageBox
    {
        private static Predicate<Exception> _exceptionFilters = (e) => true;

        /// <summary>
        /// Registers an exception filter.
        /// </summary>
        /// <param name="filter">A filter called when a non-null Exception is passed to one of the Show methods.</param>
        /// <remarks>Registered filters are called whenever OSMessageBox.Show() is called with a non-null Exception. If any
        /// filter returns <c>false</c>, it is an indication that the exception should be ignored. This must be used with
        /// great caution, as it may result in suppressing error reporting!</remarks>
        public static void RegisterExceptionFilter(Predicate<Exception> filter)
        {
            _exceptionFilters += filter;
        }

        /// <summary>
        /// Unregisters an exception filter.
        /// </summary>
        /// <param name="filter">The exeption filter to remove.</param>
        public static void UnregisterExceptionFilter(Predicate<Exception> filter)
        {
            _exceptionFilters -= filter;
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Action<OSMessageBoxResult> onComplete)
        {
            Show(message, title, null, null, OSMessageBoxButton.OK, OSMessageBoxIcon.None, OSMessageBoxResult.OK, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Exception exception, Action<OSMessageBoxResult> onComplete)
        {
            Show(message, title, exception, null, OSMessageBoxButton.OK, OSMessageBoxIcon.None, OSMessageBoxResult.OK, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="reportText">Additional report text. If <paramref name="exception"/> is non-<c>null</c>, will be included in the error details, otherwise it will be part of the general message.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Exception exception, string reportText, Action<OSMessageBoxResult> onComplete)
        {
            Show(message, title, exception, reportText, OSMessageBoxButton.OK, OSMessageBoxIcon.None, OSMessageBoxResult.OK, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Exception exception, OSMessageBoxButton buttons, Action<OSMessageBoxResult> onComplete)
        {
            Show(message, title, exception, null, buttons, OSMessageBoxIcon.None, OSMessageBoxResult.OK, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Exception exception, OSMessageBoxButton buttons, OSMessageBoxIcon icon, Action<OSMessageBoxResult> onComplete)
        {
            Show(message, title, exception, null, buttons, icon, OSMessageBoxResult.OK, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="reportText">Additional report text. If <paramref name="exception"/> is non-<c>null</c>, will be included in the error details, otherwise it will be part of the general message.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <param name="onComplete">Upon completion of the dialog, this delegate will be called, if not <c>null</c>,</param>
        public static void Show(string message, string title, Exception exception, string reportText, OSMessageBoxButton buttons, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult, Action<OSMessageBoxResult> onComplete)
        {
            ShowCore(message, title, exception, reportText, buttons, null, icon, defaultResult, onComplete);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title)
        {
            return Show(message, title, null, OSMessageBoxButton.OK, OSMessageBoxIcon.None, OSMessageBoxResult.OK);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, OSMessageBoxButton buttons)
        {
            return Show(message, title, null, buttons, OSMessageBoxIcon.None, OSMessageBoxResult.OK);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, Exception exception, OSMessageBoxButton buttons)
        {
            return Show(message, title, exception, buttons, OSMessageBoxIcon.None, OSMessageBoxResult.OK);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, OSMessageBoxButton buttons, OSMessageBoxIcon icon)
        {
            return Show(message, title, null, buttons, icon, OSMessageBoxResult.OK);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="customButtonLabels">Custom button names.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <returns>The result of the dialog.</returns>
        /// <remarks>Custom button names are not supported in Windows.</remarks>
        public static OSMessageBoxResult Show(string message, string title, OSMessageBoxButton buttons, Dictionary<OSMessageBoxButton, string> customButtonLabels, OSMessageBoxIcon icon)
        {
            return ShowCore(message, title, null, null, buttons, customButtonLabels, icon, OSMessageBoxResult.OK, null);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, Exception exception, OSMessageBoxButton buttons, OSMessageBoxIcon icon)
        {
            return Show(message, title, exception, buttons, icon, OSMessageBoxResult.OK);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, OSMessageBoxButton buttons, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult)
        {
            return ShowCore(message, title, null, null, buttons, null, icon, defaultResult, null);
        }

        /// <summary>
        /// Show a message box and wait for a result.
        /// </summary>
        /// <param name="message">The detailed message to display.</param>
        /// <param name="title">Title for the message box.</param>
        /// <param name="exception">Exception information to report, if applicable.</param>
        /// <param name="buttons">The buttons to show in the dialog box.</param>
        /// <param name="icon">The icon to show in the dialog box.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <returns>The result of the dialog.</returns>
        public static OSMessageBoxResult Show(string message, string title, Exception exception, OSMessageBoxButton buttons, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult)
        {
            return ShowCore(message, title, exception, null, buttons, null, icon, defaultResult, null);
        }

        private static OSMessageBoxResult ShowCore(string message, string title, Exception exception, string reportText, OSMessageBoxButton buttons, Dictionary<OSMessageBoxButton, string> customButtonLabels, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult, Action<OSMessageBoxResult> onComplete)
        {
            var result = OSMessageBoxResult.None;
            var useErrorReport = exception != null;
            var reportException = useErrorReport;
            if (useErrorReport)
            {
                foreach (Predicate<Exception> predicate in _exceptionFilters.GetInvocationList())
                {
                    reportException = predicate(exception);
                    if (!reportException)
                    {
                        break;
                    }
                }
            }
            var hideExtraButtons = false;
#if USE_BETA_ERROR_REPORT
            useErrorReport = true;
            hideExtraButtons = exception == null;
#endif
            if (useErrorReport)
            {
                var messageLine = message;
                var dialog = ReportDialog.Create(title, messageLine);
                dialog.Exception = reportException ? exception : null;
                dialog.ReportText = reportText;
                dialog.ShowCopyToClipboardButton = !hideExtraButtons;
                dialog.ShowSendEmailButton = !hideExtraButtons;
                var buttonText = GetCustomTextForButton(OSMessageBoxButton.OK, null, buttons);
                var showResult = dialog.ShowDialog(buttonText);
                if (showResult.HasValue)
                {
                    result = showResult.Value ? OSMessageBoxResult.OK : OSMessageBoxResult.Cancel;
                    if (buttons != OSMessageBoxButton.OK)
                    {
                        result = showResult.Value ? OSMessageBoxResult.Yes : OSMessageBoxResult.No;
                    }
                }
            }
            else
            {
                var dialogMessage = new System.Text.StringBuilder(message);
                if (!string.IsNullOrWhiteSpace(reportText))
                {
                    dialogMessage.AppendLine().AppendLine().Append(reportText);
                }
                result = PlatformShowCore(dialogMessage.ToString(), title, exception, buttons, customButtonLabels, icon, defaultResult, onComplete);
            }
            return result;
        }

        private static string GetCustomTextForButton(OSMessageBoxButton button, Dictionary<OSMessageBoxButton, string> customButtonLabels, OSMessageBoxButton dialogButtons)
        {
            var buttonText = string.Empty;
            if ((customButtonLabels == null) || !customButtonLabels.TryGetValue(button, out buttonText))
            {
                switch (button)
                {
                    case OSMessageBoxButton.OK:
                        if (dialogButtons == OSMessageBoxButton.OK)
                        {
                            buttonText = Resources.Strings.OKButton_Text;
                        }
                        else
                        {
                            buttonText = Resources.Strings.YesButton_Text;
                        }
                        break;
                    case OSMessageBoxButton.YesNo:
                        buttonText = Resources.Strings.NoButton_Text;
                        break;
                    case OSMessageBoxButton.YesNoCancel:
                        buttonText = Resources.Strings.CancelButtonText;
                        break;
                }
            }
            return buttonText;
        }
    }
}
