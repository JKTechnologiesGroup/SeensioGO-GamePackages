using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class GameEngineServiceInitializer : MonoBehaviour
    {
        [SerializeField] private GameEngineServiceSimulation gameEngineServiceSimulation;
        private void Awake()
        {
            gameEngineServiceSimulation.Initialize();
        }
        
        private void OnDestroy()
        {
            gameEngineServiceSimulation.DeInitialize();
        }
    }
}