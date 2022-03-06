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
    public InputField teamField;
    public InputField ammoField;
    public InputField damegeField;
    public InputField squadField;
    public InputField rankField;
    public Toggle fleeToggle;
    public Toggle seekToggle;
    public Toggle arriveToggle;
    public Toggle pursueToggle;
    public Toggle titanToggle;
    int i = 0;
    private void Start()
    {
        agents = Resources.LoadAll<GameObject>("Agents");
        fleePerception.text = agents[i].GetComponent<AgentManager>().fleePerception.ToString();
        pursuePerception.text = agents[i].GetComponent<AgentManager>().seekPerception.ToString();
        forceField.text = agents[i].GetComponent<AgentManager>().maxForce.ToString();
        teamField.text = agents[i].GetComponent<AgentManager>().team.ToString();
        squadField.text = agents[i].GetComponent<AgentManager>().squad.ToString();
        rankField.text = agents[i].GetComponent<AgentManager>().rank.ToString();
        //forceField.text = "0";
        velFied.text = agents[i].GetComponent<AgentManager>().maxVel.ToString();
        massScollBar.value = agents[i].GetComponent<AgentManager>().mass;
        meeleDamageField.text = agents[i].GetComponent<AgentManager>().meleeDamage.ToString();
        capCarryField.text = agents[i].GetComponent<AgentManager>().resourcesCarryLimt.ToString();
        rateFireField.text = agents[i].GetComponent<AgentManager>().startTimeBtwShots.ToString();
        bulletSpeedField.text = agents[i].GetComponent<AgentManager>().bulletForce.ToString();
        shootingRangField.text = agents[i].GetComponent<AgentManager>().shootingRaange.ToString();
        hpField.text = agents[i].GetComponent<AgentManager>().healthAmount.ToString();
        ammoField.text = agents[i].GetComponent<AgentManager>().ammo.ToString();
        damegeField.text = agents[i].GetComponent<AgentManager>().bulletDamage.ToString();
        collectorToggle.isOn = agents[i].GetComponent<AgentManager>().isCollector;
        attackerToggle.isOn = agents[i].GetComponent<AgentManager>().isAtacker;
        shooterToggle.isOn = agents[i].GetComponent<AgentManager>().isShooter;
        pursueToggle.isOn = agents[i].GetComponent<AgentManager>().isPursue;
        fleeToggle.isOn = agents[i].GetComponent<AgentManager>().isFlee;
        seekToggle.isOn = agents[i].GetComponent<AgentManager>().isSeek;
        arriveToggle.isOn = agents[i].GetComponent<AgentManager>().isArrive;
        titanToggle.isOn = agents[i].GetComponent<AgentManager>().isTitan;
        for (int i = 0; i < agents.Length; i++)
        {
           
                agentSelectD.options.Add(new Dropdown.OptionData(agents[i].name));
            

        }
        agentSelectD.onValueChanged.AddListener(delegate
        {

            i = agentSelectD.value;
            Debug.Log(i);
            fleePerception.text = agents[i].GetComponent<AgentManager>().fleePerception.ToString();
            pursuePerception.text = agents[i].GetComponent<AgentManager>().seekPerception.ToString();
            forceField.text = agents[i].GetComponent<AgentManager>().maxForce.ToString();
            teamField.text = agents[i].GetComponent<AgentManager>().team.ToString();
            //forceField.text = "0";
            squadField.text = agents[i].GetComponent<AgentManager>().squad.ToString();
            rankField.text = agents[i].GetComponent<AgentManager>().rank.ToString();
            velFied.text = agents[i].GetComponent<AgentManager>().maxVel.ToString();
            massScollBar.value = agents[i].GetComponent<AgentManager>().mass;
            meeleDamageField.text = agents[i].GetComponent<AgentManager>().meleeDamage.ToString();
            capCarryField.text = agents[i].GetComponent<AgentManager>().resourcesCarryLimt.ToString();
            rateFireField.text = agents[i].GetComponent<AgentManager>().startTimeBtwShots.ToString();
            bulletSpeedField.text = agents[i].GetComponent<AgentManager>().bulletForce.ToString();
            shootingRangField.text = agents[i].GetComponent<AgentManager>().shootingRaange.ToString();
            hpField.text = agents[i].GetComponent<AgentManager>().healthAmount.ToString();
            ammoField.text = agents[i].GetComponent<AgentManager>().ammo.ToString();
            damegeField.text = agents[i].GetComponent<AgentManager>().bulletDamage.ToString();
            collectorToggle.isOn = agents[i].GetComponent<AgentManager>().isCollector;
            attackerToggle.isOn = agents[i].GetComponent<AgentManager>().isAtacker;
            shooterToggle.isOn = agents[i].GetComponent<AgentManager>().isShooter;
            pursueToggle.isOn = agents[i].GetComponent<AgentManager>().isPursue;
            fleeToggle.isOn = agents[i].GetComponent<AgentManager>().isFlee;
            seekToggle.isOn = agents[i].GetComponent<AgentManager>().isSeek;
            arriveToggle.isOn = agents[i].GetComponent<AgentManager>().isArrive;
            titanToggle.isOn = agents[i].GetComponent<AgentManager>().isTitan;

        });
        for (int i = 0; i < agents.Length; i++)
        {
            agentsP.Add(agents[i].GetComponent<AgentManager>());
        }

        
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
        
        
        SetParametres();
        agentSelectedT.text = "Agent Selected: " + agents[i].name;
        scrollText.text = "Mass "+massScollBar.value;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Load(mousePos);
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

    public void Load(Vector3 position)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/agent" + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + "/agent.count" + SceneManager.GetActiveScene().buildIndex;
        int agentCount = 0;
       
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
            
            AgentManager agent = Instantiate(agentsP[i], position, Quaternion.identity);
            agent.name = agents[i].name;
            agent.isSeek = data.isSeek;
            agent.isFlee = data.isFlee;
            agent.isPursue = data.isPursue;
            agent.isEvade = data.isEvade;
            agent.isArrive = data.isArrive;
            agent.isAtacker = data.isAttacker;
            agent.isCollector = data.isCollector;
            agent.isTitan = data.isTitan;
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
        agents[i].GetComponent<AgentManager>().team = int.Parse(teamField.text);
        agents[i].GetComponent<AgentManager>().rank = int.Parse(rankField.text);
        agents[i].GetComponent<AgentManager>().squad = int.Parse(squadField.text);
        agents[i].GetComponent<AgentManager>().isCollector = collectorToggle.isOn;
        agents[i].GetComponent<AgentManager>().isAtacker = attackerToggle.isOn;
        agents[i].GetComponent<AgentManager>().isShooter = shooterToggle.isOn;
        agents[i].GetComponent<AgentManager>().isPursue = pursueToggle.isOn;
        agents[i].GetComponent<AgentManager>().isFlee = fleeToggle.isOn;
        agents[i].GetComponent<AgentManager>().isSeek = seekToggle.isOn;
        agents[i].GetComponent<AgentManager>().isArrive = arriveToggle.isOn;
        agents[i].GetComponent<AgentManager>().isTitan = titanToggle.isOn;
    }
}
