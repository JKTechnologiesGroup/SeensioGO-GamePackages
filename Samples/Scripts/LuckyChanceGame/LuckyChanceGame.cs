using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JKTechnologies.SeensioGo.GameEngines.Samples
{
    public class LuckyChanceGame : MonoBehaviour
    {
        [SerializeField] private UnityEvent<LuckyChanceRarity> OnSubmitResult;
        [SerializeField] private UnityEvent OnWatchAds1;
        [SerializeField] private UnityEvent OnWatchAds2;
        [SerializeField] private UnityEvent OnWatchAds3;

        [Header("[ UI Elements ]")]
        [SerializeField] private TextMeshProUGUI gameStatusText;
        [SerializeField] private TextMeshProUGUI remainTurnsText;
        [SerializeField] private TextMeshProUGUI currentRarityText;

        [Header("[ Game Status ]")]
        [SerializeField] private int remainTurns = 5;
        [SerializeField] private LuckyChanceRarity currentRaity;

        [Header("[ Ads Buttons ]")]
        [SerializeField] private Button watchAds1Button;
        [SerializeField] private Button watchAds2Button;
        [SerializeField] private Button watchAds3Button;
        [SerializeField] private Button confirmButton;

        #region Start        
        public void StartGame()
        {
            int seed = Guid.NewGuid().GetHashCode();
            UnityEngine.Random.InitState(seed);
            gameStatusText.text = "Game Started";

            currentRarityText.text = "-";
            remainTurnsText.text = $"Remain turns: {remainTurns}";
        }
        #endregion

        #region Random Lucky Chance Chances
        public void RandomLuckyChance()
        {
            if (remainTurns <= 0)
            {
                gameStatusText.text = "Cannot random. You are out of turns.";
                return;
            }

            currentRaity = GetRandomRarity();
            currentRarityText.text = currentRaity.ToString();
            currentRarityText.color = GetRarityColor(currentRaity);

            remainTurns--;
            remainTurnsText.text = $"Turns left: {remainTurns}";
        }
        #endregion

        #region Confirm
        public void SubmitResult()
        {
            confirmButton.interactable = false;
            OnSubmitResult.Invoke(currentRaity);
        }
        #endregion

        #region Watch Ads
        public void WatchAds1()
        {
            watchAds1Button.interactable = false;
            gameStatusText.text = "WatchAds1 - watching";
            OnWatchAds1.Invoke();
        }

        public void OnWatchAds1Completed()
        {
            gameStatusText.text = "WatchAds1 - completed";
            remainTurns += 1;
            remainTurnsText.text = $"Remain turns: {remainTurns}";
        }

        public void WatchAds2()
        {
            watchAds2Button.interactable = false;
            gameStatusText.text = "WatchAds2 - watching";
            OnWatchAds2.Invoke();
        }

        public void OnWatchAds2Completed()
        {
            gameStatusText.text = "WatchAds2 - completed";
            remainTurns += 2;
            remainTurnsText.text = $"Remain turns: {remainTurns}";
        }


        public void WatchAds3()
        {
            watchAds3Button.interactable = false;
            gameStatusText.text = "WatchAds3 - watching";
            OnWatchAds3.Invoke();
        }

        public void OnWatchAds3Completed()
        {
            gameStatusText.text = "WatchAds3 - completed";
            remainTurns += 3;
            remainTurnsText.text = $"Remain turns: {remainTurns}";
        }
        #endregion

        #region Private Helpers
        private LuckyChanceRarity GetRandomRarity()
        {
            // Get all enum values
            var values = Enum.GetValues(typeof(LuckyChanceRarity));
            // Pick one randomly
            return (LuckyChanceRarity)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        private Color GetRarityColor(LuckyChanceRarity rarity)
        {
            switch (rarity)
            {
                case LuckyChanceRarity.Common:
                    return Color.white;
                case LuckyChanceRarity.Rare:
                    return new Color(0.2f, 0.6f, 1f); // light blue
                case LuckyChanceRarity.SupperRare:
                    return new Color(0.7f, 0.3f, 1f); // purple
                case LuckyChanceRarity.Legendary:
                    return new Color(1f, 0.8f, 0.2f); // gold
                default:
                    return Color.gray;
            }
        }
        #endregion

    }
}