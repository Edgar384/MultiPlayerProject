using DefaultNamespace;
using DefaultNamespace.SciptableObject.PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{

    [SerializeField] PlayerData playerData;
    [SerializeField] Image _playerImage;
    //public GameTextLayers PlayerName; //There is no name in the figma, only picture and score
    public GameTextLayers PlayerScore;

    private void OnEnable()
    {
        ChangePlayerUIImage();
    }
    
    private void ChangePlayerUIImage()
    {
        if(playerData!=null)
        _playerImage.sprite = playerData.PlayerPic;
    }


    //We need to add bool to the updated
    public void UpdateUI(LocalPlayer localPlayer,bool isLeading)
    {
        //PlayerName.ChangeText(localPlayer.OnlinePlayer.NickName);
        PlayerScore.ChangeText(localPlayer.ScoreHandler.Score.ToString());
    }
}
