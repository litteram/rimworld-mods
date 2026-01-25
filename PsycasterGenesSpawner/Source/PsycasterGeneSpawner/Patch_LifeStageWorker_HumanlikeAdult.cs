using HarmonyLib;
using RimWorld;
using Verse;

namespace PsycasterGeneSpawner
{
    [HarmonyPatch(typeof(LifeStageWorker_HumanlikeAdult), "Notify_LifeStageStarted")]
    public class Patch_LifeStageWorker_HumanlikeAdult
    {
        public static void Postfix(Pawn pawn)
        {
            GeneDef psycasterGene = Utils.GetRandomPsycasterGene(pawn);
            if (psycasterGene != null)
                Utils.GivePsylink(pawn);
        }
    }
}