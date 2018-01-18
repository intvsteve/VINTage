// <copyright file="CfgVarMetadataFeatureCompatibility.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// A simple class for compatibility metadata from a .CFG file.
    /// </summary>
    public class CfgVarMetadataFeatureCompatibility : CfgVarMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.CfgVarMetadataFeatureCompatibility"/> class.
        /// </summary>
        /// <param name="type">The specific kind of compatibility metadata.</param>
        public CfgVarMetadataFeatureCompatibility(CfgVarMetadataIdTag type)
            : base(type)
        {
            Compatibility = FeatureCompatibility.Tolerates;
        }

        /// <summary>
        /// Gets the compatibility.
        /// </summary>
        public FeatureCompatibility Compatibility { get; private set; }

        /// <inheritdoc/>
        protected override void Parse(string payload)
        {
            var compatibilityString = GetCleanPayloadString(payload);
            if (!string.IsNullOrEmpty(payload))
            {
                byte compatibility;
                if (byte.TryParse(compatibilityString, out compatibility))
                {
                    switch (Type)
                    {
                        case CfgVarMetadataIdTag.EcsCompatibility:
                        case CfgVarMetadataIdTag.IntellivisionIICompatibility:
                        case CfgVarMetadataIdTag.IntellivoiceCompatibility:
                        case CfgVarMetadataIdTag.KeyboardComponentCompatibility:
                        case CfgVarMetadataIdTag.TutorvisionCompatibility:
                        case CfgVarMetadataIdTag.Jlp:
                        case CfgVarMetadataIdTag.JlpAccelerators:
                            if (compatibility < (byte)FeatureCompatibility.NumCompatibilityModes)
                            {
                                Compatibility = (FeatureCompatibility)compatibility;
                            }
                            break;
                        case CfgVarMetadataIdTag.Ecs:
                            if (compatibility == 0)
                            {
                                Compatibility = FeatureCompatibility.Tolerates;
                            }
                            else if (compatibility == 1)
                            {
                                Compatibility = FeatureCompatibility.Requires;
                            }
                            break;
                        case CfgVarMetadataIdTag.IntellivisionII:
                            if (compatibility == 0)
                            {
                                Compatibility = FeatureCompatibility.Incompatible;
                            }
                            else if (compatibility == 1)
                            {
                                Compatibility = FeatureCompatibility.Tolerates;
                            }
                            break;
                        case CfgVarMetadataIdTag.Voice:
                            if (compatibility == 0)
                            {
                                Compatibility = FeatureCompatibility.Tolerates;
                            }
                            else if (compatibility == 1)
                            {
                                Compatibility = FeatureCompatibility.Enhances;
                            }
                            break;
                        default:
                            throw new System.InvalidOperationException(string.Format(Resources.Strings.CfgVarMetadata_TypeError, Type));
                    }
                }
            }
        }
    }
}
