using System.Collections.Generic;
using System.Linq;
using Harmony;
using Studio;

namespace CharaStateX
{
    static class Utils
    {
        public static IEnumerable<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null);
        }

        public static IEnumerable<OCIChar> GetAllSelectedButMain(object __instance)
        {
            return GetSelectedCharacters().Where((chara) => chara != GetMainChara(__instance));
        }

        public static OCIChar GetMainChara(object __instance)
        {
            return Traverse.Create(__instance).Property("ociChar").GetValue<OCIChar>();
        }

        public static bool GetIsUpdateInfo(object __instance)
        {
            return Traverse.Create(__instance).Property("isUpdateInfo").GetValue<bool>();
        }
    }
}
