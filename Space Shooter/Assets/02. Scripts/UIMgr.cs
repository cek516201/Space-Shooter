using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        Debug.Log("Click Button");

        SceneManager.LoadScene("scPlay");
    }
}