// <copyright file="FileNodeColorViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Shared.Utility;

using IntvColor = INTV.Core.Model.Stic.Color;

#if WIN
using NodeColorBase = INTV.Shared.ViewModel.ViewModelBase;
using OSBrush = System.Windows.Media.SolidColorBrush;
#elif MAC
#if __UNIFIED__
using NodeColorBase = Foundation.NSObject;
using OSBrush = AppKit.NSColor;
#else
using NodeColorBase = MonoMac.Foundation.NSObject;
using OSBrush = MonoMac.AppKit.NSColor;
#endif
#endif

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// A ViewModel for an Intellivision color.
    /// </summary>
    public class FileNodeColorViewModel : NodeColorBase
    {
        private static Dictionary<IntvColor, FileNodeColorViewModel> _brushes = new Dictionary<IntvColor, FileNodeColorViewModel>();
        private OSBrush _fill;
        private IntvColor _color;

        #region Constructors

        /// <summary>
        /// Initializes a new instanced of a FileNodeColorViewModel based on an Intellivision color.
        /// </summary>
        /// <param name="color">The color of the element.</param>
        private FileNodeColorViewModel(IntvColor color)
        {
            _color = color;

            // TODO Even though redunant on Mac, could we make the code the same for both platforms?
#if WIN
            _fill = new OSBrush(color.ToColor());
#elif MAC
            _fill = color.ToColor();
#endif
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the platform-specific value to present the Intellivision color in a visual.
        /// </summary>
        public OSBrush Fill
        {
            get { return _fill; }
        }

        /// <summary>
        /// Gets the display name of the color.
        /// </summary>
        public string Name
        {
            get { return IntvColor.ToDisplayString(); }
        }

        /// <summary>
        /// Gets the original Intellivision color this ViewModel represents.
        /// </summary>
        public IntvColor IntvColor
        {
            get { return _color; }
        }

        /// <summary>
        /// Gets a ViewModel for the give Intellivision color.
        /// </summary>
        /// <param name="color">The Intellivision color for which to get a ViewModel.</param>
        /// <returns>The corresponding ViewModel for the Intellivision color.</returns>
        public static FileNodeColorViewModel GetColor(IntvColor color)
        {
            FileNodeColorViewModel colorViewModel = null;
            if (!_brushes.TryGetValue(color, out colorViewModel))
            {
                colorViewModel = new FileNodeColorViewModel(color);
                _brushes[color] = colorViewModel;
            }
            return colorViewModel;
        }

        #region object Overrides

        /// <inheritdoc />
        /// <remarks>Two of these are considered equal if the underlying Intellivision color they represent is the same.</remarks>
        public override bool Equals(object obj)
        {
            bool areEqual = obj is FileNodeColorViewModel;
            if (areEqual)
            {
                var other = (FileNodeColorViewModel)obj;
                areEqual = IntvColor == other.IntvColor;
            }
            return areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return IntvColor.GetHashCode();
        }

        /// <inheritdoc />
        /// <remarks>Just prints the name of the underlying Intellivision color.</remarks>
        public override string ToString()
        {
            return IntvColor.ToString();
        }

        #endregion // object Overrides
    }
}
