using System.Globalization;
using GamePlayLogic;
using System.Linq;
using UnityEngine;

public class GameUiHandler : MonoBehaviour
{
    [SerializeField] private PlayerUIHandler[] _playerUIHandlers;
    [SerializeField] private GameTextLayers _timer; //Until i have Jonix timer
    private int highestScore = 0;
    private int leadingPlayerID = -1;

    // Update is called once per frame
    void Update()
    {
        var players = OnlineGameManager.LocalPlayers.Values.ToList();

        for (int i = 0; i < players.Count; i++) //Turn on/off players UI
        {
            _playerUIHandlers[players[i].OnlinePlayer.PlayerData.CharacterID].gameObject.SetActive(true);
        }

        for (int i = 0; i < players.Count; i++)
        {
            //Check if is the same as highestScore
            if (players[i].ScoreHandler.Score > highestScore)
            {
                highestScore = players[players[i].OnlinePlayer.PlayerData.CharacterID].ScoreHandler.Score;
                leadingPlayerID = players[players[i].OnlinePlayer.PlayerData.CharacterID].PlayerId;
            }

            //if he has the same as the highscore, the leading id is not -1 and not himself... we can understand that someone has the same score as this player. So we reset the leading id.
            else if (players[i].ScoreHandler.Score == highestScore && leadingPlayerID != -1 && leadingPlayerID != players[i].PlayerId)
                leadingPlayerID = -1;
        }

        int time = (int)TimeManager.TimeGame;
        
        _timer.ChangeText(time.ToString());

        for (int i = 0; i < players.Count; i++)
        {
            if (i == leadingPlayerID)
                _playerUIHandlers[players[i].OnlinePlayer.PlayerData.CharacterID].UpdateUI(players[i], true);

            else
                _playerUIHandlers[players[i].OnlinePlayer.PlayerData.CharacterID].UpdateUI(players[i], false);
        }
    }
}
