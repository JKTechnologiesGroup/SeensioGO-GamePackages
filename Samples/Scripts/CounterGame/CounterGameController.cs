using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.SeensioGo.GameEngines.Samples
{
    public class CounterGameController : MonoBehaviour
    {
        public UnityEvent<int> OnGameOver;
        public UnityEvent<int> OnFinishedLevel;
        [SerializeField] private TextMeshProUGUI currentPlayerScoreText;
        [SerializeField] private static int currentPlayerScore;

        private void Start()
        {
            currentPlayerScoreText.text = $"Player Score: {currentPlayerScore}";
        }

        public void AddScore()
        {
            currentPlayerScore = currentPlayerScore += 1;
            currentPlayerScoreText.text = $"Player Score: {currentPlayerScore}";
        }

        public void GameOver()
        {
            OnGameOver.Invoke(currentPlayerScore);
        }

        public void FinishLevel()
        {
            OnFinishedLevel.Invoke(currentPlayerScore);
        }


        public void ContinueAfterAd()
        {
            Debug.LogError("Continue after ads");
        }
    }
}