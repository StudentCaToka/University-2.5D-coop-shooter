using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EndscreenManager : MonoBehaviour {

    private void OnDisable()
    {
        NetworkManager.singleton.autoCreatePlayer = true;
    }
}
