using System;
using GondrLib.Dependencies;
using HN.Code.Events;
using HN.Code.Events.Systems;
using UnityEngine;

namespace HN.Code.Core
{
    [Provide]
    public class GameTimer : MonoBehaviour, IDependencyProvider
    {
        public float CurrentTime => Time.time - _startTime;
        private float _startTime;
        
        private void Awake()
        {
            Bus<GameStartEvent>.OnEvent += HandleGameStart;
        }

        private void OnDestroy()
        {
            Bus<GameStartEvent>.OnEvent -= HandleGameStart;
        }

        private void HandleGameStart(GameStartEvent evt)
        {
            _startTime = Time.time;
        }
    }
}