using System.Linq;
using GamePlayLogic;
using UnityEngine;

public class GameUiHandler : MonoBehaviour
{
    [SerializeField] private PlayerUIHandler[] _playerUIHandlers;
    

    // Update is called once per frame
    void Update()
    {
        var player = OnlineGameManager.LocalPlayers.Values.ToList();
        for (int i = 0; i < player.Count; i++)
        {
                _playerUIHandlers[i].UpdateUI(player[i]);
        }
    }
}
