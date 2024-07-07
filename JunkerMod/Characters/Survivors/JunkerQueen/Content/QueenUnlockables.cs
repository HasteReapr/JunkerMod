using JunkerMod.Survivors.Queen.Achievements;
using RoR2;
using UnityEngine;

namespace JunkerMod.Survivors.Queen
{
    public static class QueenUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                HenryMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(HenryMasteryAchievement.identifier),
                QueenSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
