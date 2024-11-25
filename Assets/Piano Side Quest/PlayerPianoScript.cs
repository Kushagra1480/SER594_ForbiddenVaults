using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPianoScript : MonoBehaviour
{
    public KeyControl piano;
    public bool isNearPiano = false;
    public GameObject playPiano;
    // Start is called before the first frame update
    void Start()
    {
        piano = GameObject.Find("PianoSceneController").GetComponent<KeyControl>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (isNearPiano && Input.GetKeyDown(KeyCode.P))
        {
            playPiano.SetActive(false);
            piano.ShowPiano();
        }
        
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Piano"))
        {
                playPiano.SetActive(true);
                isNearPiano = true;
                Debug.Log("Piano Collision");

        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Power Up collided.");
            Destroy(other.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Piano"))
        {
                playPiano.SetActive(false);
                isNearPiano = false;
        }

    }
}
