﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

	public Text boost;
	public Text health;

    public GameObject menuPanel;
    public GameObject BoundryPanel;
    public Text menuPanelT;

    [SerializeField] private GameObject[] hintPanels = new GameObject[3];
    private int displayHintIndex = 0;


    private bool displayMenu = false;
    private bool displayBoundary = false;
    [SerializeField] private bool displayHints = true;

	// Use this for initialization
	void Start ()
    {
        setMessage(2);
        menu = displayMenu;
        BoundaryWarning = displayBoundary;
        Time.timeScale = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (displayHints)
        {
            Displayhints();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC Pressed");
            menu = !displayMenu;
        }
    }

    private void Displayhints()
    {
        //Debug.Log("display hint");
        if (displayHintIndex < 3)
        {
            Time.timeScale = 0;

            //Debug.Log("index is less then length");
            for (int i = 0; i < 3; i++)
            {
                if (displayHintIndex != i)
                {
                    //Debug.Log("turn panels off");
                    hintPanels[i].SetActive(false);
                }
                else
                {
                    //Debug.Log("activate current panel");
                    hintPanels[displayHintIndex].SetActive(true);
                }
            }
        }
        else
        {
            //Debug.Log("turn off display");
            displayHintIndex = 0; // hard reset
            displayHints = false;
            hintPanels[2].SetActive(false);
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            //Debug.Log("increment index");
            displayHintIndex++;
        }
    }

	public void UpdateShipStats(float b, float h)
	{
		boost.text = "Boost: " + b.ToString ("N0");
		health.text = "Health: " + h.ToString("N0");
	}

    public bool menu
    {
        get
        {
            return displayMenu;
        }
        set
        {
            displayMenu = value;
            menuPanel.SetActive(displayMenu);

            if (displayMenu)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
    public bool BoundaryWarning
    {
        get
        {
            return displayBoundary;
        }
        set
        {
            displayBoundary = value;
            BoundryPanel.SetActive(displayBoundary);
        }
    }

    public void setMessage(int v)
    {
        if (v == 0) menuPanelT.text = "GAME OVER!";
        else if (v == 1) menuPanelT.text = "LEVEL COMPLETE!";
        else if (v == 2) menuPanelT.text = "";
    }


    public void ResetButton()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

}
