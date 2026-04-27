using System;
using UnityEngine;

namespace HN.Code.Karts
{
    public abstract class NetworkInitCompo : MonoBehaviour
    {
        public abstract void Init();

        private void OnValidate()
        {
            this.enabled = false;
        }
    }
}