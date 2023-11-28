using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("SpiritCharmers", "AI_SDSpiritCharmers")]

    public class SpiritCharmers : DefaultExecutor
    {

        public SpiritCharmers(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.DarkRulerNoMore);
            AddExecutor(ExecutorType.Summon, CardId.FairyTailRella);
            AddExecutor(ExecutorType.SummonOrSet, CardId.BookOfEclipse, BookOfEclipse_Effect);
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.AwakeningOfThePossessed, Activate_AwakeningOfThePossessed);
            AddExecutor(ExecutorType.Activate, CardId.MastersOfSpiritualArts, MastersOfSpiritualArts);

            AddExecutor(ExecutorType.Activate, CardId.AwakeningNefariouserArch, AwakeningNefariouserArch_Effect);
            AddExecutor(ExecutorType.SpSummon, CardId.AwakeningNefariouserArch, AwakeningNefariouserArch_Effect);

            AddExecutor(ExecutorType.Activate, CardId.AwakeningGreaterInari, AwakeningGreaterInari_Effect);
            AddExecutor(ExecutorType.SpSummon, CardId.AwakeningGreaterInari);

            // Field Spells
            AddExecutor(ExecutorType.SpellSet, CardId.Metaverse, Set_Metaverse);
            AddExecutor(ExecutorType.Activate, CardId.Metaverse, GetFieldSpellFromDeck_SummonSpellCaster);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, GetFieldSpellFromDeck_SummonSpellCaster);
            AddExecutor(ExecutorType.Activate, CardId.SecretVillageOfTheSpellcasters, SummonSpellCasters);

            // Standard Summonings
            AddExecutor(ExecutorType.Summon, CardId.HiitaFireCharmer, SummonCharmerForCardAdvantage);
            AddExecutor(ExecutorType.SummonOrSet, CardId.HiitaFireCharmer);

            AddExecutor(ExecutorType.Summon, CardId.WynnWindCharmer, SummonCharmerForCardAdvantage);
            AddExecutor(ExecutorType.SummonOrSet, CardId.WynnWindCharmer);

            AddExecutor(ExecutorType.Summon, CardId.EriaWaterCharmer, SummonCharmerForCardAdvantage);
            AddExecutor(ExecutorType.SummonOrSet, CardId.EriaWaterCharmer);

            AddExecutor(ExecutorType.Summon, CardId.AussaEarthCharmer, SummonCharmerForCardAdvantage);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AussaEarthCharmer);

            //Other spellcaster executors
            AddExecutor(ExecutorType.MonsterSet, CardId.FairyTailSleeper);
            AddExecutor(ExecutorType.Activate, CardId.Aruru);

            // Elemental Traps (Accompanying charmers to the board)
            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualWindArtMiyabi);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualWindArtMiyabi);

            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualFireArtKurenai);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualFireArtKurenai, SpiritualFireArtKurenai_Effect);

            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualEarthArtKuroGane);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualEarthArtKuroGane);

            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualWaterArtAoi);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualWaterArtAoi);

            AddExecutor(ExecutorType.Summon, CardId.FamPossessedEria);
            AddExecutor(ExecutorType.Summon, CardId.FamPossessedAussa);
            AddExecutor(ExecutorType.Summon, CardId.FamPossessedEriaAlt);
            AddExecutor(ExecutorType.Summon, CardId.FamPossessedWynn);
            AddExecutor(ExecutorType.Summon, CardId.HiitaFireCharmer);

            AddExecutor(ExecutorType.Summon, CardId.FairyTailLuna);
            AddExecutor(ExecutorType.Activate, CardId.FairyTailLuna, FairyTailLuna_Effect);

            AddExecutor(ExecutorType.Summon, CardId.DenkoSekka);

            AddExecutor(ExecutorType.Summon, CardId.DarkDoriado, Summon_Doriado);
            AddExecutor(ExecutorType.Activate, CardId.DarkDoriado, Doriado_PendulumEffect);

            // Hand Traps
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, EffectVeiler);

            //Familiar Special Summons
            AddExecutor(ExecutorType.SpSummon, CardId.Jigabyte);
            AddExecutor(ExecutorType.Activate, CardId.Jigabyte);

            AddExecutor(ExecutorType.SpSummon, CardId.Ranryu);
            AddExecutor(ExecutorType.Activate, CardId.Ranryu);

            AddExecutor(ExecutorType.SpSummon, CardId.InariFire);
            AddExecutor(ExecutorType.Activate, CardId.InariFire);

            AddExecutor(ExecutorType.SpSummon, CardId.NefariousArchFiend_Eater);
            AddExecutor(ExecutorType.Activate, CardId.NefariousArchFiend_Eater, NefariousArchFiend_Eater_Effect);

            // Spells
            AddExecutor(ExecutorType.Activate, CardId.TwinTwisters, TwinTwisters);

            //Traps
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfKnowledge, SpellbookOfKnowledge);
            AddExecutor(ExecutorType.SpellSet, CardId.PossessedPartnerships);
            AddExecutor(ExecutorType.Activate, CardId.PossessedPartnerships, Activate_PossessedPartnerships);
            AddExecutor(ExecutorType.SpellSet, CardId.Unpossessed);
            AddExecutor(ExecutorType.Activate, CardId.Unpossessed);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, Set_SolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning);

            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier);

            AddExecutor(ExecutorType.SummonOrSet, CardId.WitchOfTheBlackForest, SummonCharmersFromDeck);
        }

        public bool SummonCharmersFromDeck()
        {
            AI.SelectCard(new[] {
                CardId.AussaEarthCharmer,
                CardId.EriaWaterCharmer,
                CardId.WynnWindCharmer,
                CardId.HiitaFireCharmer
            });

            AI.SelectPosition(CardPosition.FaceUpDefence);

            return true;
        }

        public bool SpiritualFireArtKurenai_Effect()
        {
            IList<ClientCard> summonedMonsters = Bot.MonsterZone.GetMonsters();
            IList<ClientCard> opponentSummonedMonsters = Enemy.MonsterZone.GetMonsters();
            List<ClientCard>  prefferedTarget = new List<ClientCard>();

            ClientCard greaterInari = Bot.MonsterZone.GetFirstMatchingCard(card => card.IsCode(CardId.AwakeningGreaterInari));
            prefferedTarget.Add(greaterInari);

            int monsterAdvantage = summonedMonsters.Count() - opponentSummonedMonsters.Count();

            if (monsterAdvantage > 1 && prefferedTarget.Count() > 0)
            {
                AI.SelectCard(greaterInari);
                return true;
            }

            return false;
        }

        public bool Doriado_PendulumEffect()
        {
            IList<ClientCard> hand_OffensiveSpellCasters = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);
            IList<ClientCard> field_OffensiveSpellCasters = Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);


            if (hand_OffensiveSpellCasters.Count() > 0 || field_OffensiveSpellCasters.Count() > 0)
                return true;
            return false;
        }

        public bool AwakeningGreaterInari_SPSummon()
        {
            IList<ClientCard> opponentMonsters = Enemy.MonsterZone.GetMatchingCards(card => card.HasType(CardType.Monster) && card.HasPosition(CardPosition.FaceUp));

            if (opponentMonsters.Count() > 0 && ActivateDescription == Util.GetStringId(CardId.AwakeningGreaterInari, 0))
            {
                return true;
            }

            return false;
        }

        public bool AwakeningNefariouserArch_Effect()
        {
            var specialSummonCondition = Util.GetStringId(CardId.AwakeningNefariouserArch, 0);
            if (ActivateDescription == specialSummonCondition)
            {
                AI.SelectCard(CardId.AwakeningNefariouserArch);

                IList<ClientCard> PrefferedCandidate = Bot.Graveyard.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster));
                int[] preffered_specialSummonCandidate = PrefferedCandidate.Select(x => x.Id).ToArray();

                IList<ClientCard> monsterOnGrave = Bot.Graveyard.GetMonsters();
                int[] random_SummonCandidate = PrefferedCandidate.Select(x => x.Id).ToArray();

                if (PrefferedCandidate.Count() > 1) AI.SelectNextCard(PrefferedCandidate);
                if (PrefferedCandidate.Count() > 1) AI.SelectNextCard(random_SummonCandidate);

                return true;
            }

            return false;
        }

        public bool AwakeningGreaterInari_Effect()
        {
            ClientCard effectTarget = Enemy.MonsterZone.GetHighestAttackMonster();
            AI.SelectCard(effectTarget);
            return true;
        }

        public bool NefariousArchFiend_Eater_Effect()
        {
            IList<ClientCard> PrefferedCandidate = Bot.MonsterZone.GetMatchingCards(card => card.IsCode(CardId.AwakeningNefariouserArch) || card.IsCode(CardId.AwakeningGreaterInari));
            int[] tributeCandidates = PrefferedCandidate.Select(x => x.Id).ToArray();

            if (PrefferedCandidate.Count() > 0)
            {
                AI.SelectCard(tributeCandidates);
                return true;
            }
                

            return false;
        }

        public bool FairyTailLuna_Effect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.FairyTailLuna, 0))
            {
                int[] searchCandidates = new[] {
                    CardId.FamPossessedAussa,
                    CardId.FamPossessedHiita,
                    CardId.FamPossessedWynn,
                    CardId.FamPossessedEria,
                    CardId.FamPossessedEriaAlt,

                    CardId.FairyTailSleeper,
                    CardId.FairyTailRella
                };

                AI.SelectCard(searchCandidates);

                return true;
            }

            //return Card to hand effect
            int monsterAdvantage = Bot.MonsterZone.GetMonsters().Count() - Enemy.MonsterZone.GetMonsters().Count();

            if (monsterAdvantage > 1 && ActivateDescription == Util.GetStringId(CardId.FairyTailLuna, 1))
            {
                ClientCard problemCard = Util.GetProblematicEnemyCard();
                AI.SelectNextCard(problemCard);

                return true;
            }

            return false;
        }

        public bool SummonCharmerForCardAdvantage()
        {
            IList<ClientCard> myMonsters = Bot.MonsterZone.GetMatchingCards(card => card.BaseAttack >= 1850 && card.HasPosition(CardPosition.Attack));
            bool FaceupAwakeningOfThePossessed = Bot.HasInSpellZone(CardId.AwakeningOfThePossessed);
            bool FieldSpells = Bot.HasInSpellZone(CardId.SecretVillageOfTheSpellcasters) || Bot.HasInSpellZone(CardId.GrandSpiritualArtIchirin);

            if (myMonsters.Count() > 0 || FaceupAwakeningOfThePossessed || FieldSpells)
                return true;

            return false;
        }

        public override void OnNewTurn()
        {
            IList<ClientCard> attackPos_charmers = Bot.MonsterZone.GetMatchingCards(card => card.BaseAttack == 500 && card.BaseDefense == 1500 && card.HasRace(CardRace.SpellCaster) && card.HasPosition(CardPosition.FaceUpAttack));
            IList<ClientCard> active_unpossessed = Bot.SpellZone.GetMatchingCards(card => card.IsCode(CardId.Unpossessed) && card.HasPosition(CardPosition.FaceUp));
            IList<ClientCard> stronger_opponent_monsters = Enemy.MonsterZone.GetMatchingCards(card => card.BaseAttack > 500);

            if ((attackPos_charmers.Count() > 0 || active_unpossessed.Count > 0) && stronger_opponent_monsters.Count() > 0)
            {
                foreach (var charmer in attackPos_charmers)
                {
                    AI.SelectPosition(CardPosition.Defence);
                }
            }
        }

        public bool Activate_PossessedPartnerships()
        {
            IList<ClientCard> candidates = Bot.Hand.GetMatchingCards(card => card.BaseDefense == 1500 && card.HasRace(CardRace.SpellCaster));
            int mySummonedMonsters = Bot.MonsterZone.GetMonsters().Count();

            foreach (var card in Bot.Graveyard.GetMatchingCards(card => card.BaseDefense == 1500 && card.HasRace(CardRace.SpellCaster)))
            {
                candidates.Add(card);
            }

            if (mySummonedMonsters > 0 && candidates.Count() > 0 && !(Enemy.IsFieldEmpty()))
            {
                ClientCard threatCard = Util.GetProblematicEnemyCard();
                int[] spc_summon_candidates = candidates.Select(x => x.Id).ToArray();

                AI.SelectCard(spc_summon_candidates);
                AI.SelectNextCard(threatCard);

                return true;
            }

            return false;
        }

        public bool Summon_Doriado()
        {
            IList<ClientCard> SpellCastersOnField = Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);
            IList<ClientCard> SpellCastersOnHand = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);

            if (SpellCastersOnField.Count() == 0 && SpellCastersOnHand.Count() == 0)
            {
                return true;
            }

            return false;
        }

        public bool SpellbookOfKnowledge()
        {
            IList<ClientCard> tribute = Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 500 && card.BaseDefense == 1500);
            IList<ClientCard> summonedSpellCasters = Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster));

            if (tribute.Count > 0 && summonedSpellCasters.Count() > 1)
                return true;
            return false;
        }

        public bool Set_Metaverse()
        {
            if (Duel.Phase == DuelPhase.Main2) {
                return true;
            }

            return false;
        }

        public bool EffectVeiler()
        {
            if (Duel.LastChainPlayer == 1)
            {
                foreach (ClientCard check in Enemy.GetMonsters())
                {
                    if (check.HasType(CardType.Effect))
                        return true;
                }
            }
            return false;
        }

        public bool BookOfEclipse()
        {
            if (Duel.Phase == DuelPhase.Main1 && Duel.Player == 1)
            {
                return true;
            }

            return false;
        }

        public bool Set_SolemnWarning()
        {
            ClientCard problemEnemyCard = Util.GetProblematicEnemyMonster();

            if ((Duel.Phase == DuelPhase.Main2) || problemEnemyCard != null)
            {
                return true;
            }

            return false;
        }

        public bool SummonSpellCasters()
        {
            IList<ClientCard> offensive_spellCastersOnHand = new List<ClientCard>();
            IList<ClientCard> charmers = new List<ClientCard>();


            offensive_spellCastersOnHand = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack >= 1850);
            charmers = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 500);


            if (offensive_spellCastersOnHand.Count > 0)
            {
                AI.SelectCard(offensive_spellCastersOnHand);
            }
            else
            {
                AI.SelectCard(charmers);
            }

            return true;
        }

        public bool MastersOfSpiritualArts()
        {
            IList<ClientCard> charmers = Bot.Deck.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseDefense == 1500);
            IList<ClientCard> familiarPossessed = Bot.Deck.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);

            if (!Bot.HasInHandOrInSpellZone(CardId.AwakeningOfThePossessed) && !Bot.HasInHandOrInSpellZone(CardId.Unpossessed))  // if none of these cards are already on haand or field
            {
                AI.SelectCard(CardId.AwakeningOfThePossessed); // Card to hand
                AI.SelectNextCard(CardId.Unpossessed); // Card set to Field

            }
            else if (!(Bot.HasInHandOrInSpellZone(CardId.AwakeningOfThePossessed)) && Bot.HasInHandOrInSpellZone(CardId.Unpossessed)) // just unpossessed already on hand or field
            {
                AI.SelectCard(familiarPossessed);
                AI.SelectNextCard(CardId.AwakeningOfThePossessed);
            }
            else if (Bot.HasInHandOrInSpellZone(CardId.AwakeningOfThePossessed) && !(Bot.HasInHandOrInSpellZone(CardId.Unpossessed))) // just Awakening of the Possessed already on hand or field
            {
                AI.SelectCard(familiarPossessed);
                AI.SelectNextCard(CardId.Unpossessed);
            }
            else // if both cards are already on hand or field no need to search for another copy. Get Set Possessed Partneships and add Familiar Possessed spellcaster to hand
            {
                AI.SelectCard(familiarPossessed);
                AI.SelectNextCard(CardId.PossessedPartnerships);
            }

            return true;
        }

        public bool TwinTwisters()
        {
            IList<ClientCard> opponentSpellsTraps = Enemy.SpellZone.GetMatchingCards(card => (card.HasPosition(CardPosition.FaceDown) || card.HasPosition(CardPosition.FaceUp) && card.HasType(CardType.Spell) || card.HasType(CardType.Trap)));
            ClientCard target2 = Enemy.GetFieldSpellCard();

            if (opponentSpellsTraps.Count > 1)
            {
                AI.SelectCard(opponentSpellsTraps);
                AI.SelectNextCard(target2);

                return true;
            }

            return false;
        }

        public bool BookOfEclipse_Effect() {
            IList<ClientCard> enemySummonedCards = Enemy.MonsterZone.GetMatchingCards(card => card.HasPosition(CardPosition.FaceUpAttack) && card.HasType(CardType.Monster));
            IList<ClientCard> mySummonedMonsters = Bot.MonsterZone.GetMatchingCards(card => card.HasType(CardType.Monster));

            if (enemySummonedCards.Count() > mySummonedMonsters.Count())
            {
                return true;
            }

            return false;
        }

        public bool SummonCharmersFromGY()
        {
            AI.SelectCard(new[] {
                    CardId.Jigabyte,
                    CardId.Ranryu,

                    CardId.FamPossessedHiita,
                    CardId.FamPossessedEria,
                    CardId.FamPossessedEriaAlt,
                    CardId.FamPossessedWynn,

                    CardId.AussaEarthCharmer,
                    CardId.EriaWaterCharmer,
                    CardId.WynnWindCharmer,
                    CardId.HiitaFireCharmer,
                });

            return true;
        }

        public bool Activate_AwakeningOfThePossessed()
        {
            IList<ClientCard> hand_offensiveSpellCasters = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);

            if (hand_offensiveSpellCasters.Count() > 0)
                return true;

            return false;
        }

        public bool GetFieldSpellFromDeck_SummonSpellCaster()
        {
            AI.SelectCard(new[]
            {
                CardId.SecretVillageOfTheSpellcasters,
                CardId.GrandSpiritualArtIchirin
            });

            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                var type = card.GetType();

                if (!card.Equals(Card) && card.Level == 4)
                    return true;
            }

            return false;
        }

        public bool AwakeningOfThePossessed_Summoning()
        {
            AI.SelectCard(new[] {
                    CardId.FamPossessedHiita,
                    CardId.FamPossessedEria,
                    CardId.FamPossessedEriaAlt,
                    CardId.FamPossessedWynn,
                    CardId.AussaEarthCharmer,
                    CardId.EriaWaterCharmer,
                    CardId.WynnWindCharmer,
                    CardId.HiitaFireCharmer
                });

            return true;
        }

        public bool LunaPickCharmer()
        {
            if (!Bot.HasInHand(CardId.FamPossessedHiita))
            {
                AI.SelectCard(CardId.FamPossessedHiita);
            }
            else
            {
                AI.SelectCard(CardId.FamPossessedAussa);
            }

            return true;
        }

        public class CardId
        {
            public const int Aruru = 71074418;

            public const int AwakeningNefariouserArch = 65268179;
            public const int AwakeningGreaterInari = 92652813;

            public const int FamPossessedHiita = 04376658;
            public const int FamPossessedAussa = 31887905;
            public const int FamPossessedWynn = 31887905;
            public const int FamPossessedEria = 68881649;
            public const int FamPossessedEriaAlt = 68881650;

            public const int FairyTailSleeper = 42921475;
            public const int FairyTailRella = 52022648;
            public const int FairyTailLuna = 86937530;

            public const int DarkDoriado = 62312469;
            public const int DenkoSekka = 13974207;

            public const int Jigabyte = 40894584;
            public const int Ranryu = 44680819;
            public const int NefariousArchFiend_Eater = 60666820;
            public const int InariFire = 62953041;
            public const int WitchOfTheBlackForest = 78010363;

            public const int WynnWindCharmer = 37744402;
            public const int AussaEarthCharmer = 37970940;
            public const int EriaWaterCharmer = 74364659;
            public const int HiitaFireCharmer = 759393;


            public const int EffectVeiler = 97268402;
            public const int Raigeki = 12580477;
            public const int SpellbookOfKnowledge = 23314220;
            public const int DarkRulerNoMore = 54693926;
            public const int Terraforming = 73628505;
            public const int BookOfEclipse = 35480699;
            public const int TwinTwisters = 43898403;
            public const int MastersOfSpiritualArts = 91530236;
            public const int AwakeningOfThePossessed = 62256492;
            public const int GrandSpiritualArtIchirin = 38057522;
            public const int SecretVillageOfTheSpellcasters = 68462976;

            public const int SpiritualWaterArtAoi = 6540606;
            public const int SpiritualFireArtKurenai = 42945701;
            public const int PossessedPartnerships = 65046521;
            public const int SpiritualEarthArtKuroGane = 70156997;
            public const int SpiritualWindArtMiyabi = 79333300;

            public const int DimensionalBarrier = 83326048;
            public const int Metaverse = 89208725;
            public const int Unpossessed = 25704359;
            public const int SolemnWarning = 84749824;
        }
    }
}
