using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Web.Management;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("BudgetLabrynths", "AI_BudgetLabrynths")]
    public class BudgetLabrynths : DefaultExecutor
    {
        public BudgetLabrynths(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardID.NIBIRU);

            AddExecutor(ExecutorType.GoToBattlePhase, EvenlyMatchedActivate);
            AddExecutor(ExecutorType.Activate);

            AddExecutor(ExecutorType.Activate, CardID.POT_OF_PROSPERITY); // TODO

            AddExecutor(ExecutorType.Activate, CardID.STOVIE_TORBIE, StovieTorbieEffect);

            AddExecutor(ExecutorType.Summon, CardID.CHANDRAGLIER, ChandraglierSummon);
            AddExecutor(ExecutorType.Activate, CardID.CHANDRAGLIER, ChandraglierEffect);

            AddExecutor(ExecutorType.SpellSet, CardID.WELCOME_LABYRINTH);
            AddExecutor(ExecutorType.Activate, CardID.WELCOME_LABYRINTH, WelcomeLabrynthActivate);

            AddExecutor(ExecutorType.Activate, CardID.COOCLOCK, CooclockEffect);

            AddExecutor(ExecutorType.Summon, CardID.ARIANNA, AriannaSummon);
            AddExecutor(ExecutorType.Activate, CardID.ARIANNA, AriannaEffect);
            AddExecutor(ExecutorType.Repos, CardID.ARIANNA, AriannaRepos);

            AddExecutor(ExecutorType.SpellSet, CardID.DOGMATIKA_PUNISHMENT, DogmatikaSet);
            AddExecutor(ExecutorType.Activate, CardID.DOGMATIKA_PUNISHMENT, DogmatikaPunishmentActivate);

            AddExecutor(ExecutorType.Activate, CardID.LABRYNTH_LABYRINTH, LabrynthLabyrinthActivate);

            AddExecutor(ExecutorType.Activate, CardID.LADY_LABRYNTH, LadyLabrynthActivate);
            AddExecutor(ExecutorType.Repos, CardID.LADY_LABRYNTH, LadyLabrynthRepos);

            AddExecutor(ExecutorType.Activate, CardID.LOVELY_LABRYNTH, LovelyLabrynthEffect);

            AddExecutor(ExecutorType.Summon, CardID.MUCKRACKER, MuckarackerLinkSummon);
            AddExecutor(ExecutorType.Activate, CardID.MUCKRACKER, MuckarackerEffect);

            AddExecutor(ExecutorType.Activate, CardID.ASH_BLOSSOM, AshBlossomActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.COMPULSORY_EVACUATION_DEVICE, CompulsoryEvacDeviceSet);
            AddExecutor(ExecutorType.Activate, CardID.COMPULSORY_EVACUATION_DEVICE, CompulsoryEvacuationDeviceActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.SOLEMN_STRIKE);
            AddExecutor(ExecutorType.Activate, CardID.SOLEMN_STRIKE, SolemnStrikeActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.ERADICATOR_EPIDEMIC_VIRUS, EradicatorSet);
            AddExecutor(ExecutorType.Activate, CardID.ERADICATOR_EPIDEMIC_VIRUS, EradicatorActivate);

            AddExecutor(ExecutorType.Activate, CardID.WIND_PEGASUS_IGNISTER, WindPegasusIngisterEffect);
            AddExecutor(ExecutorType.Activate, CardID.ELDER_ENTITY_NTSS, ElderEntityNtssEffect);
        }

        private bool LadyLabrynthActivated = false;

        private bool CompulsoryEvacDeviceSet()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (!Bot.HasInSpellZone(CardID.DOGMATIKA_PUNISHMENT))
            {
                if (problemMonster != null) return true;
                else if (ProtectLadyLabrynth()) return true;
            }
                

            return false;
        }

        private bool AriannaSummon()
        {
            ClientCard threatCard = Util.GetProblematicEnemyMonster();

            if (threatCard != null && Util.IsAllEnemyBetter()) return false;
            else return true;
        }

        private bool DogmatikaSet()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (!Bot.HasInSpellZone(CardID.DOGMATIKA_PUNISHMENT))
            {
                if (problemMonster != null) return true;
                else if (problemMonster == null && ProtectLadyLabrynth()) return true;
            }

            return false;
        }

        public bool ProtectLadyLabrynth()
        {
            return Bot.HasInMonstersZone(CardID.LADY_LABRYNTH);
        }

        private bool AriannaRepos()
        {
            bool enemyAdvantage = Util.IsAllEnemyBetter(true);

            if ((Card.HasPosition(CardPosition.FaceUpAttack) || Card.HasPosition(CardPosition.FaceDown)) && enemyAdvantage) return true;
            else if (Card.HasPosition(CardPosition.FaceUpDefence) && !enemyAdvantage) return true;

            return false;
        }

        private bool WindPegasusIngisterEffect()
        {
            ClientCard card = Util.GetBestEnemyCard();

            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }

            return false;
        }

        private bool EradicatorActivate()
        {
            if (Bot.SpellZone.GetMatchingCards(card => card.IsCode(CardID.LABRYNTH_LABYRINTH) && card.IsFaceup()).Count() > 0)
            {
                AI.SelectCard(CardID.LADY_LABRYNTH);
                return true;
            }
            else
            {
                AI.SelectCard(CardID.LADY_LABRYNTH);
                return true;
            }
        }

        private bool ChandraglierSummon()
        {
            return Bot.HasInHand(CardID.COOCLOCK) && Bot.HasInSpellZone(CardID.WELCOME_LABYRINTH);
        }

        private bool LabrynthLabyrinthActivate()
        {
            if (ActivateDescription == Util.GetStringId(CardID.LABRYNTH_LABYRINTH, 0))
            {
                // Labrynth Labyrinth add effect when Welcome Labrynth is activated
                if (Duel.LastChainPlayer == 0
                    && Util.GetLastChainCard().IsCode(CardID.WELCOME_LABYRINTH)
                    && Bot.HasInSpellZone(CardID.LABRYNTH_LABYRINTH)
                    && Enemy.GetFieldCount() > 0
                   )
                {
                    AI.SelectYesNo(true);

                    ClientCard backrow_problem = Util.GetBestEnemySpell();
                    ClientCard monster_target = Util.GetProblematicEnemyMonster();

                    if (backrow_problem != null) AI.SelectNextCard(backrow_problem);
                    else AI.SelectNextCard(monster_target);
                }
                else return false;
            }

            if (SpellActivate()) return true;
            else
            {
                ClientCard card = Util.GetLastChainCard();
                if (card != null && card.Controller == 0 && card.Id == CardID.LABRYNTH_LABYRINTH) return false;

                return true;
            }
        }

        private bool SpellActivate()
        {
            return Card.Location == CardLocation.Hand || (Card.Location == CardLocation.SpellZone && Card.IsFacedown());
        }

        private bool MuckarackerEffect()
        {
            int[] bossMonsters = new int[]
            {
                CardID.LADY_LABRYNTH,
                CardID.LOVELY_LABRYNTH
            };

            int[] other_prefferedMonsters = new int[]
            {
                CardID.ARIANNA,
                CardID.STOVIE_TORBIE
            };

            if (Bot.HasInGraveyard(CardID.LOVELY_LABRYNTH) || Bot.HasInGraveyard(CardID.LADY_LABRYNTH))
            {
                AI.SelectCard(bossMonsters);
                return true;
            }
            else
            {
                AI.SelectCard(other_prefferedMonsters);
                return true;
            }
        }

        private bool MuckarackerLinkSummon()
        {
            int bot_Monsters = Bot.MonsterZone.GetMatchingCards(card => card.Level < 4).Count();
            int bot_HandCards = Bot.Hand.Count();

            if (bot_Monsters >= 2 && bot_HandCards > 1)
                return true;

            return false;
        }

        private bool AshBlossomActivate()
        {
            if (!(Duel.LastChainPlayer == 0)) return true;
            return false;
        }

        private bool EradicatorSet()
        {
            if (Duel.Phase == DuelPhase.Main2)
            {
                IList<ClientCard> tributes = Bot.MonsterZone.GetMatchingCards(card => card.HasAttribute(CardAttribute.Dark) && card.Attack > 2500);

                if (tributes.Count > 0)
                    return true;
            }

            return false;
        }

        private bool ElderEntityNtssEffect()
        {
            ClientCard backrow_target = Util.GetBestEnemySpell();
            ClientCard monster_target = Util.GetProblematicEnemyMonster();

            if (Util.GetBestEnemySpell() != null) AI.SelectCard(backrow_target);
            else AI.SelectNextCard(monster_target);

            return true;
        }

        private bool LadyLabrynthRepos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.IsDefense())
                return true;

            return false;
        }

        private bool LadyLabrynthActivate()
        {
            if (Util.GetProblematicEnemyCard() != null && Bot.SpellZone.GetMatchingCards(card => card.HasPosition(CardPosition.FaceDown)).Count() == 0) return false;

            int[] prefferedTraps = new int[]
            {
                CardID.DOGMATIKA_PUNISHMENT,
                CardID.COMPULSORY_EVACUATION_DEVICE
            };

            IList<ClientCard> enemySpellZone = Enemy.SpellZone.GetMatchingCards(card =>
                card.HasType(CardType.Spell)
                || card.HasType(CardType.Trap)
                || card.HasPosition(CardPosition.FaceDown)
            );

            if (!Bot.HasInMonstersZone(CardID.LOVELY_LABRYNTH)) AI.SelectCard(CardID.WELCOME_LABYRINTH);
            else if (enemySpellZone.Count() > 1 && Bot.HasInMonstersZone(CardID.LOVELY_LABRYNTH)) AI.SelectCard(CardID.ERADICATOR_EPIDEMIC_VIRUS);
            else AI.SelectCard(prefferedTraps);

            LadyLabrynthActivated = true;

            return true;
        }

        private bool EvenlyMatchedActivate()
        {
            return Bot.HasInHand(CardID.EVENLY_MATCHED) && Duel.Turn >= 2 && Enemy.GetFieldCount() >= 2 && Bot.GetFieldCount() == 0;
        }

        private bool DogmatikaPunishmentActivate()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            int[] preffered_cost = new int[]
            {
                CardID.ELDER_ENTITY_NTSS,
                CardID.TRIBRIGADE_ARMS_BUCEPHALUS_II,
                CardID.WIND_PEGASUS_IGNISTER,
                CardID.COMSIC_BLAZAR_DRAGON
            };

            if (problemMonster != null)
            {
                if(Enemy.GetFieldCount() > 0)
                    AI.SelectCard(CardID.ELDER_ENTITY_NTSS);
                else
                    AI.SelectCard(CardID.WIND_PEGASUS_IGNISTER);

                AI.SelectNextCard(problemMonster);

                if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardID.COMPULSORY_EVACUATION_DEVICE))
                    return false;

                return true;
            }
                
            return false;
        }

        private bool StovieTorbieEffect()
        {
            if (Card.Location == CardLocation.Hand && !Bot.HasInGraveyard(CardID.STOVIE_TORBIE))
            {
                // Look for duplicate copies of the same card or Chandraglier and use them as materials.
                List<int> cost_candidates = new List<int>();

                if (Bot.HasInHand(CardID.CHANDRAGLIER))
                    cost_candidates.Add(CardID.CHANDRAGLIER);
                else if (Bot.HasInHand(CardID.LOVELY_LABRYNTH))
                    cost_candidates.Add(CardID.LOVELY_LABRYNTH);
                else if (Bot.HasInHand(CardID.LABRYNTH_LABYRINTH) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.LABRYNTH_LABYRINTH)).Count() > 1)
                    cost_candidates.Add(CardID.LABRYNTH_LABYRINTH);
                else if (Bot.HasInHand(CardID.STOVIE_TORBIE) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.STOVIE_TORBIE)).Count() > 1)
                    cost_candidates.Add(CardID.LABRYNTH_LABYRINTH);
                else if (Duel.Turn > 2)
                    cost_candidates.Add(CardID.NIBIRU);
                else cost_candidates.AddRange(new List<int>() { CardID.POT_OF_PROSPERITY });

                // Get the next Labrynth spell and trap
                if (cost_candidates.Count() > 0)
                {
                    AI.SelectCard(cost_candidates);

                    if (!Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH) && Bot.MonsterZone.GetMatchingCards(card => card.HasType(CardType.Monster)).Count() > 1)
                        AI.SelectNextCard(CardID.WELCOME_LABYRINTH);
                    else
                        AI.SelectNextCard(CardID.LABRYNTH_LABYRINTH);

                    return true;
                }
            } else if(Card.Location == CardLocation.Grave) return true;

            return false;
        }

        public override void OnNewTurn()
        {
            LadyLabrynthActivated = false;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == 27204312 || cardId == CardID.STOVIE_TORBIE || cardId == CardID.ARIANNA)
            {
                return CardPosition.FaceUpDefence;
            }
            else if (cardId == CardID.LADY_LABRYNTH || cardId == CardID.LOVELY_LABRYNTH)
            {
                return CardPosition.FaceUpAttack;
            }
            return base.OnSelectPosition(cardId, positions);
        }

        private bool SolemnStrikeActivate()
        {
            ClientCard problemCard = Util.GetProblematicEnemyMonster();

            if(Duel.LastChainPlayer == 0)
                return false;

            if (problemCard != null)
                return true;

            return false;
        }

        private bool LovelyLabrynthEffect()
        {
            int[] supportTraps = new int[] {
                CardID.WELCOME_LABYRINTH,
                CardID.COMPULSORY_EVACUATION_DEVICE,
                CardID.DOGMATIKA_PUNISHMENT
            };

            if (Bot.HasInGraveyard(CardID.ERADICATOR_EPIDEMIC_VIRUS))
                AI.SelectCard(CardID.ERADICATOR_EPIDEMIC_VIRUS);
            else
                AI.SelectCard(supportTraps);

            return true;
        }

        private bool CompulsoryEvacuationDeviceActivate()
        {
            ClientCard enemyBestMonster = Util.GetBestEnemyMonster();

            if (enemyBestMonster != null)
            {
                if (Duel.LastChainPlayer == 0 && (
                        Util.GetLastChainCard().IsCode(CardID.DOGMATIKA_PUNISHMENT)
                        || Util.GetLastChainCard().IsCode(CardID.COMPULSORY_EVACUATION_DEVICE)
                        || Util.GetLastChainCard().IsCode(CardID.ELDER_ENTITY_NTSS)
                        || Util.GetLastChainCard().IsCode(CardID.WELCOME_LABYRINTH)
                    )
                )
                {
                    return false;
                }

                return true;
            }

            

            

            return false;
        }

        public bool isEvenlyMatchedOnHand()
        {
            return Bot.HasInHand(CardID.EVENLY_MATCHED);
        }

        public bool WelcomeLabrynthActivate()
        {
            List<int> preferred_Monsters = new List<int>();

            if (Bot.HasInHandOrHasInMonstersZone(CardID.LADY_LABRYNTH))
                preferred_Monsters.Add(CardID.LOVELY_LABRYNTH);
            else if (Bot.HasInHandOrInGraveyard(CardID.LOVELY_LABRYNTH) || Bot.HasInHandOrHasInMonstersZone(CardID.LOVELY_LABRYNTH))
                preferred_Monsters.Add(CardID.LADY_LABRYNTH);
            else if (Bot.HasInMonstersZone(CardID.LADY_LABRYNTH) || Bot.HasInMonstersZone(CardID.LOVELY_LABRYNTH))
                preferred_Monsters.Add(CardID.ARIANNA);
            else preferred_Monsters.AddRange(new List<int> { CardID.STOVIE_TORBIE });

            AI.SelectCard(preferred_Monsters);

            return true;
        }

        public bool AriannaEffect()
        {
            int[] other_prefferedCards = new int[] {
                CardID.COOCLOCK,
                CardID.STOVIE_TORBIE
            };

            if (!Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH))
                AI.SelectCard(CardID.WELCOME_LABYRINTH);
            else if (
                    Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH)
                    && Bot.Deck.GetMatchingCards(card => card.IsCode(CardID.COOCLOCK)).Count() > 0
                    && Bot.MonsterZone.GetMatchingCards(card => card.HasPosition(CardPosition.FaceUp)).Count() > 0
                   )
                AI.SelectCard(CardID.LABRYNTH_LABYRINTH);
            else
                AI.SelectCard(other_prefferedCards);

            return true;
        }

        public bool CooclockEffect()
        {
            IList<ClientCard> faceUp_Monsters = Bot.MonsterZone.GetMatchingCards(card =>
                    card.IsCode(CardID.LOVELY_LABRYNTH)
                    || card.IsCode(CardID.LADY_LABRYNTH)
                    || card.IsCode(CardID.STOVIE_TORBIE)
                    || card.IsCode(CardID.ARIANNA)
                    || card.IsCode(CardID.CHANDRAGLIER)
                );

            if (Card.Location == CardLocation.Hand || Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH)) return true;
            else if (Card.Location == CardLocation.Grave) return true;

            return false;
        }

        public bool ChandraglierEffect()
        {
            if (Card.Location == CardLocation.Hand && !Bot.HasInGraveyard(CardID.CHANDRAGLIER))
            {
                // Look for duplicate copies of the same card or Chandraglier and use them as materials.
                List<int> cost_candidates = new List<int>();

                if (Bot.HasInHand(CardID.CHANDRAGLIER) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.CHANDRAGLIER)).Count() > 1)
                    cost_candidates.Add(CardID.CHANDRAGLIER);
                else if (Bot.HasInHand(CardID.LOVELY_LABRYNTH))
                    cost_candidates.Add(CardID.LOVELY_LABRYNTH);
                else if (Bot.HasInHand(CardID.LABRYNTH_LABYRINTH) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.LABRYNTH_LABYRINTH)).Count() > 1)
                    cost_candidates.Add(CardID.LABRYNTH_LABYRINTH);
                else if (Bot.HasInHand(CardID.STOVIE_TORBIE) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.STOVIE_TORBIE)).Count() > 1)
                    cost_candidates.Add(CardID.LABRYNTH_LABYRINTH);
                else if (Duel.Turn > 2)
                    cost_candidates.Add(CardID.NIBIRU);
                else cost_candidates.AddRange(new List<int>() { CardID.POT_OF_PROSPERITY });

                // Get the next Labrynth spell and trap
                if (cost_candidates.Count() > 0)
                {
                    AI.SelectCard(cost_candidates);

                    if (!Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH) && Bot.MonsterZone.GetMatchingCards(card => card.HasType(CardType.Monster)).Count() > 1)
                        AI.SelectNextCard(CardID.WELCOME_LABYRINTH);
                    else
                        AI.SelectNextCard(CardID.LABRYNTH_LABYRINTH);

                    return true;
                }
            }
            else if (Card.Location == CardLocation.Grave) return true;

            return false;
        }

        public class CardID
        {
            // MAIN DECK
            public const int NIBIRU = 27204311;
            public const int LADY_LABRYNTH = 81497285;
            public const int LOVELY_LABRYNTH = 2347656;
            public const int ARIANNA = 1225009;
            public const int CHANDRAGLIER = 37629703;
            public const int ASH_BLOSSOM = 14558127;
            public const int STOVIE_TORBIE = 74018812;
            public const int COOCLOCK = 2511;

            // spells and traps
            public const int POT_OF_PROSPERITY = 84211599;
            public const int LABRYNTH_LABYRINTH = 33407125;
            public const int WELCOME_LABYRINTH = 5380979;
            public const int EVENLY_MATCHED = 15693423;
            public const int DOGMATIKA_PUNISHMENT = 82956214;
            public const int COMPULSORY_EVACUATION_DEVICE = 94192409;
            public const int SOLEMN_STRIKE = 40605147;
            public const int ERADICATOR_EPIDEMIC_VIRUS = 54974237;

            // EXTRA DECK
            public const int ELDER_ENTITY_NTSS = 80532587;
            public const int COMSIC_BLAZAR_DRAGON = 21123811;
            public const int WIND_PEGASUS_IGNISTER = 98506199;
            public const int TRIBRIGADE_ARMS_BUCEPHALUS_II = 10019086;
            public const int KNIGHTMARE_PHEONIX = 2857636;
            public const int MUCKRACKER = 71607202;
            public const int IP_MASQUERENA = 65741786;
            public const int LINKURIBOH = 41999284;

        }
    }
}
