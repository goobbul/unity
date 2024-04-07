using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Debugscreen : MonoBehaviour
{
    public TextMeshProUGUI TMP;

    public GameObject Panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TMP.text = "SERVER : " + PhotonNetwork.CloudRegion;

        if (Input.GetKeyDown("slash"))
        {
            Debug.Log("key down1");
            Panel.SetActive(true);
        }
        
    }
}