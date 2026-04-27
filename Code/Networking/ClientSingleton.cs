using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace HN.Code.Networking
{
    public class ClientSingleton : MonoSingleton<ClientSingleton>
    {
        public ClientGameManager GameManager { get; private set; }

        public void CreateClient()
        {
            GameManager = new ClientGameManager();

            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            Debug.Log($"{clientId} is disconnected");
        }
    }
}