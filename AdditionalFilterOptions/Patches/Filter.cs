using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdditionalFilterOptions.Patches
{
    internal class Filter
    {
        static public List<SongSelectManager.Song> FilterGenres(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.SongGenre, bool> EnabledGenres)
        {
            var list = new List<SongSelectManager.Song>();
            foreach (SongSelectManager.Song s in SongList)
            {
                // Check that the key exists in the dictionary
                // And check that the value for the key is true (enabled)
                if (EnabledGenres.ContainsKey((EnsoData.SongGenre)s.SongGenre) && EnabledGenres[(EnsoData.SongGenre)s.SongGenre])
                {
                    list.Add(s);
                }
            }
            return list;
        }

        static public List<SongSelectManager.Song> FilterDifficulty(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties,
                                                                    int MinDifficulty, int MaxDifficulty)
        {
            var list = new List<SongSelectManager.Song>();
            foreach (SongSelectManager.Song s in SongList)
            {
                for (int i = 0; i < s.Stars.Length; i++)
                {
                    EnsoData.EnsoLevelType diff = (EnsoData.EnsoLevelType)i;
                    // Check that the difficulty isn't 0 (meaning it doesn't exist)
                    // And that the key exists
                    // And that the key is enabled
                    if (s.Stars[i] != 0 && EnabledDifficulties.ContainsKey(diff) && EnabledDifficulties[diff])
                    {
                        if (s.Stars[i] >= MinDifficulty && s.Stars[i] <= MaxDifficulty)
                        {
                            list.Add(s);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        static public List<SongFilterData> FilterDifficulty(List<SongFilterData> SongList, int MinDifficulty, int MaxDifficulty)
        {
            var list = new List<SongFilterData>();
            foreach (SongFilterData s in SongList)
            {
                if (s.Star != 0)
                {
                    if (s.Star >= MinDifficulty && s.Star <= MaxDifficulty)
                    {
                        list.Add(s);
                    }
                }
            }
            return list;
        }

        static public List<SongSelectManager.Song> FilterCrowns(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties,
                                                                Dictionary<DataConst.CrownType, bool> EnabledCrowns)
        {
            var list = new List<SongSelectManager.Song>();
            foreach (SongSelectManager.Song s in SongList)
            {
                for (int i = 0; i < s.Stars.Length; i++)
                {
                    EnsoData.EnsoLevelType diff = (EnsoData.EnsoLevelType)i;
                    if (s.Stars[i] != 0 && EnabledDifficulties.ContainsKey(diff) && EnabledDifficulties[diff])
                    {
                        var crown = s.HighScores[i].crown;
                        if (EnabledCrowns.ContainsKey(crown) && EnabledCrowns[crown])
                        {
                            list.Add(s);
                            break;
                        }
                    }
                }
            }
            return list;
        }
        static public List<SongFilterData> FilterCrowns(List<SongFilterData> SongList, Dictionary<DataConst.CrownType, bool> EnabledCrowns)
        {
            var list = new List<SongFilterData>();
            foreach (SongFilterData s in SongList)
            {
                if (s.Star != 0)
                {
                    if (EnabledCrowns.ContainsKey(s.Crown) && EnabledCrowns[s.Crown])
                    {
                        list.Add(s);
                    }
                }
            }
            return list;
        }

        public static List<SongFilterData> FilterBonus(List<SongFilterData> SongList, bool IsBonus)
        {
            if (!IsBonus)
            {
                return SongList;
            }
            var list = new List<SongFilterData>();
            foreach (var song in SongList)
            {
                if (song.IsBonus)
                {
                    list.Add(song);
                }
            }
            return list;
        }

        public static List<SongFilterData> FilterFavorite(List<SongFilterData> SongList, bool IsFavorite)
        {
            if (!IsFavorite)
            {
                return SongList;
            }
            var list = new List<SongFilterData>();
            foreach (var song in SongList)
            {
                if (song.IsFavorite)
                {
                    list.Add(song);
                }
            }
            return list;
        }

        static WordDataInterface JpWordData;
        static WordDataInterface EngWordData;

        static public List<SongSelectManager.Song> FilterText(List<SongSelectManager.Song> SongList, string text)
        {
            if (text == null)
            {
                return new List<SongSelectManager.Song>(SongList);
            }
            text = text.ToLower();
            if (JpWordData == null)
            {
                JpWordData = new WordDataInterface(Application.streamingAssetsPath + "/ReadAssets/newwordlist.bin", "Japanese");
            }
            if (EngWordData == null)
            {
                EngWordData = new WordDataInterface(Application.streamingAssetsPath + "/ReadAssets/newwordlist.bin", "English");
            }

            var list = new List<SongSelectManager.Song>();
            foreach (SongSelectManager.Song s in SongList)
            {
                List<string> songStrings = new List<string>();
                //// Check JP Strings
                //songStrings.Add(GetWordlistTextFromKey(JpWordData, "song_" + s.Id));
                //songStrings.Add(GetWordlistTextFromKey(JpWordData, "song_sub_" + s.Id));
                //songStrings.Add(GetWordlistTextFromKey(JpWordData, "song_detail_" + s.Id));
                //// Check Eng Strings
                //songStrings.Add(GetWordlistTextFromKey(EngWordData, "song_" + s.Id));
                //songStrings.Add(GetWordlistTextFromKey(EngWordData, "song_sub_" + s.Id));
                //songStrings.Add(GetWordlistTextFromKey(EngWordData, "song_detail_" + s.Id));
                // Check current language Strings
                songStrings.Add(s.TitleText);
                songStrings.Add(s.SubText);
                songStrings.Add(s.DetailText);
                songStrings.Add(s.Id);

                for (int i = 0; i < songStrings.Count; i++)
                {
                    if (songStrings[i] == "" || songStrings[i] == null || songStrings[i] == string.Empty)
                    {
                        continue;
                    }
                    if (songStrings[i].ToLower().Contains(text))
                    {
                        list.Add(s);
                        break;
                    }
                }
            }
            return list;
        }
        static string GetWordlistTextFromKey(WordDataInterface wordData, string key)
        {
            for (int i = 0; i < wordData.wordListInfoAccessers.Count; i++)
            {
                if (wordData.wordListInfoAccessers[i].Key == key)
                {
                    return wordData.wordListInfoAccessers[i].Text;
                }
            }
            return "";
        }
    }
}
