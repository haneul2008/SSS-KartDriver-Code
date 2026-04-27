using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace HN.Code.Networking
{
    public class PingLogger : MonoBehaviour
    {
        private NetworkManager _networkManager;
        private UnityTransport _unityTransport;
        private readonly float _pingCheckInterval = 1.0f;
        private float _nextPingCheckTime;

        void Start()
        {
            _networkManager = NetworkManager.Singleton;
            if (_networkManager != null)
            {
                _unityTransport = _networkManager.GetComponent<UnityTransport>();
                if (_unityTransport == null)
                {
                    Debug.LogError("UnityTransport component not found on the NetworkManager.");
                }
            }
            else
            {
                Debug.LogError("NetworkManager not found.");
            }
        }

        void Update()
        {
            if (Time.time >= _nextPingCheckTime)
            {
                if (_networkManager != null && _networkManager.IsConnectedClient && _unityTransport != null)
                {
                    var rtt = _unityTransport.GetCurrentRtt(0);
                    Debug.Log($"Current Ping: {rtt} ms");
                }
            
                _nextPingCheckTime = Time.time + _pingCheckInterval;
            }
        }
    }
}