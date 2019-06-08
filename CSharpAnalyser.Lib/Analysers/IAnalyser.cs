using System;
using System.Collections.Generic;
using CSharpAnalyser.Lib.Models;

namespace CSharpAnalyser.Lib.Analysers
{
    public interface IAnalyser
    {
        IReadOnlyCollection<AnalyserItem> AnalyserItems { get; }
    }
}
