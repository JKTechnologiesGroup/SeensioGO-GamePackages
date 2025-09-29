using System.Threading.Tasks;

namespace JKTechnologies.CommonPackage
{
    public interface IGameEconomyService
    {
        public void RegisterEconomyListener(IGameEconomyListener gameEconomyListener);
        public void UnregisterEconomyListener(IGameEconomyListener gameEconomyListener);
        public long GetCurrencyBalance(GameCurrencyType gameCurrencyType);
        public Task<bool> IncreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount);
        public Task<bool> DecreaseCurrencyAsync(GameCurrencyType gameCurrencyType, int amount);
        public Task<bool> UpdateInventoryItemInstanceData(string playersInventoryItemId, object instanceData);
        public Unity.Services.Economy.Model.PlayersInventoryItem[] GetAllPlayersInventoryItemsByDefinitionId(string inventoryItemDefinitionId);
        public Task<Unity.Services.Economy.Model.MakeVirtualPurchaseResult> MakeVirtualPurchaseByIdAsync(string virtualPurchaseId, string[] costItemIds = null);
        public Task<bool> MakeSioPurchaseTransaction(GameSioPurchase gameSioPurchase);
        public Task<bool> MakeGoldPurchaseTransaction(GameGoldPurchase gameGoldPurchase);
    }
}