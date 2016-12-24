// <copyright file="OSDispatcherTimer.WPF.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class OSDispatcherTimer
    {
        private System.Windows.Threading.DispatcherTimer _timer;

        public OSDispatcherTimer()
        {
            _timer = new System.Windows.Threading.DispatcherTimer();
        }

        /// <summary>
        /// Raised when the timer 'ticks'.
        /// </summary>
        public event EventHandler Tick
        {
            add { _timer.Tick += value; }
            remove { _timer.Tick -= value; }
        }

        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        public TimeSpan Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }
    }
}
