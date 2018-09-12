// <copyright file="ProgramFeaturesBuilder.cs" company="INTV Funhouse">
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
    public class ProgramFeaturesBuilder : IProgramFeaturesBuilder
    {
        private ProgramFeatures _programFeatures;

        public ProgramFeaturesBuilder()
        {
            _programFeatures = ProgramFeatures.DefaultFeatures.Clone();
        }

        public IProgramFeaturesBuilder WithInitialFeatures(IProgramFeatures initialFeatures)
        {
            if (initialFeatures != null)
            {
                _programFeatures = ProgramFeatures.EmptyFeatures.Clone();
                WithGeneralFeatures(initialFeatures.GeneralFeatures);
                WithNtscCompatibility(initialFeatures.Ntsc);
                WithPalCompatibility(initialFeatures.Pal);
                WithKeyboardComponentFeatures(initialFeatures.KeyboardComponent);
                WithSuperVideoArcadeCompatibility(initialFeatures.SuperVideoArcade);
                WithIntellivoiceCompatibility(initialFeatures.Intellivoice);
                WithIntellivisionIICompatibility(initialFeatures.IntellivisionII);
                WithEcsFeatures(initialFeatures.Ecs);
                WithTutorvisionCompatibility(initialFeatures.Tutorvision);
                WithIntellicartFeatures(initialFeatures.Intellicart);
                WithCuttleCart3Features(initialFeatures.CuttleCart3);
                WithJlpFeatures(initialFeatures.Jlp);
                WithMinimumFlashSectors(initialFeatures.JlpFlashMinimumSaveSectors);
                WithJlpHardwareVersion(initialFeatures.JlpHardwareVersion);
                WithLtoFlashFeatures(initialFeatures.LtoFlash);
                WithBee3Features(initialFeatures.Bee3);
                WithHiveFeatures(initialFeatures.Hive);
            }
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithGeneralFeatures(GeneralFeatures generalFeatures)
        {
            _programFeatures.GeneralFeatures = generalFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithNtscCompatibility(FeatureCompatibility ntscCompatibility)
        {
            _programFeatures.Ntsc = ntscCompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithPalCompatibility(FeatureCompatibility palCompatibility)
        {
            _programFeatures.Pal = palCompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithKeyboardComponentFeatures(KeyboardComponentFeatures keyboardComponentFeatures)
        {
            _programFeatures.KeyboardComponent = keyboardComponentFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithSuperVideoArcadeCompatibility(FeatureCompatibility superVideoArcadeCompatibility)
        {
            _programFeatures.SuperVideoArcade = superVideoArcadeCompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithIntellivoiceCompatibility(FeatureCompatibility intellivoiceCompatibility)
        {
            _programFeatures.Intellivoice = intellivoiceCompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithIntellivisionIICompatibility(FeatureCompatibility intellivisionIICompatibility)
        {
            _programFeatures.IntellivisionII = intellivisionIICompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithEcsFeatures(EcsFeatures ecsFeatures)
        {
            _programFeatures.Ecs = ecsFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithTutorvisionCompatibility(FeatureCompatibility tutorvisionCompatibility)
        {
            _programFeatures.Tutorvision = tutorvisionCompatibility;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithIntellicartFeatures(IntellicartCC3Features intellicartFeatures)
        {
            _programFeatures.Intellicart = intellicartFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithCuttleCart3Features(CuttleCart3Features cuttleCart3Features)
        {
            _programFeatures.CuttleCart3 = cuttleCart3Features;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithJlpFeatures(JlpFeatures jlpFeatures)
        {
            _programFeatures.Jlp = jlpFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithMinimumFlashSectors(ushort minimumFlashSectors)
        {
            _programFeatures.JlpFlashMinimumSaveSectors = minimumFlashSectors;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithJlpHardwareVersion(JlpHardwareVersion jlpHardwareVersion)
        {
            _programFeatures.JlpHardwareVersion = jlpHardwareVersion;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithLtoFlashFeatures(LtoFlashFeatures ltoFlashFeatures)
        {
            _programFeatures.LtoFlash = ltoFlashFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithBee3Features(Bee3Features bee3Features)
        {
            _programFeatures.Bee3 = bee3Features;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeaturesBuilder WithHiveFeatures(HiveFeatures hiveFeatures)
        {
            _programFeatures.Hive = hiveFeatures;
            return this;
        }

        /// <inheritdoc />
        public IProgramFeatures Build()
        {
            return _programFeatures;
        }
    }
}
