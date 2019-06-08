// <copyright file="STATheoryDiscoverer.cs" company="N/A">
// Copyright (c) 2015 All Rights Reserved
// <author>AArnott</author>
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace INTV.TestHelpers.Shared.Xunit.Sdk
{
    /// <summary>
    /// Discoverer for the single-thread apartment theory attribute (STATheorAttribute).
    /// </summary>
    /// <remarks>Barely modified version of AArnott's version here:
    /// https://github.com/xunit/samples.xunit/blob/master/STAExamples/WpfTheoryDiscoverer.cs
    /// </remarks>
    public class STATheoryDiscoverer : STADiscoverer
    {
        private readonly TheoryDiscoverer _theoryDiscoverer;

        /// <summary>
        /// Initializes a new instance of <see cref="STATheoryDiscoverer"/>.
        /// </summary>
        /// <param name="diagnosticMessageSink">The diagnostic message sink for the discoverer.</param>
        public STATheoryDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _theoryDiscoverer = new TheoryDiscoverer(diagnosticMessageSink);
        }

        /// <inheritdoc />
        protected override IXunitTestCaseDiscoverer WrappedDiscoverer
        {
            get { return _theoryDiscoverer; }
        }
    }
}
