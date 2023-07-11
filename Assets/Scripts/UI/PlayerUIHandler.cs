using DefaultNamespace;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    public TMP_Text PlayerName;
    public TMP_Text PlayerScore;

    public void UpdateUI(LocalPlayer localPlayer)
    {
        PlayerName.text  = localPlayer.OnlinePlayer.NickName;
        PlayerScore.text = localPlayer.ScoreHandler.Score.ToString();
    }
}
