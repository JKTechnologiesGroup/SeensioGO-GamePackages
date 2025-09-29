using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage
{
    public class CustomMultipleGameConditionController : MonoBehaviour
    {
        public UnityEvent OnConditionCompleted = new();
        [SerializeField] private List<CustomGameConditionController> customGameConditionControllers = new();
        [SerializeField] private bool isProcessing = false;
        public async void CompleteCondition()
        {
            if(isProcessing)
            {
                return;
            }
            isProcessing = true;
            foreach (CustomGameConditionController customGameConditionController in customGameConditionControllers)
            {
                bool isSuccess = await customGameConditionController.CompleteThisCondition();
                if (isSuccess)
                {
                    Debug.Log("success get reward");
                    break;
                }
                else
                {
                    Debug.Log("fail get reward");
                }
            }
            isProcessing = false;
            OnConditionCompleted.Invoke();
        }
    }
}