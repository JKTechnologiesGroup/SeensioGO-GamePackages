using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage
{
    public class GameRewardItemLoader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI amount;
        [SerializeField] private Image icon;

        public void Setup(GameRewardItem gameRewardItem)
        {
            if(gameRewardItem != null)
            {
                if(title != null && !string.IsNullOrEmpty(gameRewardItem.title))
                {
                    title.text = gameRewardItem.title;
                }

                if(amount != null)
                {
                    amount.text = $"x{gameRewardItem.amount}";
                }

                if(icon != null && !string.IsNullOrEmpty(gameRewardItem.photoUrl))
                {
                    icon.gameObject.SetActive(true);
                    GameInstanceModel gameInstanceModel = new GameInstanceModel();
                    gameInstanceModel.UtilitiesService.LoadImageFromUrl(icon, gameRewardItem.photoUrl);
                }
            }
        }
    }
}