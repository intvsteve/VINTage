// <copyright file="RunExternalProgram.cs" company="INTV Funhouse">
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

////#define SMTP_SUPPORT
////#define SMTP_SEND_SYNCHRONOUSLY
////#define ENABLE_DEBUG_SPAM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Helper methods for calling external applications.
    /// </summary>
    public static partial class RunExternalProgram
    {
        #region Launch (Fork) Methods

        /// <summary>
        /// Launches a program and returns immediately.
        /// </summary>
        /// <param name="programPath">Fully qualified path to the program to launch.</param>
        /// <param name="commandLineArguments">Command line argument string to send to the program.</param>
        /// <param name="workingDirectory">The working directory for the program.</param>
        /// <param name="showWindow">If <c>true</c>, show the window for the program.</param>
        /// <param name="useShellExecute">If <c>true</c>, run the program in a command shell instance.</param>
        /// <returns>The process that was launched.</returns>
        public static Process Launch(string programPath, string commandLineArguments, string workingDirectory, bool showWindow, bool useShellExecute)
        {
            return Launch(programPath, commandLineArguments, workingDirectory, showWindow, useShellExecute, false);
        }

        /// <summary>
        /// Launches a program and returns immediately, optionally with elevation.
        /// </summary>
        /// <param name="programPath">Fully qualified path to the program to launch.</param>
        /// <param name="commandLineArguments">Command line argument string to send to the program.</param>
        /// <param name="workingDirectory">The working directory for the program.</param>
        /// <param name="showWindow">If <c>true</c>, show the window for the program.</param>
        /// <param name="useShellExecute">If <c>true</c>, run the program in a command shell instance.</param>
        /// <param name="requiresElevation">If <c>true</c>, attempt to run the program with elevation.</param>
        /// <returns>The process that was launched.</returns>
        public static Process Launch(string programPath, string commandLineArguments, string workingDirectory, bool showWindow, bool useShellExecute, bool requiresElevation)
        {
            VerifyIsExecutable(programPath);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = programPath;
            processStartInfo.Arguments = commandLineArguments;
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.CreateNoWindow = !showWindow;
            processStartInfo.UseShellExecute = useShellExecute || requiresElevation; // Elevation requires shell
            if (requiresElevation)
            {
                processStartInfo.Verb = "runas";
            }
            var process = Process.Start(processStartInfo);
            return process;
        }

        /// <summary>
        /// Verifies the given program is executable.
        /// </summary>
        /// <param name="programPath">Absolute path to the program to start.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the executable at the given path is not executable.</exception>
        /// <remarks>This method verifies that the file at the given path can executed. It is used to
        /// work around a fatal crash in the Mono runtime reported here:
        /// https://bugzilla.xamarin.com/show_bug.cgi?id=37138</remarks>
        static partial void VerifyIsExecutable(string programPath);

        #endregion // Launch (Fork) Methods

        #region Synchronous Call Methods

        /// <summary>
        /// Launches a program and waits for it to finish, returning any output sent do stdout.
        /// </summary>
        /// <param name="programPath">Fully qualified path to the program to launch.</param>
        /// <param name="commandLineArguments">Command line argument string to send to the program.</param>
        /// <param name="workingDirectory">The working directory for the program.</param>
        /// <returns>All output sent to stdout during the execution of the program.</returns>
        public static string CallAndReturnStdOut(string programPath, string commandLineArguments, string workingDirectory)
        {
            VerifyIsExecutable(programPath);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = programPath;
            processStartInfo.Arguments = commandLineArguments;
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();
#if ENABLE_DEBUG_SPAM
            var errorOutput = process.StandardError.ReadToEnd();
            System.Diagnostics.Debug.WriteLine("stderr for " + System.IO.Path.GetFileName(programPath) + ": " + errorOutput);
#endif
            process.WaitForExit();
            return output;
        }

        /// <summary>
        /// Launches a program and waits for it to finish, return the program's exit code.
        /// </summary>
        /// <param name="programPath">Fully qualified path to the program to launch.</param>
        /// <param name="commandLineArguments">Command line argument string to send to the program.</param>
        /// <param name="workingDirectory">The working directory for the program.</param>
        /// <returns>The exit code of the process.</returns>
        public static int Call(string programPath, string commandLineArguments, string workingDirectory)
        {
            VerifyIsExecutable(programPath);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = programPath;
            processStartInfo.Arguments = commandLineArguments;
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            var result = process.ExitCode;
            return result;
        }

        #endregion // Synchronous Call Methods

        #region OpenInDefaultProgram

        /// <summary>
        /// Opens the give file in the default application associated with the file.
        /// </summary>
        /// <param name="filePath">Absolute path to the file to open.</param>
        public static void OpenInDefaultProgram(string filePath)
        {
            Uri uriResult;
            var isValid = Uri.TryCreate(filePath, UriKind.Absolute, out uriResult);
            if (isValid)
            {
                if (uriResult.Scheme == Uri.UriSchemeFile)
                {
                    isValid = System.IO.File.Exists(filePath) || System.IO.Directory.Exists(filePath);
                }
            }
            if (isValid)
            {
                OSOpenFileInDefaultProgram(filePath, uriResult.Scheme);
            }
            else
            {
                throw new System.IO.FileNotFoundException(filePath);
            }
        }

        #endregion // OpenInDefaultProgram

        #region Email Methods

        /// <summary>
        /// Launch the default email application with given recipient, CC, subject, and attachments.
        /// </summary>
        /// <param name="sender">Email address of the sender.</param>
        /// <param name="recipient">Email address of the recipient.</param>
        /// <param name="ccList">Additional email addresses.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="fileAttachments">An enumerable containing absolute paths of files to attach to the email.</param>
        public static void SendEmail(string sender, string recipient, IEnumerable<string> ccList, string subject, string message, IEnumerable<string> fileAttachments)
        {
            var escapedSubject = System.Uri.EscapeUriString(subject);
            var escapedMessage = System.Uri.EscapeUriString(message);
            var cc = string.Empty;
            if (ccList != null)
            {
                var stringBuilder = new System.Text.StringBuilder();
                foreach (var ccEmail in ccList)
                {
                    stringBuilder.AppendFormat("&cc={0}", System.Uri.EscapeUriString(ccEmail));
                }
                cc = stringBuilder.ToString();
            }
            var attachments = string.Empty;
            if (fileAttachments != null)
            {
                var stringBuilder = new System.Text.StringBuilder();
                foreach (var file in fileAttachments)
                {
                    stringBuilder.AppendFormat("&attach={0}", System.Uri.EscapeUriString(file));
                }
                attachments = stringBuilder.ToString();
            }
            var emailString = "mailto:" + System.Uri.EscapeUriString(recipient) + "?subject=" + escapedSubject + cc + "&body=" + escapedMessage + attachments;
            OSSendEmail(emailString);
        }

        /// <summary>
        /// Attempt to directly send an email messages via SMTP with given recipient, CC, subject, and attachments.
        /// </summary>
        /// <param name="sender">Email address of the sender.</param>
        /// <param name="recipient">Email address of the recipient.</param>
        /// <param name="ccList">Additional email addresses.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        /// <param name="fileAttachments">An enumerable containing absolute paths of files to attach to the email.</param>
        /// <param name="token">Caller-specific data to be passed to the <paramref name="sendComplete"/> callback.</param>
        /// <param name="sendComplete">A function to call when the attempt to send the email message completes. May be <c>null</c>.</param>
        /// <remarks>NOTE: This code is experimental, and incomplete. To fully implement in general requires a proper SMTP configuration UI and
        /// data retention, which should properly encrypt the necessary information.</remarks>
        public static void SendEmailUsingSMTP(string sender, string recipient, IEnumerable<string> ccList, string subject, string message, IEnumerable<string> fileAttachments, object token, Action<Exception, bool, object> sendComplete)
        {
            // If the address isn't valid, just use a fake INTV Funhouse address.
            try
            {
                if (!string.IsNullOrWhiteSpace(sender))
                {
                    var address = new System.Net.Mail.MailAddress(sender);
                    sender = address.Address;
                }
                else
                {
                    sender = null;
                }
            }
            catch (FormatException)
            {
                sender = null;
            }
            sender = sender ?? "anonymous@intvfunhouse.com";
            var smtpMessage = new System.Net.Mail.MailMessage(sender, recipient);
            smtpMessage.Subject = subject;
            smtpMessage.Body = message;
            if ((ccList != null) && ccList.Any())
            {
                foreach (var address in ccList)
                {
                    try
                    {
                        var cc = new System.Net.Mail.MailAddress(address);
                        smtpMessage.CC.Add(cc);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            if (fileAttachments != null)
            {
                foreach (var attachmentPath in fileAttachments)
                {
                    if (System.IO.File.Exists(attachmentPath))
                    {
                        var attachment = new System.Net.Mail.Attachment(attachmentPath);
                        attachment.ContentDisposition.CreationDate = System.IO.File.GetCreationTime(attachmentPath);
                        attachment.ContentDisposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentPath);
                        attachment.ContentDisposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentPath);
                        attachment.ContentDisposition.FileName = System.IO.Path.GetFileName(attachmentPath);
                        attachment.ContentDisposition.Size = new System.IO.FileInfo(attachmentPath).Length;
                        attachment.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Attachment;
                        smtpMessage.Attachments.Add(attachment);
                    }
                }
            }

            // HERE IS WHERE IT GETS ICKY. Don't feel like writing all the UI and safe data
            // management for the SMTP setup. Plus, some users may not what to set up all that
            // stuff again -- LUI is not an email client! And... who wants to write the code
            // that goes poking around the OS to see if this can be dug out some other way?
#if SMTP_SUPPORT
            var smtp = new System.Net.Mail.SmtpClient("some smtp host");
            smtp.EnableSsl = true;

            // TODO: Safely encrypt credentials to a store so they can be decrypted here and used.
            //       Need to store the SMTP client, port, user name, and password.
            // TODO: Write a config UI to determine whether the certificate checker is needed.
            // TODO: Who am I kidding, this is NOT a topic I'm familiar with.
            smtp.Credentials = new System.Net.NetworkCredential("user name", "password");
            smtp.Port = 25; // have to get a proper port number here
            var completionToken = new Tuple<object, Action<Exception, bool, object>>(token, sendComplete);
            try
            {
                // Use custom certificate validation.
                System.Net.ServicePointManager.ServerCertificateValidationCallback += SendEmailCertificateChecker;
#if SMTP_SEND_SYNCHRONOUSLY
                smtp.Send(smtpMessage);
                SendEmailComplete(smtp, new System.ComponentModel.AsyncCompletedEventArgs(null, false, completionToken));
#else
                smtp.SendCompleted += SendEmailComplete;
                smtp.SendAsync(smtpMessage, completionToken);
#endif // SMTP_SEND_SYNCHRONOUSLY
            }
            catch(Exception e)
            {
                if (sendComplete != null)
                {
                    SendEmailComplete(smtp, new System.ComponentModel.AsyncCompletedEventArgs(e, false, completionToken));
                }
                else
                {
                    throw;
                }
            }
            finally
            {
#if SMTP_SEND_SYNCHRONOUSLY
                // Only do this if we use the synchronous send. If we do this when doing an async send,
                // we remove the checker before it actually gets a chance to run.
                System.Net.ServicePointManager.ServerCertificateValidationCallback -= SendEmailCertificateChecker;
#endif // SMTP_SEND_SYNCHRONOUSLY
            }
#else
            throw new NotImplementedException("SMTP email send not implemented.");
#endif // SMTP_SUPPORT
        }

        private static bool SendEmailCertificateChecker(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            var alwaysAllowSend = false;
#if DEBUG
            alwaysAllowSend = true;
#endif
            System.Net.ServicePointManager.ServerCertificateValidationCallback -= SendEmailCertificateChecker;
            return alwaysAllowSend || ((sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) && certificate.Subject.Contains("CN=my trusted entity"));
        }

        private static void SendEmailComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback -= SendEmailCertificateChecker;
            var token = args.UserState as Tuple<object, Action<Exception, bool, object>>;
            if (token.Item2 != null)
            {
                token.Item2(args.Error, args.Cancelled, token.Item1);
            }
        }

        #endregion // Email Methods
    }
}
