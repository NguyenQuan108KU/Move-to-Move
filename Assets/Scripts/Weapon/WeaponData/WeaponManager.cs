using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public WeaponDatabase weaponDB;

    public TextMeshProUGUI nameText;
    public Image image;
    public TextMeshProUGUI coin;
    public TextMeshProUGUI isLock;
    private int selectedOption = 0;

    private void Start()
    {
        UpdateWeapon(selectedOption);
    }
    public void NextOption()
    {
        Debug.Log("Next");
        selectedOption++;
        if(selectedOption >= weaponDB.WeaponCount())
        {
            selectedOption = 0;
        }
        UpdateWeapon(selectedOption);
    }
    public void BackOption()
    {
        Debug.Log("Back");
        selectedOption--;
        if(selectedOption < 0)
        {
            selectedOption = weaponDB.WeaponCount() - 1;
        }
        UpdateWeapon(selectedOption);
    }
    public void UpdateWeapon(int selectedOption)
    {
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        image.sprite = weapon.weaponImage;
        nameText.text = weapon.weaponName;
        coin.text = weapon.coin;
        isLock.text = weapon.isLock;
    }
}
