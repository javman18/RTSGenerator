using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SaveAgent : MonoBehaviour
{
    [SerializeField] AgentManager agentPrefab;
    public static List<AgentManager> agentsP = new List<AgentManager>();
    GameObject[] agents;
    public Scrollbar massScollBar;
    public Text scrollText;
    public Dropdown agentSelectD;
    public Dropdown team2;
    public InputField squadInput;
    public Text agentSelectedT;
    public GameObject agentBasicsUI;
    public GameObject behaviorsUI;
    public GameObject squadUI;
    public Toggle collectorToggle;
    public GameObject collectorStuff;
    public Toggle attackerToggle;
    public GameObject attakerStuff;
    public Toggle shooterToggle;
    public GameObject shooterStuff;
    public InputField fleePerception;
    public InputField pursuePerception;
    public InputField forceField;
    public InputField meeleDamageField;
    public InputField velFied;
    public InputField capCarryField;
    public InputField rateFireField;
    public InputField bulletSpeedField;
    public InputField shootingRangField;
    public InputField hpField;

    public InputField ammoField;
    public InputField damegeField;
    int i = 0;
    private void Start()
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
        for (int i = 0; i < agents.Length; i++)
        {
            agentsP.Add(agents[i].GetComponent<AgentManager>());
        }
        Debug.Log(agentsP.Count);
        fleePerception.text = "0";
        pursuePerception.text = "0";
        forceField.text = "0";
        velFied.text = "0";
        meeleDamageField.text = "0";
        capCarryField.text = "0";
        rateFireField.text = "0";
        bulletSpeedField.text = "0";
        shootingRangField.text = "0";
        hpField.text = "0";
        ammoField.text = "0";
        damegeField.text = "0";
    }
    private void Update()
    {
        if (collectorToggle.isOn)
        {
            collectorStuff.SetActive(true);
        }
        else
        {
            collectorStuff.SetActive(false);
        }
        if (attackerToggle.isOn)
        {
            attakerStuff.SetActive(true);
        }
        else
        {
            attakerStuff.SetActive(false);
        }
        if (shooterToggle.isOn)
        {
            shooterStuff.SetActive(true);
        }
        else
        {
            shooterStuff.SetActive(false);
        }
        
        collectorToggle.isOn = agents[i].GetComponent<AgentManager>().isCollector;
        attackerToggle.isOn = agents[i].GetComponent<AgentManager>().isAtacker;
        shooterToggle.isOn = agents[i].GetComponent<AgentManager>().isShooter;
        SetParametres();
        agentSelectedT.text = "Agent Selected: " + agents[i].name;
        scrollText.text = "Mass "+massScollBar.value;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Load();
        }
        
    }
    public void StartAgentBasics()
    {
        agentBasicsUI.gameObject.SetActive(true);
        behaviorsUI.SetActive(false);
        squadUI.SetActive(false);
    }
    public void StartBehaviorsUI()
    {
        agentBasicsUI.gameObject.SetActive(false);
        behaviorsUI.SetActive(true);
        squadUI.SetActive(false);
    }
    public void StartSquadUI()
    {
        agentBasicsUI.gameObject.SetActive(false);
        behaviorsUI.SetActive(false);
        squadUI.SetActive(true);
    }
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/agent" + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + "/agent.count" + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);
        formatter.Serialize(countStream, agentsP.Count);
        countStream.Close();
        for (int i = 0; i < agentsP.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            AgentData data = new AgentData(agentsP[i]);
            
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    public void Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/agent" + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + "/agent.count" + SceneManager.GetActiveScene().buildIndex;
        int agentCount = 0;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (File.Exists(countPath)){
            FileStream countStream = new FileStream(countPath, FileMode.Open);
            agentCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("No se encontro en " + countPath);
        }
        //for (int i = 0; i < agentCount; i++)
        //{
        if (File.Exists(path + i))
        {
            FileStream stream = new FileStream(path + i, FileMode.Open);
            AgentData data = formatter.Deserialize(stream) as AgentData;

            stream.Close();
            
            AgentManager agent = Instantiate(agentsP[i], mousePos, Quaternion.identity);
            agent.name = agents[i].name;
            agent.isSeek = data.isSeek;
            agent.isFlee = data.isFlee;
            agent.isPursue = data.isPursue;
            agent.isEvade = data.isEvade;
            agent.isArrive = data.isArrive;
            agent.isCohesion = data.isCohesion;
            agent.isCollector = data.isCollector;
            
            agent.resourcesCarryLimt = data.resourcesCarryLimt;
            agent.rank = data.type;
            agent.maxForce = data.maxForce;
            agent.maxVel = data.maxVel;
            agent.mass = data.mass;
            agent.seekPerception = data.seekPerception;
            agent.fleePerception = data.fleePerception;
            
            agent.rank = data.rank;
            agent.team = data.team;
            agent.squad = data.leaderID;
            
            agent.startTimeBtwShots = data.rateOfFire;
            agent.bulletForce = data.bulletSpeed;
            agent.shootingRaange = data.shootingRange;
            agent.ammo = data.ammo;
            agent.bulletDamage = data.damage;

            agent.meleeDamage = data.meeleDamage;

            agent.isShooter = data.isShooter;
            agent.healthAmount = data.healthAmount;
        }
        else
        {
            Debug.LogError("No se encontro en " + path + i);
        }
        //}
    }

    void SetParametres()
    {
        agents[i].GetComponent<AgentManager>().fleePerception = int.Parse(fleePerception.text);
        agents[i].GetComponent<AgentManager>().seekPerception = int.Parse(pursuePerception.text);
        agents[i].GetComponent<AgentManager>().maxForce = float.Parse(forceField.text);
        agents[i].GetComponent<AgentManager>().maxVel = float.Parse(velFied.text);
        agents[i].GetComponent<AgentManager>().resourcesCarryLimt = int.Parse(capCarryField.text);
        agents[i].GetComponent<AgentManager>().meleeDamage = int.Parse(meeleDamageField.text);
        agents[i].GetComponent<AgentManager>().startTimeBtwShots = float.Parse(rateFireField.text);
        agents[i].GetComponent<AgentManager>().bulletForce = float.Parse(bulletSpeedField.text);
        agents[i].GetComponent<AgentManager>().shootingRaange = float.Parse(shootingRangField.text);
        agents[i].GetComponent<AgentManager>().ammo = int.Parse(ammoField.text);
        agents[i].GetComponent<AgentManager>().bulletDamage = float.Parse(damegeField.text);
        agents[i].GetComponent<AgentManager>().mass = massScollBar.value;
        agents[i].GetComponent<AgentManager>().bulletDamage = float.Parse(hpField.text);
    }
}
