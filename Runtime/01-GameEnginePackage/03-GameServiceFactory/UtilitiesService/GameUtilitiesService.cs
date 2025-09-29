using UnityEngine;
using UnityEngine.UI;
namespace JKTechnologies.CommonPackage
{
    public class GameUtilitiesService : IGameUtilitiesService
    {
        public void LoadImageFromUrl(Image imageObject, string imageUrl, int maxSize = 1024, GameObject loadingObject = null)
        {
            GameEngineService.LoadImageFromUrl(imageObject, imageUrl, maxSize, loadingObject);
        }
        public void LoadScene(string sceneName)
        {
            GameEngineService.LoadScene(sceneName);
        }
        public void QuitGame()       
        {
            GameEngineService.QuitGame();
        }

        public bool IsDeveloper()
        {
            return GameEngineService.IsDeveloper();
        }
    }
}