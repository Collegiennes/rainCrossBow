using UnityEngine;

class Flyby : MonoBehaviour
{
    Vector3 StartScale = new Vector3(10, 10, 10);
    Vector3 EndScale = new Vector3(1, 1, 1);

    Vector3 EndPos = new Vector3(0, -4, 0);

    float Duration = 2;

    public AudioClip OhMyGah;

    float SinceStarted;

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        AudioSource.PlayClipAtPoint(OhMyGah, transform.position);
        SinceStarted = 0;
        enabled = true;
        Level.ScrollingSpeed = 0.8f;

        FixedUpdate();
    }

    void FixedUpdate()
    {
        GetComponent<PlayerController>().Dead = true;

        SinceStarted += Time.deltaTime;
        var step = Mathf.Clamp01(SinceStarted / Duration);

        var headz = gameObject.FindChild("headz");
        headz.transform.localRotation = Quaternion.Slerp(headz.transform.localRotation, Quaternion.AngleAxis(90 - 60 * Easing.EaseIn(step, EasingType.Cubic), Vector3.right), 0.2f);

        transform.localScale = Vector3.Lerp(StartScale, EndScale, step);
        transform.rotation = Quaternion.AngleAxis(step * 360, Vector3.up);
        transform.position = EndPos;

        if (step == 1)
        {
            GetComponent<PlayerController>().Dead = false;
            enabled = false;
            Setup.LevelOne();
        }
    }
}
