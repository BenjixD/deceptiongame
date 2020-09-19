using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
	public int limit;
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_EDITOR
    QualitySettings.vSyncCount = 0;  // VSync must be disabled
    Application.targetFrameRate = limit;
#endif
    }
}
