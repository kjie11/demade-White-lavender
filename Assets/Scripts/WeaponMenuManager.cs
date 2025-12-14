using UnityEngine;
using System.Collections.Generic;

using System;

public class WeaponMenuManager : MonoBehaviour
{
    public List<GameObject> weaponItems;
    
    public GameObject knife;

    private int currentIndex = 0;

    public event Action<GameManager.WeaponType> onWeaponChanged;

    private void OnEnable()
    {
        // get the status from gamemanager
        SyncFromGameManager();
        UpdateSelectionVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            Navigate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            Navigate(1);
        }
    }

    void Navigate(int direction)
    {
        int newIndex = currentIndex + direction;

        if (newIndex >= 0 && newIndex < weaponItems.Count)
        {
            currentIndex = newIndex;
            UpdateSelectionVisuals();
            ApplyWeapon(); 
        }
    }

    void UpdateSelectionVisuals()
    {
        for (int i = 0; i < weaponItems.Count; i++)
        {
            Transform border = weaponItems[i].transform.Find("currentSelect");
            if (border != null)
            {
                border.gameObject.SetActive(i == currentIndex);
            }
        }
    }

    void ApplyWeapon()
    {
        if (currentIndex == 0)
        {
            GameManager.Instance.SetWeapon(GameManager.WeaponType.Knife);
            
            knife.SetActive(true);
            onWeaponChanged?.Invoke(GameManager.WeaponType.Knife); //notify gamemanager and update the current weapon
        }
        else if (currentIndex == 1)
        {
            GameManager.Instance.SetWeapon(GameManager.WeaponType.ThrowBall);
            
            knife.SetActive(false);
            onWeaponChanged?.Invoke(GameManager.WeaponType.ThrowBall);//notify gamemanager and update the current weapon
        }
    }

    void SyncFromGameManager()
    {
        switch (GameManager.Instance.currentWeapon)
        {
            case GameManager.WeaponType.Knife:
                currentIndex = 0;
                break;

            case GameManager.WeaponType.ThrowBall:
                currentIndex = 1;
                break;
        }
    }
}
