using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage
{
    public abstract class GameEngineService : ScriptableObject
    {
        #region Internal Properties
        protected static GameEngineService instance;
        protected virtual GameUserInfo HandleGetUserInfo() { return null; }

        protected virtual void HandleRegisterEconomyListener(IGameEconomyListener gameEconomyListener) { }
        protected virtual void HandleUnregisterEconomyListener(IGameEconomyListener gameEconomyListener) { }
        protected virtual long HandleGetGameCurrency(GameCurrencyType gameCurrencyType) { return 0; }
        protected virtual Task<bool> HandleIncreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount) { return null; }
        protected virtual Task<bool> HandleDecreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount) { return null; }
        protected virtual Unity.Services.Economy.Model.PlayersInventoryItem[] HandleGetAllPlayersInventoryItemsByDefinitionId(string inventoryItemDefinitionId) { return null; }
        protected virtual Task<bool> HandleUpdateInventoryItemInstanceData(string playersInventoryItemId, object instanceData) { return null; }
        protected virtual Task<Unity.Services.Economy.Model.MakeVirtualPurchaseResult> HandleMakeVirtualPurchaseByIdAsync(string virtualPurchaseId, string[] costItemIds = null) { return null; }
        protected virtual Task<bool> HandleMakeSioPurchaseTransaction(GameSioPurchase gameSioPurchase) { return null; }
        protected virtual Task<bool> HandleMakeGoldPurchaseTransaction(GameGoldPurchase gameGoldPurchase) { return null; }
        protected virtual void HandleQuitGame() { }
        protected virtual void HandleLoadImageFromUrl(Image imageObject, string imageUrl, int maxSize = 1024, GameObject loadingObject = null) { }
        protected virtual void HandleLoadScene(string sceneName) { }
        protected virtual void HandleLoadSceneByIndex(int sceneIndex) { }
        protected virtual bool HandleIsDeveloper() { return true; }
        protected virtual Task<T> HandleGetGameDataByQuestPackPoolId<T>(string campaignId) { return default; }
        protected virtual Task<bool> HandleUpdateGameDataByQuestPackPoolId(string campaignId, object gameData) { return default; }
        protected virtual void HandleEnableRealtimeWeather() { }
        protected virtual void HandleDisableRealtimeWeather() { }
        protected virtual void HandleLoad3DModelFromUrl(string modelUrl, GameObject modelParent) { }
        protected virtual string HandleGetCurrentLanguage() { return string.Empty; }
        #endregion

        #region User Info
        public static GameUserInfo GetUserInfo()
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return null;
            }
            return instance.HandleGetUserInfo();
        }
        #endregion

        #region Economy
        public static void RegisterEconomyListener(IGameEconomyListener gameEconomyListener)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleRegisterEconomyListener(gameEconomyListener);
        }
        public static void UnregisterEconomyListener(IGameEconomyListener gameEconomyListener)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleUnregisterEconomyListener(gameEconomyListener);
        }

        public static long GetCurrencyBalance(GameCurrencyType gameCurrencyType)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return 0;
            }
            return instance.HandleGetGameCurrency(gameCurrencyType);
        }

        public static async Task<bool> IncreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleIncreaseCurrencyAsync(gameCurrencyType, amount);
        }
        public static async Task<bool> DecreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleDecreaseCurrencyAsync(gameCurrencyType, amount);
        }


        public static async Task<bool> UpdateInventoryItemInstanceData(string playersInventoryItemId, object instanceData)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleUpdateInventoryItemInstanceData(playersInventoryItemId, instanceData);
        }
        public static Unity.Services.Economy.Model.PlayersInventoryItem[] GetAllPlayersInventoryItemsByDefinitionId(string inventoryItemDefinitionId)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return null;
            }
            return instance.HandleGetAllPlayersInventoryItemsByDefinitionId(inventoryItemDefinitionId);
        }
        public static async Task<Unity.Services.Economy.Model.MakeVirtualPurchaseResult> MakeVirtualPurchaseByIdAsync(string virtualPurchaseId, string[] costItemIds = null)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return null;
            }
            return await instance.HandleMakeVirtualPurchaseByIdAsync(virtualPurchaseId, costItemIds);
        }
        public static async Task<bool> MakeSioPurchaseTransaction(GameSioPurchase gameSioPurchase)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleMakeSioPurchaseTransaction(gameSioPurchase);
        }
        public static async Task<bool> MakeGoldPurchaseTransaction(GameGoldPurchase gameGoldPurchase)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleMakeGoldPurchaseTransaction(gameGoldPurchase);
        }
        #endregion

        #region Utilities
        public static void LoadScene(string sceneName)
        {
#if !SEENSIOGO
            SceneManager.LoadScene(sceneName);
#endif
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleLoadScene(sceneName);
        }

        public static void LoadSceneByIndex(int sceneIndex)
        {
#if !SEENSIOGO
            SceneManager.LoadScene(sceneIndex);
#endif
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleLoadSceneByIndex(sceneIndex);
        }
        public static void LoadImageFromUrl(Image imageObject, string imageUrl, int maxSize = 1024, GameObject loadingObject = null)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleLoadImageFromUrl(imageObject, imageUrl, maxSize, loadingObject);
        }
        public static void QuitGame()
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleQuitGame();
        }
        #endregion

        #region IsDeveloper
        public static bool IsDeveloper()
        {
            if (instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return true;
            }
            return instance.HandleIsDeveloper();
        }
        #endregion

        #region Get Game Data By Campaign
        public static async Task<T> GetGameDataByQuestPackPoolId<T>(string questPackPoolId)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return default;
            }
            return await instance.HandleGetGameDataByQuestPackPoolId<T>(questPackPoolId);
        }

        public static async Task<bool> UpdateGameDataByQuestPackPoolId(string questPackPoolId, object gameData)
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return false;
            }
            return await instance.HandleUpdateGameDataByQuestPackPoolId(questPackPoolId, gameData);
        }
        #endregion

        #region Enable Realtime Weather
        public static void EnableRealtimeWeather()
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleEnableRealtimeWeather();
        }

        public static void DisableRealtimeWeather()
        {
            if (instance == null)
            {
                Debug.LogError("GameEngineService is null");
                return;
            }
            instance.HandleDisableRealtimeWeather();
        }
        #endregion

        #region Load 3D Model From URL
        public static void Load3DModelFromUrl(string modelUrl, GameObject modelParent)
        {
            if (instance == null)
            {
                Debug.LogError("Game Engine Service is null");
                return;
            }
            instance.HandleLoad3DModelFromUrl(modelUrl, modelParent);
        }
        #endregion

        #region Language
        public static string GetCurrentLanguage()
        {
            if (instance == null)
            {
                return "en";
            }

            return instance.HandleGetCurrentLanguage();
        }
        #endregion
    }   
}