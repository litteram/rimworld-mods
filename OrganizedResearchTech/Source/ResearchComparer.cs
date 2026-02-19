using System.Collections.Generic;
using RimWorld;

namespace OrganizedResearchTech
{
    public static class ResearchWeights
    {
        public static Dictionary<string, int> weigts = new Dictionary<string, int>
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
        
        // public static List<string> testData = new List<string>{"Main", "Anomaly", "SRT_BasicsResearch", "SRT_NeolithicResearch", "SRT_MedievalResearch", "SRT_IndustrialResearch", "SRT_HighTechResearch",  "SRT_SiliconResearch", "SRT_UltraResearch"};
    }

    public class ResearchComparer: IComparer<ResearchTabDef>
    {
        public int Compare(ResearchTabDef x, ResearchTabDef y)
        {
            // x = y -> 0
            // x < y -> -1
            // x > y -> 1

            if (x == y) return 0;

            if (x == null) return 1;
            if (!ResearchWeights.weigts.ContainsKey(x.defName))
                return ResearchWeights.weigts.ContainsKey(y.defName) ? 1 : 0;

            if (y == null) return -1;
            if (!ResearchWeights.weigts.ContainsKey(y.defName))
                return ResearchWeights.weigts.ContainsKey(x.defName) ? -1 : 0;

            if (ResearchWeights.weigts[x.defName] < ResearchWeights.weigts[y.defName])
              return -1;
            if (ResearchWeights.weigts[x.defName] > ResearchWeights.weigts[y.defName])
                return 1;

            return 0;
        }
    }
}
