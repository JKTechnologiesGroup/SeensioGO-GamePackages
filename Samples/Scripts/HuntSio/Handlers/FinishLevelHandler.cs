using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class FinishLevelHandler : MonoBehaviour
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponents;

        public void OnLevelFinished(int playerScore)
        {
            inGameUIComponents.SetActive(false);
            huntSioGameFlow.OnLevelFinished(playerScore);
        }
    }
}