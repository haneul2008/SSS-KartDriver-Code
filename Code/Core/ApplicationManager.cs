using System;
using System.Threading.Tasks;
using HN.Code.Events;
using HN.Code.Events.Systems;
using HN.Code.Networking;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HN.Code.Core
{
    public class ApplicationManager : MonoSingleton<ApplicationManager>
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        
        public int PlayerCnt { get; private set; }
        public bool IsHost { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            HostSingleton host = Instantiate(hostPrefab, transform);
            host.CreateHost();
            
            ClientSingleton client = Instantiate(clientPrefab, transform);
            client.CreateClient();
        }

        public void SetData(int playerCnt, bool isHost)
        {
            this.PlayerCnt = playerCnt;
            this.IsHost = isHost;
        }
    }
}