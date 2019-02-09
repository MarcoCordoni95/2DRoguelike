using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour { // per il movimento

    public float moveTime = 0.1f; // velocità a cui mi muovo
    public LayerMask blockingLayer; // layer in cui avvengono le collisioni

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime; // per avere il movimento più efficiente

	// Use this for initialization
	protected virtual void Start () { // "protected virtual" lo usiamo per fare l'override di start nelle sottoclassi
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; // memorizzio il reciproco così posso usare questo valore per moltiplicare al posto di dividere
    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit) { // out significa che il parametro verrà passato per referenza
        Vector2 start = transform.position; // avviene un cast automatico da Vector3 a Vector2
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false; // per evitare di colpire il nostro stesso collider lo disabilito mentre stiamo facendo il cast al ray
        hit = Physics2D.Linecast(start, end, blockingLayer); // salvo tutti i collider colpiti da start a end sul layer considerato
        boxCollider.enabled = true; // riabilito il boxcollider

        if(hit.transform == null) { // se non ha toccato nulla 
            StartCoroutine(SmoothMovement(end)); // lo sposto
            return true; // ritorno true
        }

        return false; // altrimenti false
    }
	
    protected IEnumerator SmoothMovement (Vector3 end) { // sposto l'unità fino alla posizione end
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude; // calcolo la distanza rimanente in cui muovermi, prima calcolo la differenze tra posizione corrente e destinazione e poi vi applico magnitude che ritrna la lunghezza del vettore ottenuto (usiamo sqrMagnitude perchè è meno costoso)

        while(sqrRemainingDistance > float.Epsilon) { // finchè lo spostameno è maggiore di un epsilon
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime); // calcolo la nuova posizione con MoveTowards 
            rb2D.MovePosition(newPosition); //sposto il rigidBody nella nuova posizione
            sqrRemainingDistance = (transform.position - end).sqrMagnitude; // ricalcolo il valore di sqrRemainingDistance
            yield return null; // attendo un frame prima di rivalutare la condizione del loop 
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir) // T usato per specificare il tipo di component con cui interagire se si viene bloccati, in caso di nemici sarà il player, in caso di Player saranno i muri così che possa distruggerli
        where T : Component { // usiamo where per specificare che T è il component

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit); // ritorna true se il movimento ha avuto successo e false altrimenti

        if (hit.transform == null) // grazie al fatto che hit è passato come parametro possiamo controllare il suo contenuto e se è null allora usciamo e non facciamo nulla
            return;

        T hitComponent = hit.transform.GetComponent<T>(); // se invece qualcosa è stato colpito prendiamo il component T attaccato ad esso

        if (!canMove && hitComponent != null) // se non possiamo muoverci ma abbiamo colpito qualcosa, tipo un muro
            OnCantMove(hitComponent); // ci muoviamo su esso
    }

    protected abstract void OnCantMove <T> (T component) // metodo astratto che verrà implementato nelle sottoclassi e prende un tipo generico
        where T : Component;
}
