using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SelectaOrganizer : MonoBehaviour {

    public static SelectaOrganizer Instance;

    public bool isNewLayout = false;
    public GameObject LayoutOld;
    public GameObject LayoutNew;

    public GameObject Food;
    public GameObject Drink;

    //public List<string> BoxValues = new List<string>();
    public Dictionary<string, string> BoxKeys = new Dictionary<string, string>();
    public Dictionary<string, GameObject> Boxes = new Dictionary<string, GameObject>();
    public Dictionary<string, TextMesh> BoxNutriLabels = new Dictionary<string, TextMesh>();
    public Dictionary<string, ProductDefinition> ProductNutris = new Dictionary<string, ProductDefinition>();

    public bool HasColor = true;

    bool highlight = false;
    public List<GameObject> HighlightedBoxes = new List<GameObject>();
    int currentWiggleStep = 0;
    bool positveWiggleDirection = true;
    int maxWiggleStep = 4;

    private void Awake()
    {

        Instance = this;
        //gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        //gameObject.GetComponent<Renderer>().material.color = new Color(0,0,0,0);

        Dictionary<string, string> InitialBoxValues = SelectaObjects.SetInitialBoxValues(isNewLayout);

        // Initialize the product nutri dict
        ProductNutris = SelectaObjects.SetProductNutrisDict();

        isNewLayout = false;
        ChangeLayout(isNewLayout);
        
        // Set the color for all Products
        SetColorActive(HasColor);
    }

    private void Update()
    {
        if (highlight)
        {
            Wiggle();
        }
    }

    public void Wiggle()
    {
        if (positveWiggleDirection)
        {
            currentWiggleStep += 1;
        }
        else
        {
            currentWiggleStep -= 1;
        }
        if (currentWiggleStep >= maxWiggleStep)
        {
            positveWiggleDirection = false;
        }
        else if (currentWiggleStep <= -1 * maxWiggleStep)
        {
            positveWiggleDirection = true;
        }
        foreach (GameObject Box in HighlightedBoxes)
        {
            Vector3 centerWorld = Box.transform.TransformPoint(Box.GetComponent<BoxCollider>().center);
            if (positveWiggleDirection)
            {
                Box.transform.RotateAround(centerWorld,
                                           Box.transform.forward,
                                           1f);
            }
            else
            {
                Box.transform.RotateAround(centerWorld,
                                           Box.transform.forward,
                                           -1f);
            }
        }
    }

    public void ChangeLayout(bool newLayout)
    {
        //Move all boxes currently in Boxes back into the corresponding Layout Gameobject
        foreach (GameObject Box in Boxes.Values)
        {
            if (isNewLayout)
            {
                Box.transform.SetParent(LayoutNew.transform);
            }
            else
            {
                Box.transform.SetParent(LayoutOld.transform);
            }
        }

        isNewLayout = newLayout;

        //Fill Boxes with the Boxes from the changed Layout
        Dictionary<string, string> InitialBoxValues = SelectaObjects.SetInitialBoxValues(newLayout);
        Boxes = new Dictionary<string, GameObject>();
        BoxNutriLabels = new Dictionary<string, TextMesh>();
        BoxKeys = new Dictionary<string, string>();
        //BoxValues = new List<string>();
        SetupOrganizer.Instance.BoxDropdownOptions = new List<string>();
        for (int x = 1; x <= 6; x++)
        {
            for (int y = 0; y <= 9; y++)
            {
                Transform Tmp;
                if (newLayout)
                {
                    Tmp = LayoutNew.transform.Find($"{x}{y}");
                }
                else
                {
                    Tmp = LayoutOld.transform.Find($"{x}{y}");
                }
                
                if (Tmp != null)
                {
                    //Put them in their respective Lists
                    //BoxValues.Add(InitialBoxValues[$"{x}{y}"]);
                    BoxKeys.Add($"{x}{y}", InitialBoxValues[$"{x}{y}"]);
                    SetupOrganizer.Instance.BoxDropdownOptions.Add($"{x}{y}");
                    Boxes.Add($"{x}{y}", Tmp.gameObject);
                    BoxNutriLabels.Add($"{x}{y}", Tmp.transform.Find("Nutrilabel").GetComponent<TextMesh>());
                }
            }
        }

        ResetBoxValues();
    }

    

    

    public void ResetBoxValues()
    {
        Dictionary<string, string> InitialBoxValues = SelectaObjects.SetInitialBoxValues(isNewLayout);

        foreach(string boxKey in Boxes.Keys)
        {
            SetProduct(boxKey, InitialBoxValues[boxKey]);
        }
    }

    /// <summary>
    /// Based on the keys in the ProductNutris the values of the dropdown is set for the Setup Menu
    /// </summary>
    public void SetProduct(string BoxKey, string ProductKey)
    {
        // change the value of the Box
        //BoxValues[BoxId] = ProductKey;
        BoxKeys[BoxKey] = ProductKey;

        // if boxes are colored Change the Color of the box
        if (HasColor)
        {
            Boxes[BoxKey].transform.Find("Corner").GetComponent<MeshRenderer>().material.color = GetNutriColor(ProductNutris[ProductKey].nutri_label);
            BoxNutriLabels[BoxKey].text = ProductNutris[ProductKey].nutri_label;
        }
        else
        {
            Boxes[BoxKey].transform.Find("Corner").GetComponent<MeshRenderer>().material.color = new Color(120, 120, 120);
            BoxNutriLabels[BoxKey].text = "";
        }

        // if the box changes drink <-> food type make it child of the correct game object
        if (Boxes[BoxKey].transform.parent.name != ProductNutris[ProductKey].type)
        {
            if (ProductNutris[ProductKey].type == "Food")
            {
                Boxes[BoxKey].transform.SetParent(Food.transform);
                Boxes[BoxKey].SetActive(true);
            }
            else if (ProductNutris[ProductKey].type == "Drink")
            {
                Boxes[BoxKey].transform.SetParent(Drink.transform);
                Boxes[BoxKey].SetActive(true);
            }
            else
            {
                Boxes[BoxKey].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Function the Set the Product Level of a Box as full or empty
    /// </summary>
    public void SetProductFull(string BoxKey, bool Full)
    {
        if (Full)
        {
            Boxes[BoxKey].SetActive(true);
        }
        else
        {
            Boxes[BoxKey].SetActive(false);
        }
    }

    /// <summary>
    /// Get the List of all product options as a string list
    /// </summary>
    public List<string> GetProductKeys()
    {
        return new List<string>(ProductNutris.Keys);
    }

    /// <summary>
    /// Allows Setting Color of Boxes to Nutriscore of Grey
    /// </summary>
    public void SetColorActive(bool UseColor)
    {
        HasColor = UseColor;

        foreach (string BoxKey in Boxes.Keys)
        {
            SetProduct(BoxKey, BoxKeys[BoxKey]);
        }
    }
        

    /// <summary>
    /// get the corresponding color on the nutrilabel for a nutriletter
    /// </summary>
    public Color GetNutriColor(string nutriLetter)
    {
        switch (nutriLetter)
        {
            case "A":
                return new Color(3.0f / 255, 129.0f / 255, 65.0f / 255, 0.8f);
            case "B":
                return new Color(133.0f / 255, 187.0f / 255, 47.0f / 255, 0.8f);
            case "C":
                return new Color(254.0f / 255, 203.0f / 255, 2.0f / 255, 0.8f);
            case "D":
                return new Color(238.0f / 255, 129.0f / 255, 0.0f / 255, 0.8f);
            case "E":
                return new Color(230.0f / 255, 62.0f / 255, 17.0f / 255, 0.8f);
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Checks if the BoxNr is the healthiest option in view
    /// If not it highlights the healthier options and returns false
    /// else returns true
    /// </summary>
    public bool IsHealthiest(string BoxNr)
    {
        string productId = BoxKeys[BoxNr];
        List<string> healthyOptions = GetHealthyAlternative(productId);
        if (!healthyOptions.Contains(productId))
        {
            StartHighlight(healthyOptions);
            return false;
        }
        else
        {
            return true;
        }
    }

    private List<string> GetHealthyAlternative(string ProductId)
    {
        List<string> options;
        bool isdrink = ProductNutris[ProductId].type == "Drink";

        if (isdrink)
        {
            options = new List<string>{"evian___50__3068320353500",
                                   "henniez_blau__33__7610235000442",
                                   "valser_classic__50__76404160",
                                   "valser_still__50__7610335002575",
                                   "coke_zero_flasche_50__5449000131836",
                                   "comella_schokodrink__33__7613100037253",
                                   "pepsi_max__50__4060800105943",
                                   "redbull_light__33__90162800"};
        }
        else
        {
            options = new List<string>{"airwaves_menthoneucalyptus_riegel_14_1_50173167",
                                    "stimorol_spearmint_riegel_14_1_0000000000005",
                                    "stimorol_wildcherry_riegel_14_1_57060330",
                                    "zweifel_paprika__90__7610095013002",
                                    "berger_zitronetoertli__50__7610404511007",
                                    "powerbar_proteinplusschoko__55__4029679520028",
                                    "twix___50__5000159459228"};
        }
        return options;
    }

    private void StartHighlight(List<string> options)
    {
        foreach(string ProductId in options)
        {
            string BoxKey = BoxKeys.FirstOrDefault(s => s.Value == ProductId).Key;
            if (!string.IsNullOrEmpty(BoxKey))
            {
                HighlightedBoxes.Add(Boxes[BoxKey]);
                Boxes[BoxKey].transform.Find("Frame").GetComponent<MeshRenderer>().material.color = new Color(23.0f / 255, 52.0f / 255, 244.0f / 255, 0.8f);
            }
        }
        highlight = true;
    }

    public void StopHighlight()
    {
        highlight = false;
        foreach (GameObject Box in HighlightedBoxes)
        {
            Vector3 centerWorld = Box.transform.TransformPoint(Box.GetComponent<BoxCollider>().center);
            Box.transform.RotateAround(centerWorld,
                                       Box.transform.forward,
                                       -1 * Box.transform.rotation.eulerAngles.z);
            Box.transform.Find("Frame").GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255);
        }
        HighlightedBoxes.Clear();
        currentWiggleStep = 0;
        positveWiggleDirection = true;
    }
}


