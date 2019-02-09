using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject { // la faccio ereditare dalla classe astratta definita

    public int wallDamage = 1; // danno ai muri
    public int pointsForFood = 10; // punti quando raccolgo food
    public int pointsForSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator; // referenzia il componente animator
    private int food; // tiene traccia del punteggio tra i livelli 

    // Use this for initialization
    protected override void Start () { // protected override per dire che avrà un comportamento diverso rispetto MovingObject
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints; // così il player può gestire e modificare lo score anche quando si cambia livello
        foodText.text = "Food " + food;
        base.Start(); // richiamo la funzione start della classe MovingObject, sarebbe il Super di java
    }

    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food; //metodo di unity chiamato automaticamente quando disabilito l'oggetto, in questo caso avviene tra un livello e l'altro e memorizzo lo score
    }

    private void CheckIfGameOver() {
        if (food <= 0) { // se food scende sotto zero si perde
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();  // disabilita il gameManager
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir) { // T è generico per specificare il tipo di component che ci aspettiamo di incotrare nello spostamento
        food--; // muoversi costa 1
        foodText.text = "Food " + food;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;

        if(Move(xDir,yDir,out hit))
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        
        CheckIfGameOver(); // controllo perchè ho perso food
        GameManager.instance.playerTurn = false; // dico che è finito il turno del player
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.instance.playerTurn)
            return; // se non è il turno del player esco

        int horizontal = 0; // per memorizzare la direzione in cui ci stiamo muovendo, può essere 1 o -1
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal"); // usiamo GetAxisRaw perchè ritorna -1,0,1 invece GetAxis ritorna un valore nell'intervallo continuo tra -1 e 1
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0; //per imperdire di muoversi diagonalmente

        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical); // gli passiamo wall perchè è qualcosa con cui potrebbe interagire
    }
    
    protected override void OnCantMove<T>(T component) { // l'azione per il player riguarda i muri
        Wall hitWall = component as Wall; // definisco una variabile di tipo Wall è le assegno il component passato castandolo a Wall
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop"); // triggero l'azione chop
    }

    private void Restart() { // chiamato quando il player raggiunge l'exit
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // ricarico l'ultima scena perchè per ora è l'unica, sarà diverso comunque perchè generato proceduralmente
    }

    public void LoseFood(int loss) { // chiamato quando un nemico attacca il player
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other) { // metodo di unity che viene richiamato quando qualcosa entra nel collider 2D, usato per exit, food e soda, tutti loro hanno un collider con IsTrigger = true
        if(other.tag == "Exit") {
            Invoke("Restart", restartLevelDelay); // invoco la funzione Restart dopo un delay se ho raggiunto l'uscita
            enabled = false; // disabilito il player
        }
        else if (other.tag == "Food"){
            food += pointsForFood;
            foodText.text = "+" + pointsForFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false); // disabilito il food toccato
        }
        else if (other.tag == "Soda")
        {
            food += pointsForFood;
            foodText.text = "+" + pointsForSoda + " Soda: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false); // disabilito la soda toccata
        }
    }

}
