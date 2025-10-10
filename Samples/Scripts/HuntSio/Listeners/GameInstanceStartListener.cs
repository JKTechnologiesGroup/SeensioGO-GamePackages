using UnityEngine;
using JKTechnologies.SeensioGo.GameInstances;
using System.Collections;

namespace JKTechnologies.SeensioGo.GameEngines.HuntSio
{
    public class GameInstanceStartListener : MonoBehaviour, IGameInstanceStartListener
    {
        [SerializeField] private HuntSioGameFlow huntSioGameFlow;

        [Header("[ In Game UI Components ]")]
        [SerializeField] private GameObject inGameUIComponentHolder;
        [SerializeField] private float delayBeforeStartGame;

        private void Start()
        {
            StartCoroutine(StartCoroutine());
        }

        private IEnumerator StartCoroutine()
        {
            yield return new WaitForSeconds(delayBeforeStartGame);
            huntSioGameFlow.Init(this);
        }

        public void StartGame()
        {
            inGameUIComponentHolder.SetActive(true);
        }
    }
}