using RoR2;
using RoR2.Skills;

namespace JunkerMod.Survivors.Queen.SkillStates
{
    public class AssignableSkillDef : SkillDef
    {

        public System.Func<GenericSkill, SkillDef.BaseSkillInstanceData> onAssign;
        public System.Action<GenericSkill> onUnassign;

        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            return onAssign?.Invoke(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            base.OnUnassigned(skillSlot);
            onUnassign?.Invoke(skillSlot);
        }
        public bool IsAssigned(CharacterBody body)
        {
            return System.Array.Exists(body.skillLocator.allSkills, IsAssigned);
        }
        public bool IsAssigned(GenericSkill skill)
        {
            return skill.skillDef == this;
        }
        public bool IsAssigned(CharacterBody body, SkillSlot skillSlot)
        {
            return body.skillLocator.GetSkill(skillSlot).skillDef == this;
        }
        public GenericSkill GetSkill(CharacterBody body)
        {
            if (body && body.skillLocator)
            {
                for (int i = 0; i < body.skillLocator.allSkills.Length; i++)
                {
                    if (IsAssigned(body.skillLocator.allSkills[i]))
                    {
                        return body.skillLocator.allSkills[i];
                    }
                }
            }
            return null;
        }
    }
}
