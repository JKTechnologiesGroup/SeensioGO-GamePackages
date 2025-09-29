using System.Threading.Tasks;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class GameConditionController : MonoBehaviour
    {
        [SerializeField] private GameConditionInstance gameConditionInstance;

        public void UpdateStringConditionValue(string stringValue)
        {
            Debug.LogError("Not implement yet" + stringValue);
        }


        public string GetConditionName()
        {
            GameCondition gameCondition = gameConditionInstance.GetGameCondition();
            return gameCondition.name;
        }

        public float GetConditionValue()
        {
            GameCondition gameCondition = gameConditionInstance.GetGameCondition();
            return gameCondition.numberValue;
        }

        public async Task<bool> UpdateNumberConditionValue(float numberCondition)
        {
            GameCondition gameCondition = gameConditionInstance.GetGameCondition();
            float numberValue = gameCondition.numberValue;
            switch(gameCondition.conditionType)
            {
                case  GameConditionType.Greater:
                    if(numberCondition > numberValue)
                    {
                        return await gameConditionInstance.CompleteCondition();
                    }
                    break;
                case GameConditionType.Less:
                    if(numberCondition < numberValue)
                    {
                        return await gameConditionInstance.CompleteCondition();
                    }
                    break;
                case GameConditionType.Equal:
                    if(numberCondition == numberValue)
                    {
                        return await gameConditionInstance.CompleteCondition();
                    }
                    break;
                case GameConditionType.EqualOrGreater:
                    if(numberCondition >= numberValue)
                    {
                        return await gameConditionInstance.CompleteCondition();
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}