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
    public class ClientGameManager
    {
        private JoinAllocation _joinAllocation;

        public async Task<bool> StartClientWithJoinCode(string joinCode)
        {
            try
            {
                _joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(_joinAllocation.ToRelayServerData("dtls"));

                return NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
    }
}