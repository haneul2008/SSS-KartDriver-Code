using System;
using TMPro;
using UnityEngine;

namespace HN.Code.UI
{
    public class RankingBoardElement : RankingElement
    {
        [SerializeField] private TextMeshProUGUI timeText;

        public void SetTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

            timeText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
        }
    }
}