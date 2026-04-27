using System;
using System.Collections.Generic;
using System.Linq;
using HN.Code.Core;
using HN.Code.Events;
using HN.Code.Events.Systems;
using HN.Code.Rank;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace HN.Code.UI
{
    public class TestRankingUI : MonoBehaviour
    {
        [SerializeField] private GameObject rankingElement;
        [SerializeField] private Transform contentTrm;
        [SerializeField] private TextMeshProUGUI currentRankText;
        [SerializeField] private TextMeshProUGUI maxRankText;
        
        private readonly List<IRankRecordable> _rankables = new List<IRankRecordable>();
        private readonly List<RankingElement> _elements = new List<RankingElement>();
        private PlayerNameContainer _nameContainer;
        private bool _isInit;
        
        private void Awake()
        {
            _nameContainer = FindAnyObjectByType<PlayerNameContainer>();
            
            Bus<RankingUpdateEvent>.OnEvent += HandleRankingUpdate;
            Bus<RankableAddEvent>.OnEvent += HandleRankableAdded;
            Bus<LoadAllPlayerNameEvent>.OnEvent += HandlePlayerNameLoaded;
        }

        private void OnDestroy()
        {
            Bus<RankingUpdateEvent>.OnEvent -= HandleRankingUpdate;
            Bus<RankableAddEvent>.OnEvent -= HandleRankableAdded;
            Bus<LoadAllPlayerNameEvent>.OnEvent -= HandlePlayerNameLoaded;
        }

        private async void HandlePlayerNameLoaded(LoadAllPlayerNameEvent _)
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            
            foreach(IRankRecordable rankable in _rankables)
            {
                if (rankable is MonoBehaviour mono && mono.TryGetComponent(out NetworkObject networkObject))
                {
                    RankingElement element = _elements.FirstOrDefault(item => item.IsInit == false);
                    element?.Initialize(_nameContainer.GetPlayerName(networkObject.OwnerClientId), rankable);
                }
            }
            
            _isInit = true;

            int playerCnt = ApplicationManager.Instance.PlayerCnt;
            maxRankText.text = $"/{playerCnt}";
        }

        private void HandleRankableAdded(RankableAddEvent evt)
        {
            _rankables.Add(evt.rankable);
            RankingElement element = Instantiate(rankingElement, contentTrm).GetComponent<RankingElement>();
            _elements.Add(element);
        }

        private void HandleRankingUpdate(RankingUpdateEvent evt)
        {
            if(_isInit == false) return;
            
            foreach (IRankRecordable rankable in evt.ranking)
            {
                int rank = evt.ranking.IndexOf(rankable) + 1;
                RankingElement element = GetElement(rankable);
                element.SetGrade(rank);
                element.transform.SetAsLastSibling();

                if (rankable is NetworkBehaviour networkBehaviour && networkBehaviour.IsOwner)
                    currentRankText.text = (evt.ranking.IndexOf(rankable) + 1).ToString();
            }
        }
        
        private RankingElement GetElement(IRankRecordable rankable) => _elements.FirstOrDefault(element => element.Rankable == rankable);
    }
}