using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public IEnumerator ShakeCamera(float duration, float amount)
    {
        Vector3 original = transform.position;
        float time = 0f;
        while (time < duration)
        {
            transform.position = transform.position+(Random.insideUnitSphere*amount);
            time += Time.deltaTime;
            yield return 0;
        }
        transform.position = original;
    }
}
