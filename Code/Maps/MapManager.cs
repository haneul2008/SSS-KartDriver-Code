using HN.Code.Events;
using HN.Code.Events.Systems;
using RVP;
using Unity.Netcode;
using UnityEngine;

namespace HN.Code.Maps
{
    public class MapManager : NetworkBehaviour
    {
        [SerializeField] private MapDataSO testMap; //Test code
        [SerializeField] private Vector3 spawnPosAdder;

        private int _currentKartIdx;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
                Bus<RankableAddEvent>.OnEvent += HandleKartAdded;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (IsServer)
                Bus<RankableAddEvent>.OnEvent -= HandleKartAdded;
        }

        private void HandleKartAdded(RankableAddEvent evt)
        {
            if (evt.rankable is HN.Code.Karts.Kart kart == false) return;

            Vector3 spawnPos = testMap.startPos + spawnPosAdder * _currentKartIdx;
            
            _currentKartIdx++;
            
            SetKartPosClientRpc(kart.NetworkObjectId, spawnPos);
        }

        [ClientRpc]
        private void SetKartPosClientRpc(ulong clientId, Vector3 pos)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clientId, out NetworkObject obj))
            {
                obj.transform.position = pos;
                obj.transform.eulerAngles = testMap.startRotation;
                
                if (obj.IsOwner && obj.TryGetComponent(out VehicleDebug debug))
                {
                    debug.spawnPos = pos;
                }
            }
        }
    }
}