using DefaultNamespace;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    //public GameTextLayers PlayerName; //There is no name in the figma, only picture and score
    public GameTextLayers PlayerScore;

    //We need to add bool to the updated
    public void UpdateUI(LocalPlayer localPlayer,bool isLeading)
    {
        //PlayerName.ChangeText(localPlayer.OnlinePlayer.NickName);
        PlayerScore.ChangeText(localPlayer.ScoreHandler.Score.ToString());
    }
}
