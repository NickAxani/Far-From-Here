using UnityEngine;

public class compass : Item
{

    void Start()
    {
        itemName = "Compass";
    }

    public override void Left(GameObject player)
    {
        float angle = player.transform.Find("Orientation").eulerAngles.y;
        string dir = "";
        if (angle <= 22.5f)
        {
            dir = "North";
        }
        else if (angle <= 67.5f)
        {
            dir = "Northeast";
        }
        else if (angle <= 112.5f)
        {
            dir = "East";
        }
        else if (angle <= 157.5f)
        {
            dir = "Southeast";
        }
        else if (angle <= 202.5f)
        {
            dir = "South";
        }
        else if (angle <= 247.5f)
        {
            dir = "Southwest";
        }
        else if (angle <= 292.5f)
        {
            dir = "West";
        }
        else if (angle <= 337.5f)
        {
            dir = "Northwest";
        }
        else
        {
            dir = "North";
        }


        Debug.Log("Player is facing: " + dir);


    }
    public override void Right(GameObject player)
    {
        return;
    }
}