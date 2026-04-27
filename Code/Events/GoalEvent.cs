using HN.Code.Events.Systems;
using HN.Code.Rank;

namespace HN.Code.Events
{
    public struct GoalEvent : IEvent
    {
        public IRankRecordable rankable;
        public int rank;
        public float goalTime;

        public GoalEvent(IRankRecordable rankable, int rank, float goalTime)
        {
            this.rankable = rankable;
            this.rank = rank;
            this.goalTime = goalTime;
        }
    }
}