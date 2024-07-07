using EntityStates;
using JunkerMod.Survivors.Queen;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JunkerMod.Survivors.Queen.SkillStates.KnifeSkills
{
    public class KnifePull : BaseSkillState
    {
        public static float baseDuration = 0.65f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float BaseDelayDuration = 0.0f;

        public static float DamageCoefficient = QueenStaticValues.knifeDamageCoefficient;

        private bool hasFired = false;
        private float firePercentTime = 0.0f;
        private float fireTime;
        private float duration = 1f;
        private Ray aimRay;

        public override void OnEnter()
        {/*
            projectilePrefab = QueenAssets.queenKnife;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "HenryBombThrow";

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = BaseDelayDuration;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            recoilAmplitude = 0.1f;
            bloom = 10;
            */
            aimRay = GetAimRay();
            base.OnEnter();
            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
            duration = baseDuration / attackSpeedStat;
            fireTime = firePercentTime * duration;
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
                    //damageTypeOverride = characterBody.HasBuff(Modules.Buffs.assassinDrugsBuff) ? (DamageType?)Modules.Projectiles.poisonDmgType : (DamageType?)Modules.Projectiles.poisonDmgType,
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime)
            {
                Shoot();
            }

            if (fixedAge >= duration && isAuthority)
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