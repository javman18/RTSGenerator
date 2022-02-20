using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    public static int copper = 0;
    public static int metal = 0;
    public static int scraps = 0;
    public TextMeshProUGUI coperText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI scrapsText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coperText.text = "Copper " + copper.ToString();
        metalText.text = "Metal " + metal.ToString();
        scrapsText.text = "Scraps " + scraps.ToString();
    }
}
