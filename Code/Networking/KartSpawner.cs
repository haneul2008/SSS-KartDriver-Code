using System.Collections.Generic;
using System.Linq;
using HN.Code.References;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HN.Code.Networking
{
    public class KartSpawner : NetworkBehaviour
    {
        [SerializeField] private List<KartSO> kartDatas;

        private Dictionary<PlayerCartType, GameObject> _prefabPairs;

        public override async void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _prefabPairs = kartDatas.ToDictionary(data => data.cartType, data => data.kartPrefab);

            if (SceneManager.GetActiveScene().isLoaded && SceneManager.GetActiveScene().name == SceneNames.GameScene)
                HandleLoadComplete(string.Empty, default, null, null);
            else
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleLoadComplete;
        }

        private async void HandleLoadComplete(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            Data data = await GetPlayerData.Instance.GetDataSelf();
            PlayerCartType kartType = data.CarTypeEnum;
            
            if (IsOwner)
                SpawnKartServerRpc(kartType);
        }

        [ServerRpc]
        private void SpawnKartServerRpc(PlayerCartType kartType, ServerRpcParams rpcParams = default)
        {
            GameObject prefab = _prefabPairs[kartType];
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            ulong clientId = rpcParams.Receive.SenderClientId;
            
            NetworkObject networkObject = obj.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId);
            gameObject.SetActive(false);
        }
    }
}