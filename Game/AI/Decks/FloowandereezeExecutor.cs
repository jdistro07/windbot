using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System;
using System.Linq;

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
        }

        public FloowandereezeExecutor(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.DimensionShifter);

            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality, PotOfDualityActivate);
            AddExecutor(ExecutorType.Summon, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);

            AddExecutor(ExecutorType.Summon, CardId.Robina);
            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenEffect);

            // Chain block for strongest effect monster
            AddExecutor(ExecutorType.Activate, CardId.Empen, EmpenEffect);
            AddExecutor(ExecutorType.Activate, CardId.Robina, RobinaBanishedEffect);
            AddExecutor(ExecutorType.Activate, CardId.Eglen, EglenBanishedEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.DreamingTown, DreamingTownSet);
            AddExecutor(ExecutorType.Activate, CardId.DreamingTown);
            AddExecutor(ExecutorType.SpellSet, CardId.HarpiesFeatherStorm);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherStorm);
        }

        private bool DreamingTownSet()
        {
            return Bot.HasInHand(CardId.Robina);
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
            if (Bot.HasInBanished(CardId.Robina) && Card.Location != CardLocation.MonsterZone)
                return true;
            return false;
        }

        private bool EglenBanishedEffect()
        {
            if (Bot.HasInBanished(CardId.Eglen) && Card.Location != CardLocation.MonsterZone)
                return true;
            return false;
        }

        private bool RobinaEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (!(Bot.HasInHandOrHasInMonstersZone(CardId.Eglen) || Bot.HasInBanished(CardId.Eglen)))
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

                    if (Bot.HasInHand(CardId.Eglen))
                    {
                        AI.SelectCard(CardId.Eglen);
                    }
                    else
                    {
                        AI.SelectCard(otherPrefferedCards);
                        AI.SelectNextCard(otherPrefferedCards);
                    }

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
                return true;
            }
            else
            {
                AI.SelectCard(other_prefferedCards);
                return true;
            }
        }

        private bool EglenEffect()
        {
            List<ClientCard> materials = new List<ClientCard>();
            materials = Bot.MonsterZone.GetMatchingCards(card => card.Level == 1).ToList();

            int[] prefferedCards = new int[]
            {
                CardId.Empen,
                CardId.Snowl,
                CardId.Avian
            };

            if (!Bot.HasInHandOrHasInMonstersZone(CardId.Empen))
            {
                // Fetch empen from deck
                AI.SelectCard(CardId.Empen);
                AI.SelectNextCard(CardId.Empen);

                AI.SelectMaterials(materials);

                return true;
            }
            else
            {
                AI.SelectCard(prefferedCards);
                AI.SelectMaterials(prefferedCards);

                return true;
            }
        }
    }

        

    

}
