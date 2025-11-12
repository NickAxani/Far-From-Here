using UnityEngine;

public class nullItem : Item
{

    void Start()
    {
        itemName = "Null";
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