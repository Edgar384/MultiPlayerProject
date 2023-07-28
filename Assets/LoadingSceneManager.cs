using GarlicStudios.Online.Managers;
using Photon.Pun;

public class LoadingSceneManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC(nameof(UpdateLoadStatus), RpcTarget.AllViaServer,PhotonNetwork.LocalPlayer.ActorNumber);
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
        Destroy(gameObject);
    }

    [PunRPC]
    private void UpdateLoadStatus(int playerId)
    {
        OnlineRoomManager.ConnectedPlayers[playerId].IsInLoading = true;
    }
}