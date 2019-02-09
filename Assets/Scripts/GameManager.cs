using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f; // quanto il gioco aspetta tra i turni
    public static GameManager instance = null; // rendo l'instanza di GameManager un singleton
    public BoardManager boardScript;
    public int playerFoodPoints = 100; // punteggio iniziale, se scende sotto 0 si muore
    [HideInInspector] public bool playerTurn = true; // nascosta dall'interfaccia

    private Text levelText;
    private GameObject levelImage;
    private int level = 1; // 1 perchè è il primo livello
    private List<Enemy> enemies; // lista di nemici usate per tenere traccia di loro e farli muovere a turno
    private bool enemiesMoving;
    private bool doingSetup; // per non far muovere il player durante il setup

	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // per evitare che l'oggetto venga distrutto quando si carica una nuova scena e quindi io possa tenere traccia dei progressi tra i vari livelli
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    private void OnLevelWasLoaded(int index) { // metodo di unity chiamato ogni volta che una scena viene caricata
        level++;
        InitGame();
    }

    void InitGame () {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear(); // ripulisco la lista all'inizio, lo faccio perchè il gameManager non viene distrutto tra un livello e l'altro
        boardScript.SetupScene(level);
	}

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver() {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false; // in caso di gameover disabilito il gameManager
    }

    IEnumerator MoveEnemies() { // IEnumerator perchè è una coroutine, cioè aspetta un attimo prima di proseguire dopo aver invocato questo metodo
        enemiesMoving = true; // inizio turno di movimento nemici
        yield return new WaitForSeconds(turnDelay); // attendo 

        if(enemies.Count == 0) 
            yield return new WaitForSeconds(turnDelay); // se non ci sono nemici perchè per esempio è il primo livello allora attendi soltanto

        for(int i=0; i < enemies.Count; i++) { // ciclo sull'array di nemici
            enemies[i].MoveEnemy(); // muovo il nemico
            yield return new WaitForSeconds(enemies[i].moveTime); // attendo tra un movimento e l'altro
        }

        playerTurn = true;
        enemiesMoving = false; // fine turno movimento nemici
    }

    public void Update() {
        if(playerTurn || enemiesMoving || doingSetup) // se o il player o i nemici si stanno muovendo o non posso muovermi perchè il livello è in setup
            return; // non faccio nulla

        StartCoroutine(MoveEnemies()); // altrimenti faccio partire una coroutine per il movimento dei nemici
    }

    public void AddEnemyToList(Enemy script) {
        enemies.Add(script);
    }
}
