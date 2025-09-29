using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JKTechnologies.CommonPackage;

namespace JKTechnologies.GameEngine.Example
{
    public class EconomyViewer : MonoBehaviour, IGameEconomyListener
    {
        [SerializeField] private List<TextMeshProUGUI> sioTextList;
        [SerializeField] private List<TextMeshProUGUI> goldTextList;
        private GameInstanceModel gameInstanceModel;
        
        private void Awake()
        {
            gameInstanceModel = new GameInstanceModel();
            OnCurrencyBalanceChanged();
            OnInventoryItemChanged();
            gameInstanceModel.EconomyService.RegisterEconomyListener(this);
        }

        public void OnCurrencyBalanceChanged()
        {
            ReloadSioBalance();
            ReloadGoldBalance();
        }

        private void ReloadSioBalance()
        {
            long newSioBalance = gameInstanceModel.EconomyService.GetCurrencyBalance(GameCurrencyType.Sio);           
            foreach (TextMeshProUGUI sioText in sioTextList)
            {
                sioText.text = newSioBalance.ToString();
            }
        }

        private void ReloadGoldBalance()
        {
            long newGoldBalance = gameInstanceModel.EconomyService.GetCurrencyBalance(GameCurrencyType.Gold);
            foreach (TextMeshProUGUI goldText in goldTextList)
            {
                goldText.text = newGoldBalance.ToString();
            }
        }

        public void OnInventoryItemChanged()
        {
            Debug.LogError("On Inventory Item Changed");
        }
    }
}