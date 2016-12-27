// <copyright file="CommandManager.MONO.cs" company="INTV Funhouse">
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

#if MAC
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif
#endif

using INTV.Shared.Utility;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Command manager implementation that imitates part of the Microsoft CommandManager class..
    /// </summary>
    public sealed class CommandManager
    {
        private static int _commandRequery = 0;

        /// <summary>
        /// This event is raised when the InvalidateRequerySuggested method is called.
        /// </summary>
        public static event System.EventHandler RequerySuggested;

        /// <summary>
        /// Raises the RequerySuggested event. Interested parties should re-evaluate whether
        /// commands can execute.
        /// </summary>
        public static void InvalidateRequerySuggested()
        {
            if (SingleInstanceApplication.MainThreadDispatcher.NativeDispatcher != OSDispatcher.Current.NativeDispatcher)
            {
                using (var pool = new NSAutoreleasePool())
                {
                    SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() => InvalidateRequerySuggested());
                }
                return;
            }
            var requerySuggested = RequerySuggested;
            if (requerySuggested != null)
            {
                ++_commandRequery;
                try
                {
                    requerySuggested(null, System.EventArgs.Empty);
                }
                finally
                {
                    --_commandRequery;
                }
            }
        }
    }
}
