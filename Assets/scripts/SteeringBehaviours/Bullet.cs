using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int id = 0;
    public float damage = 0;
    public float destroyTime;
    public GameObject damagepp;
    // Start is called before the first frame update
    void Start()
    {
        destroyTime = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Agent" && id != collision.GetComponent<AgentManager>().team || collision.tag == "Leader" && id != collision.GetComponent<AgentManager>().team)
        {
            collision.GetComponent<AgentManager>().healthAmount -= damage;
            DamagePopup.Create(collision.transform.position, damage, damagepp);
            gameObject.SetActive(false);
        }
        else if(collision.tag == "Base" && id != collision.GetComponent<HomeBase>().iD)
        {
            collision.GetComponent<HomeBase>().healthAmount -= damage;
            DamagePopup.Create(collision.transform.position, damage, damagepp);
            gameObject.SetActive(false);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        destroyTime += Time.deltaTime;
        if (destroyTime >= 5f)
        {
            gameObject.SetActive(false);
            destroyTime = 0;
        }
    }
}
