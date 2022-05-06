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
    public static int resource3T1 = 0;
    public static int resource2T1 = 0;
    public static int resource1T1 = 0;
    public static int resource3T2 = 0;
    public static int resource2T2 = 0;
    public static int resource1T2 = 0;
    public TextMeshProUGUI r3Text;
    public TextMeshProUGUI r2Text;
    public TextMeshProUGUI r1Text;
    public InputField r1Input;
    public InputField r2Input;
    public InputField r3Input;
    GameObject[] basesInGame;
    public static List<GameObject> goodBases;
    public static List<GameObject> badBases;
    public TMP_Dropdown squadDD;
    public float counter = 0;
    public GameObject winUI;
    public GameObject looseUI;
    public static bool winGame;
    public static bool LooseGame;
    //List<GameObject> agents;
    void Start()
    {
        winGame = false;
        LooseGame = false;
        winUI.SetActive(false);
        looseUI.SetActive(false);
        goodBases = new List<GameObject>();
        badBases = new List<GameObject>();
        spawns = new List<Spawn>();
        agents = Resources.LoadAll<GameObject>("Agents");
        r3Input.text = "Copper";
        r2Input.text = "Metal";
        r1Input.text = "Scraps";
        squadInput.text = "0";
        Invoke("FindBases", 0.2f);
        agentSelectD.onValueChanged.AddListener(delegate
        {

            i = agentSelectD.value;   
            
        });
        for (int i = 0; i < agents.Length; i++)
        {
            agentSelectD.options.Add(new Dropdown.OptionData(agents[i].name + " " + r1Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR1 + " " + r2Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR2 + " " + r3Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR3));

        }
        
        //for (int i = 0; i < basesInGame.Length; i++)
        //{
        //    if (basesInGame[i].GetComponent<HomeBase>().iD == 1)
        //    {
        //        goodBases.Add(basesInGame[i]);

        //    }
        //    else if (basesInGame[i].GetComponent<HomeBase>().iD == 2)
        //    {
        //        badBases.Add(basesInGame[i]);

        //    }
        //}

    }

    // Update is called once per frame
    void Update()
    {
        if (counter <= 5)
        {
            counter += Time.deltaTime;
        }
        //Debug.Log(basesInGame.Length);
        r3Text.text = r3Input.text + " " + resource3T1.ToString();
        r2Text.text = r2Input.text + " " + resource2T1.ToString();
        r1Text.text = r1Input.text + " " + resource1T1.ToString();
        for (int i = 0; i < agentSelectD.options.Count; i++)
        {
            agentSelectD.options[i] = (new Dropdown.OptionData(agents[i].name + " " + r1Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR1 + " " + r2Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR2 + " " + r3Input.text[0] + ":" + agents[i].GetComponent<AgentManager>().priceR3));

        }
        if (Input.GetMouseButtonDown(0))
        {
           
            selectedSpawn = ClickSelect();
        }
       
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go;

            if (selectedSpawn != null && resource1T1 >= agents[i].GetComponent<AgentManager>().priceR1 && resource2T1 >= agents[i].GetComponent<AgentManager>().priceR2 && resource3T1 >= agents[i].GetComponent<AgentManager>().priceR3)
            {
                go = selectedSpawn.GetComponent<Spawn>().SpawnObject(agents[i]);
                go.name = agents[i].name;
                go.GetComponent<AgentManager>().team = 1;
                go.GetComponent<AgentManager>().squad = squadDD.value;
                resource1T1 -= agents[i].GetComponent<AgentManager>().priceR1;
                resource2T1 -= agents[i].GetComponent<AgentManager>().priceR2;
                resource3T1 -= agents[i].GetComponent<AgentManager>().priceR3;


            }
            else
            {
                go = Instantiate(agents[i], mousePos, Quaternion.identity);
                go.name = agents[i].name;
                go.GetComponent<AgentManager>().team = 1;
                go.GetComponent<AgentManager>().squad = squadDD.value;
            }
            
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go = Instantiate(agents[i], mousePos, Quaternion.identity);
            go.name = agents[i].name;
            go.GetComponent<AgentManager>().team = 2;
            go.GetComponent<AgentManager>().squad = squadDD.value;
        }

        //Debug.Log(goodBases.Count);
        if (counter > 1f)
        {
            if (badBases.Count <= 0)
            {
                winGame = true;
                winUI.SetActive(true);
            }
            if (goodBases.Count <= 0)
            {
                LooseGame = true;
                looseUI.SetActive(true);
                //Debug.Log("Perdiste!");
            }
        }
    }

    void FindBases()
    {
        basesInGame = GameObject.FindGameObjectsWithTag("Base");
        Debug.Log(basesInGame.Length);
        for (int i = 0; i < basesInGame.Length; i++)
        {
            if (basesInGame[i].GetComponent<HomeBase>().iD == 1)
            {
                goodBases.Add(basesInGame[i]);

            }
            else if (basesInGame[i].GetComponent<HomeBase>().iD == 2)
            {
                badBases.Add(basesInGame[i]);

            }
        }
        Debug.Log("bases buenas "+goodBases.Count);
        Debug.Log("bases malas "+badBases.Count);
    }
    GameObject ClickSelect()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Spawn spawn;
        foreach(Spawn s in spawns)
        {
            s.SetSelected(false,new Color(1,1,1));
        }
        if (hit != null)
        {
            
            if (hit.gameObject.CompareTag("Spawn"))
            {
                spawn = hit.GetComponent<Spawn>();
                spawn.SetSelected(true, new Color(170f / 255f, 241f / 255f, 158f / 255f));
                spawns.Add(spawn);
                //selected = true;
                //Debug.Log("se eligio: " + hit.name);
                return hit.gameObject;
            }
            
        }

        return null;
    }
}