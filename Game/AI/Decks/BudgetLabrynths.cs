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
            AddExecutor(ExecutorType.GoToBattlePhase, EvenlyMatchedActivate);
            AddExecutor(ExecutorType.Activate);

            AddExecutor(ExecutorType.Activate, CardID.NIBIRU);
            AddExecutor(ExecutorType.Activate, CardID.LADY_LABRYNTH, LadyLabrynthActivate);
            AddExecutor(ExecutorType.Repos, CardID.LADY_LABRYNTH, LadyLabrynthRepos);

            AddExecutor(ExecutorType.Activate, CardID.LOVELY_LABRYNTH, LovelyLabrynthEffect);

            AddExecutor(ExecutorType.Summon, CardID.ARIANNA);
            AddExecutor(ExecutorType.Activate, CardID.ARIANNA, AriannaEffect);

            AddExecutor(ExecutorType.SpSummon, CardID.MUCKRACKER, MuckarackerLinkSummon);
            AddExecutor(ExecutorType.Activate, CardID.MUCKRACKER, MuckarackerEffect);


            AddExecutor(ExecutorType.Activate, CardID.STOVIE_TORBIE, StovieTorbieEffect);
            AddExecutor(ExecutorType.Activate, CardID.CHANDRAGLIER, ChandraglierEffect);

            AddExecutor(ExecutorType.Activate, CardID.ASH_BLOSSOM, AshBlossomActivate);
            
            AddExecutor(ExecutorType.Activate, CardID.POT_OF_PROSPERITY);
            AddExecutor(ExecutorType.Activate, CardID.LABRYNTH_LABYRINTH);

            AddExecutor(ExecutorType.SpellSet, CardID.WELCOME_LABYRINTH);

            AddExecutor(ExecutorType.Activate, CardID.COOCLOCK, CooclockEffect);

            AddExecutor(ExecutorType.Activate, CardID.WELCOME_LABYRINTH, WelcomeLabrynthActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.DOGMATIKA_PUNISHMENT);
            AddExecutor(ExecutorType.Activate, CardID.DOGMATIKA_PUNISHMENT, DogmatikaPunishmentActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.COMPULSORY_EVACUATION_DEVICE);
            AddExecutor(ExecutorType.Activate, CardID.COMPULSORY_EVACUATION_DEVICE, CompulsoryEvacuationDeviceActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.SOLEMN_STRIKE);
            AddExecutor(ExecutorType.Activate, CardID.SOLEMN_STRIKE, SolemnStrikeActivate);

            AddExecutor(ExecutorType.SpellSet, CardID.ERADICATOR_EPIDEMIC_VIRUS, EradicatorSet);
            AddExecutor(ExecutorType.Activate, CardID.ERADICATOR_EPIDEMIC_VIRUS);

            AddExecutor(ExecutorType.Activate, CardID.ELDER_ENTITY_NTSS, ElderEntityNtssEffect);

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
            return true;
        }

        private bool EvenlyMatchedActivate()
        {
            return Bot.HasInHand(CardID.EVENLY_MATCHED) && Duel.Turn >= 2 && Enemy.GetFieldCount() >= 2 && Bot.GetFieldCount() == 0;
        }

        private bool DogmatikaPunishmentActivate()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (problemMonster != null)
            {
                IList<ClientCard> threatMonster = Enemy.MonsterZone.GetMatchingCards(card => card.IsCode(problemMonster.Id) && card.Attack > 2000);

                if (threatMonster.Count() > 0)
                {
                    if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardID.COMPULSORY_EVACUATION_DEVICE))
                    {
                        return false;
                    }

                    return true;
                }
            }
                
            return false;
        }

        private bool StovieTorbieEffect()
        {
            IList<ClientCard> stovie_copies = Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.STOVIE_TORBIE));


            if (Bot.HasInHand(CardID.CHANDRAGLIER) || stovie_copies.Count() > 0)
            {
                IList<ClientCard> cost_candidates = Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.STOVIE_TORBIE) || card.IsCode(CardID.CHANDRAGLIER));
                cost_candidates.Select(x => x.Id).ToArray();

                AI.SelectCard(cost_candidates);
                AI.SelectNextCard(CardID.WELCOME_LABYRINTH);

                return true;
            }

            return false;
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
                    || Util.GetLastChainCard().IsCode(CardID.ELDER_ENTITY_NTSS))
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
            int[] prefferedMonsters = new int[] {
                CardID.LADY_LABRYNTH,
                CardID.LOVELY_LABRYNTH
            };

            if (Bot.HasInHandOrInSpellZone(CardID.LABRYNTH_LABYRINTH) && Enemy.GetFieldCount() == 0)
            {
                AI.SelectYesNo(false);
                return false;
            }
            else
            {
                AI.SelectCard(prefferedMonsters);

                AI.SelectYesNo(true);

                ClientCard backrow_problem = Util.GetBestEnemySpell();
                ClientCard monster_target = Util.GetProblematicEnemyMonster();

                if (backrow_problem != null) AI.SelectNextCard(backrow_problem);
                else AI.SelectNextCard(monster_target);
            }

            return true;
        }

        public bool AriannaEffect()
        {
            int[] prefferedCards = new int[] {
                CardID.LABRYNTH_LABYRINTH,
                CardID.COOCLOCK,
                CardID.STOVIE_TORBIE
            };

            if (!Bot.HasInHandOrInSpellZone(CardID.WELCOME_LABYRINTH))
                AI.SelectCard(CardID.WELCOME_LABYRINTH);
            else if (Bot.Deck.GetMatchingCards(card => card.IsCode(CardID.COOCLOCK)).Count() > 0)
                AI.SelectCard(CardID.COOCLOCK);
            else
                AI.SelectCard(prefferedCards);

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

            IList<ClientCard> support_Traps = Bot.SpellZone.GetMatchingCards(card => card.IsCode(CardID.WELCOME_LABYRINTH) || card.IsCode(CardID.ERADICATOR_EPIDEMIC_VIRUS) || card.IsCode(CardID.ERADICATOR_EPIDEMIC_VIRUS));

            if (Bot.HasInHand(CardID.COOCLOCK) && faceUp_Monsters.Count() > 0 || support_Traps.Count > 0)
            {
                return true;
            }

            if (Bot.HasInGraveyard(CardID.COOCLOCK))
            {
                return true;
            }

            return false;
        }

        public bool ChandraglierEffect()
        {
            if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardID.STOVIE_TORBIE))
                return false;

            // check hand for cost candidate
            IList<ClientCard> copy_chandraglier = Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.CHANDRAGLIER));
            IList<ClientCard> copy_stovie = Bot.Hand.GetMatchingCards(card => card.IsCode(CardID.STOVIE_TORBIE));

            int[] prefferedCards = new int[] {
                CardID.WELCOME_LABYRINTH,
                CardID.LABRYNTH_LABYRINTH,
            };

            IList<int> cost_candidates = new List<int>();

            // Chandraglier
            foreach (var cost in copy_chandraglier)
            {
                cost_candidates.Add(cost.Id);
            }

            // Stovie
            foreach (var cost in copy_stovie)
            {
                cost_candidates.Add(cost.Id);
            }

            if (copy_chandraglier.Count > 0 || copy_stovie.Count() > 0 || Bot.SpellZone.GetMatchingCards(card => card.HasType(CardType.Trap)).Count() == 0)
            {
                AI.SelectCard(cost_candidates);
                AI.SelectNextCard(prefferedCards);
                return true;
            }

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
