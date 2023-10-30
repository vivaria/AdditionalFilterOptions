using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterMenu : MonoBehaviour
    {
        const string FilterParentName = "AdditionalFilterMenuParent";

        GameObject realParent;
        GameObject parent;

        Dictionary<EnsoData.SongGenre, GameObject> GenreFilterButtons = new Dictionary<EnsoData.SongGenre, GameObject>();
        Dictionary<DataConst.CrownType, GameObject> CrownFilterButtons = new Dictionary<DataConst.CrownType, GameObject>();
        Dictionary<EnsoData.EnsoLevelType, GameObject> DifficultyFilterButtons = new Dictionary<EnsoData.EnsoLevelType, GameObject>();

        SongSelectManager songSelectManager { get; set; }
        static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();

        SortSettings sortSettings;
        FilterSettings filterSettings;

        CycleButton playlistButton;
        CycleButton sortingButton;

        void Start()
        {
            realParent = AssetUtility.GetOrCreateEmptyObject(null, FilterParentName, Vector2.zero);
            realParent.layer = 5;
            //parent = GameObject.Find(FilterParentName);
            //if (parent == null)
            //{
            //    parent = new GameObject(FilterParentName);
            //    parent.layer = 5;
            //}

            transform.SetParent(realParent.transform);

            Canvas parentCanvas = realParent.GetComponent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = realParent.AddComponent<Canvas>();
                parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                parentCanvas.worldCamera = null;
                parentCanvas.overrideSorting = true;
                parentCanvas.sortingOrder = 2;
                

                var parentCanvasScaler = realParent.AddComponent<CanvasScaler>();
                parentCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                parentCanvasScaler.referenceResolution = new Vector2(1920, 1080);
                parentCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                parentCanvasScaler.matchWidthOrHeight = 0;
            }

            FullSongList = new List<SongSelectManager.Song>();
            Plugin.Log.LogInfo("AdditionalFilterMenu Created");

            sortSettings = new SortSettings();
            sortSettings.PrimarySort = SortOptions.Default;
            sortSettings.SecondarySort = SortOptions.Default;

            filterSettings = new FilterSettings();
            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                if ((EnsoData.SongGenre)i == EnsoData.SongGenre.Children ||
                    (EnsoData.SongGenre)i == EnsoData.SongGenre.Num)
                {
                    continue;
                }
                if (!filterSettings.EnabledGenres.ContainsKey((EnsoData.SongGenre)i))
                {
                    filterSettings.EnabledGenres.Add((EnsoData.SongGenre)i, true);
                }
            }
            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                // Available crowns are None, Silver, Gold, and Rainbow
                if ((DataConst.CrownType)i == DataConst.CrownType.Bronze ||
                    (DataConst.CrownType)i == DataConst.CrownType.Off ||
                    (DataConst.CrownType)i == DataConst.CrownType.Num)
                {
                    continue;
                }
                if (!filterSettings.EnabledCrowns.ContainsKey((DataConst.CrownType)i))
                {
                    filterSettings.EnabledCrowns.Add((DataConst.CrownType)i, true);
                }
            }
            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var diff = (EnsoData.EnsoLevelType)i;
                if (diff == EnsoData.EnsoLevelType.Num)
                {
                    continue;
                }
                if (!filterSettings.EnabledDifficulties.ContainsKey(diff))
                {
                    filterSettings.EnabledDifficulties.Add(diff, true);
                }
            }
            filterSettings.MinDifficulty = 0;
            filterSettings.MaxDifficulty = 10;

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
            realParent.SetActive(isActive);
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

            parent = UnityObjectUtility.CreateImage("FilterBackground", new Color32(0, 0, 0, (int)(255 * 0.698f)), new Rect(0, 0, 1920, 1080), realParent.transform);

            var genreParent = AssetUtility.CreateEmptyObject(parent, "GenreFilters", new Vector2(200, 900));

            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var genre = (EnsoData.SongGenre)i;
                if (!GenreFilterButtons.ContainsKey(genre))
                {
                    GameObject buttonObject = null;
                    Vector2 rect = new Vector2(50, 100);
                    int yDifference = 50;
                    Color32 color = new Color32(0, 0, 0, 255);
                    Sprite sprite = null;
                    string genreText = string.Empty;
                    switch (genre)
                    {
                        case EnsoData.SongGenre.Pops:
                            rect = new Vector2(rect.x, rect.y);
                            var genreObj = GameObject.Find("Genre1");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Anime:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 1));
                            genreObj = GameObject.Find("Genre2");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Vocalo:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 2));
                            genreObj = GameObject.Find("Genre3");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Variety:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 3));
                            genreObj = GameObject.Find("Genre4");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Classic:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 4));
                            genreObj = GameObject.Find("Genre5");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Game:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 5));
                            genreObj = GameObject.Find("Genre6");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Namco:
                            rect = new Vector2(rect.x, rect.y - (yDifference * 6));
                            genreObj = GameObject.Find("Genre7");
                            buttonObject = GameObject.Instantiate(genreObj, genreParent.transform);
                            break;
                        case EnsoData.SongGenre.Num:
                        case EnsoData.SongGenre.Children:
                        default:
                            continue;
                    }

                    AssetUtility.SetRect(buttonObject, rect);

                    var button = AssetUtility.AddButtonComponent(buttonObject);
                    button.onClick.AddListener(() => ClickGenreButton(genre));
                    GenreFilterButtons.Add(genre, buttonObject);
                }
            }


            var crownParent = AssetUtility.CreateEmptyObject(parent, "CrownFilters", new Vector2(1300, 750));
            var crownScale = crownParent.transform.localScale;
            crownScale = new Vector3(0.5f, 0.5f, 0.5f);
            crownParent.transform.localScale = crownScale;

            var existingCrownObject = GameObject.Find("IconCrown1P");
            var existingCrownAnimation = existingCrownObject.GetComponent<Animator>();
            var existingCrownImage = existingCrownObject.GetComponent<Image>();

            for (int i = 0; i < (int)DataConst.CrownType.Num; i++)
            {
                var crown = (DataConst.CrownType)i;
                if (!CrownFilterButtons.ContainsKey(crown))
                {
                    Vector2 pos = new Vector2(0, 100);
                    int xDifference = 300;
                    Texture2D tex = null;
                    Sprite sprite = null;
                    string genreText = string.Empty;
                    GameObject buttonObject = null;
                    switch (crown)
                    {
                        case DataConst.CrownType.None:
                            existingCrownAnimation.Play("None");
                            var crownObj = GameObject.Find("IconCrown1");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            var buttonImage = buttonObject.GetComponent<Image>();
                            buttonImage.sprite = existingCrownImage.sprite;
                            AssetUtility.GetChildByName(buttonObject, "Text").GetComponent<TextMeshProUGUI>().text = "No Clear";
                            pos = new Vector2(xDifference * 0, pos.y);
                            break;
                        case DataConst.CrownType.Silver:
                            crownObj = GameObject.Find("IconCrown1");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 1, pos.y);
                            break;
                        case DataConst.CrownType.Gold:
                            crownObj = GameObject.Find("IconCrown2");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 2, pos.y);
                            break;
                        case DataConst.CrownType.Rainbow:
                            crownObj = GameObject.Find("IconCrown3");
                            buttonObject = GameObject.Instantiate(crownObj, crownParent.transform);
                            pos = new Vector2(xDifference * 3, pos.y);
                            break;
                        default:
                            continue;
                    }

                    AssetUtility.AddButtonComponent(buttonObject);
                    AssetUtility.SetRect(buttonObject, pos);
                    
                    var button = buttonObject.GetOrAddComponent<Button>();
                    button.onClick.AddListener(() => ClickCrownButton(crown));
                    CrownFilterButtons.Add(crown, buttonObject);
                }
            }

            InitializeDifficultyFilter();

            


            for (int i = 0; i < (int)EnsoData.SongGenre.Num; i++)
            {
                var genre = (EnsoData.SongGenre)i;
                if (filterSettings.EnabledGenres.ContainsKey(genre) && GenreFilterButtons.ContainsKey(genre))
                {
                    AssetUtility.ChangeButtonTransparency(GenreFilterButtons[genre], filterSettings.EnabledGenres[genre]);
                }
            }

            var searchInput = AssetUtility.CreateInputField(parent, "SearchInput", new Rect(100, 100, 1000, 200));
            var inputField = searchInput.GetComponent<TMP_InputField>();
            inputField.onValueChanged.AddListener((string x) => SearchInputChanged(x));

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

        private void InitializeDifficultyFilter()
        {
            var difficultyParent = AssetUtility.GetOrCreateEmptyObject(parent, "DifficultyFilters", new Vector2(1000, 500));
            var difficultyScale = difficultyParent.transform.localScale;
            var scale = 0.5f;
            difficultyScale = new Vector3(scale, scale, scale);
            difficultyParent.transform.localScale = difficultyScale;

            for (int i = 0; i < (int)EnsoData.EnsoLevelType.Num; i++)
            {
                var diff = (EnsoData.EnsoLevelType)i;
                if (DifficultyFilterButtons.ContainsKey(diff))
                {
                    continue;
                }
                GameObject icon;
                GameObject text;
                Vector2 pos = new Vector2(0, 0);
                int xDifference = 200;
                switch (diff)
                {
                    case EnsoData.EnsoLevelType.Easy:
                        icon = GameObject.Find("IconDiff1");
                        text = GameObject.Find("TextDiff1");
                        pos = new Vector2(xDifference * 0, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Normal:
                        icon = GameObject.Find("IconDiff2");
                        text = GameObject.Find("TextDiff2");
                        pos = new Vector2(xDifference * 1, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Hard:
                        icon = GameObject.Find("IconDiff3");
                        text = GameObject.Find("TextDiff3");
                        pos = new Vector2(xDifference * 2, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Mania:
                        icon = GameObject.Find("IconDiff4");
                        text = GameObject.Find("TextDiff4");
                        pos = new Vector2(xDifference * 3, pos.y);
                        break;
                    case EnsoData.EnsoLevelType.Ura:
                        icon = GameObject.Find("IconDiff4");
                        text = GameObject.Find("TextDiff4");
                        pos = new Vector2(xDifference * 4, pos.y);
                        break;
                    default:
                        continue;
                }
                var diffObject = AssetUtility.GetOrCreateEmptyChild(difficultyParent, diff.ToString(), pos);
                icon = GameObject.Instantiate(icon, diffObject.transform);
                text = GameObject.Instantiate(text, diffObject.transform);
                var iconPos = icon.transform.localPosition;
                var textPos = text.transform.localPosition;
                iconPos = new Vector3(0, iconPos.y, iconPos.z);
                textPos = new Vector3(0, textPos.y, textPos.z);
                icon.transform.localPosition = iconPos;
                text.transform.localPosition = textPos;
                diffObject.transform.localScale = Vector3.one;
                icon.transform.localScale = Vector3.one;
                text.transform.localScale = Vector3.one;

                var button = AssetUtility.AddButtonComponent(diffObject);
                button.onClick.AddListener(() => ClickDifficultyButton(diff));
                DifficultyFilterButtons.Add(diff, diffObject);
            }
        }

        public void ClickGenreButton(EnsoData.SongGenre genre)
        {
            Plugin.LogInfo("Button Click: " + genre);
            filterSettings.EnabledGenres[genre] = !filterSettings.EnabledGenres[genre];
            AssetUtility.ChangeButtonTransparency(GenreFilterButtons[genre], filterSettings.EnabledGenres[genre]);
            FilterSongList();
        }

        public void ClickCrownButton(DataConst.CrownType crown)
        {
            Plugin.LogInfo("Button Click: " + crown);
            filterSettings.EnabledCrowns[crown] = !filterSettings.EnabledCrowns[crown];
            AssetUtility.ChangeButtonAndTextTransparency(CrownFilterButtons[crown], filterSettings.EnabledCrowns[crown]);
            FilterSongList();
        }

        public void ClickDifficultyButton(EnsoData.EnsoLevelType diff)
        {
            Plugin.LogInfo("Button Click: " + diff);
            filterSettings.EnabledDifficulties[diff] = !filterSettings.EnabledDifficulties[diff];
            AssetUtility.ChangeDifficultyButtonAndTextTransparency(DifficultyFilterButtons[diff], filterSettings.EnabledDifficulties[diff]);
            FilterSongList();
        }

        public void SearchInputChanged(string newValue)
        {
            Plugin.LogInfo("SearchInput Changed: " + newValue);
            filterSettings.TextFilter = newValue;
            FilterSongList();
        }

        public void AssignSongSelectManager(SongSelectManager instance)
        {
            songSelectManager = instance;
        }

        public void InitializeFullSongList(List<SongSelectManager.Song> songList)
        {
            if (FullSongList.Count < songList.Count)
            {
                FullSongList = songList;
            }
            Plugin.Log.LogInfo("AdditionalFilterMenu InitializedSongList");
            FilterSongList();
        }

        private void FilterSongList()
        {
            int minDifficulty = 1;
            int maxDifficulty = 10;

            List<SongSelectManager.Song> filteredSongList = LoadPlaylist();

            filteredSongList = Filter.FilterText(filteredSongList, filterSettings.TextFilter);
            filteredSongList = Filter.FilterGenres(filteredSongList, filterSettings.EnabledGenres);
            filteredSongList = Filter.FilterCrowns(filteredSongList, filterSettings.EnabledDifficulties, filterSettings.EnabledCrowns);
            filteredSongList = Filter.FilterDifficulty(filteredSongList, filterSettings.EnabledDifficulties, minDifficulty, maxDifficulty);

            //filteredSongList = Sort.SortSongList(filteredSongList, new List<SortTypes>() { SortTypes.Difficulty, SortTypes.Accuracy }, filterSettings.EnabledDifficulties);

            UpdateSongList(filteredSongList);
        }

        List<SongSelectManager.Song> LoadPlaylist()
        {
            if (playlistButton == null || playlistButton.GetCurrentData() == "")
            {
                return new List<SongSelectManager.Song>(FullSongList);
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
            if (FullSongList.Count <= songSelectManager.SelectedSongIndex)
            {
                Plugin.LogError("FullSongList.Count "+ FullSongList.Count + " <= songSelectManager.SelectedSongIndex " + songSelectManager.SelectedSongIndex);
            }
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
            if (songSelectManager.SongList.Count <= newSongIndex)
            {
                Plugin.LogError("songSelectManager.SongList.Count <= newSongIndex");
            }
            if (songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex)
            {
                Plugin.LogError("songSelectManager.bgmCueSheets.Count <= songSelectManager.SongList[newSongIndex].PreviewIndex");
            }
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
