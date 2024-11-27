using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
 
    // Start is called before the first frame update
    void Start()
    {
        panel1.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);

    }

    public void ShowPanel2()
    {
        panel2.SetActive(true);
        panel1.SetActive(false);
    }

    public void PlayGame()
    {
        panel2.SetActive(false);
    }
}
