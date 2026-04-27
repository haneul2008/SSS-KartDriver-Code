using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace HN.Code.Networking
{
    public class NetworkOptimizer : MonoBehaviour
    {
        private UnityTransport _unityTransport;

        void Start()
        {
            _unityTransport = FindObjectOfType<UnityTransport>();
            if (_unityTransport != null)
            {
                OptimizeTransportSettings();
            }
        }

        void OptimizeTransportSettings()
        {
            _unityTransport.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");

            var utpSettings = new NetworkSettings();

                utpSettings.WithNetworkConfigParameters(
                    connectTimeoutMS: 5000,
                    disconnectTimeoutMS: 10000,
                    heartbeatTimeoutMS: 2000,
                    maxConnectAttempts: 3
                );

            utpSettings.WithReliableStageParameters(
                windowSize: 32
            );

            Debug.Log("Network transport optimized for stability");
        }
    }
}