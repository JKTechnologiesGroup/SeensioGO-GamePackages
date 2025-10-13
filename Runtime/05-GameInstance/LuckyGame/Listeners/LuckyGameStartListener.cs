using UnityEngine;
using JKTechnologies.SeensioGo.GameInstances;
using UnityEngine.Events;
using System.Collections;

namespace JKTechnologies.SeensioGo.GameEngines.LuckyGames
{
    public class LuckyGameStartListener : MonoBehaviour, IGameInstanceStartListener
    {
        [SerializeField] private UnityEvent OnStartGame;
        [SerializeField] private LuckyGameFlow luckyGameFlow;
        [SerializeField] private float delayBeforeStartGame;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponentHolder;

        private void Start()
        {
            StartCoroutine(StartCoroutine());
        }

        private IEnumerator StartCoroutine()
        {
            yield return new WaitForSeconds(delayBeforeStartGame);
            luckyGameFlow.Init(this);
        }

        public void StartGame()
        {
            OnStartGame.Invoke();
            inGameUIComponentHolder.SetActive(true);
        }
    }
}