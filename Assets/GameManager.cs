using UnityEngine;

//manager and reocord  the weapon status, singleton
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public WeaponMenuManager weaponMenuManager; //register the weaponChange event in weaponManager, and update the weapon status
    public playerController player;
    public enum WeaponType
    {
        Knife,
        ThrowBall
    }
    public WeaponType currentWeapon = WeaponType.Knife;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        if (weaponMenuManager != null)
        {
            weaponMenuManager.onWeaponChanged += WeaponChangedHandler;
        }
    }
    private void OnDisable()
    {
        if (weaponMenuManager != null)
        {
            weaponMenuManager.onWeaponChanged -= WeaponChangedHandler;
        }
    }

    private void WeaponChangedHandler(WeaponType weapon)
    {
        SetWeapon(weapon);
    }

    private void Update()
    {
        // press 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(WeaponType.Knife);
        }
        // press 2
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(WeaponType.ThrowBall);
        }
    }

    // ai: how to write and call set weapon function
    public void SetWeapon(WeaponType weapon)
    {
        if (currentWeapon == weapon)
            return;

        currentWeapon = weapon;
        if (currentWeapon == WeaponType.Knife){
            player.currentWeapon = playerController.WeaponType.Knife;
        }
        else
        {
            player.currentWeapon = playerController.WeaponType.ThrowBall;
        }
    }
    public WeaponType GetWeapon()
    {
        return currentWeapon;
    }
}