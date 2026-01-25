using RimWorld;
using Verse;

namespace PsycasterGeneSpawner
{
    public class CompProperties_UseEffectGivePsycasterGene : CompProperties_UseEffect
    {
        public GeneDef psycasterGene;

        public CompProperties_UseEffectGivePsycasterGene()
        {
            compClass = typeof(CompUseEffect_GivePsycasterGene);
        }
    }
}