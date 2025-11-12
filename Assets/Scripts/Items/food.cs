using UnityEngine;

public class food : Item
{

    void Start()
    {
        itemName = "Food";
    }

    public override void Left(GameObject player)
    {
        Debug.Log("player eats");
    }
    public override void Right(GameObject player)
    {
        return;
    }
}