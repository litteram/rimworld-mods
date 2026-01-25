using HarmonyLib;
using Verse;

namespace PsycasterGeneSpawner
{
    [StaticConstructorOnStartup]
    public static class PsycasterGeneSpawnerMod
    {
        static PsycasterGeneSpawnerMod()
        {
            Harmony harmony = new("PsycasterGeneSpawnerMod");

            harmony.PatchAll();
        }
    }
}