using System.Text;
using UnityEngine;

class TimeCounter : MonoBehaviour
{
    Flyby flyBy;

    float personalBest;

    readonly StringBuilder builder = new StringBuilder();

    void Start()
    {
        flyBy = GameObject.Find("Player").GetComponent<Flyby>();
        GetComponent<TextMesh>().text = "";
    }

    void FixedUpdate()
    {
        if (Setup.ActiveLevel != null)
        {
            builder.Length = 0;

            builder.AppendFormat("SURVIVED\n{0:0.00} SECONDS", Setup.ActiveLevel.SinceAlive);
            personalBest = Mathf.Max(Setup.ActiveLevel.SinceAlive, personalBest);
            builder.AppendFormat("\n\nPERSONAL BEST : {0:0.00}", personalBest);
        }
        if (flyBy.enabled && personalBest > 0)
        {
            builder.Length = 0;

            builder.AppendFormat("\n\n\nPERSONAL BEST : {0:0.00}", personalBest);
        }

        GetComponent<TextMesh>().text = builder.ToString();
    }
}
