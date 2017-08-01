using UnityEngine;
using UnityEngine.UI;

public class NewGameStarter : MonoBehaviour {

	void Awake () {
        GetComponent<Button>().onClick.AddListener(() => { FindObjectOfType<GameController>().StartGame(); });
	}

}
