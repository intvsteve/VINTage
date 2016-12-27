// <copyright file="ProgramFeatureSet`T.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif
#endif

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Implements most of the IProgramFeatureSet interface. Implementations need only supply static data.
    /// </summary>
    /// <typeparam name="T">The enumeration (flags) defining the features in the feature set.</typeparam>
    public abstract class ProgramFeatureSet<T> : IProgramFeatureSet<T> where T : struct
    {
        private string _keyPrefix;

        /// <summary>
        /// Initializes a new instance of ProgramFeatureSet.
        /// </summary>
        /// <param name="category">The category of features being described.</param>
        protected ProgramFeatureSet(FeatureCategory category)
        {
            _keyPrefix = string.Empty;
            if ((typeof(T) == typeof(FeatureCompatibility)) || (typeof(T) == typeof(IntellicartCC3Features)))
            {
                _keyPrefix = category.ToString();
            }
            var resourceKey = _keyPrefix + typeof(T).Name + "_Name";
            Name = typeof(IProgramFeatureSet).GetResourceString(resourceKey);
            Category = category;
        }

        #region IProgramFeatureSet Properties

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public FeatureCategory Category { get; private set; }

        /// <inheritdoc />
        public virtual bool ExtendedFeaturesRequireEnhancedFlag
        {
            get { return true; }
        }

        /// <inheritdoc />
        public Type FeatureEnumType
        {
            get { return typeof(T); }
        }

        #endregion // IProgramFeatureSet Properties

        /// <summary>
        /// Gets the dictionary of flag values to image resources.
        /// </summary>
        protected abstract Dictionary<T, string> ImageResources { get; }

        /// <summary>
        /// Gets the dictionary of flag values to their descriptions.
        /// </summary>
        protected abstract Dictionary<T, string> Descriptions { get; }

        /// <summary>
        /// Gets the dictionary of flag values to their icons.
        /// </summary>
        protected abstract Dictionary<T, OSImage> Images { get; }

        #region IProgramFeatureSet Members

        /// <inheritdoc />
        public string GetFeatureName(uint featureBit)
        {
            return GetFeatureName((T)((object)featureBit));
        }

        /// <inheritdoc />
        public OSImage GetImageForFeature(uint featureBit)
        {
            return GetImageForFeature((T)((object)featureBit));
        }

        /// <inheritdoc />
        public string GetImageResourceForFeature(uint featureBit)
        {
            return GetImageResourceForFeature((T)((object)featureBit));
        }

        /// <inheritdoc />
        public IEnumerable<string> GetImageResourcesForFeatures(uint featureBits)
        {
            return GetImageResourcesForFeatures((T)((object)featureBits));
        }

        /// <inheritdoc />
        public string GetFeatureDescription(uint featureBit)
        {
            return GetFeatureDescription((T)((object)featureBit));
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFeatureDescriptions(uint featureBits)
        {
            return GetFeatureDescriptions((T)((object)featureBits));
        }

        #endregion // IProgramFeatureSet Members

        #region IProgramFeatureSet<T> Members

        /// <inheritdoc />
        public string GetFeatureName(T featureBit)
        {
            var resourceKey = _keyPrefix + typeof(T).Name + '_' + featureBit.ToString();
            var featureName = typeof(IProgramFeatureSet).GetResourceString(resourceKey);
            return featureName;
        }

        /// <inheritdoc />
        public OSImage GetImageForFeature(T featureBit)
        {
            OSImage image = null;
            if (!Images.TryGetValue(featureBit, out image))
            {
                string imageResource = GetImageResourceForFeature(featureBit);
                if (!string.IsNullOrEmpty(imageResource))
                {
                    image = typeof(IProgramFeatureSet).LoadImageResource(imageResource);
                    Images[featureBit] = image;
                }
            }
            return image;
        }

        /// <inheritdoc />
        public string GetImageResourceForFeature(T featureBit)
        {
            string imageResource = null;
            if (!ImageResources.TryGetValue(featureBit, out imageResource))
            {
                imageResource = null;
            }
            return imageResource;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetImageResourcesForFeatures(T featureBits)
        {
            return GetFeatureData(featureBits, ImageResources);
        }

        /// <inheritdoc />
        public string GetFeatureDescription(T featureBit)
        {
            string description = null;
            if (!Descriptions.TryGetValue(featureBit, out description))
            {
                description = null;
            }
            return description;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFeatureDescriptions(T featureBits)
        {
            return GetFeatureData(featureBits, Descriptions);
        }

        private static IEnumerable<string> GetFeatureData(T featureBits, Dictionary<T, string> data)
        {
            var featureData = new List<string>();
            var allFlags = Enum.GetValues(typeof(T)).Cast<object>().Distinct().ToArray();
            var flags = (Enum)((object)featureBits);
            foreach (var flag in allFlags)
            {
                if (flags.HasFlag((Enum)flag))
                {
                    var dataValue = string.Empty;
                    if (data.TryGetValue((T)flag, out dataValue))
                    {
                        featureData.Add(dataValue);
                    }
                }
            }
            return featureData;
        }

        #endregion // IProgramFeatureSet<T> Members
    }
}
