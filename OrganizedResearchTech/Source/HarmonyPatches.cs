using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace OrganizedResearchTech
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "PostOpen")]
    [UsedImplicitly]
    class MainTabWindow_Research_PatchPostOpen
    {
        [UsedImplicitly]
        static void Postfix(ref List<MainTabWindow_Research.ResearchTabRecord> ___tabs)
        {
            IEnumerable<ResearchProjectDef> db = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
            var cmp = new ResearchComparer();

            ___tabs = ___tabs.FindAll(record => db.Any(def => def.tab.Equals(record.def)))
                    .OrderBy(r => r.def, cmp)
                    .ToList();
        }
    }

    /*
    [HarmonyPatch(typeof(MainTabWindow_Research), "PostOpen")]
    class MainTabWindow_Research_PatchPostOpenAllDefs
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getAllDefs = AccessTools.Property(typeof(DefDatabase<RimWorld.ResearchTabDef>), "AllDefs")
                .GetGetMethod();
            MethodInfo getAllAllowedDefs = AccessTools.Method(
                typeof(MainTabWindow_Research_PatchPostOpenAllDefs),
                "AllowedTabDefs",
                null,
                null
                );

            // This is a strange way to Transpile, but ok, it works.
            List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);
            int num;
            for (int i = 0; i < instructionList.Count; i = num + 1)
            {
                CodeInstruction instruction = instructionList[i];
                bool flag = instruction.opcode == OpCodes.Call && instruction.OperandIs(getAllDefs);
                if (flag)
                {
                    //Log.Message("MainTabWindow_Research.DrawRightRect match 1 of 1", false);
                    yield return instruction;
                    instruction = new CodeInstruction(OpCodes.Call, getAllAllowedDefs);
                }

                yield return instruction;
                instruction = null;
                num = i;
            }

            yield break;

        }

        [UsedImplicitly]
        private static IEnumerable<ResearchTabDef> AllowedTabDefs(IEnumerable<ResearchTabDef> originalList)
        {
            var db = DefDatabase<ResearchProjectDef>.AllDefs;
            return originalList.Where(def =>
                db.Any(i => !i.IsHidden && i.tab.defName == def.defName || def.defName == "Anomaly")
            ).OrderBy<ResearchTabDef, ResearchTabDef>(def => def, new ResearchComparer());
        }
    }
    */

    /*
    [HarmonyPatch(typeof(MainTabWindow_Research), "ListProjects")]
    class MainTabWindow_Research_PatchBeforeListProjects
    {
        static void Prefix(Rect _rightInRect, ref bool _elementClicked)
        { 
            
        }
    }
    */
}