﻿using System.Collections.Generic;
using System.Linq;
using XnaDarts.Screens.GameScreens;
using XnaDarts.Screens.Menus;

namespace XnaDarts.Gameplay.Modes
{
    public class BastardHit
    {
        public Dart Dart;
        public int Round;
        public Player SegmentOwner;
        public Player ThrownBy;

        public BastardHit(Dart dart, Player thrownBy, Player segmentOwner, int round)
        {
            Dart = dart;
            ThrownBy = thrownBy;
            SegmentOwner = segmentOwner;
            Round = round;
        }
    }

    public class Bastard : GameMode
    {
        public override int GetScore(Player player)
        {
            return calculatePlayerScores()[player];
        }

        public override int GetScore(Dart dart)
        {
            return dart.Multiplier;
        }

        private Dictionary<Player, int> calculatePlayerScores()
        {
            var playerScores = new Dictionary<Player, int>();
            Players.ForEach(x => playerScores.Add(x, StartScore));

            for (var i = 0; i <= CurrentRoundIndex; i++)
            {
                for (var j = 0; j < Players.Count; j++)
                {
                    var player = Players[j];
                    var round = player.Rounds[i];

                    for (var k = 0; k < round.Darts.Count; k++)
                    {
                        var dart = round.Darts[k];
                        //If the owner of the segment is the one we are calculating score for and the owner of the dart is the player
                        Player segementOwner;
                        if (PlayerSegments.Keys.Contains(dart.Segment))
                        {
                            segementOwner = PlayerSegments[dart.Segment];
                        }
                        else
                        {
                            continue;
                        }

                        if (segementOwner == player)
                        {
                            playerScores[player] -= dart.Multiplier;
                        }
                        else
                        {
                            playerScores[segementOwner] += dart.Multiplier;
                        }
                    }
                }
            }
            return playerScores;
        }

        #region Fields and Properties

        private BastardSummaryScreen _summaryScreen;
        public int StartScore = 10;

        public override string Name
        {
            get { return "Bastard"; }
        }

        public Dictionary<int, Player> PlayerSegments = new Dictionary<int, Player>();

        #endregion

        #region Constructor and Setup

        public Bastard(int players)
            : base(players)
        {
            setupSegments(players);
            _summaryScreen = new BastardSummaryScreen(this);

            ScoringDirection = Direction.Asc;
        }

        private void setupSegments(int players)
        {
            var segmentsPerPlayer = 20/players;

            for (var i = 0; i < segmentsPerPlayer; i++)
            {
                for (var j = i*players; j < players*(i + 1); j++)
                {
                    PlayerSegments.Add(Dartboard.SegmentOrder[j], Players[j - i*players]);
                }
            }
        }

        #endregion
    }
}