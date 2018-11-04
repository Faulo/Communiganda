using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private string keyName;
    [SerializeField] private Thought thought;
    private Player player;

    void Start () {
		GameObject sprite = transform.Find("Sprite").gameObject;
        sprite.GetComponent<Image>().sprite = thought.GetSprite();

        GameObject text = sprite.transform.Find("Text").gameObject;
        text.GetComponent<Text>().text = keyName;

        player = FindObjectOfType<Player>();

        GetComponent<Button>().onClick.AddListener(TriggerPlayer);
    }
    void Update ()
    {
        if (Input.GetKeyDown(keyCode))
        {
            TriggerPlayer();
        }
    }
    private void TriggerPlayer()
    {
        player.SetThought(thought);
        player.StartSpeechAttack();
    }
}
