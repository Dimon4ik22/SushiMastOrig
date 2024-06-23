using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerLoader : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.LoadMuteState();
    }
}
