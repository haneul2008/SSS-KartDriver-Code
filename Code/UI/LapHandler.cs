using System;
using csiimnida.CSILib.SoundManager.RunTime;
using HN.Code.Events;
using HN.Code.Events.Systems;
using TMPro;
using UnityEngine;

namespace HN.Code.UI
{
    public class LapHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lapText;
        [SerializeField] private TextMeshProUGUI[] timeTexts;
        
        private void Awake()
        {
            Bus<CompleteLapEvent>.OnEvent += HandleCompleteLap;
        }

        private void OnDestroy()
        {
            Bus<CompleteLapEvent>.OnEvent -= HandleCompleteLap;
        }

        private void HandleCompleteLap(CompleteLapEvent evt)
        {
            if (evt.isFirst)
            {
                lapText.text = "1";
                return;
            }
            
            int minutes = Mathf.FloorToInt(evt.completeTime / 60f);
            int seconds = Mathf.FloorToInt(evt.completeTime % 60f);
            int milliseconds = Mathf.FloorToInt((evt.completeTime * 1000f) % 1000f);

            int lap = Mathf.Clamp(evt.lap, 1, 3); // testCode
            lapText.text = lap.ToString();
            SoundManager.Instance.PlaySound("a3658392-b9c5-4867-8a53-86984ad5b6ae");
            timeTexts[evt.lap - 2].text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
    }
}