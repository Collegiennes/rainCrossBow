using UnityEngine;

class TimeCounter : MonoBehaviour
{
    Flyby flyBy;

    void Start()
    {
        flyBy = GameObject.Find("Player").GetComponent<Flyby>();
    }

    void FixedUpdate()
    {
        if (Setup.ActiveLevel != null)
        {
            guiText.text = string.Format(" SCORE {0:0}s", Setup.ActiveLevel.SinceAlive);
        }
        if (flyBy.enabled)
            guiText.text = "";
    }
}
