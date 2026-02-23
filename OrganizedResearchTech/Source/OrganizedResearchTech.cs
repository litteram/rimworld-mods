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

    [UsedImplicitly]
    public class OrganizedResearchTech : Mod
    {
        public static OrganizedResearchTech Instance;
        protected OrganizedResearchTechSettings Settings;
        private List<ResearchTabDef> allTabs;
        private bool cacheInitialized;

        public OrganizedResearchTech(ModContentPack content) : base(content)
        {
            Instance = this;
            Harmony harmony1 = new("zenlor.OrganizedResearchTech");
            harmony1.PatchAll(Assembly.GetExecutingAssembly());

            Settings = GetSettings<OrganizedResearchTechSettings>();
            LongEventHandler.ExecuteWhenFinished(EnsureCacheInitialized);
        }

        public override string SettingsCategory() => "Organized Research Tech";

        private Vector2 scrollPosition;

        public void EnsureCacheInitialized()
        {
            if (cacheInitialized) return;
            cacheInitialized = true;
            allTabs = DefDatabase<ResearchTabDef>.AllDefsListForReading;
            if (Settings.TabDefNames == null || Settings.TabDefNames.Count == 0)
            {
                Settings.Reset();
            }
            ResearchTabs.Initialize(Settings);
        }

        public void ForceReinitialize()
        {
            cacheInitialized = false;
            EnsureCacheInitialized();
        }

        private void OnSettingsChanged()
        {
            ResearchTabs.Initialize(Settings);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            EnsureCacheInitialized();
            base.DoSettingsWindowContents(inRect);

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
                        Settings.TabDefNames.Add(tabDef.defName);
                        OnSettingsChanged();
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(addOptions));
            }

            if (Widgets.ButtonText(resetRect, "Reset to Defaults"))
            {
                Settings.Reset();
                OnSettingsChanged();
            }

            float listHeight = Settings.TabDefNames.Count * 30f;
            Rect viewRect = new Rect(inRect.x, buttonY + 35f, inRect.width - 16f, inRect.height - 70f);
            Rect scrollRect = new Rect(0f, 0f, viewRect.width, listHeight);

            Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);

            for (int idx = 0; idx < Settings.TabDefNames.Count; idx++)
            {
                string defName = Settings.TabDefNames[idx];
                ResearchTabDef tabDef = DefDatabase<ResearchTabDef>.GetNamed(defName, errorOnFail: false);
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
                        string item = Settings.TabDefNames[idx];
                        Settings.TabDefNames.RemoveAt(idx);
                        Settings.TabDefNames.Insert(idx - 1, item);
                        OnSettingsChanged();
                    }
                }
                GUI.enabled = true;

                if (idx == Settings.TabDefNames.Count - 1)
                {
                    GUI.enabled = false;
                }
                if (Widgets.ButtonText(downBtnRect, "▼", false))
                {
                    if (idx < Settings.TabDefNames.Count - 1)
                    {
                        string item = Settings.TabDefNames[idx];
                        Settings.TabDefNames.RemoveAt(idx);
                        Settings.TabDefNames.Insert(idx + 1, item);
                        OnSettingsChanged();
                    }
                }
                GUI.enabled = true;

                string label = tabDef != null ? $"{idx + 1}. {tabDef.label}" : $"{idx + 1}. ({defName})";
                TextAnchor prevAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rowRect, label);
                Text.Anchor = prevAnchor;

                if (Mouse.IsOver(rowRect) && Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    Event.current.Use();
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    int capturedIdx = idx;
                    foreach (var otherTabDef in allTabs)
                    {
                        options.Add(new FloatMenuOption(otherTabDef.label, () =>
                        {
                            Settings.TabDefNames[capturedIdx] = otherTabDef.defName;
                            OnSettingsChanged();
                        }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                }

                if (Widgets.ButtonText(removeBtnRect, "X", false))
                {
                    Settings.TabDefNames.RemoveAt(idx);
                    OnSettingsChanged();
                }
            }

            Widgets.EndScrollView();
        }
    }

    public static class ResearchTabs
    {
        public static List<ResearchTabDef> Tabs;

        public static void Initialize(OrganizedResearchTechSettings settings)
        {
            Tabs = settings.TabDefNames
                .Select(name => DefDatabase<ResearchTabDef>.GetNamed(name, errorOnFail: false))
                .Where(def => def != null)
                .ToList();
        }
    }

    public class OrganizedResearchTechSettings : ModSettings
    {
        public List<string> TabDefNames = new List<string>();

        private static readonly string[] defaultTabDefNames =
        [
            "VFET_Basics",
            "SRT_NeolithicResearch",
            "SRT_MedievalResearch",
            "SRT_IndustrialResearch",
            "SRT_IndustrialResearchI",
            "SRT_IndustrialResearchII",
            "SRT_SpacerResearch",
            "SRT_UltraResearch",
            "SRT_ArchotechResearch",
            "Anomaly"
        ];

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref TabDefNames, "ResearchTabList", LookMode.Value);
        }

        public void Reset()
        {
            TabDefNames = defaultTabDefNames.ToList()
                .FindAll(name => DefDatabase<ResearchTabDef>.GetNamed(name, errorOnFail: false) != null);
        }
    }
}
