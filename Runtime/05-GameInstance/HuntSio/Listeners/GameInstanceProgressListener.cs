using JKTechnologies.SeensioGo.GameInstances;
using TMPro;
using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameInstanceProgressListener : MonoBehaviour, IGameInstanceProgressListener
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;
        [SerializeField] private TextMeshProUGUI progressText;
        private int targetScore;
        private int currentPlayerScore;
        private bool hasShown = false;

        private void Awake()
        {
            huntSioGameFlow.ListenToProgress(this);
        }

        public void SetupTargetScore(int targetScore, int currentPlayerScore)
        {
            this.targetScore = targetScore;
            this.currentPlayerScore = currentPlayerScore;
            progressText.text = $"<b>{currentPlayerScore}/{targetScore}</b>";
        }

        public void OnPlayerScoreUpdated(int currentPlayerScore)
        {
            this.currentPlayerScore = currentPlayerScore;
            progressText.text = $"<b>{currentPlayerScore}/{targetScore}</b>";

            if (!hasShown && currentPlayerScore > targetScore)
            {
                hasShown = true;
                huntSioGameFlow.ShowQuestCompletedPanel();
            }
        }
    }
}