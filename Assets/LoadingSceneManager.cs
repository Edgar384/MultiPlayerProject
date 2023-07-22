using GarlicStudios.Online.Managers;
using Photon.Pun;

public class LoadingSceneManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
            photonView.RPC(nameof(UpdateLoadStatus), RpcTarget.AllViaServer);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        foreach (var connectedPlayer in OnlineRoomManager.ConnectedPlayers)
        {
            if (!connectedPlayer.Value.IsInLoading)
                return;
        }   
        
        OnlineManager.LoadGameLevel();
    }

    [PunRPC]
    private void UpdateLoadStatus()
    {
        int actorId = PhotonNetwork.LocalPlayer.ActorNumber;
        OnlineRoomManager.ConnectedPlayers[actorId].IsInLoading = true;
    }
}
