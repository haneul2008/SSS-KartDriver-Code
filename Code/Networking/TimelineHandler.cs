using System;
using System.Threading.Tasks;
using GondrLib.Dependencies;
using HN.Code.Core;
using HN.Code.Events;
using HN.Code.Events.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using csiimnida.CSILib.SoundManager.RunTime;

namespace HN.Code.Networking
{
    public class TimelineHandler : NetworkBehaviour
    {
        [SerializeField] private StartFade fadeUI;
        [SerializeField] private PlayableDirector director;
        [SerializeField] private float countDelay = 1f;
        [Inject] private GameStarter _gameStarter;

        private NetworkVariable<int> _skipCount = new(0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        
        private NetworkVariable<int> _timelineEndCount = new(0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private void Awake()
        {
            _gameStarter.OnStartFadeEnd += HandleLoadAllPlayer;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            _gameStarter.OnStartFadeEnd -= HandleLoadAllPlayer;
        }

        public bool IsTimeLinePlaying() => director.state == PlayState.Playing;

        private void HandleLoadAllPlayer()
        {
            director.Play();
            SoundManager.Instance.PlaySound("Music_1");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SkipKeyPressServerRpc()
        {
            _skipCount.Value++;
            RaiseSkipEventClientRpc();

            if (_skipCount.Value == ApplicationManager.Instance.PlayerCnt)
            {
                SkipTimelineClientRpc();
            }
        }

        [ClientRpc]
        private void SkipTimelineClientRpc()
        {
            director.time = director.duration;
            director.Evaluate();
            director.Stop();
            IncreaseTimelineEndServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseTimelineEndServerRpc()
        {
            _timelineEndCount.Value++;

            if (_timelineEndCount.Value == ApplicationManager.Instance.PlayerCnt)
            {
                HandleAllTimelineEndClientRpc();
            }
        }

        [ClientRpc]
        private void RaiseSkipEventClientRpc()
        {
            Bus<SkipEventFromServer>.Raise(new SkipEventFromServer());
        }

        private async Task StartCount()
        {
            await Awaitable.WaitForSecondsAsync(countDelay);
            Bus<StartCountEvent>.Raise(new StartCountEvent());
        }

        [ClientRpc]
        private void HandleAllTimelineEndClientRpc()
        {
            Bus<AllTimelineEnd>.Raise(new AllTimelineEnd());
            fadeUI.Fade(0, 0.5f, 0, () => _ = StartCount());
        }
    }
}