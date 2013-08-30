using System.Collections.Generic;
using UnityEngine;

class BackgroundGenerator : MonoBehaviour
{
    const float ScrollSpeed = 0.0075f;
    const float MaxScroll = 30;

    float TotalScroll;

    public List<GameObject> BackgroundChoices = new List<GameObject>();

    GameObject TopBackground;
    GameObject BottomBackground;

    void Start()
    {
        var bgTemplate = RandomHelper.InEnumerable(BackgroundChoices);
        TopBackground = (GameObject)Instantiate(bgTemplate);
        TopBackground.transform.position += Vector3.up * MaxScroll;

        bgTemplate = RandomHelper.InEnumerable(BackgroundChoices);
        BottomBackground = (GameObject)Instantiate(bgTemplate);
    }

    void FixedUpdate()
    {
        TotalScroll += ScrollSpeed;

        TopBackground.transform.position += Vector3.down * ScrollSpeed;
        BottomBackground.transform.position += Vector3.down * ScrollSpeed;

        if (TotalScroll > MaxScroll)
        {
            Destroy(BottomBackground);
            BottomBackground = TopBackground;
            var bgTemplate = RandomHelper.InEnumerable(BackgroundChoices);
            TopBackground = (GameObject)Instantiate(bgTemplate);
            TopBackground.transform.position += Vector3.up * MaxScroll;

            TotalScroll = 0;
        }
    }
}
