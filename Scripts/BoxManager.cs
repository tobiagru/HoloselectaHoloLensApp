using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class BoxManager : MonoBehaviour, IInputClickHandler, IInputHandler {

    private void Update()
    {
        //todo wiggle the box that is active by a few degrees
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        NutriboardOrganizer.Instance.ShowOrHideNutriboard(gameObject.name);
    }
    public void OnInputDown(InputEventData eventData) { }
    public void OnInputUp(InputEventData eventData) { }
}
