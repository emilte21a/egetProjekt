using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerController : MonoBehaviour
{
   public void LoadScene(string SceneName)
   {
      SceneManager.LoadScene(SceneName);
   }
}
