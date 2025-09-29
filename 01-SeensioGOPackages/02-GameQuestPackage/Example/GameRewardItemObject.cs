using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage
{
    public class GameRewardItemObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI amount;
        [SerializeField] private Image icon;
        [SerializeField] private GameConditionInstance gameConditionInstance;
        public void Start()
        {
            GameRewardItem[] gameRewardItems = gameConditionInstance.GetGameRewardItems();
            if(gameRewardItems.Length > 0)  
            {
                if(title != null)
                {
                    title.text = gameRewardItems[0].title;
                }

                if(amount != null)
                {
                    amount.text = $"x{gameRewardItems[0].amount}";
                }
                if(icon != null && !string.IsNullOrEmpty(gameRewardItems[0].photoUrl))
                {
                    GameInstanceModel gameInstanceModel = new GameInstanceModel();
                    gameInstanceModel.UtilitiesService.LoadImageFromUrl(icon, gameRewardItems[0].photoUrl);
                }
            }
        }
    }
}