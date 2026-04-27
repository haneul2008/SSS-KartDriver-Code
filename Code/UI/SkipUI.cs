using System;
using HN.Code.Core;
using HN.Code.Events;
using HN.Code.Events.Systems;
using TMPro;
using UnityEngine;

namespace HN.Code.UI
{
    public class SkipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skipCountText;
        [SerializeField] private TextMeshProUGUI playerCountText;

        private bool _isSkipped;
        private int _skipCnt;
        
        private void Awake()
        {
            playerCountText.text = $"/{ApplicationManager.Instance.PlayerCnt}";
            Bus<SkipEventFromServer>.OnEvent += HandleSkip;
        }

        private void OnDestroy()
        {
            Bus<SkipEventFromServer>.OnEvent -= HandleSkip;
        }

        private void HandleSkip(SkipEventFromServer evt)
        {
            if(_isSkipped) return;
            
            _skipCnt++;
            skipCountText.text = _skipCnt.ToString();
            _isSkipped = true;
        }
    }
}