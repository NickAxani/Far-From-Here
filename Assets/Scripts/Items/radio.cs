using UnityEngine;

public class radio : Item
{

    void Start()
    {
        itemName = "Radio";
    }

    public override void Left(GameObject player)
    {
        Debug.Log("Player needs to go to: "); //no currently generated destination
    }
    public override void Right(GameObject player)
    {
        return;
    }
}