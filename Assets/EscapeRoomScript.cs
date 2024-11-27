using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeRoomScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panel1;
 
    // Start is called before the first frame update
    void Start()
    {
        panel1.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        panel1.SetActive(false);
    }
}
