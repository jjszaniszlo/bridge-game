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
    public Button removeLastButton;
    
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
            var bridgeExtendableComponent = interactable.transform.gameObject.GetComponent<BridgeExtendable>();
            if (bridgeExtendableComponent != null)
            {
                extendButton.gameObject.SetActive(true);
                removeLastButton.gameObject.SetActive(bridgeExtendableComponent.HasSegments());
            }
            else
            {
                extendButton.gameObject.SetActive(false);
            }
        }
        else
        {
            extendButton.gameObject.SetActive(false);
            removeLastButton.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Propagate Extend to the selected bridge root object.
    /// </summary>
    public void ExtendBridgeButton()
    {
        var interactable = xrInteractionGroup?.focusInteractable;
        if (interactable == null) return;

        if (interactable.transform
            .TryGetComponent<BridgeExtendable>(out var bridgeExtendable))
        {
            int cost = bridgeExtendable.segmentCost;

            // try to deduct the cost first
            if (BudgetManager.Instance.TrySpend(cost))
            {
                bridgeExtendable.Extend();
            }
            else
            {
                Debug.LogWarning("Not enough budget to extend the bridge.");
            }
        }
    }

    /// <summary>
    /// Propagate removing a segment to the selected bridge root object.
    /// </summary>
    public void RemoveLastBridgeSegmentButton()
    {
        var interactable = xrInteractionGroup?.focusInteractable;
        if (interactable == null) return;

        if (interactable.transform
            .TryGetComponent<BridgeExtendable>(out var bridgeExtendable))
        {
            if (bridgeExtendable.HasSegments())
            {
                // remove the segment
                bridgeExtendable.RemoveLast();

                // refund its cost
                int refund = bridgeExtendable.segmentCost;
                BudgetManager.Instance.Refund(refund);
            }
        }
    }
}
