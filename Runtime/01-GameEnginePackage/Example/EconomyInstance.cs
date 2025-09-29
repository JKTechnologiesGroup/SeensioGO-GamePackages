using UnityEngine;

using JKTechnologies.CommonPackage;

namespace JKTechnologies.GameEngine.Example
{
    public class EconomyInstance : MonoBehaviour
    {
        private IGameEconomyService economyService => gameInstanceModel.EconomyService;
        private GameInstanceModel gameInstanceModel;
        private void Awake()
        {
            gameInstanceModel = new GameInstanceModel();
        }


        public void IncreaseSio(int amount)
        {
            if(amount < 0)
            {
                Debug.LogError("Amount cannot be negative");
                return;
            }
            economyService.IncreaseCurrencyAsync(GameCurrencyType.Sio, amount);
        }

        public void DecreaseSio(int amount)
        {
            if(amount < 0)
            {
                Debug.LogError("Amount cannot be negative");
                return;
            }
            economyService.DecreaseCurrencyAsync(GameCurrencyType.Sio, amount);
        }

        public void IncreaseGold(int amount)
        {
            if(amount < 0)
            {
                Debug.LogError("Amount cannot be negative");
                return;
            }
            economyService.IncreaseCurrencyAsync(GameCurrencyType.Gold, amount);
        }

        public void DecreaseGold(int amount)
        {
            if(amount < 0)
            {
                Debug.LogError("Amount cannot be negative");
                return;
            }
            economyService.DecreaseCurrencyAsync(GameCurrencyType.Gold, amount);
        }
    }
}