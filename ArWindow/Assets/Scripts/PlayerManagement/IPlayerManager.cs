using UnityEngine;

namespace PlayerManagement
{
    public abstract class IPlayerManager : MonoBehaviour
    {
        public abstract PlayerData GetPlayerData();
    }
}