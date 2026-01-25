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

    [HarmonyPatch(typeof(MainTabWindow_Research), "ListProjects")]
    class MainTabWindow_Research_PatchBeforeListProjects
    {
        static void Prefix(Rect _rightInRect, ref bool _elementClicked)
        { 
            
        }
    }
}


/*

// Decompiled with JetBrains decompiler
// Type: LeoCleanResearchSort.CleanResearchSort
// Assembly: LeoCleanResearchSort, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7D0D0D1-7E6B-4CBC-82B7-488DA74A9FFE
// Assembly location: /media/pny/SteamLibrary/steamapps/workshop/content/294100/3566639513/Assemblies/LeoCleanResearchSort.dll

using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

#nullable disable
namespace LeoCleanResearchSort;

[StaticConstructorOnStartup]
public static class CleanResearchSort
{
  private static readonly Dictionary<ResearchProjectDef, Vector2> s_newPositions = new Dictionary<ResearchProjectDef, Vector2>();
  private const float s_gridSize = 1f;

  static CleanResearchSort()
  {
    LongEventHandler.QueueLongEvent(new Action(CleanResearchSort.SortProjectsAndApply), "Research Project Sorting", true, (Action<Exception>) null);
  }

  private static void SortProjectsAndApply()
  {
    List<ResearchProjectDef> defsListForReading = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
    if (defsListForReading.NullOrEmpty<ResearchProjectDef>())
      return;
    CleanResearchSortModSettings settings = LoadedModManager.GetMod<CleanResearchSortMod>().GetSettings<CleanResearchSortModSettings>();
    List<IGrouping<ResearchTabDef, ResearchProjectDef>> list1 = defsListForReading.GroupBy<ResearchProjectDef, ResearchTabDef>((Func<ResearchProjectDef, ResearchTabDef>) (p => p.tab)).ToList<IGrouping<ResearchTabDef, ResearchProjectDef>>();
    string str1 = "N/A";
    string str2 = "N/A";
    int num1 = 0;
    foreach (IGrouping<ResearchTabDef, ResearchProjectDef> source in list1)
    {
      List<ResearchProjectDef> list2 = source.ToList<ResearchProjectDef>();
      int num2 = 0;
      num1 = 0;
      foreach (ResearchProjectDef key in (IEnumerable<ResearchProjectDef>) source)
        CleanResearchSort.s_newPositions[key] = new Vector2(key.researchViewX * settings.horizontalSpacingPercentage, key.researchViewY * settings.verticalSpacingPercentage);
      bool flag1;
      do
      {
        flag1 = false;
        bool flag2 = false;
        if (settings.enableVerticalCompaction && CleanResearchSort.HandleVerticalCompaction(list2, settings.verticalCompactionY))
        {
          flag1 = true;
          flag2 = true;
        }
        if (CleanResearchSort.HandlePrerequisites(list2, settings.horizontalSpacingPercentage))
        {
          flag1 = true;
          flag2 = true;
        }
        if (CleanResearchSort.HandleOverlaps(list2, settings.horizontalSpacingPercentage, settings.verticalSpacingPercentage))
        {
          flag1 = true;
          flag2 = true;
        }
        ++num2;
        int num3 = num2;
        if (flag2)
        {
          str1 = list2.Last<ResearchProjectDef>().label;
          str2 = source.Key?.label ?? (string) "MainTabs.label".Translate();
        }
        if (settings.enableSafetyCounter && num2 > settings.maxPasses)
        {
          Log.Error($"[Clean Research Sort]: The research tree couldn't be organized after {settings.maxPasses} passes. The last pass occurred on tab '{str2}' after {num3} iterations. The last project calculated was '{str1}'. Try increasing the 'Max Sorting Passes' in the mod setting. If the issue continues, please check for mod conflicts or research that requires itself in a loop.");
          break;
        }
      }
      while (flag1);
      if (settings.enableRemoveWhiteSpace)
        CleanResearchSort.HandleLeftAndTopWhiteSpace(list2);
    }
    CleanResearchSort.ApplyNewPositions();
    Log.Message("[Clean Research Sort]: Successfully applied sorted research tree layout.");
  }

  private static bool HandleVerticalCompaction(
    List<ResearchProjectDef> projects,
    float verticalCompactionY)
  {
    bool flag = false;
    float num1 = float.MinValue;
    List<ResearchProjectDef> list = new List<ResearchProjectDef>();
    foreach (ResearchProjectDef project in projects)
    {
      float y = CleanResearchSort.s_newPositions[project].y;
      if ((double) y > (double) num1)
        num1 = y;
      if ((double) y > (double) verticalCompactionY)
        list.Add(project);
    }
    if (list.NullOrEmpty<ResearchProjectDef>() || (double) num1 <= (double) verticalCompactionY)
      return false;
    float num2 = num1 - verticalCompactionY;
    foreach (ResearchProjectDef key in list)
    {
      Vector2 newPosition = CleanResearchSort.s_newPositions[key];
      newPosition.y -= num2;
      CleanResearchSort.s_newPositions[key] = newPosition;
      flag = true;
    }
    return flag;
  }

  private static bool HandlePrerequisites(
    List<ResearchProjectDef> projects,
    float horizontalSpacingPercentage)
  {
    bool flag = false;
    foreach (ResearchProjectDef project in projects)
    {
      float a = -1f;
      if (project.prerequisites != null)
      {
        foreach (ResearchProjectDef prerequisite in project.prerequisites)
        {
          if (prerequisite.tab == project.tab)
          {
            if (CleanResearchSort.IsCircularDependency(project, prerequisite))
            {
              Log.Warning($"[Clean Research Sort]: Detected circular dependency: {project.label} requires {prerequisite.label}, which eventually requires itself. This link will be ignored.");
            }
            else
            {
              Vector2 newPosition = CleanResearchSort.s_newPositions[prerequisite];
              a = Mathf.Max(a, newPosition.x);
            }
          }
        }
      }
      if ((double) a >= 0.0)
      {
        Vector2 newPosition = CleanResearchSort.s_newPositions[project];
        float num = Mathf.Max(newPosition.x, a + 1f * horizontalSpacingPercentage);
        if ((double) num > (double) newPosition.x)
        {
          newPosition.x = num;
          CleanResearchSort.s_newPositions[project] = newPosition;
          flag = true;
        }
      }
    }
    return flag;
  }

  private static bool IsCircularDependency(
    ResearchProjectDef startProject,
    ResearchProjectDef currentProject,
    HashSet<ResearchProjectDef> visited = null)
  {
    if (visited == null)
      visited = new HashSet<ResearchProjectDef>();
    if (currentProject == startProject)
      return true;
    if (!visited.Add(currentProject) || currentProject.prerequisites == null)
      return false;
    foreach (ResearchProjectDef prerequisite in currentProject.prerequisites)
    {
      if (CleanResearchSort.IsCircularDependency(startProject, prerequisite, visited))
        return true;
    }
    return false;
  }

  private static bool HandleOverlaps(
    List<ResearchProjectDef> projects,
    float horizontalSpacingPercentage,
    float verticalSpacingPercentage)
  {
    bool flag = false;
    Dictionary<Vector2Int, List<ResearchProjectDef>> dictionary = new Dictionary<Vector2Int, List<ResearchProjectDef>>();
    foreach (ResearchProjectDef project in projects)
    {
      Vector2Int gridKey = CleanResearchSort.GetGridKey(CleanResearchSort.s_newPositions[project]);
      if (!dictionary.ContainsKey(gridKey))
        dictionary[gridKey] = new List<ResearchProjectDef>();
      dictionary[gridKey].Add(project);
    }
    foreach (ResearchProjectDef project in projects)
    {
      Vector2 newPosition1 = CleanResearchSort.s_newPositions[project];
      Vector2Int gridKey = CleanResearchSort.GetGridKey(newPosition1);
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          Vector2Int key1 = new Vector2Int(gridKey.x + index1, gridKey.y + index2);
          List<ResearchProjectDef> researchProjectDefList;
          if (dictionary.TryGetValue(key1, out researchProjectDefList))
          {
            foreach (ResearchProjectDef key2 in researchProjectDefList)
            {
              if (project != key2)
              {
                Vector2 newPosition2 = CleanResearchSort.s_newPositions[key2];
                if ((double) Mathf.Abs(newPosition1.x - newPosition2.x) < 1.0 * (double) horizontalSpacingPercentage && (double) Mathf.Abs(newPosition1.y - newPosition2.y) < 0.5 * (double) verticalSpacingPercentage)
                {
                  ResearchProjectDef key3 = (double) project.researchViewX == (double) key2.researchViewX ? key2 : ((double) project.researchViewX > (double) key2.researchViewX ? project : key2);
                  Vector2 newPosition3 = CleanResearchSort.s_newPositions[key3];
                  newPosition3.x += 1f * horizontalSpacingPercentage;
                  CleanResearchSort.s_newPositions[key3] = newPosition3;
                  flag = true;
                }
              }
            }
          }
        }
      }
    }
    return flag;
  }

  private static Vector2Int GetGridKey(Vector2 pos)
  {
    return new Vector2Int(Mathf.FloorToInt(pos.x / 1f), Mathf.FloorToInt(pos.y / 1f));
  }

  private static bool HandleLeftAndTopWhiteSpace(List<ResearchProjectDef> projects)
  {
    if (projects.NullOrEmpty<ResearchProjectDef>())
      return false;
    float num1 = projects.Min<ResearchProjectDef>((Func<ResearchProjectDef, float>) (p => CleanResearchSort.s_newPositions[p].x));
    float num2 = projects.Min<ResearchProjectDef>((Func<ResearchProjectDef, float>) (p => CleanResearchSort.s_newPositions[p].y));
    bool flag = false;
    float num3 = 0.0f;
    float num4 = 0.0f;
    if ((double) num1 > 0.009999999776482582)
    {
      num3 = num1;
      flag = true;
    }
    if ((double) num2 > 0.009999999776482582)
    {
      num4 = num2;
      flag = true;
    }
    if (flag)
    {
      foreach (ResearchProjectDef project in projects)
      {
        Vector2 newPosition = CleanResearchSort.s_newPositions[project];
        newPosition.x -= num3;
        newPosition.y -= num4;
        CleanResearchSort.s_newPositions[project] = newPosition;
      }
    }
    return flag;
  }

  private static void ApplyNewPositions()
  {
    FieldInfo fieldInfo1 = AccessTools.Field(typeof (ResearchProjectDef), "x");
    FieldInfo fieldInfo2 = AccessTools.Field(typeof (ResearchProjectDef), "y");
    if (fieldInfo1 == (FieldInfo) null || fieldInfo2 == (FieldInfo) null)
    {
      Log.Error("[Clean Research Sort]: Failed to find private fields 'x' and 'y' in ResearchProjectDef. This mod may be incompatible with the current game version.");
    }
    else
    {
      foreach (KeyValuePair<ResearchProjectDef, Vector2> newPosition in CleanResearchSort.s_newPositions)
      {
        ResearchProjectDef key = newPosition.Key;
        Vector2 vector2 = newPosition.Value;
        fieldInfo1.SetValue((object) key, (object) vector2.x);
        fieldInfo2.SetValue((object) key, (object) vector2.y);
      }
    }
  }
}

*/