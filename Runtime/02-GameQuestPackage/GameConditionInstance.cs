using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "GameConditionInstance", menuName = "JKTechnologies/SeensioGoPackage/GameConditionInstance", order = 2)]
    public class GameConditionInstance :ScriptableObject
    {
        public string Id => gameConditionId;
        public bool IsActive => isActive;
        public int Index => index;
        public UnityEvent OnActiveEvent;

        [SerializeField] private string gameConditionId;
        [SerializeField] private GameCondition gameCondition;
        [SerializeField] private GameRewardItem[] gameRewardItems;
        [SerializeField] private string conditionRewardId;
        [SerializeField] private bool isActive;
        [SerializeField] private int index;

        public void Activate(GameCondition gameCondition, string conditionRewardId, GameRewardItem[] gameRewardItems = null)
        {
            // Debug.LogError("Activate condition: " + gameCondition.id + " - " + gameConditionId);
            if(gameCondition.id == gameConditionId)
            {
                this.gameCondition = gameCondition;
                this.conditionRewardId = conditionRewardId;
                if(gameRewardItems != null)
                {
                    this.gameRewardItems = gameRewardItems;
                }
                isActive = true;
                OnActiveEvent.Invoke();
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

        public GameRewardItem GetReward()
        {
            return gameRewardItems?.First();
        }

        public async Task<bool> CompleteCondition()
        {
            if (!isActive)
            {
                return false;
            }

            bool isSuccess = await GameQuestService.CompleteCondition(conditionRewardId, gameCondition.id);
            if (!isSuccess)
            {
                Debug.LogError("Failed to complete this condition: " + gameCondition.id);
                return false;
            }
            return true;
        }

        public async Task<bool> HitWin()
        {
            if(!isActive)
            {
                return false;
            }

            bool isSuccess = await GameQuestService.HitWinByCondition(conditionRewardId, gameCondition.id);
            if(!isSuccess)
            {
                Debug.LogError("Failed to hit win condition: " + gameCondition.id);
                return false;
            }
            return true;
        }

        public async Task<bool> CompleteConditionForce()
        {
            if(!isActive)
            {
                return false;
            }

            bool isSuccess = await GameQuestService.CompleteConditionForce(conditionRewardId, gameCondition.id);
            if(!isSuccess)
            {
                Debug.LogError("Failed to complete this condition: " + gameCondition.id);
                return false;
            }
            return true;
        }
    }
}