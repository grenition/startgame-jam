using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveSceneDebugger : MonoBehaviour
{
    void Update()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
    }
}

