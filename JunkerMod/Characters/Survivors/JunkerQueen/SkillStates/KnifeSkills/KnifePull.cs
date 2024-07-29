using EntityStates;
using JunkerMod.Survivors.Queen;
using JunkerMod.Survivors.Queen.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace JunkerMod.Survivors.Queen.SkillStates.KnifeSkills
{
    public class KnifePull : BaseSkillState
    {
        public static float baseDuration = 0.1f;
        public GameObject knifeProjectile;
        public bool knifeReturned;

        public override void OnEnter()
        {
            //play looping animation
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= baseDuration && isAuthority && knifeReturned)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}