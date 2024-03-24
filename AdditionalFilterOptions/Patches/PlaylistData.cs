using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (jsonFilePath == "")
            {
                Name = "None";
                JsonFilePath = "";
                return;
            }
            FileInfo file = new FileInfo(jsonFilePath);
            if (file.Exists)
            {
                LWJson node = LWJson.Parse(File.ReadAllText(jsonFilePath));
                JsonFilePath = jsonFilePath.Remove(0, Plugin.Instance.ConfigPlaylistLocation.Value.Length + 1);
                InitializeData(node);
            }
            else
            {
                Name = "None";
                JsonFilePath = "";
            }
        }

        private void InitializeData(LWJson node)
        {
            Name = node["playlistName"].AsString();
            Order = node["order"].AsInteger();
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
