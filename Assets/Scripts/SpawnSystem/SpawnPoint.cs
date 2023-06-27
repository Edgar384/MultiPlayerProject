using Sirenix.OdinInspector;
using UnityEngine;

namespace SpawnSystem
{
    [Serializable]
    public class SpawnPoint
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField,ReadOnly] private int _id;
        private bool _isTaken;

        public int ID => _id;

        public bool IsTaken => _isTaken;

        public Vector2 GetPosition => _spawnPoint.position;

        public void Init(int id)
        {
            _id = id;
            _isTaken = false;
        }
 
        public void DrawGizmos()
        {
            Gizmos.DrawSphere(_spawnPoint.position, 0.5f);
        }

        public void SetSpawnPointToTaken() =>
            _isTaken  = true;
    }

}