using System;
using System.Collections.Generic;
using System.Linq;
using GondrLib.Dependencies;
using HN.Code.Core;
using HN.Code.Events;
using HN.Code.Events.Systems;
using HN.Code.Maps;
using UnityEngine;

namespace HN.Code.Rank
{
    [Provide]
    public class RankManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private MapDataSO testMap; //test code
        [SerializeField] private float _updateRankingInterval = 0.02f;

        [Inject] private GameTimer _timer;
        private HashSet<CornerFlag> _corners = new HashSet<CornerFlag>();
        private RankingContainer _rankingContainer;
        private float _lastUpdateTime;

        private void Awake()
        {
            Bus<RankableAddEvent>.OnEvent += HandleRankableAdded;
            InitCorners();
            _rankingContainer = new RankingContainer(_corners.ToList());
        }

        private void OnDestroy()
        {
            Bus<RankableAddEvent>.OnEvent -= HandleRankableAdded;

            foreach (CornerFlag corner in _corners)
            {
                corner.OnCornerEnterEvent -= HandleCornerEnter;
            }
        }

        private void InitCorners()
        {
            _corners = FindObjectsByType<CornerFlag>(FindObjectsSortMode.None).ToHashSet();
            foreach (CornerFlag corner in _corners)
            {
                corner.OnCornerEnterEvent += HandleCornerEnter;
            }
        }

        private void Update()
        {
            if (_lastUpdateTime + _updateRankingInterval > Time.time) return;

            _lastUpdateTime = Time.time;
            _rankingContainer?.CalculateRanks();
            Bus<RankingUpdateEvent>.Raise(new RankingUpdateEvent(GetRakingList()));
        }

        private void HandleCornerEnter(IRankRecordable rankable, int cornerIdx, bool isGoal)
        {
            rankable.CurrentCorner = cornerIdx;

            if (isGoal)
            {
                float goalTime = _timer.CurrentTime;

                rankable.CurrentLap++;
                rankable.CompleteLap(goalTime);

                if (rankable.CurrentLap == testMap.lap + 1)
                {
                    if (_rankingContainer.IsStartRetire() == false)
                        Bus<RetireStartEvent>.Raise(new RetireStartEvent());

                    int rank = _rankingContainer.Ranking.IndexOf(rankable) + 1;
                    Bus<GoalEvent>.Raise(new GoalEvent(rankable, rank, goalTime));
                    _rankingContainer.Goal(rankable, goalTime);
                    rankable.Goal();
                }
            }
        }

        public List<IRankRecordable> GetRakingList() => _rankingContainer.Ranking;
        public int GetRanking(IRankRecordable rankable) => _rankingContainer.Ranking.IndexOf(rankable) + 1;
        private void HandleRankableAdded(RankableAddEvent evt) => _rankingContainer.AddRanking(evt.rankable);
        public bool TryGetGoalTime(IRankRecordable rankable, out float goalTime) => _rankingContainer.TryGetGoalTime(rankable, out goalTime);
    }
}