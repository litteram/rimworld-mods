using System.Collections.Generic;
using Verse;
using RimWorld;

namespace OrganizedResearchTech
{
    public class ResearchComparer : IComparer<ResearchTabDef>
    {
        public int Compare(ResearchTabDef x, ResearchTabDef y)
        {
            if (x == y) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            int xIndex = ResearchTabs.Tabs?.IndexOf(x) ?? -1;
            int yIndex = ResearchTabs.Tabs?.IndexOf(y) ?? -1;

            if (xIndex == -1 && yIndex == -1) return 0;
            if (xIndex == -1) return 1;
            if (yIndex == -1) return -1;

            return xIndex.CompareTo(yIndex);
        }
    }
}
