using UnityEngine;

public class PauseMenuUGUI : MonoBehaviour
{
    public GameObject equipmentPanel;  // UGUI 的 Panel

    private bool isPaused = false;

    void Start()
    {
        if (equipmentPanel != null)
            equipmentPanel.SetActive(false); // 初始隐藏
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;                       
        equipmentPanel.SetActive(true);            
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;    
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;                       //
        equipmentPanel.SetActive(false);           // 
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;  // ）
        Cursor.visible = false;
    }
}
