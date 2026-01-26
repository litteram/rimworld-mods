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
            if (pawn.psychicEntropy.PsychicSensitivity < 100 || pawn.DevelopmentalStage != DevelopmentalStage.Adult) return;

            GeneDef psycasterGene = Utils.GetRandomPsycasterGene(pawn);
            if (psycasterGene == null) return;

            Hediff_PsycastAbilities implant = Utils.GivePsylink(pawn);
            if (implant == null) return;

            Utils.GivePsycasterPath(pawn, psycasterGene, implant);
        }
    }
}