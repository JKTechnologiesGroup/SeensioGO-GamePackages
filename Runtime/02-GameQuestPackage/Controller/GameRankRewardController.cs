using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class GameRankRewardController : MonoBehaviour
    {
        [SerializeField] private GameRankRewardInstance gameRankRewardInstance;

        public async Task UpdateRankValue(int achievedRankIndex)
        {
            await gameRankRewardInstance.CompleteRankReward(achievedRankIndex);
        }
    }
}