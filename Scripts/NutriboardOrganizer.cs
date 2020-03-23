using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class NutriboardOrganizer : MonoBehaviour {

    public static NutriboardOrganizer Instance;

    public GameObject Test;
    public GameObject Control;
    private bool testgroup;

    public GameObject TestBoard2;
    private float testBoard2_y_closed = -0.162f;
    private float testBoard2_y_open = -0.244f;
    public GameObject TestBoard3;

    private Material Calories_Ring;
    private TextMesh Calories_Value;
    private TextMesh Calories_Label;
    private TextMesh Calories_Explain;
    private Material Sugar_Ring;
    private TextMesh Sugar_Value;
    private TextMesh Sugar_Label;
    private TextMesh Sugar_Explain;
    private Material Fat_Ring;
    private TextMesh Fat_Value;
    private TextMesh Fat_Label;
    private TextMesh Fat_Explain;
    private Material Natrium_Ring;
    private TextMesh Natrium_Value;
    private TextMesh Natrium_Label;
    private TextMesh Natrium_Explain;
    private Material Protein_Ring;
    private TextMesh Protein_Value;
    private TextMesh Protein_Label;
    private TextMesh Protein_Explain;
    private Material Fiber_Ring;
    private TextMesh Fiber_Value;
    private TextMesh Fiber_Label;
    private TextMesh Fiber_Explain;
    private Material HealthPercentage_Ring;
    private TextMesh HealthPercentage_Value;
    private TextMesh HealthPercentage_Label;
    private TextMesh HealthPercentage_Explain;
    private TextMesh ProductName;
    private TextMesh WarningLabel;

    private TextMesh WeightLabel;
    private TextMesh Weight;
    private TextMesh ProductName2;
    private TextMesh GTIN;

    public string CurrentBoxNr = "10";

    private Dictionary<string, string> CaloriesLabels = new Dictionary<string, string>();
    private Dictionary<string, string> SugarLabels = new Dictionary<string, string>();
    private Dictionary<string, string> FatLabels = new Dictionary<string, string>();
    private Dictionary<string, string> NatriumLabels = new Dictionary<string, string>();
    private Dictionary<string, string> ProteinLabels = new Dictionary<string, string>();
    private Dictionary<string, string> FiberLabels = new Dictionary<string, string>();
    private Dictionary<string, string> HealthPercentageLabels = new Dictionary<string, string>();

    private Dictionary<string, string> WarningLabels = new Dictionary<string, string>();

    private Dictionary<string, string> WeightLabels = new Dictionary<string, string>();

    private Dictionary<string, Dictionary<string, string>> PlusLabels = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, Dictionary<string, string>> MinusLabels = new Dictionary<string, Dictionary<string, string>>();

    private string language;

    private void Awake () {
        Instance = this;

        gameObject.SetActive(false);

        Transform Calories = gameObject.transform.Find("Test/Board/Calories");
        Calories_Ring = Calories.Find("default").GetComponent<Renderer>().material;
        Calories_Value = Calories.Find("Value").GetComponent<TextMesh>();
        Calories_Label = Calories.Find("Name").GetComponent<TextMesh>();
        Calories_Explain = Calories.Find("Explain").GetComponent<TextMesh>();
        Transform Sugar = gameObject.transform.Find("Test/Board/Sugar");
        Sugar_Ring = Sugar.Find("default").GetComponent<Renderer>().material;
        Sugar_Value = Sugar.Find("Value").GetComponent<TextMesh>();
        Sugar_Label = Sugar.Find("Name").GetComponent<TextMesh>();
        Sugar_Explain = Sugar.Find("Explain").GetComponent<TextMesh>();
        Transform Fat = gameObject.transform.Find("Test/Board/Fat");
        Fat_Ring = Fat.Find("default").GetComponent<Renderer>().material;
        Fat_Value = Fat.Find("Value").GetComponent<TextMesh>();
        Fat_Label = Fat.Find("Name").GetComponent<TextMesh>();
        Fat_Explain = Fat.Find("Explain").GetComponent<TextMesh>();
        Transform Natrium = gameObject.transform.Find("Test/Board/Natrium");
        Natrium_Ring = Natrium.Find("default").GetComponent<Renderer>().material;
        Natrium_Value = Natrium.Find("Value").GetComponent<TextMesh>();
        Natrium_Label = Natrium.Find("Name").GetComponent<TextMesh>();
        Natrium_Explain = Natrium.Find("Explain").GetComponent<TextMesh>();
        Transform Protein = gameObject.transform.Find("Test/Board/Protein");
        Protein_Ring = Protein.Find("default").GetComponent<Renderer>().material;
        Protein_Value = Protein.Find("Value").GetComponent<TextMesh>();
        Protein_Label = Protein.Find("Name").GetComponent<TextMesh>();
        Protein_Explain = Protein.Find("Explain").GetComponent<TextMesh>();
        Transform Fiber = gameObject.transform.Find("Test/Board/Fiber");
        Fiber_Ring = Fiber.Find("default").GetComponent<Renderer>().material;
        Fiber_Value = Fiber.Find("Value").GetComponent<TextMesh>();
        Fiber_Label = Fiber.Find("Name").GetComponent<TextMesh>();
        Fiber_Explain = Fiber.Find("Explain").GetComponent<TextMesh>();
        Transform HealthPercentage = gameObject.transform.Find("Test/Board/HealthPercentage");
        HealthPercentage_Ring = HealthPercentage.Find("default").GetComponent<Renderer>().material;
        HealthPercentage_Value = HealthPercentage.Find("Value").GetComponent<TextMesh>();
        HealthPercentage_Label = HealthPercentage.Find("Name").GetComponent<TextMesh>();
        HealthPercentage_Explain = HealthPercentage.Find("Explain").GetComponent<TextMesh>();
        ProductName = gameObject.transform.Find("Test/Board2/ProductName").gameObject.GetComponent<TextMesh>();
        WarningLabel = gameObject.transform.Find("Test/Board3/Warning").gameObject.GetComponent<TextMesh>();

        WeightLabel = gameObject.transform.Find("Control/WeightLabel").GetComponent<TextMesh>();
        Weight = gameObject.transform.Find("Control/Weight").GetComponent<TextMesh>();
        ProductName2 = gameObject.transform.Find("Control/Product").GetComponent<TextMesh>();
        GTIN = gameObject.transform.Find("Control/GTIN").GetComponent<TextMesh>();

        CaloriesLabels.Add("de", "Kalorien");
        CaloriesLabels.Add("en", "Calories");
        SugarLabels.Add("de", "Zucker");
        SugarLabels.Add("en", "Sugar");
        FatLabels.Add("de", "Ges. Fett");
        FatLabels.Add("en", "Sat. Fat");
        NatriumLabels.Add("de", "Natrium");
        NatriumLabels.Add("en", "Sodium");
        ProteinLabels.Add("de", "Protein");
        ProteinLabels.Add("en", "Protein");
        FiberLabels.Add("de", "Ballastst.");
        FiberLabels.Add("en", "Fiber");
        HealthPercentageLabels.Add("de", "Nuss/Veg.");
        HealthPercentageLabels.Add("en", "Nuts/Veg.");

        WarningLabels.Add("en", "Are you sure you want to buy this product?\nThere are healthier options available. They are now highlighted.");
        WarningLabels.Add("de", "Bist du dir sicher das du diese Produkt willst?\nEs gibt gesündere Optionen. Diese sind jetzt hervorgehoben.");

        WeightLabels.Add("de", "Gewicht");
        WeightLabels.Add("en", "Weight");

        Dictionary<string, string> PlusLabelsEN = new Dictionary<string, string>();
        PlusLabelsEN.Add("A","Low");
        PlusLabelsEN.Add("B","Low");
        PlusLabelsEN.Add("C","Neutral");
        PlusLabelsEN.Add("D","High");
        PlusLabelsEN.Add("E","High");
        PlusLabelsEN.Add("None", "-");
        PlusLabels.Add("en", PlusLabelsEN);
        Dictionary<string, string> PlusLabelsDE = new Dictionary<string, string>();
        PlusLabelsDE.Add("A", "Wenig");
        PlusLabelsDE.Add("B", "Wenig");
        PlusLabelsDE.Add("C", "Neutral");
        PlusLabelsDE.Add("D", "Viel");
        PlusLabelsDE.Add("E", "Viel");
        PlusLabelsDE.Add("None", "-");
        PlusLabels.Add("de", PlusLabelsDE);
        Dictionary<string, string> MinusLabelsEN = new Dictionary<string, string>();
        MinusLabelsEN.Add("A", "High");
        MinusLabelsEN.Add("B", "Med");
        MinusLabelsEN.Add("C", "Neutral");
        MinusLabelsEN.Add("None", "-");
        MinusLabels.Add("en", MinusLabelsEN);
        Dictionary<string, string> MinusLabelsDE = new Dictionary<string, string>();
        MinusLabelsDE.Add("A", "Viel");
        MinusLabelsDE.Add("B", "Mittel");
        MinusLabelsDE.Add("C", "Neutral");
        MinusLabelsDE.Add("None", "-");
        MinusLabels.Add("de", MinusLabelsDE);
    }

    public void TranslateNutriPanel(string lang)
    {
        language = lang;
        if (testgroup)
        {
            //translate the labels of the Nutripanel
            Calories_Label.text = CaloriesLabels[language];
            Sugar_Label.text = SugarLabels[language];
            Fat_Label.text = FatLabels[language];
            Natrium_Label.text = NatriumLabels[language];
            Protein_Label.text = ProteinLabels[language];
            Fiber_Label.text = FiberLabels[language];
            HealthPercentage_Label.text = HealthPercentageLabels[language];
            WarningLabel.text = WarningLabels[language];
        }
        else
        {
            WeightLabel.text = WeightLabels[language];
        }
    }

    /// <summary>
    /// Handle the TapEvent
    /// </summary>
    public void OrganizeNutriPanel(string BoxKey)
    {
        TranslateNutriPanel(SetupOrganizer.Instance.language);

        CurrentBoxNr = BoxKey;

        string ProductId = SelectaOrganizer.Instance.BoxKeys[BoxKey];

        // Move the board just below the box selected
        transform.position = new Vector3(transform.position.x,
                                         SelectaOrganizer.Instance.Boxes[BoxKey].transform.position.y - 0.08f,
                                         transform.position.z);

        // Move the x_angle of the board between [0, 20] for [0.5 to -0.5] for better viewing angel
        //Nutriboard.transform.Rotate(new Vector3((Nutriboard.transform.position.x + 0.5f) * 15, 0, 0));


        //get all values for that class
        ProductDefinition productDefinition = SelectaOrganizer.Instance.ProductNutris[ProductId];

        if (testgroup)
        {
            //change the values of the panel
            Calories_Value.text = productDefinition.calories;
            Calories_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.caloriesColor);
            Calories_Explain.text = PlusLabels[language][productDefinition.caloriesColor];
            Calories_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.caloriesColor);

            Sugar_Value.text = productDefinition.sugar;
            Sugar_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.sugarColor);
            Sugar_Explain.text = PlusLabels[language][productDefinition.sugarColor];
            Sugar_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.sugarColor);

            Fat_Value.text = productDefinition.fat;
            Fat_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.fatColor);
            Fat_Explain.text = PlusLabels[language][productDefinition.fatColor];
            Fat_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.fatColor);

            Natrium_Value.text = productDefinition.natrium;
            Natrium_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.natriumColor);
            Natrium_Explain.text = PlusLabels[language][productDefinition.natriumColor];
            Natrium_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.natriumColor);

            Protein_Value.text = productDefinition.protein;
            Protein_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.proteinColor);
            Protein_Explain.text = MinusLabels[language][productDefinition.proteinColor];
            Protein_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.proteinColor);

            Fiber_Value.text = productDefinition.fiber;
            Fiber_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.fiberColor);
            Fiber_Explain.text = MinusLabels[language][productDefinition.fiberColor];
            Fiber_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.fiberColor);

            HealthPercentage_Value.text = productDefinition.healthpercentage;
            HealthPercentage_Ring.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.healthpercentageColor);
            HealthPercentage_Explain.text = MinusLabels[language][productDefinition.healthpercentageColor];
            HealthPercentage_Explain.color = SelectaOrganizer.Instance.GetNutriColor(productDefinition.healthpercentageColor);

            ProductName.text = productDefinition.name;

            TestBoard3.SetActive(false);
            TestBoard2.transform.localPosition = new Vector3(TestBoard2.transform.localPosition.x,
                                                             testBoard2_y_closed,
                                                             TestBoard2.transform.localPosition.z);
        }
        else
        {
            ProductName2.text = productDefinition.name;
            if (productDefinition.type == "Drink")
            {
                Weight.text = productDefinition.id.Split('_')[3] + " ml";
            }
            else if (productDefinition.type == "Food")
            {
                Weight.text = productDefinition.id.Split('_')[3] + " g";
            }
            else
            {
                Weight.text = "";
            }
            GTIN.text = productDefinition.gtin;
        }
    }

    public void ShowNutris(bool test)
    {
        testgroup = test;
        Test.SetActive(testgroup);
        Control.SetActive(!testgroup);
    }

    /// <summary>
    /// When the user clicks on a box the NutriInfo Menu should open up
    /// </summary>
    public void ShowOrHideNutriboard(string BoxNr)
    {
        //if Box is active for this Product close the Nutriboard else ...
        if (gameObject.activeSelf && CurrentBoxNr == BoxNr)
        {
            HideNutriboard();
        }
        else
        {
            // Change the values of the board to match those of the article
            OrganizeNutriPanel(BoxNr);
            gameObject.SetActive(true);
        }
    }

    public void HideNutriboard()
    {
        gameObject.SetActive(false);
        CurrentBoxNr = "None";
    }

    /// <summary>
    /// When the user selects a product and he is a test group
    /// user a check will be fulfilled for healthyness and in 
    /// case a bad product is selected a warning will be launched
    /// with recommendation and double opt in
    /// </summary>
    public void SelectProductWithCheck()
    {
        if (testgroup && !(TestBoard3.activeSelf || SelectaOrganizer.Instance.IsHealthiest(CurrentBoxNr)))
        {
            TestBoard2.transform.localPosition = new Vector3(TestBoard2.transform.localPosition.x,
                                                            testBoard2_y_open,
                                                            TestBoard2.transform.localPosition.z);
            TestBoard3.SetActive(true);
        }
        else
        {
            SceneOrganizer.Instance.OnFinish();
        }
    }
}
