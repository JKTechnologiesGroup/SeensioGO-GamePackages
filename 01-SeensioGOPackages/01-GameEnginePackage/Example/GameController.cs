using UnityEngine;
using JKTechnologies.CommonPackage;

namespace JKTechnologies.GameEngine.Example
{
    public class GameController : MonoBehaviour
    {
        private GameInstanceModel gameInstanceModel;
        private void Awake()
        {
            gameInstanceModel = new GameInstanceModel();
        }

        public void QuitGame()
        {
            gameInstanceModel.UtilitiesService.QuitGame();
        }
    }
}