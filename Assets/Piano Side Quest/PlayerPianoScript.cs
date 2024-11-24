using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPianoScript : MonoBehaviour
{
    public KeyControl piano;
    public GameObject playPiano;
    // Start is called before the first frame update
    void Start()
    {
        piano = GameObject.Find("PianoSceneController").GetComponent<KeyControl>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Piano"))
        {
            playPiano.SetActive(true);
            Debug.Log("Piano Collision");
        }
    }
}
