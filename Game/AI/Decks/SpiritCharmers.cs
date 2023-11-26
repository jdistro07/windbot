using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WindBot.Game.AI;
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
            AddExecutor(ExecutorType.Activate, CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.Activate, CardId.AwakeningOfThePossessed, SummonCharmersAndFamiliars);
            AddExecutor(ExecutorType.Activate, CardId.MastersOfSpiritualArts, MastersOfSpiritualArts);

            // Field Spells
            AddExecutor(ExecutorType.SpellSet, CardId.Metaverse, Set_Metaverse);
            AddExecutor(ExecutorType.Activate, CardId.Metaverse, GetFieldSpellFromDeck_SummonSpellCaster);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, GetFieldSpellFromDeck_SummonSpellCaster);
            AddExecutor(ExecutorType.Activate, CardId.SecretVillageOfTheSpellcasters, SummonSpellCasters);

            // Standard Summonings
            AddExecutor(ExecutorType.SummonOrSet, CardId.HiitaFireCharmer);
            AddExecutor(ExecutorType.SummonOrSet, CardId.WynnWindCharmer);
            AddExecutor(ExecutorType.SummonOrSet, CardId.EriaWaterCharmer);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AussaEarthCharmer);

            //Other spellcaster executors
            AddExecutor(ExecutorType.MonsterSet, CardId.FairyTailSleeper);
            AddExecutor(ExecutorType.Activate, CardId.Aruru);

            // Elemental Traps (Accompanying charmers to the board)
            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualWindArtMiyabi);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualWindArtMiyabi);
            AddExecutor(ExecutorType.SpellSet, CardId.SpiritualFireArtKurenai);
            AddExecutor(ExecutorType.Activate, CardId.SpiritualFireArtKurenai);
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
            AddExecutor(ExecutorType.Activate, CardId.FairyTailLuna);

            AddExecutor(ExecutorType.Summon, CardId.DenkoSekka);
            AddExecutor(ExecutorType.Activate, CardId.DarkDoriado);

            // Hand Traps
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, EffectVeiler);

            //Familiar Special Summons
            AddExecutor(ExecutorType.SpSummon, CardId.Jigabyte);
            AddExecutor(ExecutorType.SpSummon, CardId.Ranryu);
            AddExecutor(ExecutorType.SpSummon, CardId.InariFire);
            AddExecutor(ExecutorType.SpSummon, CardId.NefariousArchFiend_Eater);
            AddExecutor(ExecutorType.SpSummon, CardId.AwakeningNefariouserArch, Summon_AwakeningNefariouserArch);

            //Traps
            AddExecutor(ExecutorType.Activate, CardId.SpellbookOfKnowledge, SpellbookOfKnowledge);
            AddExecutor(ExecutorType.SpellSet, CardId.PossessedPartnerships);
            AddExecutor(ExecutorType.Activate, CardId.PossessedPartnerships);
            AddExecutor(ExecutorType.SpellSet, CardId.Unpossessed);
            AddExecutor(ExecutorType.Activate, CardId.Unpossessed);

            AddExecutor(ExecutorType.SpellSet, CardId.SolemnWarning, Set_SolemnWarning);
            AddExecutor(ExecutorType.Activate, CardId.SolemnWarning);

            AddExecutor(ExecutorType.SpellSet, CardId.DimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier);


            AddExecutor(ExecutorType.SummonOrSet, CardId.WitchOfTheBlackForest, SummonCharmersFromDeck);
            AddExecutor(ExecutorType.SummonOrSet, CardId.WitchOfTheBlackForest, SummonCharmersFromDeck);

            AddExecutor(ExecutorType.SummonOrSet, CardId.BookOfEclipse, SummonCharmersFromDeck);

        }

        public bool SpellbookOfKnowledge()
        {
            IList<ClientCard> tribute = Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 500 && card.BaseDefense == 1500);
            if (tribute.Count > 0)
                return true;
            return false;
        }

        public bool Summon_AwakeningNefariouserArch()
        {
            if ((Bot.Deck.GetMatchingCards(card => (card.IsCode(CardId.AwakeningNefariouserArch))).Count > 0 || Bot.Hand.GetMatchingCards(card => (card.IsCode(CardId.AwakeningNefariouserArch))).Count > 0)
                && Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster)).Count > 0
                && Bot.MonsterZone.GetMatchingCards(card => card.IsCode(CardId.NefariousArchFiend_Eater)).Count > 0)
                return true;

            if (Bot.Graveyard.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster)).Count > 0)
                AI.SelectCard(Bot.Graveyard.GetFirstMatchingCard(card => card.HasRace(CardRace.SpellCaster)).Id);

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
            if (Duel.Phase == DuelPhase.BattleStep)
            {
                return true;
            }

            return false;
        }

        public bool SummonSpellCasters()
        {
            IList<ClientCard> spellCastersOnHand = new List<ClientCard>();

            spellCastersOnHand = Bot.Hand.GetMatchingCards(card => card.HasRace(CardRace.SpellCaster) && card.BaseAttack == 1850);

            if (spellCastersOnHand.Count > 0)
            {
                AI.SelectCard(spellCastersOnHand);
            }

            return true;
        }

        public bool MastersOfSpiritualArts()
        {
            // Check field
            foreach (ClientCard spell in Bot.SpellZone.GetMatchingCards(card => card.HasType(CardType.Spell) || card.HasType(CardType.Trap)))
            {
                if (spell.IsCode(CardId.AwakeningOfThePossessed))
                    AI.SelectCard(CardId.Unpossessed);
                else if(spell.IsCode(CardId.Unpossessed))
                    AI.SelectCard(CardId.AwakeningOfThePossessed);
            }

            //Check hand
            if (Bot.SpellZone.GetMatchingCards(card => card.IsCode(CardId.AwakeningOfThePossessed) || card.IsCode(CardId.Unpossessed)).Count == 0)
            {
                foreach (ClientCard spell in Bot.Hand.GetMatchingCards(card => card.HasType(CardType.Spell) || card.HasType(CardType.Trap)))
                {
                    if (spell.IsCode(CardId.AwakeningOfThePossessed))
                        AI.SelectCard(CardId.Unpossessed);
                    else if (spell.IsCode(CardId.Unpossessed))
                        AI.SelectCard(CardId.AwakeningOfThePossessed);
                }
            }

            return true;
        }

        public bool TwinTwisters()
        {
            AI.SelectCard(CardId.TwinTwisters);

            if (Enemy.GetSpellCount() > 1)
            {
                ClientCard floodGate = Enemy.SpellZone.GetFloodgate();
                ClientCard randomSetCard = Enemy.SpellZone.GetFirstMatchingCard(card => card.HasPosition(CardPosition.FaceDown) || card.HasPosition(CardPosition.FaceUp));

                AI.SelectCard(floodGate);
                AI.SelectCard(randomSetCard);
            }

            return true;
        }

        public bool SummonCharmersFromDeck() {
            AI.SelectCard(new[] {
                CardId.AussaEarthCharmer,
                CardId.EriaWaterCharmer,
                CardId.WynnWindCharmer,
                CardId.HiitaFireCharmer
            });

            return true;
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

        public bool SummonCharmersAndFamiliars()
        {
            foreach (ClientCard card in Bot.Hand.GetMonsters())
            {
                if (!card.Equals(Card) && card.Level == 4)
                    return true;
            }

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
