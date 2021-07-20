using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadGame : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKey)
        {
            Reload();
        }
    }

    private void Reload()
    {
        SceneManager.LoadScene("Game");
    }
}
