using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterMenu : MonoBehaviour
    {
        const string FilterParentName = "AdditionalFilterMenuParent";

        GameObject parent;

        SongSelectManager songSelectManager { get; set; }
        static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();


        CycleButton playlistButton;
        CycleButton sortingButton;

        void Start()
        {
            parent = GameObject.Find(FilterParentName);
            if (parent == null)
            {
                parent = new GameObject(FilterParentName);
                parent.layer = 5;
            }

            transform.SetParent(parent.transform);

            Canvas parentCanvas = parent.GetComponent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = parent.AddComponent<Canvas>();
                parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                parentCanvas.worldCamera = null;
                parentCanvas.overrideSorting = true;
                parentCanvas.sortingOrder = 2;
                

                var parentCanvasScaler = parent.AddComponent<CanvasScaler>();
                parentCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                parentCanvasScaler.referenceResolution = new Vector2(1920, 1080);
                parentCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                parentCanvasScaler.matchWidthOrHeight = 0;
            }

            FullSongList = new List<SongSelectManager.Song>();
            Plugin.Log.LogInfo("AdditionalFilterMenu Created");

            SetFilterMenuActive(false);
            InitializeUI();
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                if (HasFilterChanged())
                {
                    FilterSongList();
                }
            }
        }

        private bool HasFilterChanged()
        {
            // Only set hasChanged to true, but look through everything to find changes
            bool hasChanged = false;
            if (playlistButton != null)
            {
                if (playlistButton.HasChanged())
                {
                    hasChanged = true;
                }
            }

            return hasChanged;
        }

        public void SetFilterMenuActive(bool isActive)
        {
            parent.SetActive(isActive);
        }

        private void InitializeUI()
        {
            // What elements are planned for this mod?

            // A dropdown for selecting a playlist
            //  - Also some way to create a new platlist in-game
            //  - This would include:
            //     - Naming the playlist
            //     - Reordering the playlists
            //     - Adding songs to the playlist
            //     - Reordering the songs in the playlist

            // A slider for Min/Max difficulty (I'd like for this to be sorta 2 sliders in 1 if possible
            // A bunch of checkboxes for Crown (None, Silver, Gold, Rainbow)
            //  - This could probably also be a slider, assuming people would probably be wanting None-Silver, or None-Gold, and not something like None AND Rainbow
            // A bunch of checkboxes for Genre
            //  - This can't be a slider, so shouldn't be a slider
            // A text field for Song Search

            // Maybe a slider for min/max accuracy?

            // Maybe a section for sorting?
            // This could include stuff like:
            // Alphabetical (Title) sorting
            // Alphabetical (Subtitle) sorting
            // Difficulty (Max of Oni/Ura) sorting
            // Accuracy sorting
            // Accuracy then Difficulty sorting
            // It'd be neat to be able to sort by number of plays, but I don't think those stats are saved in TakoTako

            UnityObjectUtility.CreateImage("FilterBackground", new Color32(0, 0, 0, (int)(255 * 0.698f)), new Rect(0, 0, 1920, 1080), parent.transform);

            DirectoryInfo playlistDirInfo = new DirectoryInfo(Plugin.Instance.ConfigPlaylistLocation.Value);
            var playlistFiles = playlistDirInfo.GetFiles("*.json", SearchOption.AllDirectories).ToList();

            List<PlaylistData> playlistDataObjects = new List<PlaylistData>();
            for (int i = 0; i < playlistFiles.Count; i++)
            {
                var playlistDataObject = new PlaylistData(playlistFiles[i].FullName);
                playlistDataObjects.Add(playlistDataObject);
            }

            playlistDataObjects.Sort((x, y) => x.Order > y.Order ? 1 : -1);


            List<string> playlistOptions = new List<string>() { "None" };
            List<string> playlistData = new List<string>() { "" };
            for (int i = 0; i < playlistDataObjects.Count; i++)
            {
                playlistOptions.Add(playlistDataObjects[i].Name);
                playlistData.Add(playlistDataObjects[i].JsonFilePath);
            }


            var playlistObject = UnityObjectUtility.CreateCycleButton("PlaylistCycle", playlistOptions, playlistData, new Rect(50, 400, 500, 50), parent.transform);
            playlistButton = playlistObject.GetComponent<CycleButton>();


            List<string> sortingOptions = new List<string>()
            {
                "Default",
                "Difficulty",
                "Accuracy",
                "Difficulty -> Accuracy",
                "Alphabetical (Title)",
                "Alphabetical (Subtitle)",
            };

            List<string> sortingData = new List<string>()
            {

            };

            var sortingObject = UnityObjectUtility.CreateCycleButton("SortingCycle", sortingOptions, sortingData, new Rect(50, 325, 500, 50), parent.transform);
            sortingButton = playlistObject.GetComponent<CycleButton>();


            //UnityObjectUtility.CreateDropdown("PlaylistDropdown", dropdownOptions, new Rect(50, 400, 500, 200), parent.transform);
        }

        public void AssignSongSelectManager(SongSelectManager instance)
        {
            songSelectManager = instance;
        }

        public void InitializeFullSongList(List<SongSelectManager.Song> songList)
        {
            FullSongList = songList;
            Plugin.Log.LogInfo("AdditionalFilterMenu InitializedSongList");
            FilterSongList();
        }

        private void FilterSongList()
        {
            int minDifficulty = 1;
            int maxDifficulty = 10;

            List<SongSelectManager.Song> filteredSongList = LoadPlaylist();


            for (int i = filteredSongList.Count - 1; i >= 0; i--)
            {
                bool toRemove = true;
                var stars = filteredSongList[i].Stars;
                for (int j = 0; j < stars.Length; j++)
                {
                    if (stars[j] >= minDifficulty && stars[j] <= maxDifficulty)
                    {
                        toRemove = false;
                        break;
                    }
                }

                if (toRemove)
                {
                    filteredSongList.RemoveAt(i);
                }

            }

            UpdateSongList(filteredSongList);

        }

        List<SongSelectManager.Song> LoadPlaylist()
        {
            if (playlistButton == null || playlistButton.GetCurrentData() == "")
            {
                return FullSongList;
            }

            PlaylistData songData = new PlaylistData(Path.Combine(Plugin.Instance.ConfigPlaylistLocation.Value, playlistButton.GetCurrentData()));

            List<SongSelectManager.Song> result = new List<SongSelectManager.Song>();

            for (int i = 0; i < songData.Songs.Count; i++)
            {
                for (int j = 0; j < FullSongList.Count; j++)
                {
                    if (FullSongList[j].Id == songData.Songs[i].SongId)
                    {
                        SongSelectManager.Song song = new SongSelectManager.Song(FullSongList[j]);
                        song.TitleKey = "song_" + songData.Songs[i].SongId;
                        song.SubKey = "song_sub_" + songData.Songs[i].SongId;
                        song.RubyKey = "song_detail_" + songData.Songs[i].SongId;
                        // Changing Genres is a little buggy, but mostly works
                        // Once you move the song list once, it works just fine
                        song.SongGenre = songData.Songs[i].GenreNo;

                        // ListGenre is not used anywhere it seems
                        //FullSongList[j].ListGenre = songData.Songs[i].GenreNo;
                        song.DLC = songData.Songs[i].isDlc;
                        result.Add(song);
                        break;
                    }
                }
            }

            return result;
        }

        void UpdateSongList(List<SongSelectManager.Song> newList)
        {
            var prevSongId = FullSongList[songSelectManager.SelectedSongIndex].Id;

            if (newList.Count == 0)
            {
                songSelectManager.SongList = new List<SongSelectManager.Song>(FullSongList);
            }
            else
            {
                songSelectManager.SongList = new List<SongSelectManager.Song>(newList);
            }

            int newSongIndex = 0;
            for (int i = 0; i < newList.Count; i++)
            {
                if (newList[i].Id == prevSongId)
                {
                    newSongIndex = i;
                    break;
                }
            }

            var currentCueSheetName = songSelectManager.songPlayer.CueSheetName;
            var currentSongSheetName = songSelectManager.bgmCueSheets[songSelectManager.SongList[newSongIndex].PreviewIndex];

            if (currentCueSheetName != currentSongSheetName)
            {
                songSelectManager.playingSongIndex = -1;
                songSelectManager.isSongLoadRequested = true;
                songSelectManager.songPlayer.Stop(true);
                songSelectManager.isSongPlaying = false;
            }

            songSelectManager.SelectedSongIndex = newSongIndex;
            songSelectManager.PlayKanbanMoveAnim(SongSelectManager.KanbanMoveType.Initialize, SongSelectManager.KanbanMoveSpeed.Normal);
            songSelectManager.UpdateKanbanSurface(false);
            songSelectManager.UpdateSortBarSurface(true);

            songSelectManager.oniUraChangeTimeCount = 0f;
            songSelectManager.kanbans[0].DiffCourseChangeAnim.Play("ChangeMania", 0, 1f);
            songSelectManager.UpdateScoreDisplay();
        }
    }
}
