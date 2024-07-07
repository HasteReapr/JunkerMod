using RoR2;
using JunkerMod.Modules.Achievements;

namespace JunkerMod.Survivors.Queen.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class HenryMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = QueenSurvivor.QUEEN_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = QueenSurvivor.QUEEN_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => QueenSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}