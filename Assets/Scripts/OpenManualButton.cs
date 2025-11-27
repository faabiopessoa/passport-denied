using UnityEngine;

public class OpenManualButton : MonoBehaviour
{
    [Header("Referência ManualUI")]
    public ManualUI manualUI; // arrastar no Inspector

    public void OnOpenManual()
    {
        if (manualUI != null)
        {
            manualUI.ToggleManual();
        }
        else
        {
            Debug.LogWarning("ManualUI não foi atribuído no Inspector!");
        }
    }
}
