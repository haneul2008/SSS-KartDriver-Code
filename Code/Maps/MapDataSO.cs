using UnityEngine;

namespace HN.Code.Maps
{
    [CreateAssetMenu(fileName = "MapData", menuName = "SO/MapData", order = 0)]
    public class MapDataSO : ScriptableObject
    {
        public GameObject map;
        public Vector3 startPos;
        public Vector3 startRotation;
        public int lap;
    }
}