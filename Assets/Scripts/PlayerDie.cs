using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour, IDie
{
    public KeyCode testDieKey = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(testDieKey))
        {
            Debug.Log("Test die key pressed! Calling OnDie().");
            OnDie();
        }
    }

    public void OnDie()
    {
        // Reset scene when player dies
        SceneManager.LoadScene(0);
        // The line below was throwing an error and has been removed.
        // throw new System.NotImplementedException();
    }
}