using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using GamePlayLogic;
using UnityEngine;

public class LeaderBordUiHandler : MonoBehaviour
{
    [SerializeField] private List<ResultsObject> results;

    private void Awake()
    {
        OnlineGameManager.OnEndGame += TurnOn;
        gameObject.SetActive(false);
    }

    private void TurnOn()
    {
        List<LocalPlayer> result = new List<LocalPlayer>();

        var players = OnlineGameManager.LocalPlayers.Values.ToArray();
        
        Array.Sort(players);

        for (int i = 0; i < players.Length; i++)
        {
            results[i].RefreshVisuals(players[i].OnlinePlayer.PlayerData,players[i].OnlinePlayer.NickName,players[i].ScoreHandler.Score.ToString());
        }
        
        gameObject.SetActive(true);
    }
}
