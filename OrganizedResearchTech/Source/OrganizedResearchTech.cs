using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace OrganizedResearchTech
{
    // ReSharper disable once UnusedType.Global
    [StaticConstructorOnStartup]
    public class OrganizedResearchTech : Mod
    {
        public OrganizedResearchTech(ModContentPack content) : base(content)
        {
            Settings = GetSettings<OrganizedResearchTechSettings>();

            // TODO: preferences
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.ColumnWidth = inRect.width / 2.05f;

            listingStandard.Label("Organized Research Tech".Translate(Settings.Something.ToString()));
            listingStandard.GapLine();
            var section = listingStandard.BeginSection(-1);

            section.ButtonText("Foo");

            listingStandard.EndSection(section);
            listingStandard.End();
        }

        private OrganizedResearchTechSettings Settings { get; }
    }

    public class OrganizedResearchTechSettings : ModSettings
    {
        public bool Something;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.Something, "Something");
        }

        public void Reset()
        {
            Something = false;
        }
    }

    // ReSharper disable once InconsistentNaming
    [StaticConstructorOnStartup]
    public static class OrganizedResearchTechStartup
    {
        static OrganizedResearchTechStartup()
        {
            new Harmony("zenlor.OrganizedResearchTech").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(MainTabWindow_Research), "PostOpen")]
    class MainTabWindow_Research_PatchPostOpenAllDefs
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getAllDefs = AccessTools.Property(typeof(DefDatabase<RimWorld.ResearchTabDef>), "AllDefs")
                .GetGetMethod();
            MethodInfo getAllAllowedDefs = AccessTools.Method(typeof(MainTabWindow_Research_PatchPostOpenAllDefs),
                "AllowedTabDefs", null, null);

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

        private static IEnumerable<ResearchTabDef> AllowedTabDefs(IEnumerable<ResearchTabDef> originalList)
        {
            const string modPrefix = "SRT_";

            return originalList.Where(def =>
                def.defName.StartsWith(modPrefix) || def.defName == "Anomaly" || def.defName == "Main");
        }
    }
}