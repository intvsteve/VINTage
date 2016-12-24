// <copyright file="Range.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Implements a simple range class. The Maximum and Minimum values are included in the range.
    /// </summary>
    /// <typeparam name="T">The type for which the range is specified.</typeparam>
    public class Range<T> where T : System.IComparable<T>
    {
        private bool _minValid;
        private bool _maxValid;
        private T _min;
        private T _max;

        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        public Range()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        /// <param name="minimum">The minimum value in the range.</param>
        /// <param name="maximum">The maximum value in the range.</param>
        public Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        /// <param name="other">Another range with which to initialize this instance.</param>
        public Range(Range<T> other)
        {
            if (other.IsValid)
            {
                Minimum = other.Minimum;
                Maximum = other.Maximum;
            }
        }

        /// <summary>
        /// Gets or sets the minimum value in the range.
        /// </summary>
        public T Minimum
        {
            get
            {
                return _min;
            }

            set
            {
                _min = value;
                _minValid = true;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value in the range.
        /// </summary>
        public T Maximum
        {
            get
            {
                return _max;
            }

            set
            {
                _max = value;
                _maxValid = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Range is correctly specified.
        /// </summary>
        public bool IsValid
        {
            get { return _minValid && _maxValid && Minimum.CompareTo(Maximum) <= 0; }
        }

        /// <summary>
        /// Determines if the provided value is inside the range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns><c>true</c> if the value is inside Range, otherwise <c>false</c>.</returns>
        public bool IsValueInRange(T value)
        {
            return (Minimum.CompareTo(value) <= 0) && (value.CompareTo(Maximum) <= 0);
        }

        /// <summary>
        /// Updates the minimum value in the range if the proposed value is less than the current minimum value.
        /// </summary>
        /// <param name="proposedMin">New candidate value for the minimum.</param>
        public void UpdateMinimum(T proposedMin)
        {
            if (_minValid)
            {
                if (Minimum.CompareTo(proposedMin) > 0)
                {
                    Minimum = proposedMin;
                }
            }
            else
            {
                Minimum = proposedMin;
            }
        }

        /// <summary>
        /// Updates the maximum value in the range if the proposed value is greater than the current maximum value.
        /// </summary>
        /// <param name="proposedMax">New candidate value for the maximum.</param>
        public void UpdateMaximum(T proposedMax)
        {
            if (_maxValid)
            {
                if (Maximum.CompareTo(proposedMax) < 0)
                {
                    Maximum = proposedMax;
                }
            }
            else
            {
                Maximum = proposedMax;
            }
        }

        /// <summary>
        /// Adds the given value to the range. The range will expand to contain the value if needed.
        /// </summary>
        /// <param name="value">The value to add to the range.</param>
        public void Add(T value)
        {
            UpdateMinimum(value);
            UpdateMaximum(value);
        }

        /// <summary>
        /// Merges this range and the given to form a new range inclusive of both.
        /// </summary>
        /// <param name="other">The range to merge with this one.</param>
        /// <returns>A new range that spans this and the other.</returns>
        public Range<T> Merge(Range<T> other)
        {
            var merged = new Range<T>(this);
            if (other.IsValid)
            {
                merged.UpdateMinimum(other.Minimum);
                merged.UpdateMaximum(other.Maximum);
            }
            return merged;
        }

        #region object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("[{0} - {1}]", Minimum, Maximum);
        }

        #endregion // object Overrides
    }
}
