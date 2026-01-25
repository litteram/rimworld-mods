using RimWorld;
using Verse;

namespace PsycasterGeneSpawner
{
    public class CompUseEffect_GivePsycasterGene : CompUseEffect
    {
        public CompProperties_UseEffectGivePsycasterGene Props => (CompProperties_UseEffectGivePsycasterGene)props;

        public override void DoEffect(Pawn user)
        {
            if (Props.psycasterGene == null)
                Props.psycasterGene = GeneDefOf.Gene_Archotechist;

            if (user.genes.HasActiveGene(Props.psycasterGene)) return;

            user.genes.AddGene(Props.psycasterGene, true);
        }
    }
}