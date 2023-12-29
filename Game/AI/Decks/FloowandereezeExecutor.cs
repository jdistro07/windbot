using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

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
        }

        public FloowandereezeExecutor(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter);
            AddExecutor(ExecutorType.Activate, CardId.BookOfMoon, BookOfMoonActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityActivate);
            AddExecutor(ExecutorType.Activate, CardId.PotOfProsperity, PotOfProsperityActivate);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, CardId.TrippleTacticsTalent, TrippleTacticsTalentActivate);


            AddExecutor(ExecutorType.Activate, CardId.AdventOfAdventure, AdventOfAdventureEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, Active_MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnexploredWinds, UnexploredWindsActivate);
            AddExecutor(ExecutorType.Activate, CardId.TrippleTacticsThrust, TrippleTacticsThrustActivate);

            // XYZ Summons
            AddExecutor(ExecutorType.SpSummon, CardId.AssembeledNightingale, AssembeledNightingaleSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DowneredMagician);
            AddExecutor(ExecutorType.SpSummon, CardId.Fucho, FuchoSpSummon);

            AddExecutor(ExecutorType.Activate, CardId.Fucho);
            AddExecutor(ExecutorType.SpSummon, CardId.GustavMax, JuggernautGustavMaxOverlay);
            AddExecutor(ExecutorType.Activate, CardId.GustavMax);
            AddExecutor(ExecutorType.SpSummon, CardId.JuggernautLiebe, JuggernautLiebeSummon);
            AddExecutor(ExecutorType.Activate, CardId.JuggernautLiebe);
            AddExecutor(ExecutorType.SpSummon, CardId.Zeus);
            AddExecutor(ExecutorType.Activate, CardId.Zeus, ZeusEffect);

            AddExecutor(ExecutorType.Summon, CardId.Snowl, SnowlSummon);
            AddExecutor(ExecutorType.Summon, CardId.Avian, AvianSummon);
            AddExecutor(ExecutorType.Summon, CardId.RaizaMegaMonarch, RaizaMegaMonarchSummon);

            AddExecutor(ExecutorType.Activate, CardId.Snowl, SnowlActivate);
            AddExecutor(ExecutorType.Activate, CardId.Avian, AvianActivate);

            AddExecutor(ExecutorType.Summon, CardId.Robina);
            AddExecutor(ExecutorType.Summon, CardId.Eglen, EglenSummon);
            AddExecutor(ExecutorType.Summon, CardId.Toccan, ToccanSummon);


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


            AddExecutor(ExecutorType.SpellSet, CardId.DreamingTown, DreamingTownSet);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown, DreamingTownGraveyardActivate);

            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherStorm);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherStorm);
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
                    CardId.UnexploredWinds
                };

                if (Enemy.Graveyard.GetDangerousMonster() != null)
                    prefferedCards.Add(Enemy.Graveyard.GetDangerousMonster().Id);

                AI.SelectCard(prefferedCards);

                if (Bot.HasInHand(CardId.Toccan) && Bot.Banished.Count() > 0) AI.SelectCard(CardId.Toccan);
                else
                {
                    AI.SelectCard(Bot.Hand.GetMatchingCards(card => card.Level > 5).Select(c => c.Id).ToList());
                    AI.SelectThirdCard(Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToList());
                }

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

        private bool robina_NormalSummonEffectActivated = false,
                    eglen_NormalSummonEffectActivated = false,
                    PotOfProsperityActivated = false;

        private bool TrippleTacticsThrustActivate()
        {
            if (Bot.Deck.GetCardCount(CardId.TrippleTacticsTalent) != 0) AI.SelectCard(CardId.TrippleTacticsTalent);
            else if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina))) AI.SelectCard(CardId.AdventOfAdventure);
            else if (!Bot.HasInSpellZone(CardId.MagnificentMap)) AI.SelectCard(CardId.MagnificentMap);
            else AI.SelectCard(new List<int> { CardId.BookOfMoon, CardId.HarpiesFeatherDuster, CardId.UnexploredWinds });

            return true;
        }

        //public override bool OnSelectYesNo(long desc)
        //{
        //    if (desc == Util.GetStringId(CardId.AssembeledNightingale, 0))
        //    {
        //        activate_pre_PressuredPlanetWraitsoth = true;
        //    }
        //    return base.OnSelectYesNo(desc);
        //}

        private bool AssembeledNightingaleSpSummon()
        {
            if (Bot.HasInMonstersZone(CardId.AssembeledNightingale) || Bot.HasInMonstersZone(CardId.Zeus)) return false;

            AI.SelectPlace(Zones.ExtraMonsterZones);
            AI.SelectPosition(CardPosition.Attack);

            // Direct Attack
            if (Duel.Player == 0 && Duel.Phase == DuelPhase.BattleStart)
                AI.SelectYesNo(true);

            return true;
        }

        private bool RaizaMegaMonarchSummon()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RaizaMegaMonarch,0) && Util.IsAllEnemyBetter() && Enemy.GetFieldCount() > 1)
            {
                // Target 1 card on the field
                AI.SelectCard(Util.GetBestEnemyCard(false, true));

                // Target card on bot's graveyard
                List<int> prefferedCards = Bot.Graveyard.Select(c => c.Id).ToList();
                AI.SelectNextCard(prefferedCards);

                // Target another card if it's tributted with Winged monster
                if (prefferedCards.Count() > 0)
                {
                    AI.OnSelectYesNo(1);
                    AI.SelectNextCard(prefferedCards);
                }
                else AI.OnSelectYesNo(0);

                return true;
            }

            return false;
        }

        private bool AvianSummon()
        {
            return UnexploredWindsTribute();
        }

        private bool SnowlSummon()
        {
            return UnexploredWindsTribute();
        }

        public override int OnSelectOption(IList<long> options)
        {
            // Unexplored Winds Option
            if (options.Count == 2 && options[1] == Util.GetStringId(CardId.UnexploredWinds, 0))
                return 2;
            return base.OnSelectOption(options);
        }


        private bool UnexploredWindsTribute()
        {
            ClientCard enemyCard = Util.GetBestEnemyCard();
            ClientCard myMonster = Util.GetWorstBotMonster(true);

            if (Bot.HasInSpellZone(CardId.UnexploredWinds) && enemyCard != null)
            {
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

        private bool ToccanSummon()
        {
            return !(eglen_NormalSummonEffectActivated || robina_NormalSummonEffectActivated);
        }

        private bool UnexploredWindsActivate()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            else if (Card.Location == CardLocation.SpellZone)
            {
                if (!Bot.Hand.ContainsMonsterWithLevel(1))
                    return true;
            }

            return false;
        }

        private bool EglenSummon()
        {
            if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina)))
                return true;

            return false;
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

            AI.SelectOption(2); // banish 6

            List<int> cardsId = new List<int>();

            if (!Bot.HasInHand(CardId.Robina) || !Bot.HasInBanished(CardId.Robina))
                cardsId.Add(CardId.Robina);
            if (Bot.HasInBanished(CardId.DreamingTown))
                cardsId.Add(CardId.Toccan);
            if (Bot.HasInGraveyard(CardId.Empen) || Bot.HasInGraveyard(CardId.Snowl) || Bot.HasInGraveyard(CardId.Avian) || Bot.HasInGraveyard(CardId.RaizaMegaMonarch))
                cardsId.Add(CardId.Stri);

            if (!Bot.HasInHandOrInSpellZone(CardId.DreamingTown))
                cardsId.Add(CardId.DreamingTown);
            if (!Bot.HasInHandOrInSpellZone(CardId.UnexploredWinds))
                cardsId.Add(CardId.UnexploredWinds);
            if (!Bot.HasInHand(CardId.MagnificentMap))
            {
                cardsId.Add(CardId.Terraforming);
                cardsId.Add(CardId.MagnificentMap);
            }

            AI.SelectCard(cardsId);

            PotOfProsperityActivated = true;
            return true;
        }

        //public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        //{
        //    if (!defender.IsMonsterHasPreventActivationEffectInBattle())
        //    {
        //        if (attacker.Attribute == (int)CardAttribute.Light && Bot.HasInHand(CardId.Honest))
        //            attacker.RealPower = attacker.RealPower + defender.Attack;
        //    }
        //    return base.OnPreBattleBetween(attacker, defender);
        //}

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
            ClientCard enemy_bestCard = Util.GetBestEnemyCard(true, true);

            if (enemy_bestCard != null && Util.IsAllEnemyBetter())
                return true;

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
            robina_NormalSummonEffectActivated = false;
            eglen_NormalSummonEffectActivated = false;
            PotOfProsperityActivated = false;

            base.OnNewTurn();
        }

        private bool DreamingTownGraveyardActivate()
        {
            return Card.Location == CardLocation.Grave && Util.IsAllEnemyBetter() && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2);
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

                int[] preffererdCards = Bot.Banished.GetMatchingCards(card =>
                                                        card.Level > 6
                                                        || card.IsCode(CardId.MagnificentMap)
                                                        || card.IsCode(CardId.UnexploredWinds)
                                                    ).Select(c => c.Id).ToArray();

                if (Bot.HasInBanished(CardId.Robina)) AI.SelectCard(CardId.Robina);


                if (Bot.HasInBanished(CardId.DreamingTown)) AI.SelectCard(CardId.DreamingTown);
                else AI.SelectCard(preffererdCards);


                return true;
            }

            return false;
        }

        private bool Active_MagnificentMapActivate()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                int[] reveal = Bot.Hand.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                if (!Bot.HasInHand(CardId.Robina))
                {
                    AI.SelectCard(reveal);
                    AI.SelectNextCard(CardId.Robina);
                }
                else
                {
                    AI.SelectCard(reveal);

                    if (!(Bot.HasInHandOrInGraveyard(CardId.Toccan) || Bot.HasInBanished(CardId.Toccan)))
                        AI.SelectCard(CardId.Toccan);
                    else AI.SelectNextCard(reveal);
                }

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
            int[] cost;

            if(!Bot.Hand.ContainsMonsterWithLevel(1))
                cost = Bot.Hand.GetMatchingCards(card => card.Level > 5 && !card.IsCode(CardId.Robina)).Select(c => c.Id).ToArray();
            else cost = Bot.Hand.GetMatchingCards(card => card.Level == 1 && !card.IsCode(CardId.Robina)).Select(c => c.Id).ToArray();

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
                if (!Bot.HasInHand(CardId.Eglen) && !eglen_NormalSummonEffectActivated)
                {
                    AI.SelectCard(CardId.Eglen);
                    AI.SelectNextCard(CardId.Eglen);

                    return true;
                }
                else
                {
                    int[] otherPrefferedCards = new int[]
                    {
                        CardId.Stri,
                        CardId.Toccan,
                        CardId.Robina
                    };

                    int[] hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.Level > 5).Select(c => c.Id).ToArray();

                    if (hand_bossMonsters.Count() > 0)
                    {
                        AI.SelectCard(hand_bossMonsters);
                        AI.SelectMaterials(Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray());
                    }
                    else
                    {
                        AI.SelectCard(otherPrefferedCards);
                        AI.SelectNextCard(otherPrefferedCards);
                    }

                    

                    robina_NormalSummonEffectActivated = true;

                    return true;
                }

                
            }

            return false;
        }

        private bool EmpenEffect()
        {
            List<int> other_prefferedCards = new List<int>();

            other_prefferedCards.AddRange(new List<int> {
                CardId.UnexploredWinds,
                CardId.AdventOfAdventure
            });

            if (!(Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.DreamingTown) || Bot.HasInBanished(CardId.DreamingTown)))
            {
                AI.SelectCard(CardId.DreamingTown);

                // decided whether to normal summon or not
                bool decision = Empen_NormalSummonAgain();

                AI.SelectYesNo(decision);

                //SP summon boss monsters
                if (decision && Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 1)
                {
                    List<int> prefferedMonster = new List<int>();
                    int[] materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                    if (Bot.HasInHand(CardId.Avian)) prefferedMonster.Add(CardId.Avian);
                    else if (Bot.HasInHand(CardId.Snowl)) prefferedMonster.Add(CardId.Snowl);
                    else
                        prefferedMonster.AddRange(new List<int> {
                            CardId.RaizaMegaMonarch
                        });

                    AI.SelectNextCard(prefferedMonster);
                }
                else
                {
                    int[] hand_Level1_Monsters = Bot.Hand.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                    if (Bot.HasInHand(CardId.Stri)
                        && (
                            Bot.HasInGraveyard(CardId.Empen)
                            || Bot.HasInGraveyard(CardId.Snowl)
                            || Bot.HasInGraveyard(CardId.Avian)
                            || Bot.HasInGraveyard(CardId.RaizaMegaMonarch)
                            || Bot.HasInBanished(CardId.UnexploredWinds)
                            || Bot.HasInBanished(CardId.DreamingTown)
                        ))
                    {
                        AI.SelectNextCard(CardId.Stri);
                    } else if (
                        Bot.HasInHand(CardId.Stri)
                        && (
                            Bot.HasInBanished(CardId.Empen)
                            || Bot.HasInBanished(CardId.Snowl)
                            || Bot.HasInBanished(CardId.Avian)
                            || Bot.HasInBanished(CardId.RaizaMegaMonarch)
                            || Bot.HasInBanished(CardId.UnexploredWinds)
                            || Bot.HasInBanished(CardId.DreamingTown)
                           ))
                    {
                        AI.SelectNextCard(CardId.Toccan);
                    }

                    else AI.SelectNextCard(CardId.Robina);
                }

                return true;
            }
            else
            {
                AI.SelectCard(other_prefferedCards);
                return true;
            }
        }

        private bool Empen_NormalSummonAgain()
        {
            return Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 1 || !robina_NormalSummonEffectActivated || !eglen_NormalSummonEffectActivated;
        }

        private bool EglenEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                List<int> materials = new List<int>();
                materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                                .Select(x => x.Id)
                                .ToList();

                int[] prefferedCards = new int[]
                {
                    CardId.Snowl
                };

                if (!Bot.HasInHandOrHasInMonstersZone(CardId.Empen))
                {
                    // Fetch empen from deck
                    AI.SelectCard(CardId.Empen);

                    // Summon empen
                    AI.SelectNextCard(CardId.Empen);
                    AI.SelectMaterials(materials);
                    return true;
                }
                else
                {
                    if (!Bot.HasInHand(CardId.Avian))
                    {
                        AI.SelectCard(CardId.Avian);
                        AI.SelectNextCard(CardId.Avian);

                        AI.SelectMaterials(materials);
                    }
                    else
                    {
                        AI.SelectCard(prefferedCards);
                        AI.SelectMaterials(materials);
                    }

                    eglen_NormalSummonEffectActivated = true;

                    return true;
                }
            }

            return false;
        }
    }

        

    

}
