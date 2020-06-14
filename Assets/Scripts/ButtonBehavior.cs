using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour {
    [SerializeField] KeyCode keyCode;
    [SerializeField] string keyName;
    [SerializeField] Thought thought;
    Player player;

    void Start() {
        var sprite = transform.Find("Sprite").gameObject;
        sprite.GetComponent<Image>().sprite = thought.GetSprite();

        var text = sprite.transform.Find("Text").gameObject;
        text.GetComponent<Text>().text = keyName;

        player = FindObjectOfType<Player>();

        GetComponent<Button>().onClick.AddListener(TriggerPlayer);
    }
    void Update() {
        if (Input.GetKeyDown(keyCode)) {
            TriggerPlayer();
        }
    }
    void TriggerPlayer() {
        player.SetThought(thought);
        player.StartSpeechAttack();
    }
}
