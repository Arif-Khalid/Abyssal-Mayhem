using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraShake : MonoBehaviour
{
    public static CameraShake cameraShake;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    public void SetStatic()
    {
        cameraShake = this;
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
