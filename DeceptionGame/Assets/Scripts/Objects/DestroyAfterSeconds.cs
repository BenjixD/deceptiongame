using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{

    public float time = 1f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.DestroyAfterSecondsCor(this.time));
    }

    private IEnumerator DestroyAfterSecondsCor(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
