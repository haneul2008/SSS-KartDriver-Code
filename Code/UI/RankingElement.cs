using HN.Code.Rank;
using TMPro;
using UnityEngine;

namespace HN.Code.UI
{
    public class RankingElement : MonoBehaviour
    {
        public bool IsInit { get; private set; }
        public IRankRecordable Rankable { get; private set; }

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;

        public void Initialize(string name, IRankRecordable rankable)
        {
            nameText.text = name;
            Rankable = rankable;
            IsInit = true;
        }

        public void SetGrade(int currentGrade) => gradeText.text = currentGrade.ToString();
    }
}