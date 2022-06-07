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
    public TMP_Dropdown agentSelectD;
    public Dropdown team2;
    public InputField squadInput;
    int i=0;
    int j = 0;
    bool selected = false;
    public static List<Spawn> goodSpawns;
    public static List<Spawn> badSpawns;
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
    Spawn[] spawns;
    public TMP_Dropdown squadDD;
    public float counter = 0;
    public GameObject winUI;
    public GameObject looseUI;
    public static bool winGame;
    public static bool LooseGame;
    bool isDone;
    public int armyLimit = 20;
    public static int spawnedSoldier = 0;
    public TextMeshProUGUI armyText;
    public static List<AgentManager> enemyLeaders;
    public static List<AgentManager> minions;
    public static List<AgentManager> hunters;
    public static List<AgentManager> tanks;
    public static List<AgentManager> attackers;
    //List<GameObject> agents;
    void Start()
    {
        resource1T1 = 30;
        resource2T1 = 20;
        resource3T1 = 50;
        winGame = false;
        LooseGame = false;
        winUI.SetActive(false);
        looseUI.SetActive(false);
        goodBases = new List<GameObject>();
        badBases = new List<GameObject>();
        goodSpawns = new List<Spawn>();
        badSpawns = new List<Spawn>();
        enemyLeaders = new List<AgentManager>();
        agents = Resources.LoadAll<GameObject>("Agents");
        r3Input.text = "Copper";
        r2Input.text = "Metal";
        r1Input.text = "Scraps";
        squadInput.text = "0";
        minions = new List<AgentManager>();
        hunters = new List<AgentManager>();
        tanks = new List<AgentManager>();
        attackers = new List<AgentManager>();
        //Invoke("FindBases", 0.2f);
        agentSelectD.onValueChanged.AddListener(delegate
        {

            i = agentSelectD.value;

        });
        for (int i = 0; i < agents.Length; i++)
        {
            agentSelectD.options.Add(new TMP_Dropdown.OptionData(agents[i].name + " " + r1Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR1 + " " + r2Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR2 + " " + r3Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR3));

        }
        //agentSelectD.transform.GetChild(0).GetComponent<TextMeshPro>().text = agents[0].name;
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
    void LateUpdate()
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
            agentSelectD.options[i] = (new TMP_Dropdown.OptionData(agents[i].name + "    " + r1Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR1 + " " + r2Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR2 + " " + r3Input.text + ":" + agents[i].GetComponent<AgentManager>().priceR3));

        }
        if (Input.GetMouseButtonDown(0))
        {
           
            selectedSpawn = ClickSelect();
        }
        armyText.text = spawnedSoldier + " / " + armyLimit + " Soldiers";
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject go;
            if (spawnedSoldier < armyLimit)
            {
                if (selectedSpawn != null && resource1T1 >= agents[i].GetComponent<AgentManager>().priceR1 && resource2T1 >= agents[i].GetComponent<AgentManager>().priceR2 && resource3T1 >= agents[i].GetComponent<AgentManager>().priceR3)
                {
                    go = selectedSpawn.GetComponent<Spawn>().SpawnObject(agents[i]);
                    go.name = agents[i].name;
                    go.GetComponent<AgentManager>().team = 1;
                    //set army squad
                    if(go.GetComponent<AgentManager>().rank > 0 || (go.GetComponent<AgentManager>().isAtacker && go.GetComponent<AgentManager>().isShooter))
                    {
                        go.GetComponent<AgentManager>().squad = 1;
                    }
                   
                    resource1T1 -= agents[i].GetComponent<AgentManager>().priceR1;
                    resource2T1 -= agents[i].GetComponent<AgentManager>().priceR2;
                    resource3T1 -= agents[i].GetComponent<AgentManager>().priceR3;
                    spawnedSoldier++;


                }
                else
                {
                    go = Instantiate(agents[i], mousePos, Quaternion.identity);
                    go.name = agents[i].name;
                    go.GetComponent<AgentManager>().team = 1;
                    go.GetComponent<AgentManager>().squad = squadDD.value;
                    spawnedSoldier++;
                }
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
        FindBases();
        //Debug.Log(goodBases.Count);
        if (UIManager.currentState == UIManager.UIStates.Game)
        {
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
            SpawnEnemies();
            Debug.Log(enemyLeaders.Count);
        }
       
        
    }

    void FindBases()
    {
        if (UIManager.currentState == UIManager.UIStates.Game)
        {
            if (!isDone)
            {
                basesInGame = GameObject.FindGameObjectsWithTag("Base");
                spawns = GameObject.FindObjectsOfType<Spawn>();
                Debug.Log(spawns.Length);
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
                for (int i = 0; i < spawns.Length; i++)
                {
                    if (spawns[i].GetComponent<Spawn>().iD == 2)
                    {
                        badSpawns.Add(spawns[i]);

                    }
                    //else if (basesInGame[i].GetComponent<HomeBase>().iD == 2)
                    //{
                    //    badBases.Add(basesInGame[i]);

                    //}
                }
                Debug.Log("bases buenas " + goodBases.Count);
                Debug.Log("spawns malos " + badSpawns.Count);
                isDone = true;
            }
        }
    }
    GameObject ClickSelect()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Spawn spawn;
        foreach(Spawn s in goodSpawns)
        {
            s.SetSelected(false,new Color(1,1,1));
        }
        if (hit != null)
        {
            
            if (hit.gameObject.CompareTag("Spawn"))
            {
                if (hit.gameObject.GetComponent<Spawn>().iD == 1)
                {
                    spawn = hit.GetComponent<Spawn>();
                    spawn.SetSelected(true, new Color(170f / 255f, 241f / 255f, 158f / 255f));
                    goodSpawns.Add(spawn);
                    //selected = true;
                    //Debug.Log("se eligio: " + hit.name);
                    return hit.gameObject;
                }
            }
            
        }

        return null;
    }

    void SpawnEnemies()
    {
        if (enemyLeaders.Count == 0)
        {
            GameObject leader = Instantiate(agents[0], badSpawns[1].transform.position, Quaternion.identity);
            leader.GetComponent<AgentManager>().team = 2;
            leader.GetComponent<AgentManager>().squad = 1;
        }
        if (minions.Count <= 6)
        {
            GameObject soldier = Instantiate(agents[5], badSpawns[1].transform.position, Quaternion.identity);
            soldier.GetComponent<AgentManager>().team = 2;
            soldier.GetComponent<AgentManager>().squad = 1;
        }
        if (tanks.Count == 0)
        {
            GameObject tank = Instantiate(agents[4], badSpawns[2].transform.position, Quaternion.identity);
            tank.GetComponent<AgentManager>().team = 2;
            tank.GetComponent<AgentManager>().squad = 1;
        }
        if (hunters.Count <= 2)
        {
            GameObject hunter = Instantiate(agents[1], badSpawns[0].transform.position, Quaternion.identity);
            hunter.GetComponent<AgentManager>().team = 2;
            hunter.GetComponent<AgentManager>().squad = 0;
        }
        if (attackers.Count <= 2)
        {
            GameObject attacker = Instantiate(agents[3], badSpawns[3].transform.position, Quaternion.identity);
            attacker.GetComponent<AgentManager>().team = 2;
            attacker.GetComponent<AgentManager>().squad = 0;
        }
    }
}