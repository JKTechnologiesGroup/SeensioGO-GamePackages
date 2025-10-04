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
        [SerializeField] private GameObject model3DParent;

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

                if(model3DParent != null && !string.IsNullOrEmpty(gameRewardItem.custom3DModelUrl))
                {
                    icon?.gameObject.SetActive(false);
                    GameInstanceModel gameInstanceModel = new GameInstanceModel();
                    gameInstanceModel.UtilitiesService.Load3DModelFromUrl(gameRewardItem.custom3DModelUrl, model3DParent);
                }
                else if(icon != null && !string.IsNullOrEmpty(gameRewardItem.photoUrl))
                {
                    icon.gameObject.SetActive(true);
                    GameInstanceModel gameInstanceModel = new GameInstanceModel();
                    gameInstanceModel.UtilitiesService.LoadImageFromUrl(icon, gameRewardItem.photoUrl);
                }
            }
        }
    }
}