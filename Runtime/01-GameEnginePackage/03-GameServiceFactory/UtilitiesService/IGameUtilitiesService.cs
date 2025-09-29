using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JKTechnologies.CommonPackage
{
    public interface IGameUtilitiesService
    {
        public void LoadImageFromUrl(Image imageObject, string imageUrl, int maxSize = 1024, GameObject loadingObject = null);
        public void LoadScene(string sceneName);
        public void QuitGame();
        public bool IsDeveloper();
    }
}