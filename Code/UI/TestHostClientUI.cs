using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace HN.Code.UI
{
    public class TestHostClientUI : MonoBehaviour
    {
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;

        private void Awake()
        {
            hostBtn.onClick.AddListener(HandleHostClick);
            clientBtn.onClick.AddListener(HandleClientClick);
        }

        private void HandleHostClick() => NetworkManager.Singleton.StartHost();

        private void HandleClientClick() => NetworkManager.Singleton.StartClient();
    }
}