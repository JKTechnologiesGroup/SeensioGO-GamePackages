using Unity.Services.Core;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class CommonPackageController : MonoBehaviour
    {
        [SerializeField] private CommonPackageInitializer commonPackageInitializer;

        private async void Start()
        {
            if(UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                // await UnityServices.InitializeAsync();
            }
            commonPackageInitializer.Initialize();
        }

        private void OnDestroy()
        {
            commonPackageInitializer.DeInitialize();    
        }

        private void OnApplicationQuit()
        {
            commonPackageInitializer.DeInitialize();
        }
    }
}