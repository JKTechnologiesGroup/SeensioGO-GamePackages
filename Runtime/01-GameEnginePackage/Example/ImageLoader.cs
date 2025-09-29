using UnityEngine;
using UnityEngine.UI;
using JKTechnologies.CommonPackage;

namespace JKTechnologies.GameEngine.Example
{
    public class ImageLoader : MonoBehaviour
    {
        [SerializeField] private Image userPhoto;

        public void LoadPhoto(string photoUrl)
        {

            if(userPhoto != null && !string.IsNullOrEmpty(photoUrl))
            {
                GameInstanceModel gameInstanceModel = new GameInstanceModel();
                gameInstanceModel.UtilitiesService.LoadImageFromUrl(userPhoto, photoUrl);
            }
        }
    }
}