// <copyright file="ProgramFeatures.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Describes various features of a program, such as hardware compatibility, incompatibilities, requirements, et. al.
    /// </summary>
    public class ProgramFeatures : IProgramFeatures, IComparable, IComparable<IProgramFeatures>, IComparable<ProgramFeatures>, IEquatable<IProgramFeatures>, IEquatable<ProgramFeatures>
    {
        /// <summary>
        /// A set of completely empty ProgramFeatures.
        /// </summary>
        public static readonly ProgramFeatures EmptyFeatures = new ProgramFeatures()
        {
            GeneralFeatures = GeneralFeatures.None,
            Ntsc = FeatureCompatibility.Incompatible,
            Pal = FeatureCompatibility.Incompatible,
            KeyboardComponent = KeyboardComponentFeatures.Incompatible,
            SuperVideoArcade = FeatureCompatibility.Incompatible,
            Intellivoice = FeatureCompatibility.Incompatible,
            IntellivisionII = FeatureCompatibility.Incompatible,
            Ecs = EcsFeatures.Incompatible,
            Tutorvision = FeatureCompatibility.Incompatible,
            Intellicart = IntellicartCC3Features.Incompatible,
            CuttleCart3 = CuttleCart3Features.Incompatible,
            Jlp = JlpFeatures.Incompatible,
            LtoFlash = LtoFlashFeatures.Incompatible,
            Bee3 = Bee3Features.Incompatible,
            Hive = HiveFeatures.Incompatible
        };

        /// <summary>
        /// A set of default features.
        /// </summary>
        /// <remarks>NOTE: This does not assume 'unknown' for the GeneralFeatures field.</remarks>
        public static readonly ProgramFeatures DefaultFeatures = new ProgramFeatures(GeneralFeatures.None, FeatureCompatibility.Tolerates, FeatureCompatibility.Tolerates);

        private Dictionary<FeatureCategory, uint> _features;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ProgramFeatures.
        /// </summary>
        public ProgramFeatures()
            : this(GeneralFeatures.None, FeatureCompatibility.Tolerates, FeatureCompatibility.Tolerates)
        {
        }

        private ProgramFeatures(GeneralFeatures generalFeatures, FeatureCompatibility ntsc, FeatureCompatibility pal)
        {
            _features = new Dictionary<FeatureCategory, uint>();
            _features[FeatureCategory.Ntsc] = (uint)ntsc;
            _features[FeatureCategory.Pal] = (uint)pal;
            _features[FeatureCategory.General] = (uint)generalFeatures;
            _features[FeatureCategory.KeyboardComponent] = (uint)KeyboardComponentFeaturesHelpers.Default;
            _features[FeatureCategory.SuperVideoArcade] = (uint)FeatureCompatibility.Tolerates;
            _features[FeatureCategory.Intellivoice] = (uint)FeatureCompatibility.Tolerates;
            _features[FeatureCategory.IntellivisionII] = (uint)FeatureCompatibility.Tolerates;
            _features[FeatureCategory.Ecs] = (uint)EcsFeaturesHelpers.Default;
            _features[FeatureCategory.Tutorvision] = (uint)FeatureCompatibility.Tolerates;
            _features[FeatureCategory.Intellicart] = (uint)IntellicartCC3FeaturesHelpers.Default;
            _features[FeatureCategory.CuttleCart3] = (uint)IntellicartCC3FeaturesHelpers.Default;
            _features[FeatureCategory.Jlp] = (uint)JlpFeaturesHelpers.Default;
            _features[FeatureCategory.LtoFlash] = (uint)LtoFlashFeaturesHelpers.Default;
            _features[FeatureCategory.Bee3] = (uint)Bee3FeaturesHelpers.Default;
            _features[FeatureCategory.Hive] = (uint)HiveFeaturesHelpers.Default;
            System.Diagnostics.Debug.Assert(_features.Count == (int)FeatureCategory.NumberOfCategories, "Failed to assign default features for all feature categories.");
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets NTSC compatibility.
        /// </summary>
        public FeatureCompatibility Ntsc
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.Ntsc]; }
            set { _features[FeatureCategory.Ntsc] = (uint)value.CoerceVideoStandardCompatibility(); }
        }

        /// <summary>
        /// Gets or sets PAL compatibility.
        /// </summary>
        public FeatureCompatibility Pal
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.Pal]; }
            set { _features[FeatureCategory.Pal] = (uint)value.CoerceVideoStandardCompatibility(); }
        }

        /// <summary>
        /// Gets or sets general features.
        /// </summary>
        public GeneralFeatures GeneralFeatures
        {
            get { return (GeneralFeatures)_features[FeatureCategory.General]; }
            set { _features[FeatureCategory.General] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Keyboard Component features and compatibility.
        /// </summary>
        public KeyboardComponentFeatures KeyboardComponent
        {
            get { return (KeyboardComponentFeatures)_features[FeatureCategory.KeyboardComponent]; }
            set { _features[FeatureCategory.KeyboardComponent] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Sears Super Video Arcade compatibility.
        /// </summary>
        public FeatureCompatibility SuperVideoArcade
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.SuperVideoArcade]; }
            set { _features[FeatureCategory.SuperVideoArcade] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Intellivoice compatibility.
        /// </summary>
        public FeatureCompatibility Intellivoice
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.Intellivoice]; }
            set { _features[FeatureCategory.Intellivoice] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Intellivision II compatibility.
        /// </summary>
        public FeatureCompatibility IntellivisionII
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.IntellivisionII]; }
            set { _features[FeatureCategory.IntellivisionII] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets ECS features and compatibility.
        /// </summary>
        public EcsFeatures Ecs
        {
            get { return (EcsFeatures)_features[FeatureCategory.Ecs]; }
            set { _features[FeatureCategory.Ecs] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Tutorvision compatibility.
        /// </summary>
        public FeatureCompatibility Tutorvision
        {
            get { return (FeatureCompatibility)_features[FeatureCategory.Tutorvision]; }
            set { _features[FeatureCategory.Tutorvision] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Intellicart features and compatibility.
        /// </summary>
        public IntellicartCC3Features Intellicart
        {
            get { return (IntellicartCC3Features)_features[FeatureCategory.Intellicart]; }
            set { _features[FeatureCategory.Intellicart] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Cuttle Cart 3 features and compatibility.
        /// </summary>
        public CuttleCart3Features CuttleCart3
        {
            get { return (CuttleCart3Features)_features[FeatureCategory.CuttleCart3]; }
            set { _features[FeatureCategory.CuttleCart3] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets JLP features and compatibility.
        /// </summary>
        public JlpFeatures Jlp
        {
            get { return (JlpFeatures)_features[FeatureCategory.Jlp]; }
            set { _features[FeatureCategory.Jlp] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets the JLP hardware version.
        /// </summary>
        public JlpHardwareVersion JlpHardwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of JLP Flash save data sectors required by a program.
        /// </summary>
        public ushort JlpFlashMinimumSaveSectors
        {
            get { return Jlp.MinimumFlashSaveDataSectorsCount(); }
            set { Jlp = (Jlp & ~JlpFeaturesHelpers.FlashSaveDataSectorsCountMask) | value.MinimumFlashSaveDataSectorsCountToJlpFeatures(); }
        }

        /// <summary>
        /// Gets or sets LTO Flash features and compatibility.
        /// </summary>
        public LtoFlashFeatures LtoFlash
        {
            get { return (LtoFlashFeatures)_features[FeatureCategory.LtoFlash]; }
            set { _features[FeatureCategory.LtoFlash] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Bee3 features and compatibility.
        /// </summary>
        public Bee3Features Bee3
        {
            get { return (Bee3Features)_features[FeatureCategory.Bee3]; }
            set { _features[FeatureCategory.Bee3] = (uint)value; }
        }

        /// <summary>
        /// Gets or sets Hive features and compatibility.
        /// </summary>
        public HiveFeatures Hive
        {
            get { return (HiveFeatures)_features[FeatureCategory.Hive]; }
            set { _features[FeatureCategory.Hive] = (uint)value; }
        }

        /// <summary>
        /// Gets program features as compatibility bits for LTO Flash!
        /// </summary>
        public LuigiFeatureFlags LuigiFeaturesLo
        {
            get
            {
                var luigiFeatures = LuigiFeatureFlags.None;
                foreach (var featureFlags in _features)
                {
                    switch (featureFlags.Key)
                    {
                        case FeatureCategory.Intellivoice:
                            luigiFeatures |= Intellivoice.ToLuigiFeatureFlags(FeatureCategory.Intellivoice);
                            break;
                        case FeatureCategory.Ecs:
                            luigiFeatures |= Ecs.ToLuigiFeatureFlags();
                            break;
                        case FeatureCategory.IntellivisionII:
                            luigiFeatures |= IntellivisionII.ToLuigiFeatureFlags(FeatureCategory.IntellivisionII);
                            break;
                        case FeatureCategory.Jlp:
                            luigiFeatures |= Jlp.ToLuigiFeatureFlags();
                            break;
                        case FeatureCategory.LtoFlash:
                            luigiFeatures |= LtoFlash.ToLuigiFeatureFlags();
                            break;
                    }
                }
                return luigiFeatures;
            }
        }

        /// <summary>
        /// Gets program features as compatibility bits for LTO Flash!
        /// </summary>
        public LuigiFeatureFlags2 LuigiFeaturesHi
        {
            get
            {
                return LuigiFeatureFlags2.None;
            }
        }

        #endregion // Properties

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the operator.</param>
        /// <param name="rhs">The value on the right hand side of the operator.</param>
        /// <returns><c>true</c> if <paramref name="lhs"/> is considered to be equal to <paramref name="rhs"/>.</returns>
        public static bool operator ==(ProgramFeatures lhs, ProgramFeatures rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                areEqual = lhs.CompareTo(rhs) == 0;
            }
            return areEqual;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="lhs">The value on the left hand side of the operator.</param>
        /// <param name="rhs">The value on the right hand side of the operator.</param>
        /// <returns><c>true</c> if <paramref name="lhs"/> is considered to be NOT equal to <paramref name="rhs"/>.</returns>
        public static bool operator !=(ProgramFeatures lhs, ProgramFeatures rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Creates a set of ProgramFeatures for use with ROMs that are not recognized by the database.
        /// </summary>
        /// <returns>Program features that describe a ROM not recognized by the database.</returns>
        public static ProgramFeatures GetUnrecognizedRomFeatures()
        {
            // NTSC and PAL use 'Enhances' as 'Unknown'.
            return new ProgramFeatures(GeneralFeatures.UnrecognizedRom, FeatureCompatibility.Enhances, FeatureCompatibility.Enhances);
        }

        /// <summary>
        /// Combines two sets of ProgramFeatures into a new set of ProgramFeatures.
        /// </summary>
        /// <param name="features1">The first set of features.</param>
        /// <param name="features2">The second set of features.</param>
        /// <returns>The combined set of features.</returns>
        /// <remarks>NOTE: Only basic consistency checks are done. The default behavior is to OR the feature bits of each category
        /// together, with additional comparison against the default set of bits for the feature set. If the bits are different from
        /// each other, and one set matches the default bits while the other does not, then the non-default bits "win".
        /// If both feature sets define a JLP Hardware version, the larger of the two versions is used.</remarks>
        public static ProgramFeatures Combine(ProgramFeatures features1, ProgramFeatures features2)
        {
            ProgramFeatures combinedFeatures = null;
            if ((features1 == null) && (features2 == null))
            {
                combinedFeatures = GetUnrecognizedRomFeatures();
            }
            else if (features1 == null)
            {
                combinedFeatures = features2.Clone();
            }
            else if (features2 == null)
            {
                combinedFeatures = features1.Clone();
            }
            else
            {
                combinedFeatures = EmptyFeatures.Clone();
                foreach (var feature in features1._features.Keys)
                {
                    var defaultFeatureBits = DefaultFeatures._features[feature];
                    var features1Bits = features1._features[feature];
                    var features2Bits = features2._features[feature];
                    var features1BitsMatchDefault = features1Bits == defaultFeatureBits;
                    var features2BitsMatchDefault = features2Bits == defaultFeatureBits;
                    var featureBits = features1Bits;
                    if (features1BitsMatchDefault && !features2BitsMatchDefault)
                    {
                        featureBits = features2Bits;
                    }
                    else if (features2BitsMatchDefault && !features1BitsMatchDefault)
                    {
                        featureBits = features1Bits;
                    }
                    else if (!features1BitsMatchDefault && !features2BitsMatchDefault)
                    {
                        featureBits = features1Bits | features2Bits;
                    }
                    switch (feature)
                    {
                        case FeatureCategory.Ntsc:
                        case FeatureCategory.Pal:
                            // If we've combined "Tolerates" and "Enhances" we'll get "required", which, for video standards, is not suitable.
                            featureBits = (uint)((FeatureCompatibility)featureBits).CoerceVideoStandardCompatibility();
                            break;
                        case FeatureCategory.General:
                        case FeatureCategory.SuperVideoArcade:
                            // Leave these alone...
                            break;
                        default:
                            // For these, we don't want to have a 'tolerates' combine with 'enhances' to turn into 'requires', so, if
                            // one set contains only 'tolerates' and the other contains only 'enhances' then just keep 'enhances'.
                            var feature1Compat = features1Bits & FeatureCompatibilityHelpers.CompatibilityMask;
                            var feature2Compat = features2Bits & FeatureCompatibilityHelpers.CompatibilityMask;
                            if ((feature1Compat != 0) && (feature2Compat != 0) && ((feature1Compat ^ feature2Compat) == (uint)FeatureCompatibility.Requires))
                            {
                                // Retain 'enhances' only.
                                featureBits &= (uint)~FeatureCompatibility.Tolerates;
                            }
                            break;
                    }
                    combinedFeatures._features[feature] = featureBits;
                }
                combinedFeatures.JlpHardwareVersion = (JlpHardwareVersion)Math.Max((int)features1.JlpHardwareVersion, (int)features2.JlpHardwareVersion);
            }
            return combinedFeatures;
        }

        /// <summary>
        /// Creates a copy of the instance.
        /// </summary>
        /// <returns>A copy of the object.</returns>
        public ProgramFeatures Clone()
        {
            var programFeatures = new ProgramFeatures();
            programFeatures._features = new Dictionary<FeatureCategory, uint>(_features);
            programFeatures.JlpHardwareVersion = JlpHardwareVersion;
            return programFeatures;
        }

        /// <summary>
        /// Sets or clears feature bits for a specific category of features.
        /// </summary>
        /// <param name="category">The feature category whose feature bits are to be updated.</param>
        /// <param name="features">The bits to be set or cleared. See Remarks.</param>
        /// <param name="addFeatures">If <c>true</c>, the given features are ORed into the exiting ones (see Remarks). If <c>false</c>,
        /// the given bits will be cleared.</param>
        /// <remarks>If adding NO bits (i.e. features == 0), then all feature bits are cleared, or none, depending on the category. See implementation.</remarks>
        public void UpdateFeatureBits(FeatureCategory category, uint features, bool addFeatures)
        {
            if (addFeatures)
            {
                if (features == 0)
                {
                    switch (category)
                    {
                        case FeatureCategory.General:
                        case FeatureCategory.Jlp: // applies only to accelerator features
                        case FeatureCategory.LtoFlash: // applies only to accelerator features
                        case FeatureCategory.Bee3: // applies only to accelerator features
                        case FeatureCategory.Hive: // applies only to accelerator features
                            break;
                        default:
                            // A value of zero means INCOMPATIBLE WITH ALL FEATURES, so clear ALL the bits in such a case.
                            _features[category] = 0;
                            break;
                    }
                }
                else
                {
                    _features[category] |= features;
                }
            }
            else
            {
                _features[category] &= ~features;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is IProgramFeatures))
            {
                return false;
            }
            return CompareTo((IProgramFeatures)obj) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 0;
            foreach (var feature in _features)
            {
                hashCode = CombineHashCodes(hashCode, feature.Key.GetHashCode());
                hashCode = CombineHashCodes(hashCode, feature.Value.GetHashCode());
            }
            return hashCode;
        }

        #region IComparable

        /// <inheritdoc />
        /// <exception cref="System.ArgumentException">Thrown if <param name="obj"/> is not a <see cref="IProgramFeatures"/>.</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is IProgramFeatures))
            {
                throw new ArgumentException();
            }
            return CompareTo((IProgramFeatures)obj);
        }

        #endregion // IComparable

        #region IComparable<IProgramFeatures>

        /// <inheritdoc />
        public int CompareTo(IProgramFeatures other)
        {
            return CompareTo(other as ProgramFeatures);
        }

        #endregion // IComparable<IProgramFeatures>

        #region IComparable<ProgramFeatures>

        /// <inheritdoc />
        public int CompareTo(ProgramFeatures other)
        {
            var result = 1;
            if (other != null)
            {
                result = 0;
                if (!ReferenceEquals(this, other))
                {
                    result = (int)JlpHardwareVersion - (int)other.JlpHardwareVersion;
                    if (result == 0)
                    {
                        foreach (var feature in _features)
                        {
                            var myFeatureBits = feature.Value;
                            var otherFeatureBits = other._features[feature.Key];
                            if (myFeatureBits > otherFeatureBits)
                            {
                                result = 1;
                                break;
                            }
                            else if (myFeatureBits < otherFeatureBits)
                            {
                                result = -1;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion // IComparable<ProgramFeatures>

        #region IEquatable<IProgramFeatures>

        /// <inheritdoc />
        public bool Equals(IProgramFeatures other)
        {
            return CompareTo(other) == 0;
        }

        #endregion // IEquatable<IProgramFeatures>

        #region IEquatable<ProgramFeatures>

        /// <inheritdoc />
        public bool Equals(ProgramFeatures other)
        {
            return CompareTo(other) == 0;
        }

        #endregion // IEquatable<ProgramFeatures>

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }
    }
}
