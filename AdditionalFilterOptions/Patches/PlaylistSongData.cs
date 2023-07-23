using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class PlaylistSongData
    {
        public string SongId { get; set; }
        public int GenreNo { get; set; }
        public bool isDlc { get; set; } = false;

        public PlaylistSongData(JsonNode node)
        {
            SongId = node["songId"].GetValue<string>();
            GenreNo = node["genreNo"].GetValue<int>();
            if (node["isDlc"] != null)
            {
                isDlc = node["isDlc"].GetValue<bool>();
            }
        }
    }
}
