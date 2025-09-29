using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JKTechnologies.CommonPackage;

namespace JKTechnologies.GameEngine.Example
{
    public class UserInfoLoader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI displayName;
        [SerializeField] private Image userPhoto;
        [SerializeField] private GameUserInfo gameUserInfo;
        private GameInstanceModel gameInstanceModel;
        
        private void Awake()
        {
            gameInstanceModel = new GameInstanceModel();
            LoadUserInfo();
        }

        public void LoadUserInfo()
        {
            gameUserInfo = gameInstanceModel.UserService.GetUserInfo();
            if(displayName != null)
            {
                displayName.text = gameUserInfo.displayName;
            }

            if(userPhoto != null)
            {
                string userPhotoUrl = gameUserInfo.photoUrl;
                gameInstanceModel.UtilitiesService.LoadImageFromUrl(userPhoto, userPhotoUrl);
            }
        }
    }
}