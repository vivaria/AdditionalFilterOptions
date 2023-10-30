using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    enum SortOptions
    {
        Default,
        Difficulty,
        Accuracy,
        AlphabeticalTitle,
        AlphabeticalSubtitle
    }
    internal class SortSettings
    {
        public SortOptions PrimarySort { get; set; } = SortOptions.Default;
        public SortOptions SecondarySort { get; set; } = SortOptions.Default;
    }
}
