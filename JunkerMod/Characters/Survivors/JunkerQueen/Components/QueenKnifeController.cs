using Unity;
using RoR2;
using UnityEngine.Networking;

namespace JunkerMod.Survivors.Queen.Components
{
    public class QueenKnifeController : NetworkBehaviour
    {
        public bool knifeReturned = false;

        public void KnifeHasReturnith()
        {
            knifeReturned = true;
        }

        public void KnifeCTRLReset()
        {
            knifeReturned = false;
        }
    }
}
