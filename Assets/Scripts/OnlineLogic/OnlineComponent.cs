using GarlicStudios.Online.Data;
using Photon.Pun;

namespace OnlineLogic
{
    public abstract class OnlineComponent : MonoBehaviourPun
    {
        public OnlinePlayer Player { get; }
    }
}