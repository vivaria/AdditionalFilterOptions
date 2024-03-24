using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class PlaylistSongData
    {
        public string SongId { get; set; }
        public int GenreNo { get; set; }
        public bool IsDlc { get; set; } = false;

        public PlaylistSongData(LWJson node)
        {
            SongId = node["songId"].AsString();
            GenreNo = node["genreNo"].AsInteger();
            try
            {
                IsDlc = node["isDlc"].AsBoolean();
            }
            catch
            {
                IsDlc = false;
            }
        }
    }
}
