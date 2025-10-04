using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage
{
    public class CustomMultipleGameConditionController : MonoBehaviour
    {
        public UnityEvent OnConditionCompleted = new();
        public bool HasWon => hasWon;
        [SerializeField] private List<CustomGameConditionController> customGameConditionControllers = new();
        [SerializeField] private bool isProcessing = false;
        [SerializeField] private bool hasWon = false;
        [SerializeField] private CustomGameConditionController winningCondition = null;
        public async Task<bool> CompleteCondition()
        {
            if(isProcessing)
            {
                return false;
            }
            hasWon = false;
            winningCondition = null;
            isProcessing = true;
            foreach (CustomGameConditionController customGameConditionController in customGameConditionControllers)
            {
                bool isSuccess = await customGameConditionController.CompleteThisCondition();
                if (isSuccess)
                {
                    Debug.Log("success get reward");
                    hasWon = true;
                    winningCondition = customGameConditionController;
                    break;
                }
                else
                {
                    Debug.Log("fail get reward");
                }
            }
            isProcessing = false;
            OnConditionCompleted.Invoke();
            return hasWon;
        }

        public GameRewardItem[] GetWinningRewards()
        {
            if(hasWon == true && winningCondition != null)
            {
                return winningCondition.GetGameRewardItems();
            }
            return new GameRewardItem[0];
        }
    }
}