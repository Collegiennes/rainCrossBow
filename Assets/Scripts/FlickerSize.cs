using UnityEngine;

class FlickerSize : MonoBehaviour
{
    public float Radius = 0.1f;

    Vector3 OriginalSize;

    void Start()
    {
        RefreshSize();
    }

    public void RefreshSize()
    {
        OriginalSize = transform.localScale;
    }

    void FixedUpdate()
    {
        float rv = Random.value;
        transform.localScale = OriginalSize + new Vector3(rv, rv, rv) * Radius;
    }
}
