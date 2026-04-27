using System;
using System.Collections.Generic;
using GondrLib.Dependencies;
using HN.Code.Events;
using HN.Code.Events.Systems;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

namespace HN.Code.Core
{
    [Provide]
    public class PlayerNameContainer : NetworkBehaviour, IDependencyProvider
    {
        private NetworkList<PlayerNamePair> _namePairs = new NetworkList<PlayerNamePair>();

        private NetworkVariable<int> _playerNameCnt = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        [ServerRpc(RequireOwnership = false)]
        public void AddNameServerRpc(ulong playerId, string playerName)
        {
            _namePairs.Add(new PlayerNamePair { PlayerId = playerId, PlayerName = playerName });

            _playerNameCnt.Value++;

            if (_playerNameCnt.Value == ApplicationManager.Instance.PlayerCnt)
                LoadAllNameClientRpc();
        }

        [ClientRpc]
        private void LoadAllNameClientRpc()
        {
            Bus<LoadAllPlayerNameEvent>.Raise(new LoadAllPlayerNameEvent());
        }

        public string GetPlayerName(ulong playerId)
        {
            foreach (var pair in _namePairs)
            {
                if (pair.PlayerId == playerId)
                    return pair.PlayerName.ToString();
            }

            return null;
        }

        [ContextMenu("Print Names")]
        public void PrintNames()
        {
            foreach (PlayerNamePair pair in _namePairs)
            {
                print($"player name : {pair.PlayerName}");
            }
        }

        public struct PlayerNamePair : INetworkSerializable, IEquatable<PlayerNamePair>
        {
            public ulong PlayerId;
            public FixedString64Bytes PlayerName;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref PlayerId);
                serializer.SerializeValue(ref PlayerName);
            }

            public bool Equals(PlayerNamePair other)
            {
                return PlayerId == other.PlayerId && PlayerName.Equals(other.PlayerName);
            }

            public override bool Equals(object obj)
            {
                return obj is PlayerNamePair other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(PlayerId, PlayerName.GetHashCode());
            }
        }
    }
}