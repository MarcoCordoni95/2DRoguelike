using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour { // per caricare tutto, instanzia GameManager se non è già instanziato

    public GameObject gameManager;

	void Awake () {
        if (GameManager.instance == null) // sfrutto il fatto che è di tipo statico, se è null lo instanzio
            Instantiate(gameManager);
	}
}
