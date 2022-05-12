using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainTitleAnimation : MonoBehaviour
{
    public Material textMaterial;
    [SerializeField] private float time = -1;
    // Start is called before the first frame update
    void Start()
    {
        textMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, time);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / 5f;
        if (time <= 0.02)
        {
            textMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, time);
            
        }
    }
}
