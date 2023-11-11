using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalFilterOptions.Patches
{
    internal class SongFilterData
    {
        public string SongId { get; set; }
        public string SongTitle { get; set; }
        public string SongSubtitle { get; set; }
        public int GenreNo { get; set; }
        public int Order { get; set; }
        public EnsoData.EnsoLevelType Difficulty { get; set; }
        public int Star { get; set; }
        public DataConst.CrownType Crown { get; set; }
        SongSelectManager.Score HighScore { get; set; }
        float acc = -1;
        public float Accuracy
        {
            get
            {
                if (acc == -1)
                {
                    acc = CalculateAccuracy(HighScore);
                }
                return acc;
            }
        }

        public SongFilterData(SongSelectManager.Song song, EnsoData.EnsoLevelType levelType)
        {
            SongId = song.Id;
            SongTitle = song.TitleText;
            SongSubtitle = song.SubText;
            GenreNo = song.SongGenre;
            Order = song.Order;
            Difficulty = levelType;
            Star = song.Stars[(int)levelType];
            Crown = song.HighScores[(int)levelType].crown;
            HighScore = song.HighScores[(int)levelType];
        }

        float CalculateAccuracy(SongSelectManager.Score score)
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
