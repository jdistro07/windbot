using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    [Deck("BudgetLabrynths", "AI_BudgetLabrynths")]
    public class BudgetLabrynths : DefaultExecutor
    {
        public BudgetLabrynths(GameAI ai, Duel duel)
        : base(ai, duel)
        {

            // Break board
            AddExecutor(ExecutorType.Activate, CardId.NIBIRU);
            AddExecutor(ExecutorType.GoToBattlePhase, EvenlyMatchedActivate);
            AddExecutor(ExecutorType.Activate);

            // Setup
            AddExecutor(ExecutorType.Activate, CardId.POT_OF_PROSPERITY, PotOfProsperityActivate);
            AddExecutor(ExecutorType.Activate, CardId.LORD_OF_HEAVENLY_PRISON, LordOfHeavenlyPrisonActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.WELCOME_LABYRINTH, WelcomeLabrynthSet);

            // Activate furnitures to supplement board setup
            AddExecutor(ExecutorType.Activate, CardId.STOVIE_TORBIE, StovieTorbieEffect);
            AddExecutor(ExecutorType.Activate, CardId.CHANDRAGLIER, ChandraglierEffect);

            //Labrynth Spell Traps
            AddExecutor(ExecutorType.Activate, CardId.LABRYNTH_LABYRINTH, LabrynthLabyrinthActivate);
            AddExecutor(ExecutorType.Activate, CardId.COOCLOCK, CooclockEffect);

            // Monster summons and effects
            AddExecutor(ExecutorType.Summon, CardId.CHANDRAGLIER, FunitureSummon);
            AddExecutor(ExecutorType.Summon, CardId.CHANDRAGLIER, FunitureSummon);
            AddExecutor(ExecutorType.Summon, CardId.ARIANNA);
            AddExecutor(ExecutorType.Activate, CardId.ARIANNA, AriannaEffect);

            // Monster Activations
            AddExecutor(ExecutorType.Activate, CardId.LADY_LABRYNTH, LadyLabrynthActivate);
            AddExecutor(ExecutorType.Activate, CardId.LOVELY_LABRYNTH, LovelyLabrynthEffect);
            AddExecutor(ExecutorType.Activate, CardId.ASH_BLOSSOM, DefaultAshBlossomAndJoyousSpring);

            // Link monsters summon and effects
            AddExecutor(ExecutorType.SpSummon, CardId.KNIGHTMARE_PHEONIX, KnightmarePheonix_LinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.KNIGHTMARE_PHEONIX, KnightmarePheonix_Effect);
            AddExecutor(ExecutorType.SpSummon, CardId.MUCKRACKER, MuckarackerLinkSummon);
            AddExecutor(ExecutorType.Activate, CardId.MUCKRACKER, MuckarackerEffect);

            // Backrow setup
            AddExecutor(ExecutorType.SpellSet, EvenlyMatchedSet);
            AddExecutor(ExecutorType.SpellSet, CardId.DOGMATIKA_PUNISHMENT, DogmatikaSet);
            AddExecutor(ExecutorType.SpellSet, CardId.COMPULSORY_EVACUATION_DEVICE, CompulsoryEvacDeviceSet);
            AddExecutor(ExecutorType.Activate, CardId.COMPULSORY_EVACUATION_DEVICE, CompulsoryEvacuationDeviceActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.ERADICATOR_EPIDEMIC_VIRUS, EradicatorSet);
            AddExecutor(ExecutorType.SpellSet, CardId.SOLEMN_STRIKE, SolemnStrikeSet);

            // Backrow activations
            AddExecutor(ExecutorType.Activate, CardId.DOGMATIKA_PUNISHMENT, DogmatikaPunishmentActivate);
            AddExecutor(ExecutorType.Activate, CardId.ERADICATOR_EPIDEMIC_VIRUS, EradicatorActivate);
            AddExecutor(ExecutorType.Activate, CardId.WELCOME_LABYRINTH, WelcomeLabrynthActivate);
            AddExecutor(ExecutorType.Activate, CardId.SOLEMN_STRIKE, DefaultSolemnStrike);

            // GY effects
            AddExecutor(ExecutorType.Activate, CardId.ELDER_ENTITY_NTSS, ElderEntityNtssEffect);
            AddExecutor(ExecutorType.Activate, CardId.WIND_PEGASUS_IGNISTER, WindPegasusIngisterEffect);
            AddExecutor(ExecutorType.Activate, CardId.TRIBRIGADE_ARMS_BUCEPHALUS_II, TririgadeArmsEffect);
            AddExecutor(ExecutorType.Activate, CardId.GARURA);

            // Finalize monster positions
            AddExecutor(ExecutorType.Repos, CardId.LADY_LABRYNTH, LadyLabrynthRepos);
            AddExecutor(ExecutorType.Repos, CardId.ARIANNA, AriannaRepos);
        }

        private bool EvenlyMatchedSet()
        {
            if(Util.IsAllEnemyBetter() && Bot.Hand.GetCardCount(CardId.EVENLY_MATCHED) > 1 && Bot.GetFieldCount() != 0 && ProtectLadyLabrynth())
                return true;

            return false;
        }

        private bool SolemnStrikeSet()
        {
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
                return true;
            }

            return true;
        }

        private bool WelcomeLabrynthActivated = false;
        private bool ArrianaActivated = false;
        private bool LadyLabrynthActivated = false;

        List<int> Impermanence_list = new List<int>();

        public override void OnSelectChain(IList<ClientCard> cards)
        {
            foreach (var imperm in cards.GetMatchingCards(card => card.IsCode(10045474)))
            {
                Impermanence_list.Add(10045474); // Imperm ID
            }

            base.OnSelectChain(cards);
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

        private bool PotOfProsperityActivate()
        {
            if (Bot.ExtraDeck.Count <= 3) return false;

            int[] cost = new int[] {
                CardId.IP_MASQUERENA,
                CardId.LINKURIBOH,
                CardId.MUCKRACKER,
                CardId.KNIGHTMARE_UNICORN,
                CardId.KNIGHTMARE_PHEONIX,
                CardId.COSMIC_BLAZAR_DRAGON
            };

            AI.SelectCard(cost);

            List<int> prefferedCard = new List<int>();

            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.WELCOME_LABYRINTH))
                prefferedCard.Add(CardId.WELCOME_LABYRINTH);
            else if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LADY_LABRYNTH))
                prefferedCard.Add(CardId.LADY_LABRYNTH);
            else if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.ERADICATOR_EPIDEMIC_VIRUS))
                prefferedCard.Add(CardId.ERADICATOR_EPIDEMIC_VIRUS);
            else if (Enemy.GetFieldCount() > 1 && Bot.HasInExtra(CardId.ELDER_ENTITY_NTSS) && !Bot.HasInHandOrInGraveyard(CardId.DOGMATIKA_PUNISHMENT))
                prefferedCard.Add(CardId.DOGMATIKA_PUNISHMENT);
            else if (Enemy.GetFieldCount() > 1 && Bot.HasInExtra(CardId.ELDER_ENTITY_NTSS) && !Bot.HasInHandOrInGraveyard(CardId.COMPULSORY_EVACUATION_DEVICE) && Bot.HasInMonstersZone(CardId.LOVELY_LABRYNTH) && Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH))
                prefferedCard.Add(CardId.COMPULSORY_EVACUATION_DEVICE);
            else if (!Bot.HasInHand(CardId.ASH_BLOSSOM))
                prefferedCard.Add(CardId.ASH_BLOSSOM);
            else if (!Bot.HasInHandOrHasInMonstersZone(CardId.ARIANNA))
                prefferedCard.Add(CardId.ARIANNA);
            else if (!Bot.HasInHandOrInSpellZone(CardId.LABRYNTH_LABYRINTH))
                prefferedCard.Add(CardId.LABRYNTH_LABYRINTH);
            else if (!Bot.HasInHandOrInSpellZone(CardId.LABRYNTH_LABYRINTH))
                prefferedCard.Add(CardId.LABRYNTH_LABYRINTH);
            else if (Bot.GetFieldCount() <= 1 && Util.IsAllEnemyBetter())
                prefferedCard.Add(CardId.EVENLY_MATCHED);

            AI.SelectNextCard(prefferedCard);

            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }

            return true;
        }

        private bool LordOfHeavenlyPrisonActivate()
        {
            if (Card.Location == CardLocation.Hand && Duel.Player == 0)
            {
                if (Util.GetLastChainCard() != null)
                {
                    if (Util.GetLastChainCard().HasType(CardType.Spell) || Util.GetLastChainCard().HasType(CardType.Trap))
                        return false;
                }

                return true;
            }
            return false;
        }

        private bool TririgadeArmsEffect()
        {
            AI.SelectCard(CardId.GARURA);
            return true;
        }

        private bool FunitureSummon()
        {
            if (Bot.Deck.GetMatchingCards(card => card.IsCode(CardId.WELCOME_LABYRINTH)).Count() == 0 || Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH))
                return true;

            return false;
        }

        private bool WelcomeLabrynthSet()
        {
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }

            return !Bot.HasInSpellZone(CardId.WELCOME_LABYRINTH);
        }

        private bool KnightmarePheonix_Effect()
        {
            List<int> cost = PrefferedDiscardCost();

            ClientCard target = Util.GetBestEnemySpell();

            if (target != null)
            {
                AI.SelectCard(cost); // discard cost
                AI.SelectNextCard(target);

                return true;
            }

            return false;
        }

        private bool KnightmarePheonix_LinkSummon()
        {
            if(Util.GetBestEnemySpell() == null)
                return false;

            if ((Bot.HasInMonstersZone(CardId.CHANDRAGLIER)
                || Bot.HasInMonstersZone(CardId.STOVIE_TORBIE)
                || Bot.HasInMonstersZone(CardId.COOCLOCK))
                && Bot.MonsterZone.Count() > 1
                && Bot.Hand.Count() > 1
                )
            {
                int[] materials = new int[]
                {
                    CardId.STOVIE_TORBIE,
                    CardId.CHANDRAGLIER,
                    CardId.COOCLOCK
                };

                AI.SelectMaterials(materials);

                return true;
            }

            return false;
        }

        private List<int> PrefferedDiscardCost()
        {
            List<int> cost_candidates = new List<int>();

            if (Bot.HasInHand(CardId.CHANDRAGLIER) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.CHANDRAGLIER)).Count() > 1)
                cost_candidates.Add(CardId.CHANDRAGLIER);
            else if (Bot.HasInHand(CardId.LOVELY_LABRYNTH))
                cost_candidates.Add(CardId.LOVELY_LABRYNTH);
            else if (Bot.HasInHand(CardId.LABRYNTH_LABYRINTH) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.LABRYNTH_LABYRINTH)).Count() > 1)
                cost_candidates.Add(CardId.LABRYNTH_LABYRINTH);
            else if (Bot.HasInHand(CardId.STOVIE_TORBIE) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.STOVIE_TORBIE)).Count() > 1)
                cost_candidates.Add(CardId.LABRYNTH_LABYRINTH);
            else if (Bot.HasInHand(CardId.LORD_OF_HEAVENLY_PRISON) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.LORD_OF_HEAVENLY_PRISON)).Count() > 1)
                cost_candidates.Add(CardId.LORD_OF_HEAVENLY_PRISON);
            else if (Duel.Turn > 2)
                cost_candidates.Add(CardId.NIBIRU);
            else cost_candidates.AddRange(new List<int>() { CardId.POT_OF_PROSPERITY });

            return cost_candidates;
        }

        private int EradicatorOption()
        {
            int hand_enemySpells = Enemy.Hand.GetMatchingCards(card => card.HasType(CardType.Spell)).Count();
            int deck_enemySpells = Enemy.Deck.GetMatchingCards(card => card.HasType(CardType.Spell)).Count();

            int hand_enemyTraps = Enemy.Hand.GetMatchingCards(card => card.HasType(CardType.Trap)).Count();
            int deck_enemyTraps = Enemy.Deck.GetMatchingCards(card => card.HasType(CardType.Trap)).Count();


            int overallSpells = hand_enemySpells + deck_enemySpells;
            int overallTraps = hand_enemyTraps + deck_enemyTraps;

            if (overallSpells > overallTraps) return 1; // Spells
            else return 2; // Traps
        }

        private bool CompulsoryEvacDeviceSet()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (!Bot.HasInSpellZone(CardId.DOGMATIKA_PUNISHMENT))
            {
                if (problemMonster != null || ProtectLadyLabrynth() || Bot.GetFieldCount() <= 1)
                {
                    if (Card.Location == CardLocation.Hand)
                    {
                        AI.SelectPlace(SelectSTPlace(Card, true));
                    }

                    return true;
                }
            }

            return false;
        }

        private bool DogmatikaSet()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (!Bot.HasInSpellZone(CardId.DOGMATIKA_PUNISHMENT))
            {
                if (Card.Location == CardLocation.Hand)
                {
                    AI.SelectPlace(SelectSTPlace(Card, true));
                }

                if (problemMonster != null || Bot.GetFieldCount() <= 1) return true;
                else if (problemMonster == null && ProtectLadyLabrynth()) return true;
                else if (Bot.GetFieldCount() == 0) return true;
            }

            return false;
        }

        public bool ProtectLadyLabrynth()
        {
            return Bot.HasInMonstersZone(CardId.LADY_LABRYNTH);
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
            ClientCard card = Util.GetBestEnemyMonster();

            if (card != null)
            {
                AI.SelectCard(card);
                return true;
            }

            return false;
        }

        private bool EradicatorActivate()
        {
            if (Bot.HasInMonstersZone(CardId.LADY_LABRYNTH))
            {
                ClientCard target = Util.GetBestEnemySpell();

                // Tribute
                AI.SelectCard(CardId.LADY_LABRYNTH);

                if (target != null)
                {
                    if (target.HasType(CardType.Spell)) AI.SelectOption(1);
                    else AI.SelectOption(2);
                }
                else // if there are no backrows, decide if their build is more on spells or traps
                {
                    int option = EradicatorOption();
                    AI.SelectOption(option);
                }

                return true;
            }
            else return false;
        }

        private bool ChandraglierSummon()
        {
            if (Bot.Deck.GetMatchingCards(card => card.IsCode(CardId.WELCOME_LABYRINTH)).Count() == 0 || Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH))
                return true;

            return false;
        }

        private bool LabrynthLabyrinthActivate()
        {
            if (Card.Location == CardLocation.SpellZone && Card.IsFaceup()) // special summon labrynth monster effect
            {
                if (ActivateDescription == Util.GetStringId(CardId.LABRYNTH_LABYRINTH, 0))
                {
                    if (Bot.HasInGraveyard(CardId.LOVELY_LABRYNTH))
                        AI.SelectCard(CardId.LOVELY_LABRYNTH);
                    else if((Bot.HasInGraveyard(CardId.LADY_LABRYNTH)))
                        AI.SelectCard(CardId.LADY_LABRYNTH);

                    return true;
                }

                return false;
            }
            else
            {
                if (!Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH)) return true;
                else if (Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH) && Card.Location == CardLocation.SpellZone && Card.IsFacedown()) return true;

                return false;
            }
        }

        private bool MuckarackerEffect()
        {
            int[] prefferedMonsters = new int[]
            {
                    CardId.LOVELY_LABRYNTH,
                    CardId.LADY_LABRYNTH
            };

            List<int> cost = PrefferedDiscardCost();

            if (cost.Count() > 0 && Bot.HasInGraveyard(CardId.LADY_LABRYNTH) || Bot.HasInGraveyard(CardId.LOVELY_LABRYNTH))
            {
                AI.SelectCard(prefferedMonsters);
                AI.SelectNextCard(cost);

                return true;
            }

            // Protect Key MOnsters
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (ActivateDescription == Util.GetStringId(CardId.MUCKRACKER, 0))
                {
                    List<int> cost_candidates = new List<int>();

                    if (Enemy.BattlingMonster != null)
                    {
                        if (Enemy.BattlingMonster.IsCode(CardId.LOVELY_LABRYNTH) || Enemy.BattlingMonster.IsCode(CardId.LOVELY_LABRYNTH))
                        {
                            cost_candidates.AddRange(new List<int>() {
                                CardId.CHANDRAGLIER,
                                CardId.STOVIE_TORBIE,
                                CardId.COOCLOCK,
                                CardId.ARIANNA,
                                CardId.MUCKRACKER
                            });

                            AI.SelectThirdCard(cost_candidates);

                            return true;
                        }
                        return false;
                    }
                }
            }

            return false;
        }

        private bool MuckarackerLinkSummon()
        {
            int bot_Monsters = Bot.MonsterZone.GetMatchingCards(card => card.Level <= 4).Count();
            int bot_HandCards = Bot.Hand.Count();

            int[] materials = new int[]
            {
                CardId.STOVIE_TORBIE,
                CardId.CHANDRAGLIER,
                CardId.ARIANNA,
                CardId.COOCLOCK,
                CardId.KNIGHTMARE_PHEONIX,
                CardId.KNIGHTMARE_UNICORN
            };

            if (bot_HandCards >= 1 && !Bot.HasInMonstersZone(CardId.MUCKRACKER) && (Bot.HasInMonstersZoneOrInGraveyard(CardId.LADY_LABRYNTH) || Bot.HasInMonstersZoneOrInGraveyard(CardId.LOVELY_LABRYNTH)))
            {
                AI.SelectMaterials(materials);
                return true;
            }

            if (Bot.HasInMonstersZone(CardId.STOVIE_TORBIE) && Bot.HasInMonstersZone(CardId.CHANDRAGLIER))
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
                {
                    if (Card.Location == CardLocation.Hand)
                    {
                        AI.SelectPlace(SelectSTPlace(Card, true));
                        return true;
                    }
                }
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
                CardId.DOGMATIKA_PUNISHMENT,
                CardId.COMPULSORY_EVACUATION_DEVICE
            };

            IList<ClientCard> enemySpellZone = Enemy.SpellZone.GetMatchingCards(card =>
                card.HasType(CardType.Spell)
                || card.HasType(CardType.Trap)
                || card.HasPosition(CardPosition.FaceDown)
            );

            if (!Bot.HasInHandOrInSpellZoneOrInGraveyard(CardId.ERADICATOR_EPIDEMIC_VIRUS))
            {
                ClientCard eradicator = Bot.Deck.GetFirstMatchingCard(card => card.IsCode(CardId.ERADICATOR_EPIDEMIC_VIRUS));
                AI.SelectCard(CardId.ERADICATOR_EPIDEMIC_VIRUS);
                AI.SelectPlace(SelectSTPlace(eradicator, true));
            }
                
            else AI.SelectCard(prefferedTraps);

            LadyLabrynthActivated = true;

            return true;
        }

        private bool EvenlyMatchedActivate()
        {
            if (Card.Location == CardLocation.Hand)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }

            return Bot.HasInHand(CardId.EVENLY_MATCHED) && Duel.Turn >= 2 && Enemy.GetFieldCount() >= 2 && Bot.GetFieldCount() == 0;
        }

        private bool DogmatikaPunishmentActivate()
        {
            ClientCard problemMonster = Util.GetProblematicEnemyMonster();

            if (problemMonster != null)
            {
                if (!Enemy.IsFieldEmpty() && Enemy.GetFieldCount() > 1)
                    AI.SelectCard(CardId.ELDER_ENTITY_NTSS);
                else
                {
                    List<int> prefferedMonsters = new List<int>
                    {
                        CardId.WIND_PEGASUS_IGNISTER,
                        CardId.TRIBRIGADE_ARMS_BUCEPHALUS_II
                    };

                    AI.SelectCard(prefferedMonsters);
                }
                    

                AI.SelectNextCard(problemMonster);

                if (Duel.LastChainPlayer == 0 && Util.GetLastChainCard().IsCode(CardId.COMPULSORY_EVACUATION_DEVICE))
                    return false;

                return true;
            }

            return false;
        }

        private bool StovieTorbieEffect()
        {
            return FurnitureActivation();
        }

        public override void OnNewTurn()
        {
            LadyLabrynthActivated = false;
            WelcomeLabrynthActivated = false;
            ArrianaActivated = false;
        }

        public override CardPosition OnSelectPosition(int CardID, IList<CardPosition> positions)
        {
            if (CardID == 27204312 || CardID == CardId.STOVIE_TORBIE || CardID == CardId.COOCLOCK)
            {
                return CardPosition.FaceUpDefence;
            }
            else if (CardID == CardId.LADY_LABRYNTH || CardID == CardId.LOVELY_LABRYNTH)
            {
                return CardPosition.FaceUpAttack;
            }
            else if ((CardID == CardId.ARIANNA || CardID == CardId.CHANDRAGLIER) && !Util.IsAllEnemyBetter())
            {
                return CardPosition.FaceUpAttack;
            }

            return base.OnSelectPosition(CardID, positions);
        }

        private bool SolemnStrikeActivate()
        {
            ClientCard problemCard = Util.GetProblematicEnemyMonster();

            if(Duel.LastChainPlayer == 0)
                return false;

            if (problemCard != null && Duel.LastChainPlayer == 1)
                return true;

            return false;
        }

        private bool LovelyLabrynthEffect()
        {
            int[] supportTraps = new int[] {
                CardId.WELCOME_LABYRINTH,
                CardId.COMPULSORY_EVACUATION_DEVICE,
                CardId.DOGMATIKA_PUNISHMENT
            };

            if (Bot.HasInGraveyard(CardId.ERADICATOR_EPIDEMIC_VIRUS))
            {
                ClientCard eradicator = Bot.Deck.GetFirstMatchingCard(card => card.IsCode(CardId.ERADICATOR_EPIDEMIC_VIRUS));
                AI.SelectCard(CardId.ERADICATOR_EPIDEMIC_VIRUS);
                AI.SelectPlace(SelectSTPlace(eradicator, true));
            }
                
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
                        Util.GetLastChainCard().IsCode(CardId.DOGMATIKA_PUNISHMENT)
                        || Util.GetLastChainCard().IsCode(CardId.COMPULSORY_EVACUATION_DEVICE)
                        || Util.GetLastChainCard().IsCode(CardId.ELDER_ENTITY_NTSS)
                        || Util.GetLastChainCard().IsCode(CardId.WELCOME_LABYRINTH)
                    )
                )
                {
                    return false;
                }

                AI.SelectCard(enemyBestMonster);

                return true;
            }

            return false;
        }

        public bool isEvenlyMatchedOnHand()
        {
            return Bot.HasInHand(CardId.EVENLY_MATCHED);
        }

        public bool WelcomeLabrynthActivate()
        {
            if(Duel.Player == 0 && !(Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return true;

            List<int> preferred_Monsters = new List<int>();

            if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.LOVELY_LABRYNTH))
                preferred_Monsters.Add(CardId.LOVELY_LABRYNTH);

            if (!Bot.HasInHandOrHasInMonstersZone(CardId.LADY_LABRYNTH))
                preferred_Monsters.Add(CardId.LADY_LABRYNTH);

            if(Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.LADY_LABRYNTH) && Bot.HasInMonstersZoneOrInGraveyard(CardId.LOVELY_LABRYNTH) || ArrianaActivated != false)
                preferred_Monsters.Add(CardId.ARIANNA);

            AI.SelectCard(preferred_Monsters);

            WelcomeLabrynthActivated = true;

            // Added destroy one card effect by Labrynth Labirynth to Welcome Labrynth
            if (Card.Location == CardLocation.SpellZone)
            {
                if (ActivateDescription == Util.GetStringId(CardId.WELCOME_LABYRINTH, 0))
                {
                    ClientCard problemCard = Util.GetBestEnemyCard();

                    if (problemCard != null && Enemy.GetFieldCount() > 0)
                    {
                        AI.SelectYesNo(true);
                        AI.SelectNextCard(problemCard);

                        return true;
                    }
                    else
                    {
                        AI.SelectYesNo(false);
                        return false;
                    };
                }
            }
            else if (Card.Location == CardLocation.Grave)
            {
                AI.SelectPlace(SelectSTPlace(Card, true));
            }

            return true;
        }

        public bool AriannaEffect()
        {
            int[] other_prefferedCards = new int[] {
                CardId.COOCLOCK,
                CardId.STOVIE_TORBIE
            };

            if (!Bot.HasInHandOrInSpellZone(CardId.WELCOME_LABYRINTH))
                AI.SelectCard(CardId.WELCOME_LABYRINTH);
            else if (
                    Bot.HasInHandOrInSpellZone(CardId.WELCOME_LABYRINTH)
                    && Bot.Deck.GetMatchingCards(card => card.IsCode(CardId.COOCLOCK)).Count() > 0
                    && Bot.MonsterZone.GetMatchingCards(card => card.HasPosition(CardPosition.FaceUp)).Count() > 0
                   )
                AI.SelectCard(CardId.LABRYNTH_LABYRINTH);
            else
                AI.SelectCard(other_prefferedCards);

            ArrianaActivated = true;

            return true;
        }

        public bool CooclockEffect()
        {

            IList<ClientCard> faceUp_Monsters = Bot.MonsterZone.GetMatchingCards(card =>
                    card.IsCode(CardId.LOVELY_LABRYNTH)
                    || card.IsCode(CardId.LADY_LABRYNTH)
                    || card.IsCode(CardId.STOVIE_TORBIE)
                    || card.IsCode(CardId.ARIANNA)
                    || card.IsCode(CardId.CHANDRAGLIER)
                );

            if (Card.Location == CardLocation.Hand
                && (Bot.HasInSpellZone(CardId.WELCOME_LABYRINTH) || Bot.HasInSpellZone(CardId.ERADICATOR_EPIDEMIC_VIRUS))
                && !WelcomeLabrynthActivated
                && Duel.Player == 0
                && Bot.MonsterZone.GetMatchingCards(card => card.HasRace(CardRace.Fiend)).Count() > 0)
                return true;
            else if (Card.Location == CardLocation.Grave)
            {
                AI.SelectOption(0); // first dialogue
                AI.SelectOption(0); // choose add Cooclock to hand
                return true;
            };

            return false;
        }

        public bool ChandraglierEffect()
        {
            return FurnitureActivation();
        }

        public bool FurnitureActivation()
        {
            if (Card.Location == CardLocation.Hand && Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2)
            {
                // Look for duplicate copies of the same card or Chandraglier and use them as materials.
                List<int> cost_candidates = new List<int>();

                if (Bot.HasInHand(CardId.CHANDRAGLIER) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.CHANDRAGLIER)).Count() > 1)
                    cost_candidates.Add(CardId.CHANDRAGLIER);
                else if (Bot.HasInHand(CardId.LOVELY_LABRYNTH))
                    cost_candidates.Add(CardId.LOVELY_LABRYNTH);
                else if (Bot.HasInHand(CardId.LABRYNTH_LABYRINTH) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.LABRYNTH_LABYRINTH)).Count() > 1)
                    cost_candidates.Add(CardId.LABRYNTH_LABYRINTH);
                else if (Bot.HasInHand(CardId.STOVIE_TORBIE) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.STOVIE_TORBIE)).Count() > 1)
                    cost_candidates.Add(CardId.STOVIE_TORBIE);
                else if (Bot.HasInHand(CardId.LORD_OF_HEAVENLY_PRISON) && Bot.Hand.GetMatchingCards(card => card.IsCode(CardId.LORD_OF_HEAVENLY_PRISON)).Count() > 1)
                    cost_candidates.Add(CardId.LORD_OF_HEAVENLY_PRISON);
                else if (Duel.Turn > 2)
                    cost_candidates.Add(CardId.NIBIRU);
                else cost_candidates.AddRange(new List<int>() { CardId.POT_OF_PROSPERITY, CardId.COOCLOCK });

                // Get the next Labrynth spell and trap

                if (Duel.Player == 0)
                {
                    if (cost_candidates.Count() > 0)
                    {
                        AI.SelectCard(cost_candidates);

                        int deck_remaining_welcomes = Bot.Deck.GetMatchingCards(card => card.IsCode(CardId.WELCOME_LABYRINTH)).Count();

                        if (!Bot.HasInSpellZone(CardId.WELCOME_LABYRINTH))
                        {
                            AI.SelectNextCard(CardId.WELCOME_LABYRINTH);
                            AI.SelectPlace(SelectSTPlace(Bot.Deck.GetFirstMatchingCard(card => card.IsCode(CardId.WELCOME_LABYRINTH)), true));
                        }
                        else if (!Bot.HasInHandOrInSpellZone(CardId.LABRYNTH_LABYRINTH))
                            AI.SelectNextCard(CardId.LABRYNTH_LABYRINTH);

                        return true;
                    }
                }
            }
            else if (Card.Location == CardLocation.Grave) return true;
            else if (Card.Location == CardLocation.MonsterZone)
            {
                if (Bot.HasInSpellZone(CardId.LABRYNTH_LABYRINTH))
                {
                    int remaining_welcomes = Bot.Deck.GetMatchingCards(card => card.IsCode(CardId.WELCOME_LABYRINTH)).Count();

                    if (remaining_welcomes > 0)
                    {
                        AI.SelectNextCard(CardId.WELCOME_LABYRINTH);
                        return true;
                    }
                    else return false;
                }
                else
                {
                    AI.SelectNextCard(CardId.LABRYNTH_LABYRINTH);
                    return true;
                }
            };

            return false;
        }

        public class CardId
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
            public const int LORD_OF_HEAVENLY_PRISON = 9822220;

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
            public const int COSMIC_BLAZAR_DRAGON = 21123811;
            public const int WIND_PEGASUS_IGNISTER = 98506199;
            public const int TRIBRIGADE_ARMS_BUCEPHALUS_II = 10019086;
            public const int KNIGHTMARE_PHEONIX = 2857636;
            public const int MUCKRACKER = 71607202;
            public const int IP_MASQUERENA = 65741786;
            public const int LINKURIBOH = 41999284;
            public const int GARURA = 11765832;
            public const int FIVE_HEADED_DRAGON = 99267150;
            public const int KNIGHTMARE_UNICORN = 38342335;

        }
    }
}
