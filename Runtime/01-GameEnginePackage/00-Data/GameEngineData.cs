using System;

namespace JKTechnologies.CommonPackage
{
    [Serializable]
    public enum GameCurrencyType
    {
        None,
        Sio,
        Gold
    }

    [Serializable]
    public enum GameInventoryItemType
    {
        None,
    }

    [Serializable]
    public class GameCurrencyBalance
    {
        public GameCurrencyType gameCurrencyType;
        public long balance;
    }

    [Serializable]
    public class GameInventoryItem
    {
        public GameInventoryItemType gameInventoryItemType;
        public long quantity;
    }

    [Serializable]
    public class GameUserInfo
    {
        public string userId;
        public string displayName;
        public string photoUrl;
    }

    public enum GameSioPurchase
    {
        PurchaseSio90,
        PurchaseSio200,
        PurchaseSio600,
        PurchaseSio1400,
        PurchaseSio3100,
        PurchaseSio8000,
        PurchaseSio20000,
        PurchaseSio50000,
    }

    public enum GameGoldPurchase
    {
        BuyGold1000,
        BuyGold10000,
        BuyGold55000,
        BuyGold115000,
        BuyGold180000,
        BuyGold250000,
    }
}