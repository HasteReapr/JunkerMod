using JunkerMod.Survivors.Queen.SkillStates;
using JunkerMod.Survivors.Queen.SkillStates.ShotgunSkills;

namespace JunkerMod.Survivors.Queen
{
    public static class QueenStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(Axe));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Shout));

            Modules.Content.AddEntityState(typeof(Knife));
        }
    }
}
