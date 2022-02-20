using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum UIStates
    {
        Game,
        AgentEdit,
        Save,
        MainMenu
    }
    public static UIStates currentState;
    public GameObject saveUI;
    public GameObject mainUI;
    public GameObject agentUI;
    public GameObject gameUI;
    public GameObject fogOfWar;
    public Toggle fogT;
    // Start is called before the first frame update
    void Start()
    {

        fogOfWar.gameObject.SetActive(false);
        agentUI.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(true);
        gameUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(mainUI.activeInHierarchy == true)
        {
            currentState = UIStates.MainMenu;
        }else if(saveUI.activeInHierarchy == true)
        {
            currentState = UIStates.Save;
        }
        else if (gameUI.activeInHierarchy == true)
        {
            currentState = UIStates.Game;
        }
        else if (agentUI.activeInHierarchy == true)
        {
            currentState = UIStates.AgentEdit;
        }
    }
    public void ActivateSaveUi()
    {
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(true);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
    }
    public void ActivateAgentUI()
    {
        
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(true);
        gameUI.SetActive(false);
    }
    public void GoBack()
    {
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(true);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
    }
    public void Done()
    {
        if (fogT.isOn)
        {
            fogOfWar.gameObject.SetActive(true);
        }
        
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(true);
    }
}
