namespace PlayerManagement
{
    public class StaticPlayerManager : IPlayerManager
    {
        public override PlayerData GetPlayerData()
        {
            return new PlayerData {EyePosition = transform.position};
        }
    }
}


