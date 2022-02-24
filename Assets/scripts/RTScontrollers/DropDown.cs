using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropDown : MonoBehaviour
{
    public TMP_Dropdown dd;
    public MapGenerator mg;
    // Start is called before the first frame update
    void Start()
    {
        dd.onValueChanged.AddListener(delegate
        {
            valueChanged(dd);
            dd.options[0].text = "Erase";
            dd.options[1].text = "Wall " + TileMap.wallCount;
            dd.options[2].text = "Ground";
            dd.options[3].text = "Grass";
            dd.options[4].text = "Scrap " + TileMap.scrapsCount;
            dd.options[5].text = "Metal " + TileMap.metalCount;
            dd.options[6].text = "Copper " + TileMap.copperCount;
            dd.options[7].text = "Storage";
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
