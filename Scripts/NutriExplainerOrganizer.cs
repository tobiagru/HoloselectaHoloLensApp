using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutriExplainerOrganizer : MonoBehaviour {

    public static NutriExplainerOrganizer Instance;

    public TextMesh HealthyLabel;
    public TextMesh UnhealthyLabel;

    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);
    }

    private void TranslateNutriExplainer(string lang)
    {
        if (lang == "DE")
        {
            HealthyLabel.text = "Gesund";
            UnhealthyLabel.text = "Ungesund";
        }
        else
        {
            HealthyLabel.text = "Healthy";
            UnhealthyLabel.text = "Unhealthy";
        }
    }

    public void SetNutriExplainerActive(bool isActive)
    {
        TranslateNutriExplainer(SetupOrganizer.Instance.language);
        gameObject.SetActive(isActive);
    }
}
