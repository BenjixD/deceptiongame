using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingAlertObject : MonoBehaviour
{

    public float timeToFade = 0.25f;

    public float stayDuration;

    public CanvasGroup canvasGroup;
    public Sprite minimapSprite;


    void Start() {
        StartCoroutine(fadeIn());
    }


    private IEnumerator fadeIn() {
        MinimapMarkerParams minimapParams = new MinimapMarkerParams(minimapSprite, this.transform, this.gameObject.GetHashCode(), true);
        GameManager.Instance.controller.mapController.minimap.AddToMinimap(minimapParams);

        float counter = 0;

        // Fade in
        while (counter < timeToFade) {
            counter += Time.deltaTime;
            float t = counter / timeToFade;
            canvasGroup.alpha = t;
            yield return null;
        }

        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(this.stayDuration);

        counter = 0;

        while (counter < timeToFade) {
            counter += Time.deltaTime;
            float t = counter / timeToFade;
            canvasGroup.alpha = 1 - t;
            yield return null;
        }

        canvasGroup.alpha = 0;

        this.DestroyMe();
    }

    private void DestroyMe() {
        GameManager.Instance.controller.mapController.minimap.RemoveFromMinimap(this.gameObject.GetHashCode(), true);
        Destroy(this.gameObject);
    }

}
