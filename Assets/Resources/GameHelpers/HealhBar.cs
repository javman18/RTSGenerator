using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealhBar : MonoBehaviour
{
    AgentManager agent;
    
    float maxHp;
    public void Set(AgentManager agent, float mhp)
    {
        this.agent = agent;
        maxHp = mhp;
    }

    

    void LateUpdate()
    {
        if (agent != null)
        {
            transform.root.position = agent.transform.position + new Vector3(0, 5f, 0);
            transform.localScale = new Vector3(agent.healthAmount / maxHp, transform.localScale.y, transform.localScale.z);
            if (!agent.gameObject.activeSelf)
            {
                transform.root.gameObject.SetActive(false);
            }
           
        }
        
    }

}
