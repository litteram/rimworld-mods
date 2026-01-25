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
                    .FirstOrFallback();

            if (path == null)
            {
                Log.Message(
                    $"{psycasterGene.defName} - Does not have an correspondent in DefDatabase<PsycasterPathDef>");
                return;
            }

            implant.UnlockPath(path);

            CompAbilities comp = pawn.GetComp<CompAbilities>();
            if (comp == null) return;

            if (implant.level == 0) implant.ChangeLevel(1, false);
            AbilityDef mainAbility = path.abilityLevelsInOrder[0].RandomElement();
            if (mainAbility != null)
            {
                comp.GiveAbility(mainAbility);
            }
            implant.ChangeLevel(1, false);

            var r = Rand.Value;
            if (r > PsycastsMod.Settings.additionalAbilityChance)
            {
                implant.points = 0;
                return;
            }


            var ability_number = (int)(r / (PsycastsMod.Settings.additionalAbilityChance / 3)) + 1;
            Log.Message("pawn " + pawn.NameShortColored + " receives " + ability_number + " abilities");

            foreach (AbilityDef[] level in path.abilityLevelsInOrder.Take(ability_number))
            foreach (AbilityDef ab in level)
            {
                if (comp.HasAbility(ab)) {
                    continue;
                }
                
                comp.GiveAbility(ab);
                implant.ChangeLevel(1, false);
            }

            implant.points = 0;
        }
    }
}