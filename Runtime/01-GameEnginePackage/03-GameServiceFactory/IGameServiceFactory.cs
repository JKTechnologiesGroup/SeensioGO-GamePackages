namespace JKTechnologies.CommonPackage
{
    public interface IGameServiceFactory
    {
        public IGameUserService CreateGameUserService();
        public IGameEconomyService CreateGameEconomyService();
        public IGameUtilitiesService CreateGameUtilitiesService();
        public IGameDataService CreateGameDataService();
    }
}