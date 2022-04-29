using UnityEngine;

namespace ARWindow.PlayerManagement
{
    public abstract class IPlayerManager : MonoBehaviour
    {
        public abstract PlayerData GetPlayerData();
    }
}