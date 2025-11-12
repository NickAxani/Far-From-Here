using UnityEngine;

public class gps : Item
{

    void Start()
    {
        itemName = "GPS";
    }

    public override void Left(GameObject player)
    {
        string loc = new Vector2(Mathf.Floor(player.transform.position.x), Mathf.Floor(player.transform.position.z)).ToString();
        Debug.Log("Player location is: " + loc);
    }
    public override void Right(GameObject player)
    {
        return;
    }
}