using System;
using System.Linq;
using HN.Code.Events;
using HN.Code.Events.Systems;
using Unity.Netcode;
using UnityEngine;

namespace HN.Code.Rank
{
    public class CornerFlag : MonoBehaviour
    {
        public delegate void CornerFlagDelegate(IRankRecordable rankable, int cornerIdx, bool isGoal);

        public event CornerFlagDelegate OnCornerEnterEvent;

        public int CornerIdx => cornerIdx;

        [SerializeField] private Transform restartTrm;
        [SerializeField] private int cornerIdx;
        [SerializeField] private bool isGoal;
        [SerializeField] private BoxCollider _collider;

        private int _lastIdx;

        private void Awake()
        {
            _lastIdx = FindObjectsByType<CornerFlag>(FindObjectsSortMode.None).ToList()
                .OrderByDescending(corner => corner.CornerIdx).First().cornerIdx;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.TryGetComponent(out IRankRecordable rankable) && other.CompareTag("Body"))
            {
                int currentCorner = rankable.CurrentCorner;
                
                if (isGoal == false && cornerIdx - 1 != currentCorner) return;
                if (rankable is HN.Code.Karts.Kart kart && isGoal &&
                    (currentCorner != _lastIdx && currentCorner > 0))
                {
                    kart.ResetPos();
                    return;
                }

                OnCornerEnterEvent?.Invoke(rankable, cornerIdx, isGoal);
                if(rankable is NetworkBehaviour networkBehaviour && networkBehaviour.IsOwner)
                    Bus<SetRestartPosEvent>.Raise(new SetRestartPosEvent(restartTrm.position,restartTrm.localRotation));
            }
        }

        private void OnDrawGizmos()
        {
            if (_collider == null) return;

            Gizmos.color = isGoal ? Color.green : Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_collider.center, _collider.size);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}