using UnityEngine;

public abstract class Item : MonoBehaviour
{

    public string itemName;

    public abstract void Left(GameObject player);
    public abstract void Right(GameObject player);
}
