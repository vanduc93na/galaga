using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Slider _barSlider;
    [SerializeField] private GameObject _player;
    
    // Use this for initialization
    IEnumerator Start()
    {
        _player.SetActive(false);
        float value = 0;

        while (value < 100)
        {
            value += 1;
            if (value > 100) value = 100;
            yield return new WaitForSeconds(0.015f);
            _barSlider.value = (float)value / 100;
        }
        _player.SetActive(true);
        InventoryHelper.Instance.LoadInventory();
        int level = InventoryHelper.Instance.UserInventory.selectedLevel;
        print(level);
        GameController.Instance.StartGame(level);
        gameObject.SetActive(false);
    }
    
}
