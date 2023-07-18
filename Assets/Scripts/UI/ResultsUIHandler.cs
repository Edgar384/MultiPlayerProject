using DefaultNamespace;
using GamePlayLogic;
using GarlicStudios.Online.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResultsUIHandler : MonoBehaviour
{
    [SerializeField] ResultsObject[] resultsObjects = new ResultsObject[4];

    private List<LocalPlayer> _organizedList;

    private void Awake()
    {
        for (int i = 0; i < resultsObjects.Length; i++) 
        {
            resultsObjects[i].SetActive(false);
        }
        this.gameObject.SetActive(false); //Turn it off that it wont get to the on enable method
    }

    private void OnEnable()
    {
        OrganizeResults();
        //Need to add event for return to lobby/room
    }

    private void OrganizeResults()
    {
        var players = OnlineGameManager.LocalPlayers.Values.ToList();
        for (int i = 0; i < players.Count; i++) //turn on on the amount of players in the game
        {
            resultsObjects[i].SetActive(true);
        }

        _organizedList = players.OrderByDescending(players => players.ScoreHandler.Score).ToList(); //Need to check that the list is of the right class
        for (int i = 0; i < players.Count; i++)
        {
            resultsObjects[i].RefreshVisuals(OnlineRoomManager.PlayerDatas[_organizedList[i].PlayerId], _organizedList[i].name, _organizedList[i].ScoreHandler.Score.ToString());
        }
    }
}
