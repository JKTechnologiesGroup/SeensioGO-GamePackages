using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.LuckyGames
{
    public class LuckyGameResultSubmitter : MonoBehaviour
    {
        [SerializeField] private LuckyGameFlow luckyGameFlow;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponentHolder;
        public void SubmitResult(LuckyChanceRarity rarity)
        {
            inGameUIComponentHolder.SetActive(false);
            luckyGameFlow.SubmitLuckyChance(rarity);
        }
    }
}