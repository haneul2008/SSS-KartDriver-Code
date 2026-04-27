using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace HN.Code.Networking
{
    public class HostSingleton : MonoSingleton<HostSingleton>
    {
        public string JoinCode => GameManager.JoinCode;
    
        public HostGameManager GameManager { get; private set; }
        
        public void CreateHost()  //나중에 Async들어갈 수도 있어.
        {
            GameManager = new HostGameManager();
        }
    }
}