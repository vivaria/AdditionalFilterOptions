using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    public enum SortTypes
    {
        Default,
        Genre,
        Difficulty,
        Alphabetical,
        Accuracy,
    }

    internal class Sort
    {
        static public List<SongSelectManager.Song> SortSongList(List<SongSelectManager.Song> SongList, List<SortTypes> SortList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            if (SortList.Count == 0)
            {
                return SongList;
            }
            IOrderedEnumerable<SongSelectManager.Song> OrderedList = null;
            switch (SortList[0])
            {
                case SortTypes.Default:
                    OrderedList = SortByDefault(SongList);
                    break;
                case SortTypes.Genre:
                    OrderedList = SortByGenre(SongList);
                    break;
                case SortTypes.Difficulty:
                    OrderedList = SortByDifficulty(SongList, EnabledDifficulties);
                    break;
                case SortTypes.Alphabetical:
                    OrderedList = SortByAlphabetical(SongList);
                    break;
                case SortTypes.Accuracy:
                    OrderedList = SortByAccuracy(SongList, EnabledDifficulties);
                    break;
            }
            for (int i = 1; i < SortList.Count; i++)
            {
                switch (SortList[i])
                {
                    case SortTypes.Default:
                        OrderedList = SortByDefault(OrderedList);
                        break;
                    case SortTypes.Genre:
                        OrderedList = SortByGenre(OrderedList);
                        break;
                    case SortTypes.Difficulty:
                        OrderedList = SortByDifficulty(OrderedList, EnabledDifficulties);
                        break;
                    case SortTypes.Alphabetical:
                        OrderedList = SortByAlphabetical(OrderedList);
                        break;
                    case SortTypes.Accuracy:
                        OrderedList = SortByAccuracy(OrderedList, EnabledDifficulties);
                        break;
                }
            }
            if (OrderedList == null)
            {
                return SongList;
            }
            return OrderedList.ToList();
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDefault(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.SongGenre)
                           .ThenBy((x) => x.Order)
                           .ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDefault(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.SongGenre)
                           .ThenBy((x) => x.Order)
                           .ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByGenre(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.SongGenre);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByGenre(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.SongGenre);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDifficulty(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            return SongList.OrderBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? x.Stars[0] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? x.Stars[1] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? x.Stars[2] : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? x.Stars[3] : 0,
                                                    EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? x.Stars[4] : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByDifficulty(IOrderedEnumerable<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            return SongList.ThenBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? x.Stars[0] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? x.Stars[1] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? x.Stars[2] : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? x.Stars[3] : 0,
                                                   EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? x.Stars[4] : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabetical(List<SongSelectManager.Song> SongList)
        {
            return SongList.OrderBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAlphabetical(IOrderedEnumerable<SongSelectManager.Song> SongList)
        {
            return SongList.ThenBy((x) => x.TitleText);
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAccuracy(List<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            // Actually sorting by accuracy is sorta wrong right now
            // If it's filtered by difficulty, then by accuracy
            // It will take the max difficuly value, and then the max accuracy value, even if it may be from a different difficulty
            // It's fine enough to start with I guess
            return SongList.OrderBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? CalculateAccuracy(x.HighScores[0]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? CalculateAccuracy(x.HighScores[1]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? CalculateAccuracy(x.HighScores[2]) : 0,
                                           Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? CalculateAccuracy(x.HighScores[3]) : 0,
                                                    EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? CalculateAccuracy(x.HighScores[4]) : 0)))));
        }

        static IOrderedEnumerable<SongSelectManager.Song> SortByAccuracy(IOrderedEnumerable<SongSelectManager.Song> SongList, Dictionary<EnsoData.EnsoLevelType, bool> EnabledDifficulties)
        {
            // This is ugly, but it might actually be accurate
            return SongList.ThenBy((x) => Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)0] ? CalculateAccuracy(x.HighScores[0]) : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)1] ? CalculateAccuracy(x.HighScores[1]) : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)2] ? CalculateAccuracy(x.HighScores[2]) : 0,
                                          Math.Max(EnabledDifficulties[(EnsoData.EnsoLevelType)3] ? CalculateAccuracy(x.HighScores[3]) : 0,
                                                   EnabledDifficulties[(EnsoData.EnsoLevelType)4] ? CalculateAccuracy(x.HighScores[4]) : 0)))));
        }

        static float CalculateAccuracy(SongSelectManager.Score score)
        {
            var record = score.hiScoreRecordInfos;
            if ((record.excellent + record.good + record.bad) == 0)
            {
                return 0.0f;
            }
            else
            {
                return ((record.excellent + (record.good / 2.0f)) / (record.excellent + record.good + record.bad)) * 100;
            }
        }
    }
}
