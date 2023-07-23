using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterOptionsPatch
    {
        const string FilterObjectName = "AdditionalFilterMenu";

        public static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();

        static AdditionalFilterMenu filterMenu;


        private static bool pressedF3 = false;

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_Start_Postfix(SongSelectManager __instance)
        {
            GameObject additionalFilterMenu = GameObject.Find(FilterObjectName);
            if (additionalFilterMenu == null)
            {
                additionalFilterMenu = new GameObject(FilterObjectName);
            }

            filterMenu = additionalFilterMenu.GetComponent<AdditionalFilterMenu>();
            if (filterMenu == null)
            {
                filterMenu = additionalFilterMenu.AddComponent<AdditionalFilterMenu>();
                filterMenu.AssignSongSelectManager(__instance);
            }
        }

        [HarmonyPatch(typeof(SongSelectManager), "SortSongList", new Type[] { typeof(DataConst.SongSortCourse), typeof(DataConst.SongSortType), typeof(DataConst.SongFilterType), typeof(DataConst.SongFilterTypeFavorite) })]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_SortSongList_Postfix(SongSelectManager __instance, DataConst.SongSortCourse sortDifficulty, DataConst.SongSortType sortType, DataConst.SongFilterType filterType, DataConst.SongFilterTypeFavorite filterTypeFavorite)
        {
            if (filterMenu != null)
            {
                filterMenu.InitializeFullSongList(new List<SongSelectManager.Song>(__instance.SongList));
            }
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.UpdateSongSelect))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectManager_UpdateSongSelect_Prefix(SongSelectManager __instance)
        {
            bool currentlyPressingF3 = Input.GetKey(KeyCode.F3);
            if (currentlyPressingF3 && !pressedF3)
            {
                pressedF3 = true;
                ToggleFilterMenuActive();
            }
            else if (!currentlyPressingF3)
            {
                pressedF3 = false;
            }
            return true;
        }

        private static void SetFilterMenuActive(bool isActive)
        {
            filterMenu.SetFilterMenuActive(isActive);
            List<string> ObjectsToDisableRaycasting = new List<string>()
            {
                "mid_canvas",
                "fg_canvas(ModeSelect)",
                "input_guide_canvas",
            };
            for (int i = 0; i < ObjectsToDisableRaycasting.Count; i++)
            {
                var gameObject = GameObject.Find(ObjectsToDisableRaycasting[i]);
                if (gameObject != null)
                {
                    var rayCaster = gameObject.GetComponent<GraphicRaycaster>();
                    if (rayCaster != null)
                    {
                        rayCaster.enabled = !isActive;
                    }
                }
            }
        }

        private static void ToggleFilterMenuActive()
        {
            var isActive = filterMenu.gameObject.activeInHierarchy;
            SetFilterMenuActive(!isActive);
        }

        private static bool IsFilterActive()
        {
            return filterMenu.gameObject.activeInHierarchy;
        }
    }
}
