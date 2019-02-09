using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target; // memorizza la posizione del player per sapere dove spostare i nemici
    private bool skipMove; // per sapere quando si devono muovere
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {
        GameManager.instance.AddEnemyToList(this); // si auto aggiunge alla lista di enemy
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform; // memorizzo il transform del player
        base.Start();		
	}

    protected override void AttemptMove<T>(int xDir, int yDir) {
        if (skipMove) { // controllo se devo saltare il turno
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) { // se la differenza tra le 2 X, del player e del nemico, sono quasi nulle allora si trovano nella stessa colonna
            if (target.position.y > transform.position.y) // se l'y del player è maggiore di quella dell'enemy lo raggiungo muovendomi in su e viceversa
                yDir = 1;
            else
                yDir = -1;
        }
        else {
            if (target.position.x > transform.position.x) // se l'x del player è maggiore di quella dell'enemy lo raggiungo muovendomi a destra e viceversa
                xDir = 1;
            else
                xDir = -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component) {
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
