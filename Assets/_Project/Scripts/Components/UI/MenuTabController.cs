using UnityEngine;
using UnityEngine.UI;

public class MenuTabController : MonoBehaviour
{
    [SerializeField] private Image[] tabImages;
    [SerializeField] private GameObject[] pages;

    private void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNumber)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNumber].SetActive(true);
        tabImages[tabNumber].color = Color.white;
    }
}
