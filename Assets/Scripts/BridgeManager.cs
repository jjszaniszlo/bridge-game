using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BridgeManager : MonoBehaviour
{
    [Header("UI")]
    public Button extendButton;
    
    [Header("XR Related")]
    public XRInteractionGroup xrInteractionGroup;
    
    private void Update()
    {
        HandleExtendButtonVisibility();
    }

    /// <summary>
    /// Hides or shows extend button depending on if a bridge that can be extended is selected.
    /// </summary>
    private void HandleExtendButtonVisibility()
    {
        var interactable = xrInteractionGroup?.focusInteractable;
        if (interactable != null)
        {
            extendButton.gameObject.SetActive(interactable.transform.gameObject.GetComponent<BridgeExtendable>() != null);
        }
        else
        {
            extendButton.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Propagate Extend to the selected bridge object.
    /// </summary>
    public void ExtendBridgeButton()
    {
        var interactable = xrInteractionGroup?.focusInteractable;
        if (interactable != null)
        {
            var obj = interactable.transform.gameObject;
            if (obj.TryGetComponent<BridgeExtendable>(out var bridgeExtendable))
            {
                bridgeExtendable.Extend();
            }
        }
    }
}
