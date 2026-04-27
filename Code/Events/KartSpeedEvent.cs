using HN.Code.Events.Systems;

namespace HN.Code.Events
{
    public struct KartSpeedEvent : IEvent
    {
        public int speed;

        public KartSpeedEvent(int speed)
        {
            this.speed = speed;
        }
    }
}