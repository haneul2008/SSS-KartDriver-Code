using HN.Code.Events.Systems;
using UnityEngine;

namespace HN.Code.Events
{
    public struct SetRestartPosEvent : IEvent
    {
        public Vector3 pos;
        public Quaternion rot;

        public SetRestartPosEvent(Vector3 pos,Quaternion roation)
        {
            this.pos = pos;
            rot = roation;
        }
    }
}