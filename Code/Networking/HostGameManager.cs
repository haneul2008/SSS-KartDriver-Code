using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HN.Code.Networking
{
    public class HostGameManager
    {
        public string JoinCode => _joinCode;
        
        private Allocation _relayAllocation;
        private string _joinCode;

        public async Task<bool> MakeJoinCode(int playerCnt)
        {
            try
            {
                _relayAllocation = await RelayService.Instance.CreateAllocationAsync(playerCnt, "asia-northeast3");
                _joinCode = await RelayService.Instance.GetJoinCodeAsync(_relayAllocation.AllocationId);
                Debug.Log(_joinCode);

                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(_relayAllocation.ToRelayServerData("dtls"));
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public bool StartHost() => NetworkManager.Singleton.StartHost();

        public void ChangeNetworkScene(string sceneName)
            => NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}