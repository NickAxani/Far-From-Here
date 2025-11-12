using UnityEngine;

public class gun : Item
{

    void Start()
    {
        itemName = "Gun";
    }

    public override void Left(GameObject player)
    {
        //shoot
        Debug.Log("Player shoots");
        player.GetComponent<PlayerController>().shoot();

    }
    public override void Right(GameObject player)
    {
        //aim down sights
        return;
    }
}