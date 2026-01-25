using HarmonyLib;
using SpawnThoseGenes;
using VanillaPsycastsExpanded;
using Verse;

namespace PsycasterGeneSpawner
{
    [HarmonyPatch(typeof(SpawnThoseGenesMod), "PostfixGenerator")]
    public class Patch_PostfixGenerator
    {
        public static void Postfix(Pawn pawn)
        {
            // Is the pawn dull?
            if (pawn.genes.HasActiveGene(GeneDefOf.PsychicAbility_Deaf)) return;
            if (pawn.genes.HasActiveGene(GeneDefOf.PsychicAbility_Dull)) return;

            GeneDef psycasterGene = Utils.GetRandomPsycasterGene(pawn);
            if (psycasterGene == null) return;

            if (pawn.DevelopmentalStage == DevelopmentalStage.Adult)
            {
                Hediff_PsycastAbilities implant = Utils.GivePsylink(pawn);
                
                if (implant != null) 
                    Utils.GivePsycasterPath(pawn, psycasterGene, implant);
            }
        }
    }
}