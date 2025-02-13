using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance;
        public Transform target;
        public float lerpSpeed = 1.0f;
        //private Vector3 shakeOffset;

        private Vector3 offset;

        private Vector3 targetPos;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            if (target == null) return;

            offset = Vector3.zero;
            offset.z = -3;
        }

        private void Update()
        {
            if (target == null) return;

            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }


    public void TriggerShake(float duration, float magnitude)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            offset = new Vector3(offsetX, offsetY, -3);

            elapsed += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore timeScale

            yield return null; // Still allows the shake to update even when timeScale is 0
        }

        offset = Vector3.zero;
        offset.z = -3;
    }


}
