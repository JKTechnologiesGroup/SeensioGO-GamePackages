using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; // Needed for UnityAction

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class CountdownTimer : MonoBehaviour
    {
        [Header("Timer Settings")]
        public float countdownDuration = 60f;

        [Header("UI Reference")]
        public TextMeshProUGUI timerText;

        [Header("Timer Complete Action")]
        public UnityEvent onTimerComplete; // This will be invoked when the timer ends

        private float remainingTime;
        private bool isTimerRunning = false;

        void Start()
        {
            remainingTime = countdownDuration;
            UpdateTimerUI();
        }

        void Update()
        {
            if (!isTimerRunning) return;

            if (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                remainingTime = 0f;
                isTimerRunning = false;
                UpdateTimerUI();
                OnTimerComplete();
            }
        }

        public void StartTimer()
        {
            remainingTime = countdownDuration;
            isTimerRunning = true;
            UpdateTimerUI();
        }

        public void StopTimer()
        {
            isTimerRunning = false;
        }

        private void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void OnTimerComplete()
        {
            Debug.Log("⏰ Timer completed!");
            onTimerComplete?.Invoke(); // 🔥 Invoke the UnityAction if assigned
        }
    }
}