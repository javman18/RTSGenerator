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
