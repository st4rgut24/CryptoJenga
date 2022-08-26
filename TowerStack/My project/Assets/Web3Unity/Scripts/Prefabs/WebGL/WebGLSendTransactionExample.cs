using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL
public class WebGLSendTransactionExample : MonoBehaviour
{
    public GameObject AdmitButton;
    public CryptoJenga JengaContract;

    string to = "0x428066dd8A212104Bc9240dCe3cdeA3D3A0f7979";

    public void onExit()
    {
        SceneManager.LoadScene("GameLogin");
    }

    public void onGrantAdmission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
#endif