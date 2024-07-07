﻿using EntityStates;
using JunkerMod.Survivors.Queen;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace JunkerMod.Survivors.Queen.SkillStates.ShotgunSkills
{
	public class EnterReload : BaseState
	{
		//public static string enterSoundString;
		public static float baseDuration = 0.12f;

		private float duration
		{
			get
			{
				return EnterReload.baseDuration / this.attackSpeedStat;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			//base.PlayCrossfade("Gesture, Override", "EnterReload", "Reload.playbackRate", this.duration, 0.1f);
			//Util.PlaySound(EnterReload.enterSoundString, base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge > this.duration)
			{
				this.outer.SetNextState(new Reload());
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}