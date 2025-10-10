using UnityEngine;
using JKTechnologies.SeensioGo.GameInstances;
using System.Collections;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameInstanceNextLevelListener : MonoBehaviour, IGameInstanceNextLevelListner
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;

        [Header("[ Settings ]")]
        [SerializeField] private string nextLevelSceneName;
        [SerializeField] private int nextLevelSceneIndex;

        private void Awake()
        {
            huntSioGameFlow.ListenToNextLevel(this);
        }
        

        public void OpenNextLevel()
        {
            if (!string.IsNullOrEmpty(nextLevelSceneName))
            {
                GameEngineManager.Instance.LoadSceneByName(nextLevelSceneName);
            }
            else if (nextLevelSceneIndex > -1)
            {
                GameEngineManager.Instance.LoadSceneByIndex(nextLevelSceneIndex);
            }
            else
            {
                Debug.LogError("Invalid scene name or scene index: " + nextLevelSceneName + " - " + nextLevelSceneIndex);
            }
        }
    }
}