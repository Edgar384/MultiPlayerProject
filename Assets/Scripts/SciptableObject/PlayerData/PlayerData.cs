using UnityEngine;

namespace DefaultNamespace.SciptableObject.PlayerData
{
    [CreateAssetMenu(fileName = "NewPLayerData", menuName = "MENUNAME", order = 0)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private string _preFabName;

        public string PreFabName => _preFabName;
    }
}