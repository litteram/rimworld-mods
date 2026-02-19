using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OrganizedResearchTech
{
    public static class ResearchWeights
    {
        private static Dictionary<string, int> _weights;

        public static Dictionary<string, int> weigts
        {
            get
            {
                if (_weights == null)
                {
                    return new Dictionary<string, int>();
                }
                return _weights;
            }
        }

        public static void Initialize(OrganizedResearchTechSettings settings)
        {
            _weights = settings.ResearchWeights;
        }
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
