using RoR2;
using RoR2.Projectile;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace JunkerMod.Survivors.Queen.Components
{
    public class QueenKnifeComponent : NetworkBehaviour
    {
        private const float MAX_STICK_TIME = 3.0f;
        private const float SPEED = 90;
        private bool hasStuck = false;
        private bool returnKnife = false;
        private float stuckTime = 0;
        GameObject parent;

        public void Start()
        {
            parent = base.GetComponent<ProjectileController>().owner;
        }

        public void FixedUpdate()
        {
            // once we stick, start to count up, once we count up enough activate the return
            if (hasStuck && stuckTime <= MAX_STICK_TIME)
            {
                stuckTime += Time.fixedDeltaTime;
            }

            // weve counted up enough, time to start returning
            if(hasStuck && stuckTime >= MAX_STICK_TIME)
            {
                returnKnife = true;
                // We disable our collider, stickonimpact, and change our layer to debris, then enable overlap attack
                base.GetComponent<SphereCollider>().enabled = false;
                base.GetComponent<ProjectileStickOnImpact>().enabled = false;
                this.gameObject.layer = 13; //13 is debris, 14 is projectile.
                base.GetComponent<ProjectileOverlapAttack>().enabled = true;
            }

            if (returnKnife && hasStuck)
            {
                Recall();
            }
        }

        //move our velocity to the player
        private void Recall()
        { 
            Vector3 returnPos = parent.transform.position;
            if(Vector3.Distance(returnPos, base.transform.position) > 0.3f)
            {
                base.transform.position += (returnPos - base.transform.position).normalized * SPEED * Time.deltaTime;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void OnDestroy()
        {
            gameObject.BroadcastMessage("KnifeHasReturnith");
        }

        //this overrides the timer and makes the knife instantly return
        public void KnifeComethToMe()
        {
            hasStuck = true;
            returnKnife = true;
        }

        //this method gets activated when the knife sticks, and will set the knife timer to comeback
        public void KnifeStuckyStuck()
        {
            hasStuck = true;
        }
    }
}
