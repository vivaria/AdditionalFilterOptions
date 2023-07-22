using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdditionalFilterOptions.Patches
{
    internal class AdditionalFilterOptionsPatch
    {
        const string FilterObjectName = "AdditionalFilterMenu";

        public static List<SongSelectManager.Song> FullSongList = new List<SongSelectManager.Song>();

        static AdditionalFilterMenu filterMenu;

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
            }
        }

        [HarmonyPatch(typeof(SongSelectManager), "SortSongList", new Type[] { typeof(DataConst.SongSortCourse), typeof(DataConst.SongSortType), typeof(DataConst.SongFilterType), typeof(DataConst.SongFilterTypeFavorite) })]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        private static void SongSelectManager_SortSongList_Postfix(SongSelectManager __instance, DataConst.SongSortCourse sortDifficulty, DataConst.SongSortType sortType, DataConst.SongFilterType filterType, DataConst.SongFilterTypeFavorite filterTypeFavorite)
        {
            
        }
    }
}
