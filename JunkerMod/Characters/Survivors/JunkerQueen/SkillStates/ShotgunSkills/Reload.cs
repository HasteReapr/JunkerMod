﻿using EntityStates;
using JunkerMod.Survivors.Queen;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace JunkerMod.Survivors.Queen.SkillStates.ShotgunSkills
{
	public class Reload : BaseState
	{
		public static float enterSoundPitch = 1f;
		public static float exitSoundPitch = 1f;
		public static string enterSoundString = "Play_Moffein_RocketSurvivor_M1_Reload";
		//public static GameObject reloadEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion();
		public static float baseDuration = 1f;

		private bool hasGivenStock;

		private float duration
		{
			get
			{
				return Reload.baseDuration / this.attackSpeedStat;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			//base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ReloadSimple" : "Reload", "Reload.playbackRate", this.duration);
			base.PlayCrossfade("Reload, Override", "EnterReload", "Reload.playbackRate", this.duration, 0.1f);
			Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration / 2f)
			{
				this.GiveStock();
			}
			if (!base.isAuthority || base.fixedAge < this.duration)
			{
				return;
			}
			if (base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				this.outer.SetNextState(new Reload());
				return;
			}
			this.outer.SetNextStateToMain();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		private void GiveStock()
		{
			if (this.hasGivenStock)
			{
				return;
			}
			if (base.isAuthority && base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				//base.skillLocator.primary.AddOneStock();
				base.skillLocator.primary.stock = base.skillLocator.primary.maxStock;
			}
			/*if (Reload.reloadEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(Reload.reloadEffectPrefab, base.gameObject, "MuzzleRocketLauncher", false);
			}*/
			this.hasGivenStock = true;
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}