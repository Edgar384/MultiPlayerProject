using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers
{
    public class PhotonHandler
    {
        public void LoginToPhoton(string nickName)
        {
            PhotonNetwork.NickName = nickName;
            Debug.Log("Player nickname is " + PhotonNetwork.NickName);
            PhotonNetwork.ConnectUsingSettings();
        } 
        
        public void CreateRoom(string roomName)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add(Constants.GAME_MODE, "FreeForAll");
            RoomOptions roomOptions =
                new RoomOptions
                {
                    MaxPlayers = 4, EmptyRoomTtl = 30000, PlayerTtl = 25000,
                    CustomRoomProperties = hashtable
                };
            PhotonNetwork.JoinOrCreateRoom(roomName,
                roomOptions,
                null );
        }
        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public void SetUserScore(string scoreString)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError(
                    "We tried to set the user score while not connected!");
                return;
            }
            int score = int.Parse(scoreString);
            ExitGames.Client.Photon.Hashtable hashtable 
                = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add(Constants.PLAYER_STRENGTH_SCORE_PROPERTY_KEY, score);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }
}