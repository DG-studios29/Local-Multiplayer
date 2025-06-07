using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    public GameObject panel1; 
    public GameObject panel2; 
    public Button firstButtonPanel1;  
    public Button firstButtonPanel2;  

    private void Start()
    {
        // Set the initial selected UI element
        EventSystem.current.SetSelectedGameObject(firstButtonPanel1.gameObject);
    }

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstButtonPanel1.gameObject);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonPanel2.gameObject);
    }
}
