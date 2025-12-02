using UnityEngine;
using System.Collections.Generic;

public class WeaponMenuManager : MonoBehaviour
{
    // 请将所有武器项的根对象（例如：item1, item2, item3...）拖入这个列表
    public List<GameObject> weaponItems;

    private int currentIndex = 0;
    public enum WeaponType
{
    Knife,
    Ball
}
public WeaponType currentWeapon = WeaponType.Knife;
public playerController player;

public GameObject knife;


    void Start()
    {
        UpdateSelectionVisuals();
    }

    void Update()
    {
        // 按 A 键（左移）
        if (Input.GetKeyDown(KeyCode.A))
        {
            Navigate(-1); 
        }
        // 按 D 键（右移）
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Navigate(1);
        }
    }

    void Navigate(int direction)
    {
        int newIndex = currentIndex + direction;

        // 确保索引在列表范围内
        if (newIndex >= 0 && newIndex < weaponItems.Count)
        {
            currentIndex = newIndex;
            UpdateSelectionVisuals();
        }
    }

    /// <summary>
    /// 根据当前索引显示或隐藏边框
    /// </summary>
    void UpdateSelectionVisuals()
    {
        for (int i = 0; i < weaponItems.Count; i++)
        {
            GameObject currentWeaponItem = weaponItems[i];
            
            // ❗ 使用 Transform.Find 通过名称查找子对象 "currentSelect"
            // 确保你的所有武器项中，边框父物体都叫 "currentSelect"
            Transform borderTransform = currentWeaponItem.transform.Find("currentSelect"); 

            if (borderTransform == null)
            {
                Debug.LogError("Weapon Item " + currentWeaponItem.name + " is missing the 'currentSelect' child object.");
                continue; 
            }

            // 获取边框父物体
            GameObject borderParent = borderTransform.gameObject;

            if (i == currentIndex)
            {
                // 如果是当前选中的，显示整个边框父物体
                borderParent.SetActive(true);
            }
            else
            {
                // 如果不是当前选中的，隐藏整个边框父物体
                borderParent.SetActive(false);
            }
        }
         UpdateWeaponType();
    }

    void UpdateWeaponType()
{
    if (currentIndex == 0)
        {
            player.currentWeapon = playerController.WeaponType.Knife;
            knife.SetActive(true);
        }
        
    else if (currentIndex == 1)
        {
            player.currentWeapon = playerController.WeaponType.ThrowBall;
            knife.SetActive(false);
        }
        
}

}