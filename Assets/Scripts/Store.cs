using UnityEngine;
using UnityEngine.UI;


public class Store : MonoBehaviour
{

    public Image close;
    public GameObject store;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button btnClose = close.GetComponent<Button>();
        btnClose.onClick.AddListener(CloseButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void CloseButton()
    {
        store.gameObject.SetActive(false);
    }    



}
