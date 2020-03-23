using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneOrganizer : MonoBehaviour {

    public static SceneOrganizer Instance;

    //The Selecta Machine Object
    public GameObject Selecta;
    public GameObject ExplainBoard;
    public GameObject StartButton;
    public Text StartButtonLabel;
    public GameObject Explanation;
    public GameObject FinishButton;
    public GameObject SetupMenu;
    public GameObject Nutriboard;
    public GameObject SelectButton;

    //Dictonary of all the explanations (multilanguage)
    public Dictionary<string, Dictionary<int, string>> Labels = new Dictionary<string, Dictionary<int, string>>();
    public Dictionary<int, string> DE = new Dictionary<int, string>();  
    private Dictionary<int, string> EN = new Dictionary<int, string>();

    public int state = 0;
    private int MAX_STATE = 5;

    public bool BoxesShowing = false;

    public bool testgroup = true;

    private void Awake()
    {
        //make this work like a singlton
        Instance = this;

        //Before we start add all the texts to the dict
        DE.Add(0, "Willkommen!\nAls Einführung: Den Zeiger bewegen Sie mit ihrem Kopf und\nklicken können Sie mit der Fernbedienung in ihrer Hand.\nBitte drücken Sie Start um zu beginnen.");
        DE.Add(1, "Aufgabe 1:\nBitte kaufen Sie nun einen SNACK zum\nEssen ihrer Wahl (kein Getränk).\nMit Klick auf einem Produkt\nerhalten Sie mehr Infos.\nUnd Auswählen können sie mit Select.");
        DE.Add(2, "Aufgabe 2:\nBitte kaufen Sie nun ein GETRÄNK\nihrer Wahl (kein Essen).\nMit Klick auf einem Produkt\nerhalten Sie mehr Infos.\nUnd Auswählen können sie mit Select.");
        DE.Add(3, "Aufgabe 3:\nBitte wählen Sie nun den GESÜNDESTEN SNACK\nim Automat aus, indem Sie ein Produkt auswählen\nund Select drücken.");
        DE.Add(4, "Aufgabe 4:\nBitte wählen Sie nun das GESÜNDESTE GETRÄNK\n im Automat aus, indem Sie ein Produkt auswählen\nund Select drücken.");
        DE.Add(5, "Vielen Dank für die Teilnahme,\nSie können die Brille jetzt abnehmen");
        Labels.Add("de", DE);
        EN.Add(0, "Welcome!\nTo Start: You can move the cursor by tilting your head\nand you can click by pressing the button on the remote control.\nPlease press Start to begin.");
        EN.Add(1, "Task 1:\nPlease buy a SNACK of your choice at\nthe machine (no drink). With a click\non a product you can recieve extra info.\nAnd with clicking Select you choose a product.");
        EN.Add(2, "Task 2:\nPlease buy a DRINK of your choice at\nthe machine (no food). With a click\non a product you can recieve extra info.\nAnd with clicking Select you choose a product.");
        EN.Add(3, "Task 3:\nPlease select the HEALTHIEST SNACK\n in the machine, by clicking on a product\n and clicking Select.");
        EN.Add(4, "Task 4:\nPlease select the HEALTHIEST DRINK\n in the machine, by clicking on a product\n and clicking Select.");
        EN.Add(5, "Thank you!\nYou can now take off the headset.");
        Labels.Add("en", EN);

        ExplainBoard.SetActive(false);
    }

    private void Start()
    {
        //Call Setup Screen
        OnSetup();
    }

    /// <summary>
    /// Handle the State Toggeling
    /// </summary>
    private void TogglState()
    {
        if(state >= MAX_STATE)
        {
            state = 0;
        }
        else
        {
            state++;
        }
    }

    /// <summary>
    /// Handle the Start Button Click Event
    /// </summary>
    public void OnStart()
    {
        //Hide the Explainboard
        ExplainBoard.SetActive(false);
        
        if(state == 0)
        {
            //Put Explainboard into next state
            TogglState();

            //Show first task
            Explanation.GetComponent<TextMesh>().text = Labels[SetupOrganizer.Instance.language][state];
            ExplainBoard.SetActive(true);
        }
        else if (state == MAX_STATE)
        {
            //restart the states
            TogglState();
        }
        else
        {
            if (state == 1)
            {
                //StartCoroutine(ShowBoxAfterTimer(true));
                SelectaOrganizer.Instance.Food.SetActive(true);
            }
            else if (state == 2)
            {
                SelectaOrganizer.Instance.Drink.SetActive(true);
            }
            else if (state == 3)
            {
                SelectaOrganizer.Instance.Food.SetActive(true);
            }
            else if (state == 4)
            {
                SelectaOrganizer.Instance.Drink.SetActive(true);
            }

            //Activate the Select Product Button inside the Nutriboard
            SelectButton.SetActive(true);

            if (SetupOrganizer.Instance.TestGroupToggle.isOn)
            {
                //Show the NutriExplainer
                NutriExplainerOrganizer.Instance.SetNutriExplainerActive(true);
            }
            
            //Allow Clicking on Boxes
            BoxesShowing = true;

            //Put Explainboard into next state
            TogglState();

            //start tracking
            Tracking.Instance.StartTracker();
        }
    }

    private IEnumerator ShowBoxAfterTimer(bool Food)
    {
        yield return new WaitForSeconds(2);

        if (Food)
        {
            SelectaOrganizer.Instance.Food.SetActive(true);
        }
        else
        {
            SelectaOrganizer.Instance.Drink.SetActive(true);
        }
    }

    private IEnumerator SleepBeforeSetup()
    {
        yield return new WaitForSecondsRealtime(10);

        ExplainBoard.SetActive(false);

        //Open the setup Menu
        OnSetup();
    }

    /// <summary>
    /// Handle the Finish Button Click Event
    /// </summary>
    public void OnFinish()
    {
        //start tracking
        Tracking.Instance.StopTracker();

        //Hide the boxes
        SelectaOrganizer.Instance.Food.SetActive(false);
        SelectaOrganizer.Instance.Drink.SetActive(false);
        SelectaOrganizer.Instance.StopHighlight();

        //Hide the Nutriboard
        Nutriboard.SetActive(false);


        //Hide the NutriExplainer;
        NutriExplainerOrganizer.Instance.SetNutriExplainerActive(false);

        //Hide the finish button
        FinishButton.SetActive(false);

        //On the last explainboard hide the start button
        if (state == MAX_STATE)
        {
            StartButton.SetActive(false);
            
            //Set a waiting timer for 30sec for people to take the glasses off
            StartCoroutine(SleepBeforeSetup());
        }

        //Show the Explanation for the next Task
        Explanation.GetComponent<TextMesh>().text = Labels[SetupOrganizer.Instance.language][state];
        ExplainBoard.SetActive(true);
    }

    /// <summary>
    /// Function that calls reset on the server and open the start screen.
    /// </summary>
    public void OnReset()
    {
        //Show the Start Menu
        state = 0;
        Explanation.GetComponent<TextMesh>().text = Labels[SetupOrganizer.Instance.language][state];
        StartButton.SetActive(true);
        ExplainBoard.SetActive(true);
    }

    /// <summary>
    /// Function to Call the Setup menu to adjust the product type and shelf emptiness
    /// </summary>
    public void OnSetup()
    {
        //Activate SpatialMapping & Understanding
        SpatialMappingManager.Instance.StartObserver();
        SpatialMappingManager.Instance.DrawVisualMeshes = true;

        SetupOrganizer.Instance.OrganizeSetupMenu();

        SetupMenu.SetActive(true);
    }

    /// <summary>
    /// Function called once the Setup Menu is closed with the finish button
    /// </summary>
    public void OnSetupFinish()
    {
        //Stop Showing the Spatial Mapping
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
        SpatialMappingManager.Instance.StopObserver();

        //Track all relevant info in the Tracker
        Tracking.Instance.GetMachineLayoutInfo();

        //Close the Setup display
        SetupMenu.SetActive(false);

        //Either Open StartScreen or LoadingScreen
        //Todo make this event based
        OnReset();
    }
}
