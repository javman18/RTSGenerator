using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveAgent : MonoBehaviour
{
    [SerializeField] AgentManager agentPrefab;
    public static List<AgentManager> agents = new List<AgentManager>();

    private void Update()
    {
        
    }
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/agent" + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + "/agent.count" + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);
        formatter.Serialize(countStream, agents.Count);
        countStream.Close();
        for (int i = 0; i < agents.Count; i++)
        {
            FileStream stream = new FileStream(path+i, FileMode.Create);
            AgentData data = new AgentData(agents[i]);

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
        if (File.Exists(countPath)){
            FileStream countStream = new FileStream(countPath, FileMode.Open);
            agentCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("No se encontro en " + countPath);
        }
        for (int i = 0; i < agentCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                AgentData data = formatter.Deserialize(stream) as AgentData;

                stream.Close();

                Vector2 position = new Vector2(data.position[0], data.position[1]);
                AgentManager agent = Instantiate(agentPrefab, position, Quaternion.identity);
                agent.isSeek = data.isSeek;
                agent.isFlee = data.isFlee;
                agent.isPursue = data.isPursue;
                agent.isEvade = data.isEvade;
                agent.isArrive = data.isArrive;
                agent.isCohesion = data.isCohesion;
                agent.isCollector = data.isCollector;
                //agent.copperInv = data.reourcesInv;
                agent.resourcesCarryLimt = data.resourcesCarryLimt;
                agent.rank = data.type;
                agent.maxForce = data.maxForce;
                agent.maxVel = data.maxVel;
                agent.mass = data.mass;
                agent.seekPerception = data.seekPerception;
                agent.fleePerception = data.fleePerception;
                agent.ePerception = data.ePerception;
                
                agent.viewAngle = data.viewAngle;
                //agent.separationDistance = data.separationDistance;
                agent.decayCoefficient = data.decayCoefficient;
                agent.alignDist = data.alignDist;
                agent.isLeader = data.isLeader;
                agent.hasLeader = data.hasLeader;
                agent.rank = data.rank;
                agent.team = data.team;
                agent.squad = data.leaderID;
                agent.alignmentWeight = data.alignmentWeight;
                agent.separationWeigh = data.separationWeigh;
                agent.cohesionWeight = data.cohesionWeight;
            }
            else
            {
                Debug.LogError("No se encontro en " + path + i);
            }
        }
    }
}
