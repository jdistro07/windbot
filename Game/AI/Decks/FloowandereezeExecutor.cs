using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Configuration;
using YGOSharp.OCGWrapper;
using YGOSharp.Network.Enums;
using WindBot.Game.AI.Enums;
using System.Diagnostics.Eventing.Reader;

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
                    ActivateEmpenBattleEffect = false,
                    ActivateLiebe = false,
                    MagnificentMapActivated = false,
                    Snowl_Activated = false;

        // spell actions
        private bool PotOfProsperityActivated = false;
        private bool BookOfMoonActivated = false;

        // battle phase Actions
        private bool SlackerMagicianAttacks = false;

        //xyz activations
        private bool ZeusActivated = false;

        List<int> Impermanence_list = new List<int>();


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

            // Reposition Monsters
            AddExecutor(ExecutorType.Repos, CardId.Empen, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Snowl, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.RaizaMegaMonarch, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Avian, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.DowneredMagician, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.GustavMax, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.JuggernautLiebe, BossMonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Zeus, BossMonsterRepos);

            AddExecutor(ExecutorType.Repos, CardId.Robina, Level1MonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Eglen, Level1MonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Stri, Level1MonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Toccan, Level1MonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.SlackerdMagician, Level1MonsterRepos);
            AddExecutor(ExecutorType.Repos, CardId.Fucho, Level1MonsterRepos);

            // XYZ Summons
            AddExecutor(ExecutorType.SpSummon, CardId.GustavMax, JuggernautGustavMaxOverlay);
            AddExecutor(ExecutorType.Activate, CardId.GustavMax, GustavMaxActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.JuggernautLiebe, JuggernautLiebeSummon);
            AddExecutor(ExecutorType.Activate, CardId.JuggernautLiebe, JuggernautLiebeActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SlackerdMagician, SlackerdMagicianSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.SlackerdMagician, SlackerdMagicianActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.DowneredMagician, DowneredMagicianSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Zeus, ZeusSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.Zeus, ZeusEffect);

            AddExecutor(ExecutorType.Summon, CardId.Snowl, SnowlSummon);
            AddExecutor(ExecutorType.Summon, CardId.Avian, AvianSummon);
            AddExecutor(ExecutorType.Summon, CardId.Empen, EmpenSummon);
            AddExecutor(ExecutorType.Summon, CardId.RaizaMegaMonarch, RaizaMegaMonarchSummon);

            AddExecutor(ExecutorType.Activate, CardId.RaizaMegaMonarch, RaizaMegaMonarchActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.MagnificentMap, Active_MagnificentMapActivate);
            AddExecutor(ExecutorType.Activate, CardId.AdventOfAdventure, AdventOfAdventureEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnexploredWinds, UnexploredWindsActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.DreamingTown, DreamingTownSet);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown, DreamingTownActivate);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown, DreamingTownGraveyardActivate);

            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherStorm, HarpiesFeatherStormSet);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherStorm, HarpiesFeatherStormActivate);

            AddExecutor(ExecutorType.Activate, CardId.Snowl, SnowlActivate);
            AddExecutor(ExecutorType.Activate, CardId.Avian, AvianActivate);

            AddExecutor(ExecutorType.Summon, CardId.Robina, NormalSummonRobina);
            AddExecutor(ExecutorType.Summon, CardId.Eglen, NormalSummonEglen);

            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenEffect);
            AddExecutor(ExecutorType.Activate, CardId.Toccan, ToccanEffect);
            AddExecutor(ExecutorType.Activate, CardId.Stri, StriEffect);

            // Chain block for strongest effect monster
            AddExecutor(ExecutorType.Activate, CardId.Empen, EmpenEffect);
            AddExecutor(ExecutorType.Activate, CardId.Empen, EmpenBattleEffect);

            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Toccan, ToccanBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Stri, StriBanishedEffect);
        }

        private bool SlackerdMagicianActivate()
        {
            return Util.IsChainTarget(Card);
        }

        private bool EmpenBattleEffect()
        {
            if (!(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
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

                        if (cost.Count() > 0 && ActivateEmpenBattleEffect)
                        {
                            AI.SelectCard(cost);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool RaizaMegaMonarchActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RaizaMegaMonarch, 0))
            {
                if (Enemy.GetFieldCount() > 1)
                {
                    AI.SelectYesNo(true);
                    AI.SelectCard(Util.GetBestEnemyCard());
                }
                else AI.SelectYesNo(false);
            }

            return true;
        }

        private bool JuggernautLiebeActivate()
        {
            return !Card.IsDisabled();
        }

        public int SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && Impermanence_list.Contains(seq)) continue;
                    return zone;
                };
            }

            return 0;
        }

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            foreach (var imperm in cards.GetMatchingCards(card => card.IsCode(10045474)))
            {
                Impermanence_list.Add(10045474); // Imperm ID
            }

            base.OnSelectChain(cards);
        }

        private bool GustavMaxActivate()
        {
            return !Card.IsDisabled();
        }

        private bool Level1MonsterRepos()
        {
            return Util.IsAllEnemyBetter();
        }

        private bool BossMonsterRepos()
        {
            return !Card.HasPosition(CardPosition.FaceUpAttack);
        }

        public override CardPosition OnSelectPosition(int CardID, IList<CardPosition> positions)
        {
            if (CardID == CardId.SlackerdMagician)
            {
                if (!(Bot.HasInExtra(CardId.Zeus) || Bot.HasInExtra(CardId.DowneredMagician)))
                    return CardPosition.FaceUpDefence;
                else return CardPosition.FaceUpAttack;
            }

            return base.OnSelectPosition(CardID, positions);
        }

        private bool HarpiesFeatherStormSet()
        {
            AI.SelectPlace(SelectSTPlace(Card, true));
            return !Bot.HasInSpellZone(CardId.HarpiesFeatherStorm);
        }

        private bool EmpenSummon()
        {
            if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return UnexploredWindsTribute();
            }

            return false;
        }

        // Prefer always first if RPS is won
        public override bool OnSelectHand()
        {
            return true;
        }

        private bool NormalSummonRobina()
        {
            AI.SelectPlace(SelectSTPlace(Card));
            return (!robina_NormalSummonEffectActivated || Snowl_Activated) && !PrioritizeBossMonstersViaUnexploredWindsTribute();
        }

        private bool PrioritizeBossMonstersViaUnexploredWindsTribute()
        {
            return Bot.SpellZone.GetMatchingCardsCount(card => card.IsCode(CardId.UnexploredWinds) && card.HasPosition(CardPosition.FaceUp)) > 0
                    && Bot.MonsterZone.GetMatchingCardsCount(card => card.Level == 1 && card.HasPosition(CardPosition.FaceUp)) > 0;
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

                    if (slackerTarget != null)
                    {
                        if (OnPreBattleBetween(card, slackerTarget))
                        {
                            SlackerMagicianAttacks = true;
                            return AI.Attack(card, slackerTarget);
                        }
                    }
                    else
                    {
                        SlackerMagicianAttacks = true;
                    }
                }
            }

            return base.OnBattle(attackers, defenders);
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            int realDefenderPower = defender.Attack + defender.RealPower;
            int realAttackerPower = attacker.Attack + attacker.RealPower;

            if (attacker.IsCode(CardId.Empen) && realDefenderPower > realAttackerPower)
            {
                int empenPower = attacker.Attack + attacker.RealPower;
                int target_preEffectPower = (defender.Attack + defender.RealPower) / 2;

                if (target_preEffectPower < empenPower)
                    ActivateEmpenBattleEffect = true;

                return (target_preEffectPower < empenPower) && Bot.Hand.Count() > 0;
            }
                

            if (attacker.IsCode(CardId.SlackerdMagician) && realDefenderPower > realAttackerPower)
            {
                int potentialSelfDamage = (defender.Attack + defender.RealPower) - (attacker.Attack + attacker.RealPower);
                return Bot.LifePoints > potentialSelfDamage;
            }

            return base.OnPreBattleBetween(attacker, defender);
        }

        private bool ZeusSpSummon()
        {
            if (Bot.HasInMonstersZone(CardId.Zeus)) return false;
            AI.SelectPosition(CardPosition.Attack);

            return true;
        }

        private bool DreamingTownActivate()
        {
            if (Bot.HasInHand(CardId.Robina) && !robina_NormalSummonEffectActivated)
                AI.SelectCard(CardId.Robina);

            return Card.Location == CardLocation.SpellZone
                    && Duel.Player == 1
                    && Bot.Hand.ContainsMonsterWithLevel(1)
                    && !MagnificentMapActivated;
        }

        private bool DowneredMagicianSpSummon()
        {
            AI.SelectPosition(CardPosition.Attack);
            return SlackerMagicianAttacks;
        }

        private bool StriBanishedEffect()
        {
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
            if (Util.IsAllEnemyBetter())
                AI.SelectOption(2);
            else
            {
                AI.SelectOption(3);
                AI.SelectNextCard(Enemy.Hand.GetDangerousMonster());
            }
            AI.SelectPlace(SelectSTPlace(Card, true));
            return true;
        }

        private bool TrippleTacticsThrustActivate()
        {
            if (Bot.Deck.GetCardCount(CardId.TrippleTacticsTalent) > 0) AI.SelectCard(CardId.TrippleTacticsTalent);
            else if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina)))
            {
                if (PotOfProsperityActivated) AI.SelectCard(CardId.PotOfDuality);
                else AI.SelectCard(CardId.PotOfProsperity);
            }
            else AI.SelectCard(new List<int> { CardId.HarpiesFeatherDuster });

            AI.SelectPlace(SelectSTPlace(Card, true));

            return true;
        }

        private bool SlackerdMagicianSpSummon()
        {
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.HasType(CardType.Xyz)) > 0) return false;

            if (Bot.HasInExtra(CardId.UnderworldGoddess)) AI.SelectPlace(SelectSTPlace(Card, true));
            else AI.SelectPlace(Zones.ExtraMonsterZones);

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
            if (Util.IsAllEnemyBetter() && Enemy.GetFieldCount() > 1)
            {
                if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                    return UnexploredWindsTribute();
                }      
            }

            return false;
        }

        private bool AvianSummon()
        {
            if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return UnexploredWindsTribute();
            }

            return false;
        }

        private bool SnowlSummon()
        {
            if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1 && card.HasRace(CardRace.WindBeast)).Count() > 0)
            {
                AI.SelectPlace(Zones.ExtraMonsterZones);
                return UnexploredWindsTribute();
            }
            return false;
        }

        private bool UnexploredWindsTribute()
        {
            ClientCard enemyCard;
            if (Enemy.SpellZone.Count() > 0) enemyCard = Util.GetBestEnemySpell();
            else enemyCard = Util.GetBestEnemyCard();

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
            AI.SelectPlace(SelectSTPlace(Card, true));
            if (Card.Location == CardLocation.Hand)
                return true;
            else if (Card.Location == CardLocation.SpellZone)
            {
                List<int> reveal = Bot.Hand.GetMatchingCards(card => card.Level > 5 && card.HasRace(CardRace.WindBeast)).Select(c => c.Id).ToList();

                if (!(Bot.Hand.ContainsMonsterWithLevel(1) && Bot.MonsterZone.ContainsMonsterWithLevel(1))
                    && reveal.Count() > 0
                   )
                {
                    AI.SelectCard(reveal);
                    return true;
                }
            }

            return false;
        }

        private bool NormalSummonEglen()
        {
            if (Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).Count() > 0 && Snowl_Activated) return true;
            else
            {
                if ((!Bot.HasInHandOrHasInMonstersZone(CardId.Robina) && !robina_NormalSummonEffectActivated) && !PrioritizeBossMonstersViaUnexploredWindsTribute())
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                    return true;
                }
            }

            return false;
        }

        private bool SnowlActivate()
        {
            // Activated effect: Allows Normal 3 summon in a turn
            if (Duel.Player == 0 && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)) {
                Snowl_Activated = true;
                return true;
            }

            // Quick effect
            else if (ActivateDescription == Util.GetStringId(CardId.Snowl, 0))
            {
                if (Duel.Player == 1 && !(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                {
                    List<int> prefferedCost = new List<int>();
                    prefferedCost.AddRange(new List<int> {
                        CardId.Stri,
                        CardId.Eglen,
                        CardId.Toccan,
                        CardId.AdventOfAdventure,
                        CardId.BookOfMoon,
                        CardId.TrippleTacticsThrust
                    });

                    AI.SelectCard(prefferedCost);

                    return true;
                }    
            }

            return false;
        }

        private bool PotOfProsperityActivate()
        {
            if (Bot.ExtraDeck.Count <= 3) return false;
            if(!(Bot.HasInHand(CardId.Robina)
                || Bot.HasInHand(CardId.Eglen)
                || Bot.HasInHand(CardId.AdventOfAdventure)
                || Bot.HasInHand(CardId.PotOfDuality)
               )) return false;

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
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;
            }

            return false;
        }

        private bool ZeusEffect()
        {
            if (Util.GetLastChainCard() == Card) return false;

            if ((Util.IsAllEnemyBetter() || Util.IsChainTarget(Card) || Enemy.GetFieldCount() >= 3) && (Duel.Player == 0 || Duel.Player == 1) && !Card.IsDisabled())
                return true;

            return false;
        }

        private bool JuggernautLiebeSummon()
        {
            return true;
        }

        private bool BookOfMoonActivate()
        {
            if (!(Duel.Player == 0)) return false;
            if(Duel.CurrentChain.Contains(Card)) return false;

            ClientCard enemy_bestCard = Util.GetBestEnemyCard(true);

            if (enemy_bestCard != null && Enemy.MonsterZone.GetMatchingCardsCount(card => card.HasPosition(CardPosition.FaceUp)) > 0) {
                AI.SelectCard(enemy_bestCard);
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;

                BookOfMoonActivated = true;
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
            if (Bot.HasInExtra(CardId.UnderworldGoddess)) AI.SelectPlace(SelectSTPlace(Card, true));
            else AI.SelectPlace(Zones.ExtraMonsterZones);
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
            ActivateEmpenBattleEffect = false;

            // Battle phase actions
            SlackerMagicianAttacks = false;

            // XYZ Activation
            ZeusActivated = false;
            Snowl_Activated = false;
            ActivateLiebe = false;

            Impermanence_list.Clear();

            BookOfMoonActivated = false;

            base.OnNewTurn();
        }

        private bool DreamingTownGraveyardActivate()
        {
            return Card.Location == CardLocation.Grave
                    && Util.IsAllEnemyBetter();
        }

        private bool ToccanBanishedEffect()
        {
            if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                return true;

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
                else if(Bot.HasInBanished(CardId.Empen))
                    AI.SelectCard(CardId.Empen);
                else
                {
                    AI.SelectCard(new int[] {
                                CardId.AdventOfAdventure,
                                CardId.Avian,
                                CardId.MagnificentMap
                            });

                    return true;
                }

                NormalSummonBossMonsters();

                toccan_NormalSummonEffectActivated = true;
            }

            return false;
        }

        private bool Active_MagnificentMapActivate()
        {
            if (Card.Location == CardLocation.SpellZone && !(Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina)) && Duel.Player == 0)
            {
                int[] reveal = Bot.Hand.GetMatchingCards(card => card.Level == 1).Select(c => c.Id).ToArray();

                if (!Bot.HasInHand(CardId.Robina))
                {
                    AI.SelectCard(reveal);
                    AI.SelectNextCard(CardId.Robina);
                }

                return true;
            }
            else if (Card.Location == CardLocation.SpellZone && Bot.HasInHandOrHasInMonstersZone(CardId.Robina) && Duel.Player == 0)
            {
                // reveal robina
                AI.SelectCard(CardId.Robina);

                if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Toccan) || Bot.HasInBanished(CardId.Toccan))) AI.SelectNextCard(CardId.Toccan);
                else if (eglen_NormalSummonEffectActivated && !Bot.HasInHand(CardId.Eglen)) AI.SelectNextCard(CardId.Eglen);
                else AI.SelectNextCard(CardId.Stri);

                return true;
            }

            if (ActivateDescription == Util.GetStringId(CardId.MagnificentMap, 0) && Duel.Player == 1)
            {
                int levelOneMonsters = Bot.MonsterZone.GetMatchingCardsCount(card => card.Level == 1);

                if (!robina_NormalSummonEffectActivated) AI.SelectCard(CardId.Robina);
                else NormalSummonBossMonsters();

                MagnificentMapActivated = true;

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
            if (Bot.HasInHandOrHasInMonstersZone(CardId.Robina)) return false;
            if (robina_NormalSummonEffectActivated) return false;

            int[] cost;

            if(!Bot.Hand.ContainsMonsterWithLevel(1)) cost = Bot.Hand.GetMatchingCards(card => (card.Level > 5 || card.Level == 1) && !card.IsCode(CardId.Robina) && !card.IsCode(CardId.Empen) || !card.IsCode(CardId.Avian) || !card.IsCode(CardId.Snowl)).Select(c => c.Id).ToArray();
            else cost = Bot.Hand.GetMatchingCards(card => card.Level == 1 && !(card.IsCode(CardId.Robina) || card.IsCode(CardId.Eglen))).Select(c => c.Id).ToArray();

            if (!Bot.HasInHand(CardId.Robina) && cost.Count() > 0)
            {
                AI.SelectCard(cost); // pay cost
                AI.SelectNextCard(CardId.Robina);

                return true;
            }

            int hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.HasAttribute(CardAttribute.Wind) && card.Level > 6).Count();

            // Search Floowandereeze Spell/Trap
            if (Bot.HasInHand(CardId.Robina) && Bot.HasInHand(CardId.Eglen) && hand_bossMonsters > 0 && cost.Count() > 0)
            {
                AI.SelectCard(cost); // pay cost

                if(!Bot.HasInHand(CardId.UnexploredWinds)) AI.SelectNextCard(CardId.UnexploredWinds);
                else AI.SelectNextCard(CardId.MagnificentMap);

                return true;
            }

            AI.SelectPlace(SelectSTPlace(Card, true));

            return false;
        }

        private bool DreamingTownSet()
        {
            AI.SelectPlace(SelectSTPlace(Card, true));
            return Bot.HasInHandOrHasInMonstersZone(CardId.Robina) || Bot.HasInBanished(CardId.Robina) || Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen);
        }

        private bool PotOfDualityActivate()
        {
            AI.SelectPlace(SelectSTPlace(Card, true));
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
            if (!(Card.Location == CardLocation.MonsterZone || Card.Location == CardLocation.Grave || Card.Location == CardLocation.Hand))
                return true;

            return false;
        }

        private bool RobinaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Snowl_Activated) return false;

                // Search eglen
                if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen))) AI.SelectCard(CardId.Eglen);
                else if (Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen) && Bot.Graveyard.Count() > 0) AI.SelectCard(CardId.Stri);
                else AI.SelectCard(CardId.Toccan);

                int materials = Bot.MonsterZone.GetMatchingCardsCount(card => card.Level == 1);

                if(materials > 1)
                    NormalSummonBossMonsters();

                // Normal summon again
                if (!eglen_NormalSummonEffectActivated)
                {
                    AI.SelectYesNo(true);
                    AI.SelectNextCard(CardId.Eglen);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Eglen))
                        ));
                }
                else if (!toccan_NormalSummonEffectActivated && BanishedRecycleCards().Count() > 0)
                {
                    AI.SelectYesNo(true);
                    AI.SelectNextCard(CardId.Toccan);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Toccan))
                        ));
                }
                else if (!stri_NormalSummonEffectActivated && BanishedRecycleCards().Count() > 0)
                {
                    AI.SelectYesNo(true);
                    AI.SelectNextCard(CardId.Stri);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Stri))
                        ));
                }

                robina_NormalSummonEffectActivated = true;

                return true;
            }

            return false;
        }

        public List<int> GraveyardRecycleCards()
        {
            List<int> recycleCards = Bot.Banished.GetMatchingCards(card => card.IsCode(CardId.UnexploredWinds)
                                                            || card.IsCode(CardId.Empen)
                                                            || card.IsCode(CardId.Avian)).Select(c => c.Id).ToList();

            return recycleCards;
        }

        public List<int> BanishedRecycleCards()
        {
            List<int> recycleCards = Bot.Banished.GetMatchingCards(card => card.IsCode(CardId.UnexploredWinds)
                                                            || card.IsCode(CardId.DreamingTown)
                                                            || card.IsCode(CardId.Empen)
                                                            || card.IsCode(CardId.Avian)).Select(c => c.Id).ToList();

            return recycleCards;
        }

        public void NormalSummonBossMonsters()
        {
            int[] hand_bossMonsters = Bot.Hand.GetMatchingCards(card => card.Level > 7).Select(c => c.Id).ToArray();

            List<int> materials = new List<int>();
            materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                            .Select(x => x.Id)
                            .ToList();

            // Tribute opponent's card using Unexplored winds
            if (Bot.HasInSpellZone(CardId.UnexploredWinds) && Util.GetBestEnemyCard() != null && materials.Count() > 0)
            {
                AI.SelectOption(1);

                materials.Clear();
                materials.Add(Bot.MonsterZone.GetFirstMatchingCard(card => card.Level == 1).Id);

                if(Enemy.SpellZone.Count() > 0 && Util.GetBestEnemySpell() != null) materials.Add(Util.GetBestEnemySpell().Id);
                else materials.Add(Util.GetBestEnemyCard().Id);
            }

            // summon boss monsters
            if (hand_bossMonsters.Count() > 0 && materials.Count() > 1)
            {
                AI.SelectYesNo(true);

                if (Bot.HasInHand(CardId.Empen))
                {
                    AI.SelectCard(CardId.Empen);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Empen))
                        ));
                }
                else if (Bot.HasInHand(CardId.Avian))
                {
                    AI.SelectCard(CardId.Avian);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Avian))
                        ));
                }
                else if (Bot.HasInHand(CardId.Snowl))
                {
                    AI.SelectCard(CardId.Snowl);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.Snowl))
                        ));
                }
                else
                {
                    AI.SelectCard(CardId.RaizaMegaMonarch);
                    AI.SelectPlace(SelectSTPlace(
                            Bot.Hand.GetFirstMatchingCard(card => card.IsCode(CardId.RaizaMegaMonarch))
                        ));
                }

                AI.SelectMaterials(materials);
            }
            else AI.SelectYesNo(false);
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

            return false;
        }

        private bool Empen_NormalSummonAgain()
        {
            return ((Bot.Hand.ContainsMonsterWithLevel(8) || Bot.Hand.ContainsMonsterWithLevel(7))
                    && Bot.MonsterZone.GetMatchingCardsCount(card => card.Level == 1) > 1)
                    || Bot.SpellZone.GetMatchingCardsCount(card => card.IsCode(CardId.UnexploredWinds) && card.HasPosition(CardPosition.FaceUpDefence)) > 0;
        }

        private bool EglenEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Snowl_Activated) return false;

                List<int> materials = new List<int>();
                materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1)
                                .Select(x => x.Id)
                                .ToList();

                if (!Bot.HasInHandOrHasInMonstersZone(CardId.Empen)
                    || Bot.MonsterZone.GetCardCount(CardId.Empen) > 0
                    && Duel.Player == 1)
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
