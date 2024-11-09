﻿using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Grammar;
using RimWorld;
using Verse;

namespace ProjectAnomalySyndicate.Quests
{
    public class QuestNode_Fingerspike : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> inSignal;

        public SlateRef<string> customLetterLabel;
            
        public SlateRef<string> customLetterText;

        public SlateRef<RulePack> customLetterLabelRules;

        public SlateRef<RulePack> customLetterTextRules;

        public SlateRef<IntVec3?> walkInSpot;
        public PawnKindDef animalKind;

        [NoTranslate]
        public SlateRef<string> tag;

        private const string RootSymbol = "root";

        protected override bool TestRunInt(Slate slate)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return false;
            }
            if (!Find.Storyteller.difficulty.allowViolentQuests)
            {
                return false;
            }
            if (!slate.Exists("map"))
            {
                return false;
            }
            return true;
        }

        protected override void RunInt()
        {
            animalKind = PawnKindDefOf.Fingerspike;
            Slate slate = QuestGen.slate;
            Map map = QuestGen.slate.Get<Map>("map");
            float points = QuestGen.slate.Get("points", 0f);
            QuestPart_Incident questPart_Incident = new QuestPart_Incident();
            questPart_Incident.incident = IncidentDefOf.ManhunterPack;
            IncidentParms incidentParms = new IncidentParms();
            incidentParms.forced = true;
            incidentParms.target = map;
            incidentParms.points = points;
            incidentParms.questTag = QuestGenUtility.HardcodedTargetQuestTagWithQuestID(tag.GetValue(slate));
            incidentParms.spawnCenter = walkInSpot.GetValue(slate) ?? QuestGen.slate.Get<IntVec3?>("walkInSpot") ?? IntVec3.Invalid;
            incidentParms.pawnCount = 1;
            incidentParms.pawnKind = animalKind;
            slate.Set("animalKindDef", animalKind);
            int num = ((incidentParms.pawnCount > 0) ? incidentParms.pawnCount : AggressiveAnimalIncidentUtility.GetAnimalsCount(animalKind, points));
            QuestGen.slate.Set("animalCount", num);
            if (!customLetterLabel.GetValue(slate).NullOrEmpty() || customLetterLabelRules.GetValue(slate) != null)
            {
                QuestGen.AddTextRequest("root", delegate (string x)
                {
                    incidentParms.customLetterLabel = x;
                }, QuestGenUtility.MergeRules(customLetterLabelRules.GetValue(slate), customLetterLabel.GetValue(slate), "root"));
            }
            if (!customLetterText.GetValue(slate).NullOrEmpty() || customLetterTextRules.GetValue(slate) != null)
            {
                QuestGen.AddTextRequest("root", delegate (string x)
                {
                    incidentParms.customLetterText = x;
                }, QuestGenUtility.MergeRules(customLetterTextRules.GetValue(slate), customLetterText.GetValue(slate), "root"));
            }
            questPart_Incident.SetIncidentParmsAndRemoveTarget(incidentParms);
            questPart_Incident.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
            QuestGen.quest.AddPart(questPart_Incident);
            List<Rule> rules = new List<Rule>
        {
            new Rule_String("animalKind_label", animalKind.label),
            new Rule_String("animalKind_labelPlural", animalKind.GetLabelPlural(num))
        };
            QuestGen.AddQuestDescriptionRules(rules);
            QuestGen.AddQuestNameRules(rules);
        }
    }
}
