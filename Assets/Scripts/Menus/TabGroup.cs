using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private GameObject[] tabs;

    public void ShowTab(int index)
    {
        foreach (var tab in tabs)
        {
            tab.SetActive(false);
        }

        if (index >= 0 && index < tabs.Length)
        {
            tabs[index].SetActive(true);
        }
    }
}
