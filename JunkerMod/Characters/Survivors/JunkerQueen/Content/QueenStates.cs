using JunkerMod.Survivors.Queen.SkillStates;
using JunkerMod.Survivors.Queen.SkillStates.ShotgunSkills;
using JunkerMod.Survivors.Queen.SkillStates.KnifeSkills;

namespace JunkerMod.Survivors.Queen
{
    public static class QueenStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Knife));
            Modules.Content.AddEntityState(typeof(KnifePull));

            Modules.Content.AddEntityState(typeof(Shout));

            Modules.Content.AddEntityState(typeof(Axe));

            Modules.Content.AddEntityState(typeof(PassiveHeal.AdrenalineHealState));
        }
    }
}
