using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HN.Code.Rank
{
    public class GoalInfo
    {
        public float goalTime;
    }
    
    public class RankingContainer
    {
        private readonly Dictionary<IRankRecordable, GoalInfo> _goalPairs = new Dictionary<IRankRecordable, GoalInfo>();
        public List<IRankRecordable> Ranking { get; private set; } = new List<IRankRecordable>();
        private List<CornerFlag> _corners;

        public RankingContainer(List<CornerFlag> corners)
        {
            _corners = corners.OrderBy(corner => corner.CornerIdx).ToList();
        }
        
        public void AddRanking(IRankRecordable rankRecordable)
        {
            if(Ranking.Contains(rankRecordable) == false)
                Ranking.Add(rankRecordable);
        }

        public void Goal(IRankRecordable rankRecordable, float goalTime)
        {
            _ = _goalPairs.TryAdd(rankRecordable, new GoalInfo { goalTime = goalTime });
        }

        public void CalculateRanks()
        {
            Ranking = Ranking
                .OrderBy(GetGoalTimeOrDefault)
                .ThenByDescending(rankable => rankable.CurrentLap)
                .ThenByDescending(rankable => rankable.CurrentCorner)
                .ThenBy(GetCornerDistance)
                .ToList();
        }

        private float GetCornerDistance(IRankRecordable rankable)
        {
            if (rankable is MonoBehaviour monoBehaviour == false || monoBehaviour == null) return float.MaxValue;
            int nextCornerIdx = (rankable.CurrentCorner + 1) % _corners.Count;
            
            Vector3 rankablePos = monoBehaviour.transform.position;
            Vector3 cornerPos = _corners[nextCornerIdx].transform.position;
            
            return Vector3.Distance(rankablePos, cornerPos);
        }

        private float GetGoalTimeOrDefault(IRankRecordable rankable)
        {
            if(IsGoal(rankable))
                return _goalPairs[rankable].goalTime;

            return float.MaxValue;
        }

        private bool IsGoal(IRankRecordable rankable) => _goalPairs.GetValueOrDefault(rankable) != null;
        public bool IsStartRetire() => _goalPairs.GetValueOrDefault(Ranking[0]) != null;

        public bool TryGetGoalTime(IRankRecordable rankable, out float goalTime)
        {
            if (IsGoal(rankable))
            {
                goalTime = _goalPairs[rankable].goalTime;
                return true;
            }

            goalTime = 0f;
            return false;
        }
    }
}