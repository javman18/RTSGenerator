using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RTSManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] agents;
    
    public Dropdown agentSelectD;
    public Dropdown team2;
    public InputField squadInput;
    int i=0;
    int j = 0;

    //List<GameObject> agents;
    void Start()
    {
        agents = Resources.LoadAll<GameObject>("Agents");
      
        for (int i = 0; i < agents.Length; i++)
        {
            agentSelectD.options.Add(new Dropdown.OptionData(agents[i].name));
            
        }
        agentSelectD.onValueChanged.AddListener(delegate
        {

            i = agentSelectD.value;   
            
        });
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go = Instantiate(agents[i], mousePos, Quaternion.identity);
            go.name = agents[i].name;
            go.GetComponent<AgentManager>().team = 1;
            go.GetComponent<AgentManager>().squad = int.Parse(squadInput.text);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go = Instantiate(agents[i], mousePos, Quaternion.identity);
            go.name = agents[i].name;
            go.GetComponent<AgentManager>().team = 2;
            go.GetComponent<AgentManager>().squad = int.Parse(squadInput.text);
        }
    }
}
