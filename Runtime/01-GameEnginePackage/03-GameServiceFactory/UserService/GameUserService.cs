namespace JKTechnologies.CommonPackage
{
    public class GameUserService : IGameUserService
    {
        public GameUserInfo GetUserInfo()
        {
            return GameEngineService.GetUserInfo();
        }
    }
}