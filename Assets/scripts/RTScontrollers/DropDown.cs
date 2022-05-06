using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropDown : MonoBehaviour
{
    public TMP_Dropdown dd;
    public MapGenerator mg;
    public TMP_InputField basiIdInput;
    public TMP_Dropdown baseDD;
    // Start is called before the first frame update
    void Start()
    {
        baseDD.gameObject.SetActive(false);
        dd.onValueChanged.AddListener(delegate
        {
            valueChanged(dd);
            dd.options[0].text = "Erase";
            dd.options[1].text = "Wall " + TileMap.wallCount;
            dd.options[2].text = "Ground";
            dd.options[3].text = "Grass";
            dd.options[4].text = "Scrap " + TileMap.resource1Count;
            dd.options[5].text = "Metal " + TileMap.resource2Count;
            dd.options[6].text = "Copper " + TileMap.resource3Count;
            dd.options[7].text = "Storage";
            if(dd.value == 10)
            {
                baseDD.gameObject.SetActive(true);
            }
            else
            {
                baseDD.gameObject.SetActive(false);
            }
        });
    }
    public void valueChanged(TMP_Dropdown d)
    {
        mg.ActivateTile(d.value);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
