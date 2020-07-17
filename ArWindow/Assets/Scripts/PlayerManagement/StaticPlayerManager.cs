using UnityEngine;

namespace PlayerManagement
{
    public class StaticPlayerManager : MonoBehaviour, IPlayerManager
    {
        public PlayerData GetPlayerData()
        {
            return new PlayerData {EyePosition = transform.position};
        }
    }
}


