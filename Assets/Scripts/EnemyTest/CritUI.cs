using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CritUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    private void LateUpdate()
    {
        canvas.transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    public void DestroySelf()
    {
        gameObject.SetActive(false);
    }
}
