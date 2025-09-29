using UnityEngine;
namespace JKTechnologies.CommonPackage
{
    public class RealtimeWeatherLoader : MonoBehaviour
    {
        private void Start()
        {
            LoadRealtimeWeather();
        }

        private void LoadRealtimeWeather()
        {
            GameInstanceModel gameInstanceModel = new GameInstanceModel();
            gameInstanceModel.UtilitiesService.EnableRealtimeWeather();
        }
    }
}