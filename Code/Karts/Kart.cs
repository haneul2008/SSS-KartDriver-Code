using System;
using System.Collections.Generic;
using System.Linq;
using GondrLib.Dependencies;
using HN.Code.Core;
using HN.Code.Events;
using HN.Code.Events.Systems;
using HN.Code.Rank;
using HN.Code.References;
using RVP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HN.Code.Karts
{
    public class Kart : NetworkBehaviour, IRankRecordable
    {
        public int CurrentCorner { get; set; }
        public int CurrentLap { get; set; }

        [SerializeField] private float stopDelayWhenGoal = 1f;

        private VehicleParent _vehicleParent;
        private VehicleDebug _vehicleDebug;
        private BasicInput _input;
        private bool _isStarted;
        private bool _isStartLapPassed;
        private bool _isGoal;
        private bool _isRetireEnd;
        private float _goalTime;
        private PlayerNameContainer _container;

        private void Awake()
        {
            _input = GetComponent<BasicInput>();
            _input.SetEnable(false);
            _vehicleParent = GetComponent<VehicleParent>();
            _vehicleDebug = GetComponent<VehicleDebug>();
        }

        public override async void OnNetworkSpawn()
        {
            Bus<RankableAddEvent>.Raise(new RankableAddEvent(this));

            _vehicleDebug.Initialize(IsOwner);

            InitNetworkComponents();
            if (IsOwner == false) return;

            await Awaitable.NextFrameAsync();

            PlayerTracker playerTracker = FindAnyObjectByType<PlayerTracker>();
            playerTracker.SpawnPlayerServerRpc();

            Stop();

            _container = FindAnyObjectByType<PlayerNameContainer>();
            ulong playerId = GetComponent<NetworkObject>().OwnerClientId;
            _ = await GetPlayerData.Instance.GetDataSelf(data => _container.AddNameServerRpc(playerId, data.UserName));

            Bus<AllTimelineEnd>.OnEvent += HandleAllTimelineEnd;
            Bus<GameStartEvent>.OnEvent += HandleGameStart;
            Bus<RetireEndEvent>.OnEvent += HandleRetireEnd;

            NetworkManager.OnClientDisconnectCallback += HandleDisconnect;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (IsOwner == false) return;

            Bus<AllTimelineEnd>.OnEvent -= HandleAllTimelineEnd;
            Bus<GameStartEvent>.OnEvent -= HandleGameStart;
            Bus<RetireEndEvent>.OnEvent -= HandleRetireEnd;

            NetworkManager.OnClientDisconnectCallback -= HandleDisconnect;
        }

        private void HandleDisconnect(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                SceneManager.LoadScene(SceneNames.Lobby);
            }
        }

        private void HandleRetireEnd(RetireEndEvent evt)
        {
            _isRetireEnd = true;
        }

        private void HandleGameStart(GameStartEvent evt)
        {
            _input.SetEnable(IsOwner);
            _isStarted = true;
        }

        private void HandleAllTimelineEnd(AllTimelineEnd evt)
        {
            CameraControl cam = FindAnyObjectByType<CameraControl>();
            cam.target = transform;
            cam.Initialize();
        }

        private void Update()
        {
            if (IsOwner == false) return;

            if (_isStarted == false || _isGoal && _goalTime + stopDelayWhenGoal < Time.time || _isRetireEnd) Stop(false);

            Bus<KartSpeedEvent>.Raise(new KartSpeedEvent((int)(_vehicleParent.velMag * 2.23694f)));
        }

        private void InitNetworkComponents()
        {
            GetComponentsInChildren<NetworkInitCompo>().ToList().ForEach(compo =>
            {
                compo.enabled = true;
                
                compo.Init();
            });
        }

        public void CompleteLap(float completeTime)
        {
            if (IsOwner == false) return;

            Bus<CompleteLapEvent>.Raise(new CompleteLapEvent(CurrentLap, _isStartLapPassed == false, completeTime));
            _isStartLapPassed = true;
        }

        public void ResetPos()
        {
            if(IsOwner == false) return;
            
            StartCoroutine(_vehicleDebug.ResetRotation());
            StartCoroutine(_vehicleDebug.ResetPosition());
        }

        public void Stop(bool yStop = true)
        {
            if (_vehicleParent == null || _vehicleParent.rb == null) return;

            Rigidbody rigid = _vehicleParent.rb;
            
            if (yStop)
                rigid.linearVelocity = Vector3.zero;
            else
                rigid.linearVelocity = new Vector3(0, rigid.linearVelocity.y, 0);

            rigid.angularVelocity = Vector3.zero;
        }

        public void Goal()
        {
            if (IsOwner == false) return;

            _input.SetEnable(false);
            _isGoal = true;
            _goalTime = Time.time;
        }
    }
}