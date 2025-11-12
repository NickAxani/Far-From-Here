using UnityEngine;

public class knife : Item
{

    void Start()
    {
        itemName = "Knife";
    }

    public override void Left(GameObject player)
    {
        return;
    }
    public override void Right(GameObject player)
    {
        return;
    }
}