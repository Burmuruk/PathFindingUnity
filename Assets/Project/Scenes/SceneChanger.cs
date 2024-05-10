using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float time = 3;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Change_Scene", time);
    }

    void Change_Scene()
    {
        SceneManager.LoadScene(1);
    }
}
