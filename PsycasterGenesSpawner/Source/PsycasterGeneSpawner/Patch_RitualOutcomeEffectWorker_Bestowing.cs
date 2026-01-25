using HarmonyLib;
using RimWorld;
using Verse;

namespace PsycasterGeneSpawner
{
    [HarmonyPatch(typeof(RitualOutcomeEffectWorker_Bestowing), nameof(RitualOutcomeEffectWorker_Bestowing.Apply))]
    public class Patch_RitualOutcomeEffectWorker_Bestowing
    {
        public static void Postfix(LordJob_Ritual jobRitual)
        {
            LordJob_BestowingCeremony lordJob_BestowingCeremony = (LordJob_BestowingCeremony)jobRitual;
            Pawn target = lordJob_BestowingCeremony.target;

            if (target.genes.HasActiveGene(GeneDefOf.Gene_Archotechist)) return;

            target.genes.AddGene(GeneDefOf.Gene_Archotechist, true);
        }
    }
}