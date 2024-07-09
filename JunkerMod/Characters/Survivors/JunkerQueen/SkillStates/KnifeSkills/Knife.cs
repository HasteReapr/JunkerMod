using EntityStates;
using JunkerMod.Survivors.Queen;
using JunkerMod.Survivors.Queen.Components;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JunkerMod.Survivors.Queen.SkillStates.KnifeSkills
{
    public class Knife : BaseSkillState
    {
        public static float baseDuration = 0.65f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float BaseDelayDuration = 0.0f;
        public static float DamageCoefficient = QueenStaticValues.knifeDamageCoefficient;

        public bool knifeReturned = false;
        private bool hasFired = false;
        private float firePercentTime = 0.0f;
        private float fireTime;
        private float duration = 1f;
        private Ray aimRay;

        public override void OnEnter()
        {
            aimRay = GetAimRay();
            base.OnEnter();
            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
            knifeReturned = false;
        }

        private void Shoot()
        {
            if (!hasFired)
            {
                hasFired = true;
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = DamageCoefficient * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = QueenAssets.queenKnife,
                    speedOverride = 128,
                    damageTypeOverride = DamageType.BleedOnHit,
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // if we press our utility button again, recall the knife.
            if (inputBank.skill2.justPressed && fixedAge >= 0.2f)
            {
                Chat.AddMessage("Knife return call");
                gameObject.BroadcastMessage("KnifeComethToMe");
            }

            if (fixedAge >= fireTime)
            {
                Shoot();
            }

            if (fixedAge >= duration && isAuthority && knifeReturned)
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