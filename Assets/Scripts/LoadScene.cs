using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

// ========================
// Revision 2020.10.14
// ========================

namespace NightFramework
{
    public class LoadScene : MonoBehaviour
    {
        // ========================================================================================
        public SceneReference SceneToLoad;
        public LoadSceneMode LoadingMode;


        // ========================================================================================
        public void Load()
        {
            if (SceneToLoad.IsAssigned)
                SceneToLoad.LoadScene(LoadingMode);
        }
    }
}