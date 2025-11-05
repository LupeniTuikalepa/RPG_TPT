using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
   
    public int sceneIndex = 1;

   
    public void LoadScene()
    {
        Event onButtonClick;
        Debug.Log("JE CLIQUE LA PTN");
        Time.timeScale = 1f;

       
        SceneManager.LoadScene(sceneIndex);
    }
}