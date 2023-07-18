using UnityEngine;

namespace DefaultNamespace.SciptableObject.PlayerData
{
    [CreateAssetMenu(fileName = "NewPLayerData", menuName = "MENUNAME", order = 0)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private string _preFabName;
        [SerializeField] private int _characterID;
        [SerializeField] private Sprite _playerPic;
        [SerializeField] private Sprite _playerBackground;
        [SerializeField] private Color _playerColor;
        public string PreFabName => _preFabName;
        public int CharacterID => _characterID;

        public Sprite PlayerPic => _playerPic;
        public Sprite PlayerBackground => _playerBackground;
        public Color PlayerColor => _playerColor;
    }
}