using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace OrganizedResearchTech
{

    [StaticConstructorOnStartup]
    [UsedImplicitly]
    public class OrganizedResearchTech : Mod
    {
        protected OrganizedResearchTechSettings Settings;
        private List<ResearchTabDef> allTabs;

        public OrganizedResearchTech(ModContentPack content) : base(content)
        {
            Harmony harmony1 = new("zenlor.OrganizedResearchTech");
            harmony1.PatchAll(Assembly.GetExecutingAssembly());

            Settings = GetSettings<OrganizedResearchTechSettings>();
            ResearchTabs.Initialize(Settings);
            allTabs = DefDatabase<ResearchTabDef>.AllDefsListForReading;
        }

        public override string SettingsCategory() => "Organized Research Tech";

        private Vector2 scrollPosition;

        private void RefreshTabs()
        {
            ResearchTabs.Initialize(Settings);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 30f), "ORT_ResearchTabs".Translate());

            float buttonY = inRect.y + 35f;
            Rect addTabRect = new Rect(inRect.x, buttonY, 120f, 30f);
            Rect resetRect = new Rect(inRect.x + 130f, buttonY, 120f, 30f);

            if (Widgets.ButtonText(addTabRect, "Add Tab"))
            {
                List<FloatMenuOption> addOptions = new List<FloatMenuOption>();
                foreach (var tabDef in allTabs)
                {
                    addOptions.Add(new FloatMenuOption(tabDef.label, () =>
                    {
                        Settings.ResearchTabList.Add(new ResearchTabEntry(tabDef));
                        RefreshTabs();
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(addOptions));
            }

            if (Widgets.ButtonText(resetRect, "Reset to Defaults"))
            {
                Settings.Reset();
                RefreshTabs();
            }

            float listHeight = Settings.ResearchTabList.Count * 30f;
            Rect viewRect = new Rect(inRect.x, buttonY + 35f, inRect.width - 16f, inRect.height - 70f);
            Rect scrollRect = new Rect(0f, 0f, viewRect.width, listHeight);

            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);

            for (int idx = 0; idx < Settings.ResearchTabList.Count; idx++)
            {
                var entry = Settings.ResearchTabList[idx];
                float rowY = idx * 30f;

                Rect upBtnRect = new Rect(0f, rowY, 30f, 30f);
                Rect downBtnRect = new Rect(30f, rowY, 30f, 30f);
                Rect rowRect = new Rect(60f, rowY, scrollRect.width - 90f, 30f);
                Rect removeBtnRect = new Rect(scrollRect.width - 30f, rowY, 30f, 30f);

                if (idx % 2 == 1)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.1f);
                    GUI.DrawTexture(new Rect(0f, rowY, scrollRect.width, 30f), BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }

                Widgets.DrawHighlightIfMouseover(new Rect(0f, rowY, scrollRect.width, 30f));

                if (idx == 0)
                {
                    GUI.enabled = false;
                }
                if (Widgets.ButtonText(upBtnRect, "▲", false))
                {
                    if (idx > 0)
                    {
                        var item = Settings.ResearchTabList[idx];
                        Settings.ResearchTabList.RemoveAt(idx);
                        Settings.ResearchTabList.Insert(idx - 1, item);
                        RefreshTabs();
                    }
                }
                GUI.enabled = true;

                if (idx == Settings.ResearchTabList.Count - 1)
                {
                    GUI.enabled = false;
                }
                if (Widgets.ButtonText(downBtnRect, "▼", false))
                {
                    if (idx < Settings.ResearchTabList.Count - 1)
                    {
                        var item = Settings.ResearchTabList[idx];
                        Settings.ResearchTabList.RemoveAt(idx);
                        Settings.ResearchTabList.Insert(idx + 1, item);
                        RefreshTabs();
                    }
                }
                GUI.enabled = true;

                string label = entry.TabDef != null ? $"{idx + 1}. {entry.TabDef.label}" : $"{idx + 1}. (missing)";
                TextAnchor prevAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rowRect, label);
                Text.Anchor = prevAnchor;

                if (Mouse.IsOver(rowRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    Event.current.Use();
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    int capturedIdx = idx;
                    foreach (var tabDef in allTabs)
                    {
                        options.Add(new FloatMenuOption(tabDef.label, () =>
                        {
                            Settings.ResearchTabList[capturedIdx] = new ResearchTabEntry(tabDef);
                            RefreshTabs();
                        }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                }

                if (Widgets.ButtonText(removeBtnRect, "X", false))
                {
                    Settings.ResearchTabList.RemoveAt(idx);
                    RefreshTabs();
                }
            }

            Widgets.EndScrollView();
        }
    }

    public class ResearchTabEntry
    {
        public ResearchTabDef TabDef;
        public string DefName => TabDef?.defName;

        public ResearchTabEntry() { }

        public ResearchTabEntry(ResearchTabDef tabDef)
        {
            TabDef = tabDef;
        }

        public override string ToString() => DefName ?? "null";
    }

    public static class ResearchTabs
    {
        public static List<ResearchTabDef> Tabs;

        public static void Initialize(OrganizedResearchTechSettings settings)
        {
            Tabs = settings.ResearchTabList
                .Where(entry => entry.TabDef != null)
                .Select(entry => entry.TabDef)
                .ToList();
        }
    }

    public class OrganizedResearchTechSettings : ModSettings
    {
        private List<string> defNames = new List<string>();
        public List<ResearchTabEntry> ResearchTabList = new List<ResearchTabEntry>();

        private static readonly string[] defaultTabDefNames =
        [
            "VFET_Basics",
            "SRT_BasicsResearch",
            "SRT_NeolithicResearch",
            "SRT_MedievalResearch",
            "SRT_IndustrialResearch",
            "SRT_IndustrialResearchI",
            "SRT_IndustrialResearchII",
            "SRT_SpacerResearch",
            "SRT_UltraResearch",
            "Anomaly"
        ];

        public OrganizedResearchTechSettings()
        {
            Log.Message($"[ORT] Constructor called, ResearchTabList count: {ResearchTabList.Count}");
        }

        public override void ExposeData()
        {
            Log.Message($"[ORT] ExposeData called, mode: {Scribe.mode}, defNames count before: {defNames?.Count ?? -1}, ResearchTabList count: {ResearchTabList?.Count ?? -1}");
            
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                defNames = ResearchTabList
                    .Where(e => e.DefName != null)
                    .Select(e => e.DefName)
                    .ToList();
                Log.Message($"[ORT] Saving, defNames synced: {string.Join(", ", defNames)}");
            }
            base.ExposeData();
            Scribe_Collections.Look(ref defNames, "ResearchTabList", LookMode.Value);
            Log.Message($"[ORT] After Look, defNames count: {defNames?.Count ?? -1}, values: {(defNames != null ? string.Join(", ", defNames) : "null")}");
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (defNames == null || defNames.Count == 0)
                {
                    Log.Message("[ORT] PostLoadInit: defNames empty, calling Reset");
                    Reset();
                }
                else
                {
                    Log.Message($"[ORT] PostLoadInit: calling ApplyLoadedData with {defNames.Count} items");
                    ApplyLoadedData();
                }
            }
        }

        public void ApplyLoadedData()
        {
            ResearchTabList = defNames
                .Select(name => new ResearchTabEntry(DefDatabase<ResearchTabDef>.GetNamed(name, errorOnFail: false)))
                .Where(entry => entry.TabDef != null)
                .ToList();
            Log.Message($"[ORT] ApplyLoadedData: ResearchTabList count after: {ResearchTabList.Count}");
        }

        public void Reset()
        {
            ResearchTabList = defaultTabDefNames
                .Select(name => new ResearchTabEntry(DefDatabase<ResearchTabDef>.GetNamed(name, errorOnFail: false)))
                .Where(entry => entry.TabDef != null)
                .ToList();
            defNames = ResearchTabList.Select(e => e.DefName).ToList();
            Log.Message($"[ORT] Reset: ResearchTabList count: {ResearchTabList.Count}, defNames: {string.Join(", ", defNames)}");
        }
    }
}
