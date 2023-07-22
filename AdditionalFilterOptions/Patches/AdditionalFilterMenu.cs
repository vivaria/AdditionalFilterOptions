using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterMenu : MonoBehaviour
    {
        const string FilterParentName = "AdditionalFilterMenuParent";

        SongSelectManager songSelectManager { get; set; }
        static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();


        void Start()
        {
            GameObject parentCanvasObject = GameObject.Find(FilterParentName);
            if (parentCanvasObject == null)
            {
                parentCanvasObject = new GameObject(FilterParentName);
            }

            Canvas parentCanvas = parentCanvasObject.GetComponent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = parentCanvasObject.AddComponent<Canvas>();
            }

            FullSongList = new List<SongSelectManager.Song>();
            Plugin.Log.LogInfo("AdditionalFilterMenu Created");
        }

        public void InitializeFullSongList(List<SongSelectManager.Song> songList)
        {
            FullSongList = songList;
        }

    }
}
