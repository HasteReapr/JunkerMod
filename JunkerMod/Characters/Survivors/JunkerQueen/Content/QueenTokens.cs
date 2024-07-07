using System;
using JunkerMod.Modules;
using JunkerMod.Survivors.Queen.Achievements;

namespace JunkerMod.Survivors.Queen
{
    public static class QueenTokens
    {
        public static void Init()
        {
            AddHenryTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Queen.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddHenryTokens()
        {
            string prefix = QueenSurvivor.QUEEN_PREFIX;

            string desc = "Queen is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, searching for a new identity.";
            string outroFailure = "..and so she vanished, wandering eternally.";

            Language.Add(prefix + "NAME", "Wastelander");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Queen of the Wasteland");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "DEFAULT_SKIN_NAME", "Junker");
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Deity");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Adrenaline Rush");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Heal from all damage dealt over time by bleed.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_GUN_NAME", "Scatter Gun");
            Language.Add(prefix + "PRIMARY_GUN_DESCRIPTION", $"Fire a shot from your scattergun, dealing <style=cIsDamage>{QueenStaticValues.scatterPelletCount}x{100f * QueenStaticValues.gunDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_KNIFE_NAME", "Jagged Blade");
            Language.Add(prefix + "SECONDARY_KNIFE_DESCRIPTION", Tokens.agilePrefix + $"Throw your knife, dealing <style=cIsDamage>{100f * QueenStaticValues.knifeDamageCoefficient}% impact damage</style>, plus an additional <style=cIsDamage>{50f * QueenStaticValues.knifeDamageCoefficient} damage over time.</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_SHOUT_NAME", "Commanding Shout");
            Language.Add(prefix + "UTILITY_SHOUT_DESCRIPTION", "Shout inpsiring your allies, giving you extra health and speed for a few seconds.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_AXE_NAME", "Carnage");
            Language.Add(prefix + "SPECIAL_AXE_DESCRIPTION", $"Swing your axe for <style=cIsDamage>{100f * QueenStaticValues.axeDamageCoefficient}% damage</style>, plus an addition <style=cIsDamage>{50f * QueenStaticValues.axeDamageCoefficient}% damage over time</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(HenryMasteryAchievement.identifier), "Queen: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(HenryMasteryAchievement.identifier), "As Queen, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
