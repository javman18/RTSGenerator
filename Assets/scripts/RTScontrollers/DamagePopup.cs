using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Create(Vector3 position, float damage, GameObject popupPrefab)
    {
        GameObject damagePopup = Instantiate(popupPrefab, position, Quaternion.identity);

        DamagePopup dpup = damagePopup.GetComponent<DamagePopup>();
        dpup.Setup(damage);

        return dpup;
    }
    float moveYSpeed;
    float moveXSpeed;
    // Start is called before the first frame update
    private TextMeshPro textMesh;
    private float disapearTime;
    private Color textColor;
    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float damge)
    {
        textMesh.SetText(damge.ToString());
        textColor = textMesh.color;
        disapearTime = 0.5f;
        moveYSpeed = Random.Range(-20, 20);
        moveXSpeed = Random.Range(-20, 20);
    }
    // Update is called once per frame
    void Update()
    {
        
        transform.position += new Vector3(moveXSpeed, moveYSpeed) * Time.deltaTime;
        disapearTime -= Time.deltaTime;

        if(disapearTime < 0)
        {
            float dissapearSpeed = 3f;
            textColor.a -= dissapearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
