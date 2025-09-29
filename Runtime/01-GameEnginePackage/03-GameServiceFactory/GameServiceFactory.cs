
namespace JKTechnologies.CommonPackage
{
    public class GameServiceFactory : IGameServiceFactory
    {
        public IGameUtilitiesService CreateGameUtilitiesService()
        {
            IGameUtilitiesService gameUtilitiesService = new GameUtilitiesService();
            return gameUtilitiesService;   
        }
        public IGameUserService CreateGameUserService()
        {
            IGameUserService gameUserService = new GameUserService();
            return gameUserService;

        }
        public IGameEconomyService CreateGameEconomyService()
        {
            IGameEconomyService gameEconomyService = new GameEconomyService();
            return gameEconomyService;
        }

        public IGameDataService CreateGameDataService()
        {
            IGameDataService gameDataService = new GameDataService();
            return gameDataService;
        }
    }
}