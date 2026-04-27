using System;
using System.Collections.Generic;
using GondrLib.Dependencies;
using HN.Code.Core;
using HN.Code.Events.Systems;
using HN.Code.Rank;
using HN.Code.References;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HN.Code.UI
{
    public class RankingBoardUI : MonoBehaviour
    {
        [SerializeField] private RankingBoardElement element;
        [SerializeField] private Transform contentTrm;
        [SerializeField] private Button nextBtn;

        [Inject] private PlayerNameContainer _nameContainer;
        [Inject] private RankManager _rankManager;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            nextBtn.onClick.AddListener(HandleNextBtnClick);
        }

        private void OnDestroy()
        {
            nextBtn.onClick.AddListener(HandleNextBtnClick);
        }

        private void HandleNextBtnClick()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene(SceneNames.Lobby);
            }
            else if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene(SceneNames.Lobby);
            }
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;

            foreach (IRankRecordable rankable in _rankManager.GetRakingList())
            {
                if (rankable is MonoBehaviour mono && mono.TryGetComponent(out NetworkObject networkObject))
                {
                    ulong playerId = networkObject.OwnerClientId;
                    int rank = _rankManager.GetRanking(rankable);
                    
                    RankingBoardElement newElement = Instantiate(element, contentTrm);
                    newElement.Initialize(_nameContainer.GetPlayerName(playerId), rankable);
                    newElement.SetGrade(rank);
                    
                    if(_rankManager.TryGetGoalTime(rankable, out float goalTime))
                    {
                        newElement.SetTime(goalTime);
                    }

                    if (rankable is NetworkBehaviour { IsOwner: true })
                    {
                        Bus<GameEndEventBus>.Raise(new GameEndEventBus(rank));
                    }
                }
            }
        }
    }
}