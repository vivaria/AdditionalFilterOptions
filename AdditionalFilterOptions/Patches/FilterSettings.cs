using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class FilterSettings
    {
        public PlaylistData PlaylistData { get; set; }
        public Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties { get; set; } = new Dictionary<EnsoData.EnsoLevelType, bool>();
        public Dictionary<EnsoData.SongGenre, bool> EnabledGenres { get; set; } = new Dictionary<EnsoData.SongGenre, bool>();
        public Dictionary<DataConst.CrownType, bool> EnabledCrowns { get; set; } = new Dictionary<DataConst.CrownType, bool>();
        public int MinDifficulty { get; set; }
        public int MaxDifficulty { get; set; }
        public string TextFilter { get; set; }
    }
}
