// Economy Game: Static to using for all game instances
namespace JKTechnologies.CommonPackage
{
    public class GameInstanceModel
    {
        public IGameUtilitiesService UtilitiesService;
        public IGameEconomyService EconomyService;
        public IGameUserService UserService;
        public IGameDataService DataService;

        public GameInstanceModel()
        {
            IGameServiceFactory gameServiceFactory = new GameServiceFactory();
            UtilitiesService = gameServiceFactory.CreateGameUtilitiesService();
            EconomyService = gameServiceFactory.CreateGameEconomyService();
            UserService = gameServiceFactory.CreateGameUserService();
            DataService = gameServiceFactory.CreateGameDataService();
        }
    }
}