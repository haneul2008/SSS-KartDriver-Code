using System;
using GondrLib.Dependencies;
using HN.Code.Events;
using HN.Code.Events.Systems;
using UnityEngine;

namespace HN.Code.Core
{
    [Provide]
    public class GameStarter : MonoBehaviour, IDependencyProvider
    {
        public event Action OnStartFadeEnd;
        
        [SerializeField] private StartFade fade;
        [SerializeField] private float fadeStartDelay = 2f;
        
        private void Awake()
        {
            Bus<LoadAllKartEvent>.OnEvent += HandleAllKartLoaded;
        }

        private void OnDestroy()
        {
            Bus<LoadAllKartEvent>.OnEvent -= HandleAllKartLoaded;
        }

        private void HandleAllKartLoaded(LoadAllKartEvent evt)
        {
            fade.Fade(0, 0.5f, fadeStartDelay, () =>
            {
                OnStartFadeEnd?.Invoke();
            });
        }
    }
}