using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Configuration;
using YGOSharp.OCGWrapper;
using YGOSharp.Network.Enums;
using WindBot.Game.AI.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("Floowandereeze", "AI_Floowandereeze")]
    class FloowandereezeExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Snowl = 53212882;
            public const int Empen = 80611581;
            public const int RaizaMegaMonarch = 69327790;
            public const int Avian = 29587993;
            public const int DimensionShifter = 91800273;
            public const int Eglen = 54334420;
            public const int Stri = 80433039;
            public const int Robina = 18940725;
            public const int Toccan = 17827173;

            // spells
            public const int HarpiesFeatherDuster = 18144507;
            public const int TrippleTacticsTalent = 25311006;
            public const int TrippleTacticsThrust = 35269904;
            public const int Terraforming = 73628505;
            public const int PotOfProsperity = 84211599;
            public const int PotOfDuality = 98645731;
            public const int BookOfMoon = 14087893;
            public const int AdventOfAdventure = 69087397;
            public const int UnexploredWinds = 55521751;

            // Traps
            public const int MagnificentMap = 28126717;
            public const int DreamingTown = 41215808;
            public const int HarpiesFeatherStorm = 87639778;

            // extra
            public const int Zeus = 90448279;
            public const int JuggernautLiebe = 26096328;
            public const int GustavMax = 56910167;
            public const int DowneredMagician = 72167543;
            public const int SlackerdMagician = 58058134;
            public const int AssembeledNightingale = 48608796;
            public const int UnderworldGoddess = 98127546;
            public const int Linkuriboh = 41999284;
            public const int Fucho = 27240101;
            public const int SkStrikerAceZeke = 75147529;
        }

        // summon actions
        private bool robina_NormalSummonEffectActivated = false,
                    eglen_NormalSummonEffectActivated = false,
                    stri_NormalSummonEffectActivated = false,
                    toccan_NormalSummonEffectActivated = false,
                    ActivateempenBattleEffect = false;

        // spell actions
        private bool PotOfProsperityActivated = false;

        // battle phase Actions
        private bool SlackerMagicianAttacks = false;

        //xyz activations
        private bool ZeusActivated = false;

        public FloowandereezeExecutor(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, BookOfMoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfProsperity, PotOfProsperityActivate);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingActivate);
            AddExecutor(ExecutorType.Activate, CardId.TrippleTacticsThrust, TrippleTacticsThrustActivate);
            AddExecutor(ExecutorType.Activate, CardId.TrippleTacticsTalent, TrippleTacticsTalentActivate);

            // XYZ Summons
            AddExecutor(ExecutorType.SpSummon, CardId.SlackerdMagician, SlackerdMagicianSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DowneredMagician, DowneredMagicianSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.GustavMax, JuggernautGustavMaxOverlay);
            AddExecutor(ExecutorType.Activate, CardId.GustavMax);
            AddExecutor(ExecutorType.SpSummon, CardId.JuggernautLiebe, JuggernautLiebeSummon);
            AddExecutor(ExecutorType.Activate, CardId.JuggernautLiebe);
            AddExecutor(ExecutorType.SpSummon, CardId.Zeus, ZeusSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.Zeus, ZeusEffect);

            AddExecutor(ExecutorType.Summon, CardId.Snowl, SnowlSummon);
            AddExecutor(ExecutorType.Summon, CardId.Avian, AvianSummon);
            AddExecutor(ExecutorType.Summon, CardId.Empen, EmpenSummon);
            AddExecutor(ExecutorType.Summon, CardId.RaizaMegaMonarch, RaizaMegaMonarchSummon);

            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, Active_MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.AdventOfAdventure, AdventOfAdventureEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnexploredWinds, UnexploredWindsActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.DreamingTown, DreamingTownSet);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown, DreamingTownActivate);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown, DreamingTownGraveyardActivate);

            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherStorm);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherStorm, HarpiesFeatherStormActivate);

            AddExecutor(ExecutorType.Activate, CardId.Snowl, SnowlActivate);
            AddExecutor(ExecutorType.Activate, CardId.Avian, AvianActivate);

            AddExecutor(ExecutorType.Summon, CardId.Robina, NormalSummonRobina);
            AddExecutor(ExecutorType.Summon, CardId.Eglen, EglenSummon);

            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenEffect);
            AddExecutor(ExecutorType.Activate, CardId.Toccan, ToccanEffect);
            AddExecutor(ExecutorType.Activate, CardId.Stri, StriEffect);

            // Chain block for strongest effect monster
            AddExecutor(ExecutorType.Activate, CardId.Empen, EmpenEffect);

            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, ToccanBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Stri, StriBanishedEffect);
        }

        private bool EmpenSummon()
        {
            return UnexploredWindsTribute();
        }

        // Prefer always first if RPS is won
        public override bool OnSelectHand()
        {
            return true;
        }

        private bool NormalSummonRobina()
        {
            return !robina_NormalSummonEffectActivated;
        }

        private bool HarpiesFeatherStormActivate()
        {
            return Duel.Player == 1; // activate only on opponent's turn
        }

        public override BattlePhaseAction OnBattle(IList<ClientCard> attackers, IList<ClientCard> defenders)
        {
            foreach (ClientCard card in attackers)
            {
                if (card.IsCode(CardId.Empen))
                {
                    ClientCard empenTarget = defenders.GetHighestAttackMonster();
                    if (empenTarget != null && OnPreBattleBetween(card, empenTarget))
                        return AI.Attack(card, empenTarget);
                }
                else if (card.IsCode(CardId.SlackerdMagician))
                {
                    ClientCard slackerTarget = defenders.GetLowestAttackMonster();

                    if (slackerTarget != null && OnPreBattleBetween(card,slackerTarget))
                        return AI.Attack(card, slackerTarget);
                }
            }

            return base.OnBattle(attackers, defenders);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            if (attacker.IsCode(CardId.Empen) && defender.RealPower > attacker.RealPower)
            {
                int empenPower = attacker.Attack + defender.RealPower;
                int target_preEffectPower = (defender.Attack + defender.RealPower) / 2;

                return (target_preEffectPower < empenPower) && Bot.Hand.Count() > 0;
            }
                

            if (attacker.IsCode(CardId.SlackerdMagician) && defender.RealPower > attacker.RealPower)
            {
                int potentialSelfDamage = (defender.Attack + defender.RealPower) - (attacker.Attack + attacker.RealPower);
                SlackerMagicianAttacks = Bot.LifePoints > potentialSelfDamage ? true : false;
                return Bot.LifePoints > potentialSelfDamage;
            }

            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool ZeusSpSummon()
        {
            AI.SelectPosition(CardPosition.Attack);
            AI.SelectPlace(Zones.ExtraMonsterZones);

            return true;
        }

        private bool DreamingTownActivate()
        {
            if (Bot.HasInHand(CardId.Robina) && !robina_NormalSummonEffectActivated)
                AI.SelectCard(CardId.Robina);

            return Card.Location == CardLocation.SpellZone
                    && Duel.Player == 1
                    && Bot.Hand.ContainsMonsterWithLevel(1);
        }

        private bool DowneredMagicianSpSummon()
        {
            AI.SelectPosition(CardPosition.Attack);
            AI.SelectPlace(Zones.ExtraMonsterZones);

            return SlackerMagicianAttacks;
        }

        private bool StriBanishedEffect()
        {
            ClientCard robina = Bot.Banished.GetFirstMatchingCard(card => card.IsCode(CardId.Robina));

            if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                return true;

            return false;
        }

        private bool StriEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {

                List<int> prefferedCards = new List<int>()
                {
                    CardId.UnexploredWinds,
                    CardId.AdventOfAdventure,
                    CardId.MagnificentMap,
                    CardId.Robina,
                    CardId.Eglen,
                    CardId.Empen,
                    CardId.Avian,
                    CardId.Snowl,
                    CardId.Toccan
                };

                AI.SelectCard(prefferedCards);

                int[] hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.Level > 7).Select(c => c.Id).ToArray();

                List<int> materials = new List<int>();
                materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                                .Select(x => x.Id)
                                .ToList();

                // Tribute opponent's card using Unexplored winds
                if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null && Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 0)
                {
                    AI.SelectOption(1);

                    materials.Clear();
                    materials.Add(Bot.MonsterZone.GetFirstMatchingCard(card => card.Level == 1).Id);
                    materials.Add(Util.GetBestEnemyCard().Id);
                }

                // summon boss monsters
                if (hand_bossMonsters.Count() > 0 && materials.Count() > 1)
                {
                    AI.SelectYesNo(true);

                    // Summon empen
                    if (Bot.HasInHand(CardId.Empen)) AI.SelectNextCard(CardId.Empen);
                    else if (Bot.HasInHand(CardId.Avian)) AI.SelectNextCard(CardId.Avian);
                    else if (Bot.HasInHand(CardId.Snowl)) AI.SelectNextCard(CardId.Snowl);

                    AI.SelectMaterials(materials);
                }
                else AI.SelectYesNo(false);

                stri_NormalSummonEffectActivated = true;

                return true;
            }

            return false;
        }

        private bool TrippleTacticsTalentActivate()
        {
            if (Bot.MonsterZone.Count() > 1)
                AI.SelectOption(1);
            else if (Util.IsAllEnemyBetter())
                AI.SelectOption(2);
            else
            {
                AI.SelectOption(3);
                AI.SelectNextCard(Enemy.Hand.GetDangerousMonster());
            }

            return true;
        }

        private bool TrippleTacticsThrustActivate()
        {
            if (Bot.Deck.GetCardCount(CardId.TrippleTacticsTalent) != 0) AI.SelectCard(CardId.TrippleTacticsTalent);
            else if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina))) AI.SelectCard(CardId.AdventOfAdventure);
            else if (!Bot.HasInSpellZone(CardId.MagnificentMap)) AI.SelectCard(CardId.MagnificentMap);
            else AI.SelectCard(new List<int> { CardId.BookOfMoon, CardId.HarpiesFeatherDuster, CardId.UnexploredWinds });

            return true;
        }

        private bool SlackerdMagicianSpSummon()
        {
            if (Bot.HasInMonstersZone(CardId.SlackerdMagician) || Bot.HasInMonstersZone(CardId.Zeus)) return false;

            AI.SelectPlace(Zones.ExtraMonsterZones);
            AI.SelectPosition(CardPosition.Attack);

            if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
            {
                SlackerMagicianAttacks = true;
                return Card.Attacked;
            }

            return true;
        }

        private bool RaizaMegaMonarchSummon()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RaizaMegaMonarch,0) && Util.IsAllEnemyBetter() && Enemy.GetFieldCount() > 1)
            {
                if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
                    return UnexploredWindsTribute();
            }

            return false;
        }

        private bool AvianSummon()
        {
            if(Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
                return UnexploredWindsTribute();

            return false;
        }

        private bool SnowlSummon()
        {
            if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
                return UnexploredWindsTribute();

            return false;
        }

        private bool UnexploredWindsTribute()
        {
            ClientCard enemyCard = Util.GetBestEnemyCard();
            ClientCard myMonster = Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.Level == 1 && card.HasRace(CardRace.WindBeast));

            if (Bot.HasInSpellZone(CardId.UnexploredWinds) && enemyCard != null && myMonster != null)
            {
                AI.SelectOption(1);
                List<int> materials = new List<int>
                {
                    enemyCard.Id,
                    myMonster.Id
                };

                // tribute summon using opponent's cards
                AI.SelectMaterials(materials);

                return true;
            }

            return false;
        }

        private bool UnexploredWindsActivate()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            else if (Card.Location == CardLocation.SpellZone)
            {
                List<int> reveal = Bot.Hand.GetMatchingCards(card => card.Level > 5 && card.HasRace(CardRace.WindBeast)).Select(c => c.Id).ToList();

                if (!Bot.Hand.ContainsMonsterWithLevel(1) && reveal.Count() > 0)
                {
                    AI.SelectCard(reveal);
                    return true;
                }
            }

            return false;
        }

        private bool EglenSummon()
        {
            return Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 1;
        }

        private bool SnowlActivate()
        {
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2) && !(robina_NormalSummonEffectActivated || eglen_NormalSummonEffectActivated))
                return true;
            else if ((Duel.Player == 1 && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2 || Duel.Phase == DuelPhase.Battle) && ActivateDescription == Util.GetStringId(CardId.Snowl, 0)))
                return true;

            return false;
        }

        private bool PotOfProsperityActivate()
        {
            if (Bot.ExtraDeck.Count <= 3) return false;

            List<int> cardsId = new List<int>();

            if (!Bot.HasInHand(CardId.Robina) || !Bot.HasInBanished(CardId.Robina))
                cardsId.Add(CardId.Robina);
            if (Bot.HasInBanished(CardId.DreamingTown))
                cardsId.Add(CardId.Toccan);
            if (Bot.HasInGraveyard(CardId.Empen) || Bot.HasInGraveyard(CardId.Snowl) || Bot.HasInGraveyard(CardId.Avian) || Bot.HasInGraveyard(CardId.RaizaMegaMonarch))
                cardsId.Add(CardId.Stri);

            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.DreamingTown) || !Bot.HasInBanished(CardId.DreamingTown))
                cardsId.Add(CardId.DreamingTown);
            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.UnexploredWinds) || !Bot.HasInBanished(CardId.UnexploredWinds))
                cardsId.Add(CardId.UnexploredWinds);
            if (!Bot.HasInHandOrInSpellZone(CardId.MagnificentMap))
            {
                cardsId.Add(CardId.Terraforming);
                cardsId.Add(CardId.MagnificentMap);
            }

            PotOfProsperityActivated = true;

            List<ProsperityMaterials> extraDeckMaterials = new List<ProsperityMaterials>
            {
                new ProsperityMaterials { id = CardId.SlackerdMagician, preserveCopy = true },
                new ProsperityMaterials { id = CardId.Fucho, preserveCopy = false },
                new ProsperityMaterials { id = CardId.AssembeledNightingale, preserveCopy = false },
                new ProsperityMaterials { id = CardId.SkStrikerAceZeke, preserveCopy = false },
                new ProsperityMaterials { id = CardId.Linkuriboh, preserveCopy = false },
                new ProsperityMaterials { id = CardId.DowneredMagician, preserveCopy = true }
            };

            return PotOfProsperityFunction(extraDeckMaterials, cardsId);
        }

        public class ProsperityMaterials
        {
            public int id { get; set; }
            public bool preserveCopy { get; set; }
        }

        private bool PotOfProsperityFunction(List<ProsperityMaterials> extraDeckMonsters, List<int> cardExavationPriority)
        {
            // select materials
            List<int> banishList = new List<int>();



            foreach (var card in extraDeckMonsters)
            {
                if (card.preserveCopy)
                {
                    if (Bot.ExtraDeck.GetCardCount(card.id) > 1)
                        banishList.Add(card.id);
                }
                else banishList.Add(card.id);
            }

            if (banishList.Count > 3)
                AI.SelectOption(1); // Banish 6
            else AI.SelectOption(0); // banish 3

            if (banishList.Count() > 3)
            {
                AI.SelectCard(banishList);
                AI.SelectNextCard(cardExavationPriority);
                return true;
            }

            return false;
        }

        private bool AvianActivate()
        {
            if (Duel.LastChainPlayer == 1)
            {
                if (Bot.HasInMonstersZone(CardId.Empen)) AI.SelectCard(Util.GetBestEnemySpell(true));
                else AI.SelectCard(Util.GetBestEnemyCard(true));

                return true;
            }

            return false;
        }

        private bool TerraformingActivate()
        {
            if (!Bot.HasInHandOrInSpellZone(CardId.MagnificentMap))
                return true;

            return false;
        }

        private bool ZeusEffect()
        {
            if (ZeusActivated && Duel.Player == 0) return false;

            ZeusActivated = true;

            ClientCard enemy_bestCard = Util.GetBestEnemyCard();

            if (enemy_bestCard != null)
            {
                if (Duel.Player == 1 && enemy_bestCard.Attack > Card.Attack)
                    return true;

                return true;
            }

            return false;
        }

        private bool JuggernautLiebeSummon()
        {
            AI.SelectPlace(Zones.ExtraMonsterZones);
            return true;

        }

        private bool BookOfMoonActivate()
        {
            if (!(Duel.Player == 0)) return false;
            if(!(Util.IsChainTarget(Card) && Duel.CurrentChain.Contains(Card) && Duel.LastChainPlayer == 1)) return false;

            ClientCard enemy_bestCard = Util.GetBestEnemyMonster(true, true);

            if (enemy_bestCard != null) {
                AI.SelectCard(enemy_bestCard);
                return true;
            }

            return false;
        }

        private bool FuchoSpSummon()
        {
            if (Bot.HasInExtra(CardId.AssembeledNightingale)) return false;

            AI.SelectPlace(Zones.ExtraMonsterZones);
            AI.SelectPosition(CardPosition.Attack);
            return !Bot.HasInMonstersZone(CardId.Fucho);
        }

        private bool JuggernautGustavMaxOverlay()
        {
            AI.SelectPlace(Zones.ExtraMonsterZones);
            return true;
        }

        
        public override void OnNewTurn()
        {
            // engine monsters
            robina_NormalSummonEffectActivated = false;
            eglen_NormalSummonEffectActivated = false;
            PotOfProsperityActivated = false;
            stri_NormalSummonEffectActivated = false;
            toccan_NormalSummonEffectActivated = false;

            //Battle monsters
            ActivateempenBattleEffect = false;

            // Battle phase actions
            SlackerMagicianAttacks = false;

            // XYZ Activation
            ZeusActivated = false;

            base.OnNewTurn();
        }

        private bool DreamingTownGraveyardActivate()
        {
            return Card.Location == CardLocation.Grave
                    && Util.IsAllEnemyBetter(true)
                    && Duel.Player == 1;
        }

        private bool ToccanBanishedEffect()
        {
            ClientCard robina = Bot.Banished.GetFirstMatchingCard(card => card.IsCode(CardId.Robina));
            ClientCard eglen = Bot.Banished.GetFirstMatchingCard(card => card.IsCode(CardId.Eglen));


            if (Duel.CurrentChain.Contains(robina) || Duel.CurrentChain.Contains(eglen))
            {
                if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                    return true;
            }

            return false;
        }

        private bool ToccanEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.HasInBanished(CardId.UnexploredWinds))
                    AI.SelectCard(CardId.UnexploredWinds);
                else if (Bot.HasInBanished(CardId.DreamingTown))
                    AI.SelectCard(CardId.DreamingTown);
                else
                {
                    AI.SelectNextCard(new int[] {
                                CardId.AdventOfAdventure,
                                CardId.Empen,
                                CardId.Avian,
                                CardId.MagnificentMap
                            });

                    toccan_NormalSummonEffectActivated = true;
                    return true;
                }

                // Normal Summon again?
                int[] hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.Level > 7).Select(c => c.Id).ToArray();
                int[] materials = Bot.Hand.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                // summon boss monsters
                if (hand_bossMonsters.Count() > 0 && materials.Count() > 1)
                {
                    AI.SelectYesNo(true);

                    AI.SelectCard(hand_bossMonsters);
                    AI.SelectMaterials(Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray());
                } else if (Bot.Deck.GetCardCount(CardId.Stri) > 0
                        && (Bot.HasInGraveyard(CardId.DreamingTown)
                            || Bot.HasInGraveyard(CardId.UnexploredWinds)
                            || Bot.HasInGraveyard(CardId.AdventOfAdventure)
                            || Bot.HasInGraveyard(CardId.Empen)
                            || Bot.HasInGraveyard(CardId.Avian)
                            || Bot.HasInGraveyard(CardId.MagnificentMap)
                       ))
                {
                    AI.SelectYesNo(true);
                    AI.SelectCard(CardId.Stri);
                }
                else AI.SelectYesNo(false);
            }

            return false;
        }

        private bool Active_MagnificentMapActivate()
        {
            if (Card.Location == CardLocation.SpellZone && !(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina)))
            {
                int[] reveal = Bot.Hand.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                if (!Bot.HasInHand(CardId.Robina))
                {
                    AI.SelectCard(reveal);
                    AI.SelectNextCard(CardId.Robina);
                }

                return true;
            }
            else if (Card.Location == CardLocation.SpellZone && Bot.HasInHandOrHasInMonstersZone(CardId.Robina))
            {
                // reveal robina
                AI.SelectCard(CardId.Robina);

                if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Toccan) || Bot.HasInBanished(CardId.Toccan))) AI.SelectNextCard(CardId.Toccan);
                else if (eglen_NormalSummonEffectActivated && !Bot.HasInHand(CardId.Eglen)) AI.SelectNextCard(CardId.Eglen);
                else AI.SelectNextCard(CardId.Stri);

                return true;
            }

            return false;
        }

        private bool MagnificentMapActivate()
        {
            return !Bot.HasInSpellZone(CardId.MagnificentMap) && Card.Location == CardLocation.Hand;
        }

        private bool AdventOfAdventureEffect()
        {
            if (Duel.LastChainPlayer == 0 && Duel.CurrentChain.Contains(Bot.SpellZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.MagnificentMap)))) return false;

            if (Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina))
                return false;

            int[] cost;

            if(!Bot.Hand.ContainsMonsterWithLevel(1)) cost = Bot.Hand.GetMatchingCards(card => card.Level > 5 && !card.IsCode(CardId.Robina)).Select(c => c.Id).ToArray();
            else cost = Bot.Hand.GetMatchingCards(card => card.Level == 1 && !(card.IsCode(CardId.Robina) || card.IsCode(CardId.Eglen))).Select(c => c.Id).ToArray();

            if (!Bot.HasInHand(CardId.Robina) && cost.Count() > 0)
            {
                AI.SelectCard(cost); // pay cost
                AI.SelectNextCard(CardId.Robina);

                return true;
            }

            int hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.HasAttribute(CardAttribute.Wind) && card.Level > 6).Count();

            // Search unexplored winds
            if (Bot.HasInHand(CardId.Robina) && Bot.HasInHand(CardId.Eglen) && hand_bossMonsters > 0 && cost.Count() > 0)
            {
                AI.SelectCard(cost); // pay cost

                if(!Bot.HasInHand(CardId.UnexploredWinds))
                    AI.SelectNextCard(CardId.UnexploredWinds);

                if (!Bot.HasInHandOrInSpellZone(CardId.MagnificentMap))
                    AI.SelectNextCard(CardId.MagnificentMap);

                return true;
            }

            return false;
        }

        private bool DreamingTownSet()
        {
            return Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina) || Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen);
        }

        private bool PotOfDualityActivate()
        {
            if (!Bot.HasInHand(CardId.Robina))
            {
                AI.SelectCard(CardId.Robina);
                return true;
            }
            else if (!Bot.HasInHand(CardId.Eglen))
            {
                AI.SelectCard(CardId.Robina);
                return true;
            }
            else if (!Bot.HasInHandOrInSpellZone(CardId.MagnificentMap))
            {
                AI.SelectCard(CardId.MagnificentMap);
                return true;
            }
            else
            {
                int[] otherPrefferedCards = new int[]
                {
                    CardId.Stri,
                    CardId.Toccan
                };

                AI.SelectCard(otherPrefferedCards);
                return true;
            }
        }

        private bool RobinaBanishedEffect()
        {
            ClientCard empen = Bot.MonsterZone.GetFirstMatchingCard(card => card.IsCode(CardId.Empen));

            if (Duel.CurrentChain.Contains(empen) || !Bot.HasInHand(CardId.Robina))
            {
                if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                    return true;
            }

            return false;
        }

        private bool EglenBanishedEffect()
        {
            ClientCard robina = Bot.Banished.GetFirstMatchingCard(card => card.IsCode(CardId.Robina));

            if (Duel.CurrentChain.Contains(robina))
            {
                if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                    return true;
            }

            
            return false;
        }

        private bool RobinaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (!Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen))
                {
                    AI.SelectCard(CardId.Eglen);
                    AI.SelectNextCard(CardId.Eglen);

                    robina_NormalSummonEffectActivated = true;

                    return true;
                }
                else
                {
                    if (eglen_NormalSummonEffectActivated)
                    {
                        int[] hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.Level > 7).Select(c => c.Id).ToArray();

                        List<int> materials = new List<int>();
                        materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                                        .Select(x => x.Id)
                                        .ToList();

                        // Tribute opponent's card using Unexplored winds
                        if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null && Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 0)
                        {
                            AI.SelectOption(1);

                            materials.Clear();
                            materials.Add(Bot.MonsterZone.GetFirstMatchingCard(card => card.Level == 1).Id);
                            materials.Add(Util.GetBestEnemyCard().Id);
                        }

                        // summon boss monsters
                        if (hand_bossMonsters.Count() > 0 && materials.Count() > 1)
                        {
                            AI.SelectYesNo(true);

                            // Summon empen
                            if (Bot.HasInHand(CardId.Empen)) AI.SelectNextCard(CardId.Empen);
                            else if (Bot.HasInHand(CardId.Avian)) AI.SelectNextCard(CardId.Avian);
                            else if (Bot.HasInHand(CardId.Snowl)) AI.SelectNextCard(CardId.Snowl);

                            AI.SelectMaterials(materials);
                        }
                        else AI.SelectYesNo(false);

                        return true;
                    }

                    robina_NormalSummonEffectActivated = true;

                    return true;
                }
            }

            return false;
        }

        private bool EmpenEffect()
        {
            if (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
            {
                List<int> other_prefferedCards = new List<int>();

                other_prefferedCards.AddRange(new List<int> {
                    CardId.UnexploredWinds
                });

                if (!(Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.DreamingTown) || Bot.HasInBanished(CardId.DreamingTown)))
                    AI.SelectCard(CardId.DreamingTown);
                else AI.SelectCard(other_prefferedCards);

                bool decision = Empen_NormalSummonAgain();

                if (decision)
                {
                    AI.SelectYesNo(true);

                    List<int> prefferedMonster = new List<int>();
                    int[] materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                    if (Bot.HasInHand(CardId.Empen)) prefferedMonster.Add(CardId.Empen);
                    else if (Bot.HasInHand(CardId.Snowl)) prefferedMonster.Add(CardId.Snowl);
                    else if (Bot.HasInHand(CardId.Avian)) prefferedMonster.Add(CardId.Avian);
                    else
                        prefferedMonster.AddRange(new List<int> {
                            CardId.RaizaMegaMonarch
                        });

                    AI.SelectNextCard(prefferedMonster);
                    AI.SelectMaterials(materials);
                }
                else AI.SelectYesNo(false);

                return true;
            }
            else
            {
                if (ActivateDescription == Util.GetStringId(CardId.Empen, 1))
                {
                    ClientCard BestMonster = Util.GetBestEnemyMonster();

                    if (BestMonster != null)
                    {
                        // prioritize theres cards as cost (Do not discard Unexplored Winds and Dreaming Town if you can)
                        List<int> cost = new List<int>();

                        if (Bot.Hand.Count() > 2)
                        {
                            cost = Bot.Hand.GetMatchingCards(card => !(card.IsCode(CardId.UnexploredWinds)) || card.IsCode(CardId.DreamingTown))
                                        .Select(c => c.Id)
                                        .ToList();
                        }
                        else cost = Bot.Hand.Select(c => c.Id).ToList();

                        int strongestMonsterAttack = BestMonster.Attack + BestMonster.RealPower;
                        int empenPower = Card.Attack + Card.RealPower;

                        if (cost.Count() > 0 && strongestMonsterAttack >= empenPower)
                        {
                            AI.SelectCard(cost);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool Empen_NormalSummonAgain()
        {
            return (Bot.Hand.ContainsMonsterWithLevel(8) || Bot.Hand.ContainsMonsterWithLevel(7))
                    && Bot.MonsterZone.GetMatchingCardsCount(card => card.Level == 1) > 1;
        }

        private bool EglenEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                List<int> materials = new List<int>();
                materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                                .Select(x => x.Id)
                                .ToList();

                if (!Bot.HasInHandOrHasInMonstersZone(CardId.Empen)
                    || Bot.MonsterZone.GetCardCount(CardId.Empen) > 0 && Duel.Player == 1)
                {
                    // Fetch empen from deck
                    AI.SelectCard(CardId.Empen);

                    // Tribute opponent's card using Unexplored winds
                    if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null && Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 0)
                    {
                        AI.SelectOption(1);

                        materials.Clear();
                        materials.Add(Bot.MonsterZone.GetFirstMatchingCard(card => card.Level == 1).Id);
                        materials.Add(Util.GetBestEnemyCard().Id);
                    }

                    // Summon empen
                    AI.SelectNextCard(CardId.Empen);
                    AI.SelectMaterials(materials);

                    eglen_NormalSummonEffectActivated = true;

                    return true;
                }
                else
                {
                    if (!Bot.HasInHand(CardId.Avian) && Duel.Player == 1)
                    {
                        if (!Bot.HasInHand(CardId.Empen)) AI.SelectCard(CardId.Empen);
                        else AI.SelectCard(CardId.Avian);

                        // Normal summon next monster
                        if (Bot.HasInHand(CardId.Empen)) AI.SelectNextCard(CardId.Empen);
                        else AI.SelectNextCard(CardId.Avian);

                        if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null && Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 0)
                        {
                            AI.SelectOption(2);
                            materials.Clear();
                            materials.Add(Bot.MonsterZone.GetFirstMatchingCard(card => card.Level == 1).Id);
                            materials.Add(Util.GetBestEnemyCard().Id);
                        }

                        AI.SelectMaterials(materials);
                    }
                    else
                    {
                        if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null)
                        {
                            AI.SelectOption(2);
                            materials.Add(Util.GetBestEnemyCard().Id);
                        }
                    }

                    eglen_NormalSummonEffectActivated = true;

                    return true;
                }
            }

            return false;
        }
    }

        

    

}
