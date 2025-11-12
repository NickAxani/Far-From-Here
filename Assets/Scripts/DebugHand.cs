using TMPro;
using UnityEngine;

public class DebugHand : MonoBehaviour
{

    public TextMeshProUGUI text;
    public PlayerController player;
    private string item;

    // Update is called once per frame
    void Update()
    {
        if (!player.inDebug)
        {
            text.text = "";
            return;
        }
        item = player.inHand.GetComponent<Item>().itemName;

        text.text = "Player State: " + item;
    }
}