// <copyright file="ProgramFeaturesHelpers.cs" company="INTV Funhouse">
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
using INTV.Shared.Model.Program;
using INTV.Shared.ViewModel;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Provides data to produce a small visual to represent the features of a program.
    /// </summary>
    public static partial class ProgramFeaturesHelpers
    {
        private static readonly ProgramFeatureImageViewModel UniversallyCompatible = new ProgramFeatureImageViewModel(Resources.Strings.UniversallyCompatible_Name, typeof(ProgramFeaturesHelpers).LoadImageResource("ViewModel/Resources/Images/universally_compatible_16xMD.png"), Resources.Strings.GeneralFeatures_UniversallyCompatible_Tip, FeatureCategory.General, 0);

        /// <summary>
        /// Produces an enumerable containing icons and other data that represent all the given flags.
        /// </summary>
        /// <typeparam name="TFlag">The data type of the flag whose view models are desired.</typeparam>
        /// <param name="flags">The flags for which to retrieve view models.</param>
        /// <param name="featureSet">The feature set that provides detailed information about each flag.</param>
        /// <returns>An enumerable containing a view model for each flag specified in the flags argument.</returns>
        public static IEnumerable<ProgramFeatureImageViewModel> ToFeatureViewModels<TFlag>(this IEnumerable<TFlag> flags, IProgramFeatureSet<TFlag> featureSet) where TFlag : struct
        {
            var viewModels = new List<ProgramFeatureImageViewModel>();
            foreach (var flag in flags)
            {
                viewModels.Add(FetchProgramFeatureViewModelForFlag(flag, featureSet, true));
            }
            return viewModels;
        }

        /// <summary>
        /// Produces an enumerable containing icons that represent program features.
        /// </summary>
        /// <param name="features">The features to represent as icons.</param>
        /// <returns>The enumerable containing visuals representing the program features.</returns>
        public static IEnumerable<ProgramFeatureImageViewModel> ToFeatureViewModels(this ProgramFeatures features)
        {
            var anyIncompatibilities = false;
            var featuresToAdd = features.GeneralFeatures.GetImagesForFlags(GeneralFeatureSet.Data, CompatibilityCheckMode.None, ref anyIncompatibilities);

            var images = new List<ProgramFeatureImageViewModel>(featuresToAdd);

            featuresToAdd = features.KeyboardComponent.GetImagesForFlags(KeyboardComponentFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            var requiresPeripheral = featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.SuperVideoArcade.GetImagesForFlags(SuperVideoArcadeFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Intellivoice.GetImagesForFlags(IntellivoiceFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.IntellivisionII.GetImagesForFlags(IntellivisionIIFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Ecs.GetImagesForFlags(EcsFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Tutorvision.GetImagesForFlags(TutorvisionFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Intellicart.GetImagesForFlags(IntellicartFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.CuttleCart3.GetImagesForFlags(CuttleCart3FeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Jlp.GetImagesForFlags(JlpFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.LtoFlash.GetImagesForFlags(LtoFlashFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Bee3.GetImagesForFlags(Bee3FeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);
            featuresToAdd = features.Hive.GetImagesForFlags(HiveFeatureSet.Data, CompatibilityCheckMode.Default, ref anyIncompatibilities);
            requiresPeripheral |= featuresToAdd.ContainsRequiredFeature();
            images.AddRange(featuresToAdd);

            var ntscMode = features.Ntsc.GetImagesForFlags(NtscFeatureSet.Data, CompatibilityCheckMode.OnlyCheckIncompatible, ref anyIncompatibilities).FirstOrDefault();
            var palMode = features.Pal.GetImagesForFlags(PalFeatureSet.Data, CompatibilityCheckMode.OnlyCheckIncompatible, ref anyIncompatibilities).FirstOrDefault();

            if (palMode != null)
            {
                if (images.Any())
                {
                    images.Insert(0, palMode);
                }
                else
                {
                    images.Add(palMode);
                }
            }
            if (ntscMode != null)
            {
                if (images.Any())
                {
                    images.Insert(0, ntscMode);
                }
                else
                {
                    images.Add(ntscMode);
                }
            }

            if (!requiresPeripheral && (!anyIncompatibilities || !images.Any()) && !features.GeneralFeatures.HasFlag(GeneralFeatures.UnrecognizedRom))
            {
                if (!images.Any())
                {
                    images.Add(UniversallyCompatible);
                }
            }

            return images;
        }

        private static bool ContainsRequiredFeature(this List<ProgramFeatureImageViewModel> features)
        {
            var containsRequiredFeature = features.Any(f => ((FeatureCompatibility)f.Flags).HasFlag(FeatureCompatibility.Requires));
            return containsRequiredFeature;
        }

        private static List<ProgramFeatureImageViewModel> GetImagesForFlags<TFlag>(this TFlag flags, IProgramFeatureSet<TFlag> featureSet, CompatibilityCheckMode compatibilityCheck, ref bool hasIncompatibility) where TFlag : struct
        {
            var featureViewModels = new List<ProgramFeatureImageViewModel>();
            var allFlags = Enum.GetValues(typeof(TFlag)).Cast<object>().Distinct().ToArray();
            var currentFlags = (Enum)((object)flags);
            var setFlags = allFlags.Where(f => currentFlags.HasFlag((Enum)f)).Select(f => (TFlag)f);

            var featureCompatibility = ((FeatureCompatibility)(object)flags) & FeatureCompatibilityHelpers.ValidFeaturesMask;
            var incompatible = false;

            switch (compatibilityCheck)
            {
                case CompatibilityCheckMode.None:
                    foreach (var flag in setFlags)
                    {
                        var image = flag.FetchProgramFeatureViewModelForFlag(featureSet, false);
                        if (image != null)
                        {
                            featureViewModels.Add(image);
                        }
                    }
                    break;

                case CompatibilityCheckMode.Default:
                    incompatible = featureCompatibility == FeatureCompatibility.Incompatible;
                    if (incompatible && featureSet.ExtendedFeaturesRequireEnhancedFlag)
                    {
                        var image = default(TFlag).FetchProgramFeatureViewModelForFlag(featureSet, false);
                        if (image != null)
                        {
                            featureViewModels.Add(image);
                        }
                    }
                    else
                    {
                        var image = ((TFlag)allFlags.GetValue((int)featureCompatibility)).FetchProgramFeatureViewModelForFlag(featureSet, false);
                        if (image != null)
                        {
                            featureViewModels.Add(image);
                        }
                        if (!featureSet.ExtendedFeaturesRequireEnhancedFlag || featureCompatibility.HasFlag(FeatureCompatibility.Enhances))
                        {
                            foreach (var flag in allFlags.Skip((int)FeatureCompatibility.NumCompatibilityModes))
                            {
                                if (((Enum)(object)flags).HasFlag((Enum)flag))
                                {
                                    image = ((TFlag)flag).FetchProgramFeatureViewModelForFlag(featureSet, false);
                                    if (image != null)
                                    {
                                        featureViewModels.Add(image);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case CompatibilityCheckMode.OnlyCheckIncompatible:
                    incompatible = featureCompatibility == FeatureCompatibility.Incompatible;
                    if (incompatible)
                    {
                        var image = default(TFlag).FetchProgramFeatureViewModelForFlag(featureSet, false);
                        if (image != null)
                        {
                            featureViewModels.Add(image);
                        }
                    }
                    break;
            }

            hasIncompatibility |= incompatible;
            return featureViewModels;
        }

        private static ProgramFeatureImageViewModel FetchProgramFeatureViewModelForFlag<TFlag>(this TFlag flag, IProgramFeatureSet<TFlag> featureSet, bool allowNullImage) where TFlag : struct
        {
            ProgramFeatureImageViewModel featureViewModel = null;
            OSImage image = featureSet.GetImageForFeature(flag);
            if (allowNullImage || (image != null))
            {
                featureViewModel = new ProgramFeatureImageViewModel(featureSet.GetFeatureName(flag), image, featureSet.GetFeatureDescription(flag), featureSet.Category, (uint)((object)flag));
            }
            return featureViewModel;
        }

        private enum CompatibilityCheckMode
        {
            /// <summary>
            /// Do not check compatibility.
            /// </summary>
            None,

            /// <summary>
            /// Check for incompatible flag, then check if extended features require enhanced flag.
            /// </summary>
            Default,

            /// <summary>
            /// Only use strict incompatibility flag check.
            /// </summary>
            OnlyCheckIncompatible,
        }
    }
}
