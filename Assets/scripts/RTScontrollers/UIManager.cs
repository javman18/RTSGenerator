using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public enum UIStates
    {
        Game,
        AgentEdit,
        Save,
        MainMenu,
        GameMenu
    }
    public static UIStates currentState;
    public GameObject saveUI;
    public GameObject mainUI;
    public GameObject agentUI;
    public GameObject gameUI;
    public GameObject economyUI;
    public GameObject gameMenuUI;
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
        economyUI.SetActive(false);
        gameMenuUI.SetActive(false);
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
        else if (gameMenuUI.activeInHierarchy == true)
        {
            currentState = UIStates.GameMenu;
        }
        if (currentState == UIStates.MainMenu)
        {
            Time.timeScale = 0f;
        }
        else if(currentState == UIStates.Game)
        {
            Time.timeScale = 1f;
        }
        else if (currentState == UIStates.AgentEdit)
        {
            Time.timeScale = 0f;
        }
        else if (currentState == UIStates.Save)
        {
            Time.timeScale = 1f;
        }
        else if (currentState == UIStates.GameMenu)
        {
            Time.timeScale = 1f;
        }
        if (RTSManager.winGame || RTSManager.LooseGame)
        {
            fogOfWar.gameObject.SetActive(false);
            saveUI.SetActive(false);
            mainUI.SetActive(false);
            agentUI.SetActive(false);
            gameUI.SetActive(false);
            economyUI.SetActive(false);
            gameMenuUI.SetActive(false);
        }
    }
    public void ActivateSaveUi()
    {
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(true);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
        economyUI.SetActive(false);
        gameMenuUI.SetActive(false);
    }
    public void ActivateAgentUI()
    {
        
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(true);
        gameUI.SetActive(false);
        economyUI.SetActive(false);
        gameMenuUI.SetActive(false);
    }
    public void GoBack()
    {
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(true);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
        economyUI.SetActive(false);
        gameMenuUI.SetActive(false);
    }
    public void Done()
    {

        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
        economyUI.SetActive(false);
        gameMenuUI.SetActive(true);
        GetComponent<MapGenerator>().InitLevel("Master02");
    }

    public void ActivateEconomyUI()
    {
        fogOfWar.gameObject.SetActive(false);
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(false);
        economyUI.SetActive(true);
        gameMenuUI.SetActive(false);
    }

    public void StartGame()
    {
        if (fogT.isOn)
        {
            fogOfWar.gameObject.SetActive(true);
        }
        saveUI.SetActive(false);
        mainUI.SetActive(false);
        agentUI.SetActive(false);
        gameUI.SetActive(true);
        economyUI.SetActive(false);
        gameMenuUI.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
