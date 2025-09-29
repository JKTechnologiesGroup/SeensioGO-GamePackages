using System.Threading.Tasks;

namespace JKTechnologies.CommonPackage
{
    public interface IGameDataService
    {
        public Task<T> GetGameDataByQuestPackPoolId<T>(string campaignId);
        public Task<bool> UpdateGameDataByQuestPackPoolId(string campaignId, object gameData);
    }
}