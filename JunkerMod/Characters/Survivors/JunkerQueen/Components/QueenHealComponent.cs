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
using JunkerMod.Survivors.Queen.SkillStates;
using static JunkerMod.Survivors.Queen.SkillStates.PassiveHeal;

namespace JunkerMod.Survivors.Queen.Components
{
    internal class QueenHealComponent : NetworkBehaviour
    {
        //grabbed from https://github.com/yekoc/Risk-Of-Rain-2-Mods/blob/master/PassiveAgression/PassiveAgression/Characters/Mage/BleedPassive.cs
        private static void dotDamageHook(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(HealthComponent).GetMethod(nameof(HealthComponent.TakeDamage)))))
            {
                c.MoveBeforeLabels();
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Action<DotController, DotIndex>>((self, index) => {
                    if ((index != DotIndex.Bleed && index != DotIndex.SuperBleed) || (self.victimHealthComponent && !self.victimHealthComponent.alive))
                    {
                        return;
                    }
                    var pos = self.victimBody.transform.position;
                    foreach (var state in AdrenalineHealState.instances.Where((state) => state.characterBody != self.victimBody && inRange(state.characterBody.transform.position, pos)))
                    {
                        RoR2.Orbs.OrbManager.instance.AddOrb(new RoR2.Orbs.HealOrb
                        {
                            origin = pos,
                            healValue = 2f,
                            target = state.characterBody.mainHurtBox
                        });
                    }
                });
            }
            bool inRange(Vector3 origin, Vector3 target)
            {
                var vec = target - origin;
                return vec.sqrMagnitude <= 1600;
            }
        }

        public void Start()
        {

        }

        public void FixedUpdate()
        {


            bool inRange(Vector3 origin, Vector3 target)
            {
                var vec = target - origin;
                return vec.sqrMagnitude <= 2400;
            }
        }

        public void Hook()
        {
            IL.RoR2.DotController.EvaluateDotStacksForType += dotDamageHook;
        }
    }
}
