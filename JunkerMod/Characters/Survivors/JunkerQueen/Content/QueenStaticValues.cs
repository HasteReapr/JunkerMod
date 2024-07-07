using System;

namespace JunkerMod.Survivors.Queen
{
    public static class QueenStaticValues
    {
        //total damage for the shotgun, all pellets combined.
        public const float gunDamageCoefficient = 0.8f;
        //damage for the individual pellets of the shotgun, total / pellet count
        public const float pelletDamageCoefficient = gunDamageCoefficient/scatterPelletCount;
        //total amount of pellets
        public const int scatterPelletCount = 10;

        public const float knifeDamageCoefficient = 2.5f;

        public const float axeDamageCoefficient = 15f;
    }
}