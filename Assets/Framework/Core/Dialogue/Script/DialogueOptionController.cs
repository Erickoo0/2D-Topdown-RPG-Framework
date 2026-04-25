using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DialogueOptionController : MonoBehaviour
{
    [SerializeField] private GameObject dialogueOptionPanel;
    [SerializeField] private GameObject buttonPrefab;
    private readonly List<GameObject> _buttons = new List<GameObject>();

    public void CreateButtons(DialogueOption[] options, System.Action<DialogueOption> onSelected)
    {
        ClearOptions();

        for (int i = 0; i < options.Length; i++)
        {
            // Spawn buttons into the panel
            GameObject button = Instantiate(buttonPrefab, dialogueOptionPanel.transform);
            // Set the button text
            button.GetComponentInChildren<TextMeshProUGUI>().text = options[i].optionName;
            
            // Pass the current option into its on selected mthod
            DialogueOption currentOption = options[i];
            
            // Tell the button to call OnOptionSelected method in the Manager when clicked
            button.GetComponent<Button>().onClick.AddListener(() => onSelected(currentOption));
            _buttons.Add(button);
        }
    }
    
    public GameObject GetFirstButton() => _buttons.Count > 0 ? _buttons[0] : null;    
    
    public void ClearOptions()
    {
        foreach (var button in _buttons) Destroy(button);
        _buttons.Clear();
    }
}
