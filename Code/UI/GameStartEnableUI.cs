using System;
using DG.Tweening;
using HN.Code.Events;
using HN.Code.Events.Systems;
using UnityEngine;

namespace HN.Code.UI
{
    public class GameStartEnableUI : MonoBehaviour
    {
        [SerializeField] private bool enableWhenStart = true;
        [SerializeField] private float enableDuration = 0.5f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = enableWhenStart ? 0 : 1;
            Bus<StartCountEvent>.OnEvent += HandleStartCount;
        }

        private void OnDestroy()
        {
            Bus<StartCountEvent>.OnEvent -= HandleStartCount;
        }

        private void HandleStartCount(StartCountEvent evt)
        {
            DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha
                , enableWhenStart ? 1 : 0, enableDuration);
        }
    }
}