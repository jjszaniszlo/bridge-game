using UnityEngine;
using TMPro;

public class BudgetManager : MonoBehaviour
{
    public static BudgetManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI budgetText;

    /// <summary>
    /// The player's current funds.
    /// </summary>
    public int CurrentBudget { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Initialize the budget at the start of each level.
    /// </summary>
    public void InitBudget(int amount)
    {
        CurrentBudget = amount;
        UpdateUI();
    }

    /// <summary>
    /// Attempt to spend the specified amount. Returns true on success.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (amount > CurrentBudget)
            return false;

        CurrentBudget -= amount;
        UpdateUI();
        return true;
    }

    /// <summary>
    /// Refunds the specified amount back into the budget. Might or might not need.
    /// </summary>
    public void Refund(int amount)
    {
        CurrentBudget += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (budgetText != null)
            budgetText.text = $"${CurrentBudget}";
    }
}
