using System.Linq;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "GameQuestManager", menuName = "JKTechnologies/SeensioGoPackage/GameQuestManager", order = 3)]
    public class GameQuestManager : ScriptableObject
    {
        [SerializeField] private GameConditionInstance[] gameConditionInstances;
        [SerializeField] private GameRankRewardInstance gameRankRewardInstance;
        
        public void StartQuestSession()
        {
            StopQuestSession();
            SetupGameConditionByInSessionQuest();
        }

        private void SetupGameConditionByInSessionQuest()
        {
            GameQuestInstance gameQuestInstance = GameQuestService.GetGameQuestInstance();
            if(gameQuestInstance == null || gameQuestInstance.conditionRewards == null || gameQuestInstance.conditionRewards.Length == 0)
            {
                return;
            }

            foreach(GameConditionReward gameConditionReward in gameQuestInstance.conditionRewards)
            {
                foreach(GameCondition gameCondition in gameConditionReward.conditions)
                {
                    GameConditionInstance gameConditionInstance = gameConditionInstances.FirstOrDefault(item => item.Id == gameCondition.id);
                    if(gameConditionInstance == null) 
                    {
                            Debug.LogError("Can not find define of condition: " + gameCondition.id);
                            continue;
                    }
                    gameConditionInstance.Activate(gameCondition, gameConditionReward.id, gameConditionReward.rewards);
                }
            }

            if(gameQuestInstance.rankRewards.Length > 0)
            {
                gameRankRewardInstance.Activate();
            }
        }

        // For show UI
        public GameQuestInstance GetGameQuestInstance()
        {
            GameQuestInstance gameQuestInstance = GameQuestService.GetGameQuestInstance();
            return gameQuestInstance;
        }

        public void StopQuestSession()
        {
            foreach(GameConditionInstance gameConditionInstance in gameConditionInstances)
            {
                gameConditionInstance.Deactivate();
            }

            gameRankRewardInstance.Deactivate();
        }
    }
}