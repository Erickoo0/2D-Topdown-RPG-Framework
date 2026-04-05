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

        foreach (var option in options)
        {
            // Spawn buttons into the panel
            GameObject button = Instantiate(buttonPrefab, dialogueOptionPanel.transform);
            // Set the button text
            button.GetComponentInChildren<TextMeshProUGUI>().text = option.optionName;
            // Tell the button to call OnOptionSelected method in the Manager when clicked
            button.GetComponent<Button>().onClick.AddListener(() => onSelected(option));
            _buttons.Add(button);
        }
    }

    public void ClearOptions()
    {
        foreach (var button in _buttons) Destroy(button);
        _buttons.Clear();
    }
}
