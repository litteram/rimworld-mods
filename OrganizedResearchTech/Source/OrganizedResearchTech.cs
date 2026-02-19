using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;

namespace OrganizedResearchTech
{ 
    [StaticConstructorOnStartup]
    public class OrganizedResearchTech : Mod
    {
        public OrganizedResearchTech(ModContentPack content) : base(content)
        {
            Harmony harmony1 = new("zenlor.OrganizedResearchTech");
            harmony1.PatchAll(Assembly.GetExecutingAssembly());
            
            Settings = GetSettings<OrganizedResearchTechSettings>();
            ResearchWeights.Initialize(Settings);
        }
        
        public override string SettingsCategory() => "Organized Research Tech";
        
        private string newDefName = "Main";
        private string newWeightStr = "10";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.ColumnWidth = inRect.width / 2.05f;

            listingStandard.Label("Research Tab Weights".Translate());
            listingStandard.GapLine();

            var weights = Settings.ResearchWeights.ToList();
            var keysToRemove = new List<string>();
            
            var labelWidth = 140f;
            var fieldPadding = 24f;
            var numFieldWidth = 48f;
            var fieldWidth = labelWidth - fieldPadding - labelWidth;

            for (int i = 0; i < weights.Count; i++)
            {
                var entry = weights[i];
                var rowRect = listingStandard.GetRect(30f);
                var fieldNewWidth = rowRect.width - fieldPadding - numFieldWidth;

                var defNameRect = new Rect(rowRect.x, rowRect.y, fieldNewWidth, 24f);
                var weightRect = new Rect(rowRect.x + fieldNewWidth + fieldPadding, rowRect.y, numFieldWidth, 24f);
                var deleteRect = new Rect(rowRect.x + fieldNewWidth + fieldPadding + numFieldWidth + fieldPadding, rowRect.y, 24f, 24f);

                var editKey = entry.Key;
                var editValue = entry.Value.ToString();

                editKey = Widgets.TextField(defNameRect, editKey);
                editValue = Widgets.TextField(weightRect, editValue);

                if (int.TryParse(editValue, out var newValue))
                {
                    if (editKey != entry.Key || newValue != entry.Value)
                    {
                        keysToRemove.Add(entry.Key);
                        Settings.ResearchWeights[editKey] = newValue;
                    }
                }

                if (Widgets.ButtonText(deleteRect, "X", false))
                {
                    keysToRemove.Add(entry.Key);
                }

                listingStandard.Gap(5f);
            }

            foreach (var key in keysToRemove)
            {
                Settings.ResearchWeights.Remove(key);
            }

            listingStandard.GapLine();

            var newRowRect = listingStandard.GetRect(30f);
            var fieldNewNewWidth = newRowRect.width - fieldPadding - numFieldWidth;

            var newDefNameRect = new Rect(newRowRect.x, newRowRect.y, fieldNewNewWidth, 24f);
            newDefName = Widgets.TextField(newDefNameRect, newDefName);
            
            var newWeightStrRect = new Rect(newRowRect.x + fieldNewNewWidth + fieldPadding, newRowRect.y, numFieldWidth, 24f);
            newWeightStr = Widgets.TextField(newWeightStrRect, newWeightStr);

            if (listingStandard.ButtonText("Add"))
            {
                if (int.TryParse(newWeightStr, out var newWeight) && !string.IsNullOrWhiteSpace(newDefName))
                {
                    Settings.ResearchWeights[newDefName] = newWeight;
                }
            }

            listingStandard.GapLine();

            if (listingStandard.ButtonText("Reset to Defaults"))
            {
                Settings.Reset();
            }

            listingStandard.End();
        }

        private OrganizedResearchTechSettings Settings { get; }
    }

    public class OrganizedResearchTechSettings : ModSettings
    {
        public Dictionary<string, int> ResearchWeights = new Dictionary<string, int>
        {
            { "SRT_BasicsResearch",       0 },
            { "SRT_NeolithicResearch",    1 },
            { "SRT_MedievalResearch",     2 },
            { "SRT_IndustrialResearch",   3 },
            { "SRT_IndustrialResearchI",  4 },
            { "SRT_IndustrialResearchII", 4 },
            { "SRT_SpacerResearch",       6 },
            { "SRT_UltraResearch",        7 },
        };

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref ResearchWeights, "ResearchWeights", LookMode.Undefined, LookMode.Value);
        }

        public void Reset()
        {
            ResearchWeights = new Dictionary<string, int>
            {
                { "SRT_BasicsResearch",       0 },
                { "SRT_NeolithicResearch",    1 },
                { "SRT_MedievalResearch",     2 },
                { "SRT_IndustrialResearch",   3 },
                { "SRT_IndustrialResearchI",  4 },
                { "SRT_IndustrialResearchII", 4 },
                { "SRT_SpacerResearch",       6 },
                { "SRT_UltraResearch",        7 },
            };
        }
    }
}