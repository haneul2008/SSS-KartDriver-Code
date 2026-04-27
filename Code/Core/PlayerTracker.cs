using HN.Code.Events;
using HN.Code.Events.Systems;
using Unity.Netcode;
using UnityEngine;

namespace HN.Code.Core
{
    public class PlayerTracker : NetworkBehaviour
    {
        private NetworkVariable<int> _spawnedPlayerCnt = new NetworkVariable<int>
        (
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private int _targetPlayerCnt;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _targetPlayerCnt = ApplicationManager.Instance.PlayerCnt;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc()
        {
            _spawnedPlayerCnt.Value++;
            
            if (_targetPlayerCnt == _spawnedPlayerCnt.Value)
            {
                AllPlayerReadyClientRpc();
            }
        }

        [ClientRpc]
        private void AllPlayerReadyClientRpc()
        {
            Bus<LoadAllKartEvent>.Raise(new LoadAllKartEvent());
        }
    }
}