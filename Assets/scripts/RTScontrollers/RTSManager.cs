using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RTSManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] agents;
    GameObject selectedSpawn;
    public Dropdown agentSelectD;
    public Dropdown team2;
    public InputField squadInput;
    int i=0;
    int j = 0;
    bool selected = false;
    List<Spawn> spawns;

    //List<GameObject> agents;
    void Start()
    {
        spawns = new List<Spawn>();
        agents = Resources.LoadAll<GameObject>("Agents");
        squadInput.text = "0";
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
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("sepico");
            selectedSpawn = ClickSelect();
        }
       
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go;
            if (selectedSpawn != null)
            {
                go = selectedSpawn.GetComponent<Spawn>().SpawnObject(agents[i]);
                go.name = agents[i].name;
                go.GetComponent<AgentManager>().team = 1;
                go.GetComponent<AgentManager>().squad = int.Parse(squadInput.text);
                
                
            }
            else
            {
                go = Instantiate(agents[i], mousePos, Quaternion.identity);
                go.name = agents[i].name;
                go.GetComponent<AgentManager>().team = 1;
                go.GetComponent<AgentManager>().squad = int.Parse(squadInput.text);
            }
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
    GameObject ClickSelect()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Spawn spawn;
        foreach(Spawn s in spawns)
        {
            s.SetSelected(false);
        }
        if (hit != null)
        {
            
            if (hit.gameObject.CompareTag("Spawn"))
            {
                spawn = hit.GetComponent<Spawn>();
                spawn.SetSelected(true);
                spawns.Add(spawn);
                //selected = true;
                Debug.Log("se eligio: " + hit.name);
                return hit.gameObject;
            }
            else if (hit.gameObject.CompareTag("Spawn") && selected == true)
            {
                //selected = false;
                hit.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                return null;
            }
        }

        return null;
    }
}
