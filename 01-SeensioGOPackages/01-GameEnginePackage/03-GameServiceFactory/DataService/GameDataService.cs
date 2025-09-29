using System.Threading.Tasks;

namespace JKTechnologies.CommonPackage
{
    public class GameDataService : IGameDataService
    {
        public async Task<T> GetGameDataByQuestPackPoolId<T>(string questPackPoolId)
        {
            return await GameEngineService.GetGameDataByQuestPackPoolId<T>(questPackPoolId);
        }

        public Task<bool> UpdateGameDataByQuestPackPoolId(string questPackPoolId, object gameData)
        {
            return GameEngineService.UpdateGameDataByQuestPackPoolId(questPackPoolId, gameData);
        }
    }
}