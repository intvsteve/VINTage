// <copyright file="ProgramFeatureImageViewModel.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for presenting a program feature in a containing visual.
    /// </summary>
    public class ProgramFeatureImageViewModel : System.IComparable<ProgramFeatureImageViewModel>, System.IComparable
    {
        /// <summary>
        /// Initializes a new instance of the ProgramFeatureImageViewModel class.
        /// </summary>
        /// <param name="name">Display name of the feature.</param>
        /// <param name="image">Icon representing the feature. May be <c>null</c>.</param>
        /// <param name="toolTip">Tool tip for the feature, displayed when hovering over icon.</param>
        /// <param name="category">The category of features to which the specific feature belongs.</param>
        /// <param name="flags">The flag describing the specific feature.</param>
        public ProgramFeatureImageViewModel(string name, OSImage image, string toolTip, FeatureCategory category, uint flags)
        {
            Name = name;
            Image = image;
            ToolTip = toolTip;
            Category = category;
            Flags = flags;
        }

        /// <summary>
        /// Gets the display name of the feature.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the image to display.
        /// </summary>
        public OSImage Image { get; private set; }

        /// <summary>
        /// Gets the tool tip to display for the image.
        /// </summary>
        public string ToolTip { get; private set; }

        /// <summary>
        /// Gets the raw bits describing the feature.
        /// </summary>
        public uint Flags { get; private set; }

        /// <summary>
        /// Gets the category to which the feature belongs.
        /// </summary>
        public FeatureCategory Category { get; private set; }

        #region IComparable<ProgramFeatureImageViewModel>

        /// <inheritdoc />
        public int CompareTo(ProgramFeatureImageViewModel other)
        {
            var result = 1;
            if (other != null)
            {
                result = (int)Category - (int)other.Category;
                if (result == 0)
                {
                    result = (int)((long)Flags - (long)other.Flags);
                }
            }
            return result;
        }

        #endregion // IComparable<ProgramFeatureImageViewModel>

        #region IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return CompareTo(obj as ProgramFeatureImageViewModel);
        }

        #endregion // IComparable
    }
}
