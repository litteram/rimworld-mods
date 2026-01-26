using System.Collections.Generic;
using System.Linq;
using RimWorld;
using VanillaPsycastsExpanded;
using VEF.Abilities;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace PsycasterGeneSpawner
{
    [StaticConstructorOnStartup]
    public static class Utils
    {
        public static List<GeneDef> AvaliablePsycasterGenes { get; } = DefDatabase<PsycasterPathDef>
            .AllDefsListForReading
            .Select(path => path.requiredGene)
            .ToList();

        public static GeneDef GetRandomPsycasterGene(Pawn pawn)
        {
            return pawn.genes.GenesListForReading
                .Select(gene => gene.def)
                .Intersect(AvaliablePsycasterGenes)
                .RandomElementWithFallback(null);
        }

        public static Hediff_PsycastAbilities GivePsylink(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier) is not Hediff_Psylink psylink)
            {
                psylink = HediffMaker
                        .MakeHediff(HediffDefOf.PsychicAmplifier, pawn, pawn.health.hediffSet.GetBrain()) as
                    Hediff_Psylink;
                pawn.health.AddHediff(psylink);
            }

            Hediff_PsycastAbilities implant =
                pawn.health.hediffSet.GetFirstHediffOfDef(VPE_DefOf.VPE_PsycastAbilityImplant) as
                    Hediff_PsycastAbilities ??
                HediffMaker.MakeHediff(VPE_DefOf.VPE_PsycastAbilityImplant, pawn, pawn.health.hediffSet.GetBrain()) as
                    Hediff_PsycastAbilities;

            if (implant?.psylink == null)
                implant?.InitializeFromPsylink(psylink);

            return implant;
        }

        public static void GivePsycasterPath(Pawn pawn, GeneDef psycasterGene, Hediff_PsycastAbilities implant)
        {
            PsycasterPathDef path =
                DefDatabase<PsycasterPathDef>.AllDefsListForReading
                    .Where(path => path.requiredGene == psycasterGene)
                    .FirstOrFallback(null);

            if (path == null)
            {
                Log.Message(
                    $"{psycasterGene.defName} - Does not have an correspondent in DefDatabase<PsycasterPathDef>");
                return;
            }

            if (!implant.unlockedPaths.Contains(path))
            {
                implant.UnlockPath(path);
            }

            CompAbilities comp = pawn.GetComp<CompAbilities>();
            if (comp == null) return;

            AbilityDef mainAbility = path.abilityLevelsInOrder.First().RandomElement();
            if (mainAbility != null)
            {
                comp.GiveAbility(mainAbility);
                implant.ChangeLevel(1, false);
            }

            var r = Rand.Value;
            if (r > PsycastsMod.Settings.additionalAbilityChance)
            {
                implant.points = 0;
                return;
            }

            var abilityNumber = (int)(PsycastsMod.Settings.additionalAbilityChance/r)+1;
            foreach (AbilityDef[] level in path.abilityLevelsInOrder)
            {
                if (abilityNumber == 0) break;
                foreach (AbilityDef ab in level)
                {
                    if (abilityNumber == 0) break;
                    if (comp.HasAbility(ab))
                    {
                        continue;
                    }

                    comp.GiveAbility(ab);
                    implant.ChangeLevel(1, false);
                    abilityNumber--;
                }
            }

            implant.points = 0;
        }
    }
}