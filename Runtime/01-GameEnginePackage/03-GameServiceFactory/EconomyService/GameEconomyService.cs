using System.Threading.Tasks;

namespace JKTechnologies.CommonPackage
{
    public class GameEconomyService : IGameEconomyService
    {
        public void RegisterEconomyListener(IGameEconomyListener gameEconomyListener)
        {
            GameEngineService.RegisterEconomyListener(gameEconomyListener);
        }

        public void UnregisterEconomyListener(IGameEconomyListener gameEconomyListener)
        {
            GameEngineService.UnregisterEconomyListener(gameEconomyListener);
        }

        public long GetCurrencyBalance(GameCurrencyType gameCurrencyType)
        {
            return GameEngineService.GetCurrencyBalance(gameCurrencyType);
        }

        public async Task<bool> IncreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount)
        {
            return await GameEngineService.IncreaseCurrencyAsync(gameCurrencyType, amount);
        }
        public async Task<bool> DecreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount)
        {
            return await GameEngineService.DecreaseCurrencyAsync(gameCurrencyType, amount);
        }

        public async Task<bool> UpdateInventoryItemInstanceData(string playersInventoryItemId, object instanceData)
        {
            return await GameEngineService.UpdateInventoryItemInstanceData(playersInventoryItemId, instanceData);
        }

        public Unity.Services.Economy.Model.PlayersInventoryItem[] GetAllPlayersInventoryItemsByDefinitionId(string inventoryItemDefinitionId)
        {
            return GameEngineService.GetAllPlayersInventoryItemsByDefinitionId(inventoryItemDefinitionId);
        }

        public async Task<Unity.Services.Economy.Model.MakeVirtualPurchaseResult> MakeVirtualPurchaseByIdAsync(string virtualPurchaseId, string[] costItemIds = null)
        {
            return await GameEngineService.MakeVirtualPurchaseByIdAsync(virtualPurchaseId, costItemIds);
        }

        public async Task<bool> MakeSioPurchaseTransaction(GameSioPurchase gameSioPurchase)
        {
            return await GameEngineService.MakeSioPurchaseTransaction(gameSioPurchase);
        }

        public async Task<bool> MakeGoldPurchaseTransaction(GameGoldPurchase gameGoldPurchase)
        {
            return await GameEngineService.MakeGoldPurchaseTransaction(gameGoldPurchase);
        }
    }
}