using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "GameRankRewardInstance", menuName = "JKTechnologies/SeensioGoPackage/GameRankRewardInstance", order = 3)]
    public class GameRankRewardInstance : ScriptableObject
    {
        [SerializeField] private bool isActive;
        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }

        public async Task CompleteRankReward(int rankIndex)
        {
            if(!isActive)
            {
                return;
            }
            if(rankIndex < 0)
            {
                Debug.LogError("Rank index is invalid: " + rankIndex);
                return;
            }

            bool isSuccess = await GameQuestService.CompleteRankReward(rankIndex);
            if(!isSuccess)
            {
                Debug.LogError("Failed to complete this condition: " + rankIndex);
            }
        }
    }
}