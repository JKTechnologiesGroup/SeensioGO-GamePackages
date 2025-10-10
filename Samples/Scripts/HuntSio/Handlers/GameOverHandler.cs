using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponents;

        public void OnGameOver(int playerScore)
        {
            inGameUIComponents.SetActive(false);
            huntSioGameFlow.OnGameOver(playerScore);
        }
    }
}