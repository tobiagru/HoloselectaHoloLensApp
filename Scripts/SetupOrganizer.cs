using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupOrganizer : MonoBehaviour {

    public static SetupOrganizer Instance;

    public GameObject AdjustmentPane;
    public Dropdown userIdDropdown;
    public Toggle DEToggle;
    public Toggle ENToggle;
    public Toggle TestGroupToggle;
    public Toggle ControlGroupToggle;
    public Toggle NewLayout;
    public Toggle OldLayout;
    public Dropdown BoxDropdown;
    public Dropdown ProductDropdown;
    public Toggle FullToggle;
    public Text QueryButtonText;

    public List<string> BoxDropdownOptions = new List<string>();

    public string language = "en";

    public int userID = 0;

    private bool groupRequestOTW = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        language = "en";
        DEToggle.isOn = false;
        ENToggle.isOn = true;
        OnEN();
        TestGroupToggle.isOn = true;
        ControlGroupToggle.isOn = false;
        OnTestGroup();

        userID = 0;
        userIdDropdown.ClearOptions();
        List<string> userIdDropdownOptions = new List<string>();
        for (int i = 0; i < 300; i++)
        {
            userIdDropdownOptions.Add($"{i}");
        }
        userIdDropdown.AddOptions(userIdDropdownOptions);
        userIdDropdown.value = userID;
        userIdDropdown.RefreshShownValue();

        OldLayout.isOn = true;
        NewLayout.isOn = false;


        //Todo set the right Selections for all DropDowns
        ResetBoxDropdown();
        ProductDropdown.ClearOptions();
        ProductDropdown.AddOptions(SelectaOrganizer.Instance.GetProductKeys());
        ProductDropdown.RefreshShownValue();

        AdjustmentPane.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        AdjustmentPane.GetComponent<Renderer>().material.color = new Color(120, 120, 120, 0.3f);
    }

    public void OrganizeSetupMenu()
    {
        //Check all options have the correct values assigned
        BoxDropdown.value = 0;
        OnChangeBoxDropdown();

        QueryButtonText.color = new Color(0, 0, 0);
    }

    public void ResetBoxDropdown()
    {
        BoxDropdown.ClearOptions();
        BoxDropdown.AddOptions(BoxDropdownOptions);
        BoxDropdown.RefreshShownValue();
    }

    public void OnUserIDChanged()
    {
        userID = userIdDropdown.value;
    }

    public void OnNextUserId()
    {
        if (userIdDropdown.value < userIdDropdown.options.Count)
        {
            userIdDropdown.value += 1;
            userIdDropdown.RefreshShownValue();
            OnUserIDChanged();
        }
    }

    /// <summary>
    /// Function called once the Language Button is toggled to DE
    /// </summary>
    public void OnDE()
    {
        if (DEToggle.isOn)
        {
            language = "de";
            ENToggle.isOn = false;
        }
        else
        {
            language = "en";
            ENToggle.isOn = true;
        }
    }

    /// <summary>
    /// Function called once the Language Button is toggled to EN
    /// </summary>
    public void OnEN()
    {
        if (ENToggle.isOn)
        {
            language = "en";
            DEToggle.isOn = false;
        }
        else
        {
            language = "de";
            DEToggle.isOn = true;
        }
    }

    public void OnQueryGroup()
    {
        if (!groupRequestOTW)
        {
            groupRequestOTW = true;
            StartCoroutine(SendGroupRequest());
        }
    }

    public IEnumerator SendGroupRequest()
    {
        string trackingEndpoint = $"http://35.207.73.131/group/{userID}";

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(trackingEndpoint))
        {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.downloadHandler.text.Contains("test"))
            {
                TestGroupToggle.isOn = true;
                ControlGroupToggle.isOn = false;
                OnTestGroup();
                QueryButtonText.color = new Color(0, 161, 41);
            }
            else if (unityWebRequest.downloadHandler.text.Contains("control"))
            {
                ControlGroupToggle.isOn = true;
                TestGroupToggle.isOn = false;
                OnControlGroup();
                QueryButtonText.color = new Color(0, 161, 41);
            }
            else
            {
                QueryButtonText.color = new Color(255, 0, 0);
            }
        }
        groupRequestOTW = false;
    }

    /// <summary>
    /// Function called once the Group Button is toggled to Control
    /// </summary>
    public void OnControlGroup()
    {
        if (ControlGroupToggle.isOn)
        {
            SelectaOrganizer.Instance.SetColorActive(false);
            NutriboardOrganizer.Instance.ShowNutris(false);
            TestGroupToggle.isOn = false;
        }
        else
        {
            SelectaOrganizer.Instance.SetColorActive(true);
            NutriboardOrganizer.Instance.ShowNutris(true);
            TestGroupToggle.isOn = true;
        }
    }

    /// <summary>
    /// Function called once the Group Button is toggled to Test
    /// </summary>
    public void OnTestGroup()
    {
        if (TestGroupToggle.isOn)
        {
            SelectaOrganizer.Instance.SetColorActive(true);
            NutriboardOrganizer.Instance.ShowNutris(true);
            ControlGroupToggle.isOn = false;
        }
        else
        {
            SelectaOrganizer.Instance.SetColorActive(false);
            NutriboardOrganizer.Instance.ShowNutris(false);
            ControlGroupToggle.isOn = true;
        }
    }

    public void OnOldLayout()
    {
        if (OldLayout.isOn)
        {
            SelectaOrganizer.Instance.ChangeLayout(false);
            ResetBoxDropdown();
            NewLayout.isOn = false;
        }
        else
        {
            SelectaOrganizer.Instance.ChangeLayout(true);
            ResetBoxDropdown();
            NewLayout.isOn = true;
        }
    }

    public void OnNewLayout()
    {
        if (NewLayout.isOn)
        {
            SelectaOrganizer.Instance.ChangeLayout(true);
            ResetBoxDropdown();
            OldLayout.isOn = false;
        }
        else
        {
            SelectaOrganizer.Instance.ChangeLayout(false);
            ResetBoxDropdown();
            OldLayout.isOn = true;
        }
    }

    /// <summary>
    /// Function called once a new value is selected in the Box Nr Dropdown
    /// </summary>
    public void OnChangeBoxDropdown()
    {
        // change the label in the Product Dropdown
        ProductDropdown.value = ProductDropdown.options.FindIndex(s => s.text == SelectaOrganizer.Instance.BoxKeys[BoxDropdown.options[BoxDropdown.value].text]);
        ProductDropdown.RefreshShownValue();

        //change the value of the full toggleS
        FullToggle.isOn = SelectaOrganizer.Instance.Boxes[BoxDropdown.options[BoxDropdown.value].text].activeSelf;
    }

    /// <summary>
    /// Function called once a new value is selected in the Product Type Dropdown
    /// </summary>
    public void OnChangeProductDropdown()
    {
        // change the the Product registered in the Box Slot, 
        SelectaOrganizer.Instance.SetProduct(BoxDropdown.options[BoxDropdown.value].text, ProductDropdown.options[ProductDropdown.value].text);
    }

    /// <summary>
    /// Function called once a new value is selected in the Full Toggle
    /// </summary>
    public void OnToggleFull()
    {
        //Todo change the Colors of the boxes based on the position of the button
        SelectaOrganizer.Instance.SetProductFull(BoxDropdown.options[BoxDropdown.value].text, FullToggle.isOn);
    }

}
