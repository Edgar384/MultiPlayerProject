using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using GamePlayLogic;
using Photon.Pun;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class LeaderBordUiHandler : MonoBehaviour
{
    [SerializeField] private List<ResultsObject> results;
    [SerializeField] private GameObject _resetGameButton;
    private PlayerController _playerController;

    private void OnEnable()
    {
        _playerController = new PlayerController();
        _playerController.UI.Enable();
        _playerController.UI.Triangular.performed += StartGame;
    }

    private void OnDisable()
    {
        _playerController.UI.Triangular.performed -= StartGame;
    }

    public void TurnOn()
    {
        List<LocalPlayer> result = new List<LocalPlayer>();

          var players = OnlineGameManager.LocalPlayers.Values.ToArray();
        
        Array.Sort(players);

        for (int i = 0; i < players.Length; i++)
        {
            results[i].gameObject.SetActive(true);
            results[i].RefreshVisuals(players[i].OnlinePlayer.PlayerData,players[i].OnlinePlayer.NickName,players[i].ScoreHandler.Score.ToString());
        }
        
        gameObject.SetActive(true);
    }

    private void StartGame(CallbackContext callbackContext)
    {
        if (PhotonNetwork.IsMasterClient)
            OnlineManager.LoadLoadingLevel();
    }


    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            _resetGameButton.SetActive(true);

        else
            _resetGameButton.SetActive(false);
    }
}
