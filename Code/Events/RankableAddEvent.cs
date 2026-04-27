using HN.Code.Events.Systems;
using HN.Code.Rank;

namespace HN.Code.Events
{
    public struct RankableAddEvent : IEvent
    {
        public IRankRecordable rankable;

        public RankableAddEvent(IRankRecordable rankable)
        {
            this.rankable = rankable;
        }
    }
}