using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "GameConditionInstance", menuName = "JKTechnologies/SeensioGoPackage/GameConditionInstance", order = 2)]
    public class GameConditionInstance :ScriptableObject
    {
        public string Id => gameConditionId;
        [SerializeField] private string gameConditionId;
        [SerializeField] private GameCondition gameCondition;
        [SerializeField] private GameRewardItem[] gameRewardItems;
        [SerializeField] private string conditionRewardId;
        [SerializeField] private bool isActive;

        public void Activate(GameCondition gameCondition, string conditionRewardId, GameRewardItem[] gameRewardItems = null)
        {
            Debug.LogError("Activate condition: " + gameCondition.id + " - " + gameConditionId);
            if(gameCondition.id == gameConditionId)
            {
                isActive = true;
                this.gameCondition = gameCondition;
                this.conditionRewardId = conditionRewardId;
                if(gameRewardItems != null)
                {
                    this.gameRewardItems = gameRewardItems;
                }
            }
        }

        public void Deactivate()
        {
            conditionRewardId = string.Empty;
            isActive = false;
        }

        public GameCondition GetGameCondition()
        {
            return gameCondition;
        }

        public GameRewardItem[] GetGameRewardItems()
        {
            if(gameRewardItems == null)
            {
                return new GameRewardItem[0];
            }
            return gameRewardItems;
        }

        public async Task<bool> CompleteCondition()
        {
            if(!isActive)
            {
                return false;
            }

            bool isSuccess = await GameQuestService.CompleteCondition(conditionRewardId, gameCondition.id);
            if(!isSuccess)
            {
                Debug.LogError("Failed to complete this condition: " + gameCondition.id);
                return false;
            }
            return true;
        }
    }
}