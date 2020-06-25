namespace PlayerManagement
{
    public class StaticPlayerManager : IPlayerManager
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override PlayerData GetPlayerData()
        {
            return new PlayerData {EyePosition = transform.position};
        }
    }
}


