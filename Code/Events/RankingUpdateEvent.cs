using System.Collections.Generic;
using HN.Code.Events.Systems;
using HN.Code.Rank;

namespace HN.Code.Events
{
    public struct RankingUpdateEvent : IEvent
    {
        public List<IRankRecordable> ranking;

        public RankingUpdateEvent(List<IRankRecordable> ranking)
        {
            this.ranking = ranking;
        }
    }
}