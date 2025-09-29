namespace JKTechnologies.CommonPackage
{
    public interface IGameEconomyListener
    {
        public void OnCurrencyBalanceChanged();
        public void OnInventoryItemChanged();
    }
}