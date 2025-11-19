using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    
    void Start()
    {
        
    }
    public void ToGameScene()
    {
         SceneManager.LoadScene("SampleScene");  
    }
   
}
