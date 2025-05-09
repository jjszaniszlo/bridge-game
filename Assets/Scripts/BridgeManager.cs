using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BridgeManager : MonoBehaviour
{
    [Header("UI")]
    public Button extendButton;
    
    [Header("XR Related")]
    public XRInteractionGroup xrInteractionGroup;

    private GameObject currentSelectedBridge;

    private void Update()
    {
    }

    /// <summary>
    /// Hides or shows extend button depending on if a bridge that can be extended is selected.
    /// </summary>
    private void HandleExtendButtonVisibility()
    {
        var focusedObject = xrInteractionGroup?.focusInteractable;
    }
}
