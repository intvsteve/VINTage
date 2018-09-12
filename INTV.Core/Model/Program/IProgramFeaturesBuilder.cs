// <copyright file="IProgramFeaturesBuilder.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// This interface specifies a builder pattern for creating an instance of <see cref="IProgramFeatures"/>.
    /// </summary>
    public interface IProgramFeaturesBuilder
    {
        /// <summary>
        /// Adds general features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="generalFeatures">The general features to add.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithGeneralFeatures(GeneralFeatures generalFeatures);

        /// <summary>
        /// Adds NTSC compatibility to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="ntscCompatibility">The NSTC compatibility.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithNtscCompatibility(FeatureCompatibility ntscCompatibility);

        /// <summary>
        /// Adds PAl compatibility to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="palCompatibility">The PAL compatibility.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithPalCompatibility(FeatureCompatibility palCompatibility);

        /// <summary>
        /// Adds Keyboard Component features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="keyboardComponentFeatures">The Keyboard Component features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithKeyboardComponentFeatures(KeyboardComponentFeatures keyboardComponentFeatures);

        /// <summary>
        /// Adds Sears Super Video Arcade behavior to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="superVideoArcadeCompatibility">The Sears Super Video Arcade behavior.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithSuperVideoArcadeCompatibility(FeatureCompatibility superVideoArcadeCompatibility);

        /// <summary>
        /// Adds Intellivoice compatibility to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="intellivoiceCompatibility">The Intellivoice compatibility.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithIntellivoiceCompatibility(FeatureCompatibility intellivoiceCompatibility);

        /// <summary>
        /// Adds Intellivision II compatibility to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="intellivisionIICompatibility">The Intellivision II compatibility.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithIntellivisionIICompatibility(FeatureCompatibility intellivisionIICompatibility);

        /// <summary>
        /// Adds ECS features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="ecsFeatures">The ECS features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithEcsFeatures(EcsFeatures ecsFeatures);

        /// <summary>
        /// Adds Tutorvision compatibility to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="tutorvisionCompatibility">The Tutorvision compatibility.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithTutorvisionCompatibility(FeatureCompatibility tutorvisionCompatibility);

        /// <summary>
        /// Adds Intellicart features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="intellicartFeatures">The Intellicart features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithIntellicartFeatures(IntellicartCC3Features intellicartFeatures);

        /// <summary>
        /// Adds CuttleCart 3 features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="cuttleCart3Features">The CuttleCart 3 features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithCuttleCart3Features(CuttleCart3Features cuttleCart3Features);

        /// <summary>
        /// Adds JLP features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="jlpFeatures">The JLP features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithJlpFeatures(JlpFeatures jlpFeatures);

        /// <summary>
        /// Adds minimum flash storage sectors to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="minimumFlashSectors">The minimum number of flash sectors used for storage by the program.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithMinimumFlashSectors(ushort minimumFlashSectors);

        /// <summary>
        /// Adds JLP hardware version to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="jlpHardwareVersion">The JLP hardware version.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithJlpHardwareVersion(JlpHardwareVersion jlpHardwareVersion);

        /// <summary>
        /// Adds LTO Flash! features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="ltoFlashFeatures">The LTO Flash! features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithLtoFlashFeatures(LtoFlashFeatures ltoFlashFeatures);

        /// <summary>
        /// Adds Bee3 features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="bee3Features">The Bee3 features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithBee3Features(Bee3Features bee3Features);

        /// <summary>
        /// Adds Hive features to the instance of <see cref="IProgramFeatures"/> being built.
        /// </summary>
        /// <param name="hiveFeatures">The Hive features.</param>
        /// <returns>The builder.</returns>
        IProgramFeaturesBuilder WithHiveFeatures(HiveFeatures hiveFeatures);

        /// <summary>
        /// Creates the concrete instance of the program features.
        /// </summary>
        /// <returns>The <see cref="IProgramFeatures"/> defined by the builder.</returns>
        IProgramFeatures Build();
    }
}
