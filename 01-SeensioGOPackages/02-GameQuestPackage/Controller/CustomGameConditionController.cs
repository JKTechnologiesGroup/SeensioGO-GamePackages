using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class CustomGameConditionController : MonoBehaviour
    {
        public int ConditionIndex => conditionIndex;
        [SerializeField] private GameConditionInstance gameConditionInstance;
        [SerializeField] private int conditionIndex;
        public async Task<bool> CompleteThisCondition()
        {
            return await gameConditionInstance.CompleteCondition();
        }

        public string GetConditionId()
        {
            return gameConditionInstance.Id;
        }

        public GameRewardItem[] GetGameRewardItems()
        {
            return gameConditionInstance.GetGameRewardItems();
        }
    }
}