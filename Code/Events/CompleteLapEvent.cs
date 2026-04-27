using HN.Code.Events.Systems;

namespace HN.Code.Events
{
    public struct CompleteLapEvent : IEvent
    {
        public int lap;
        public bool isFirst;
        public float completeTime;

        public CompleteLapEvent(int lap, bool isFirst, float completeTime)
        {
            this.lap = lap;
            this.isFirst = isFirst;
            this.completeTime = completeTime;
        }
    }
}