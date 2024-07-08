using RoR2;
using RoR2.Skills;
using EntityStates;
using RoR2.CharacterAI;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using EntityStates.Mage;
using static RoR2.DotController;


namespace JunkerMod.Survivors.Queen.SkillStates
{
    internal class PassiveHeal
    {
        public class AdrenalineHealState : BaseState
        {
            public static List<AdrenalineHealState> instances = new List<AdrenalineHealState>();

            public override void OnEnter()
            {
                base.OnEnter();
                instances.Add(this);
            }
            public override void OnExit()
            {
                base.OnExit();
                instances.Remove(this);
            }
            public override InterruptPriority GetMinimumInterruptPriority()
            {
                return InterruptPriority.Death;
            }
        }
    }
}
