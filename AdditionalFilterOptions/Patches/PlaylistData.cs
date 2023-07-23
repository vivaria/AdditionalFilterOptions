using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class PlaylistData
    {
        public string Name { get; set; }
        public string JsonFilePath { get; set; }
        public int Order { get; set; }
        public List<PlaylistSongData> Songs { get; set; }

        public PlaylistData(string jsonFilePath)
        {
            JsonNode node = JsonNode.Parse(File.ReadAllText(jsonFilePath));
            JsonFilePath = jsonFilePath.Remove(0, Plugin.Instance.ConfigPlaylistLocation.Value.Length + 1);
            InitializeData(node);
        }

        private void InitializeData(JsonNode node)
        {
            Name = node["playlistName"].GetValue<string>();
            Order = node["order"].GetValue<int>();
            Songs = new List<PlaylistSongData>();
            var songsArray = node["songs"].AsArray();
            for (int i = 0; i < songsArray.Count; i++)
            {
                var song = new PlaylistSongData(songsArray[i]);
                Songs.Add(song);
            }
        }
    }
}
