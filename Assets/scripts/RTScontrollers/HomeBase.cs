using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    public float healthAmount;
    GameObject hpb;
    public bool showlifebar;
    float maxHP = 0;
    public int iD = 0;
    GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        if (showlifebar)
        {

            hpb = Instantiate(Resources.Load<GameObject>("GameHelpers/HealthBar"));

            hpb.transform.GetChild(0).gameObject.GetComponent<HealhBar>().setHB(this, healthAmount);
        }
        maxHP = healthAmount;
        particle = Resources.Load("AgentObjects/BasePart") as GameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if(healthAmount <= 0)
        {
            gameObject.SetActive(false);
            if (iD == 1)
            {
                RTSManager.goodBases.Remove(this.gameObject);
            }
            if (iD == 2)
            {
                RTSManager.badBases.Remove(this.gameObject);
            }
        }   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Bullet" && collision.GetComponent<Bullet>().id != iD)
        {
            GameObject part = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(part, 1);
        }
    }
}
