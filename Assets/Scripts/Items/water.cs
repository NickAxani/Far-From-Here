using UnityEngine;

public class water : Item
{

    void Start()
    {
        itemName = "Water";
    }

    public override void Left(GameObject player)
    {
        Debug.Log("player drinks");
    }
    public override void Right(GameObject player)
    {
        return;
    }
}