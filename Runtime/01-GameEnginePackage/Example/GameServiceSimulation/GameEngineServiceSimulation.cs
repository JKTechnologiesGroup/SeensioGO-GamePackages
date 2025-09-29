using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage
{
    [CreateAssetMenu(fileName = "GameEngineServiceInstance", menuName = "JKTechnologies/GameEnginePackage/GameEngineServiceSimulation", order = 1)]
    public class GameEngineServiceSimulation : GameEngineService
    {
        [SerializeField] private List<IGameEconomyListener> gameEconomyListenerList;
        [SerializeField] private GameUserInfo gameUserInfo;
        [SerializeField] private GameCurrencyBalance[] gameCurrencyBalances;
        [SerializeField] private GameInventoryItem[] gameInventoryItems;

        #region Initialize
        public void Initialize()        
        {
            if(instance == null)
            {
                gameEconomyListenerList = new List<IGameEconomyListener>();
                instance = this;
            }
            else
            {
                Debug.LogError("GameEngineService is already initialized");
            }
        }
        #endregion

        #region User
        protected override GameUserInfo HandleGetUserInfo()
        {
            return gameUserInfo;
        }
        #endregion

        
        #region Economy
        protected override void HandleRegisterEconomyListener(IGameEconomyListener gameEconomyListener)
        {
            IGameEconomyListener existingListener = gameEconomyListenerList.FirstOrDefault(item => item == gameEconomyListener);
            if(existingListener == null)
            {
                gameEconomyListenerList.Add(gameEconomyListener);
            }
        }
        protected override long HandleGetGameCurrency(GameCurrencyType gameCurrencyType) 
        {
            GameCurrencyBalance gameCurrency = gameCurrencyBalances.FirstOrDefault(item => item.gameCurrencyType == gameCurrencyType);   
            if(gameCurrency == null)
            {
                Debug.LogError("Can not find game currency: " + gameCurrencyType);
                return 0;
            }
            return gameCurrency.balance;
        }

        protected override async Task<bool> HandleIncreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount) 
        { 
            await Task.Delay(500);
            
            GameCurrencyBalance gameCurrency = gameCurrencyBalances.FirstOrDefault(item => item.gameCurrencyType == gameCurrencyType);
            if(gameCurrency == null)
            {
                Debug.LogError("Can not find game currency: " + gameCurrencyType);
                return false;
            }
            if(gameCurrencyType == GameCurrencyType.Sio)
            {
                Debug.LogError("This function is not supported on SeensioGo platform");
            }
            gameCurrency.balance += amount;
            OnCurrencyBalanceChanged();
            return true;
        }
        protected override async Task<bool> HandleDecreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount) 
        {
            await Task.Delay(500);
            
            GameCurrencyBalance gameCurrency = gameCurrencyBalances.FirstOrDefault(item => item.gameCurrencyType == gameCurrencyType);
            if(gameCurrency == null)
            {
                Debug.LogError("Can not find game currency: " + gameCurrencyType);
                return false;
            }
            if(gameCurrencyType == GameCurrencyType.Sio)
            {
                Debug.LogError("This function is not supported on SeensioGo platform");
            }
            gameCurrency.balance -= amount;
            OnCurrencyBalanceChanged();
            return true;
        }

        public void OnCurrencyBalanceChanged()
        {
            foreach(IGameEconomyListener gameEconomyListener in gameEconomyListenerList)
            {
                gameEconomyListener?.OnCurrencyBalanceChanged();
            }
        }

        public void OnInventoryItemChanged()
        {
            foreach(IGameEconomyListener gameEconomyListener in gameEconomyListenerList)
            {
                gameEconomyListener?.OnInventoryItemChanged();
            }
        }
        #endregion

        #region Utilities
        protected override void HandleLoadImageFromUrl(Image imageObject, string imageUrl, int maxSize = 1024, GameObject loadingObject = null)
        {
            Debug.LogError("This function only work on SeensioGo platform");
            return;
        }
        
        protected override void HandleQuitGame() 
        { 
            DeInitialize();
        }
        #endregion

        #region IsDeveloper
        protected override bool HandleIsDeveloper()
        {
            return true;
        }
        #endregion

        #region DeInitialized
        public void DeInitialize()
        {
            if(instance != null && instance == this)
            {
                instance = null;
                gameEconomyListenerList.Clear();
            }
        }
        #endregion
    }
}