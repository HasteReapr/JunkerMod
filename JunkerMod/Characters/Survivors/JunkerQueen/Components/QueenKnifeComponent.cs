using RoR2;
using RoR2.Projectile;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using JunkerMod.Survivors.Queen.SkillStates.KnifeSkills;
using JunkerMod.Survivors.Queen;
using EntityStates;

namespace JunkerMod.Survivors.Queen.Components
{
    public class QueenKnifeComponent : NetworkBehaviour
    {
        private const float MAX_STICK_TIME = 3.0f;
        private const float SPEED = 128;

        public bool hasStuck = false;
        public bool returnKnife = false;
        public float stuckTime = 0;
        public GameObject parent;

        private GameObject stuckVictim;
        EntityStateMachine parentESM;

        public void Start()
        {
            parent = base.GetComponent<ProjectileController>().owner;

            // Get the characters EnitityStateMachine, then get the state, so we can directly modify the state variables
            parentESM = EntityStateMachine.FindByCustomName(parent, "Weapon2");
            if(parentESM.state is Knife knifeState)
            {
                knifeState.knifeReturned = false;
                knifeState.knifeProjectile = this.gameObject;
            }
        }

        public void FixedUpdate()
        {
            // once we stick, start to count up, once we count up enough activate the return
            if (hasStuck && stuckTime <= MAX_STICK_TIME)
            {
                stuckTime += Time.fixedDeltaTime;
            }

            // weve counted up enough, time to start returning
            if(hasStuck && stuckTime >= MAX_STICK_TIME && !returnKnife)
            {
                //if we have server authority, we grab the victim gameobject
                if (hasAuthority)
                {
                    stuckVictim = base.GetComponent<ProjectileStickOnImpact>().victim;
                }
                PreRecall();
            }

            if (returnKnife && hasStuck)
            {
                Recall();
            }
        }

        public void PreRecall()
        {
            returnKnife = true;
            // We disable our collider, stickonimpact, and change our layer to debris, then enable overlap attack
            base.GetComponent<SphereCollider>().enabled = false;
            base.GetComponent<ProjectileStickOnImpact>().enabled = false;
            base.GetComponent<Rigidbody>().useGravity = false;
            this.gameObject.layer = 13; //13 is debris, 14 is projectile.
            base.GetComponent<ProjectileOverlapAttack>().enabled = true;
            //base.GetComponent<HitBoxGroup>().enabled = true;
        }

        
        [Server]
        public void PrematureCall()
        {
            hasStuck = true;
            stuckTime = 5;
            returnKnife = true;
            PreRecall();
        }

        //move our velocity to the player
        private void Recall()
        {
            Vector3 returnPos = parent.transform.position;
            if(Vector3.Distance(returnPos, base.transform.position) > 1f)
            {
                base.transform.position += (returnPos - base.transform.position).normalized * SPEED * Time.deltaTime;
                base.GetComponent<ProjectileSimple>().desiredForwardSpeed = 0;
                // we also drag the victim along with the knife
                if (stuckVictim) DragVictim();
            }
            else
            {
                if (parentESM.state is Knife knifeState)
                {
                    knifeState.knifeReturned = true;
                }
                Destroy(this.gameObject);
            }
        }

        private void DragVictim()
        {
            //if the stuckVictim doesn't exist, escape out of this.
            if (!stuckVictim) return;

            stuckVictim.GetComponent<EntityStateMachine>().enabled = false;
            Vector3 dragPos = base.transform.position;
            stuckVictim.transform.position += (dragPos - stuckVictim.transform.position).normalized * SPEED * Time.deltaTime;
        }

        private void FreeVictim()
        {
            stuckVictim.GetComponent<EntityStateMachine>().enabled = true;
            stuckVictim.GetComponent<EntityStateMachine>().SetNextStateToMain();
        }

        public void OnDestroy()
        {
            if (parentESM.state is Knife knifeState)
            {
                knifeState.knifeReturned = true;
            }
            if (stuckVictim) FreeVictim();
            //BroadcastMessage("KnifeHasReturnith");
        }

        //this method gets activated when the knife sticks, and will set the knife timer to comeback
        public void KnifeStuckyStuck()
        {
            hasStuck = true;
        }
    }
}
