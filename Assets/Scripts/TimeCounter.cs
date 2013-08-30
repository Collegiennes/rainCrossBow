using UnityEngine;

class TimeCounter : MonoBehaviour
{
    Flyby flyBy;

    float personalBest;

    void Start()
    {
        flyBy = GameObject.Find("Player").GetComponent<Flyby>();
        GetComponent<TextMesh>().text = "";
    }

    void FixedUpdate()
    {
        if (Setup.ActiveLevel != null)
        {
            GetComponent<TextMesh>().text = string.Format("SURVIVED\n{0:0.00} SECONDS", Setup.ActiveLevel.SinceAlive);
            personalBest = Mathf.Max(Setup.ActiveLevel.SinceAlive, personalBest);

            GetComponent<TextMesh>().text += string.Format("\n\nPERSONAL BEST : {0:0.00}", personalBest);
        }
        if (flyBy.enabled && personalBest > 0)
            GetComponent<TextMesh>().text = string.Format("\n\n\nPERSONAL BEST : {0:0.00}", personalBest);
    }
}
