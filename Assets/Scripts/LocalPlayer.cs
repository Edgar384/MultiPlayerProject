using GarlicStudios.Online.Data;
using UnityEngine;

namespace DefaultNamespace
{
    public class LocalPlayer : MonoBehaviour
    {
        private OnlinePlayer _onlinePlayer;

        public void Init(OnlinePlayer onlinePlayer)
        {
            _onlinePlayer = onlinePlayer;
        }
    }
}