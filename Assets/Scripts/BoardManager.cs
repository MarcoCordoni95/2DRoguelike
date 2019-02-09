using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count {
        public int minimum;
        public int maximum;

        public Count(int min, int max) {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8; // dimensione del gioco, griglia 8x8
    public int rows = 8;
    public Count wallCount = new Count(5, 9); // uso count per specificare un range random di muri da creare all'interno del livello, minimo 5 muri e massimo 9
    public Count foodCount = new Count(1, 5); // idem ma per il food 
    public GameObject exit;
    public GameObject[] floorTiles; // array per scegliere random quale sprite usare
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPosition = new List<Vector3>(); // per tenere traccia degli oggetti spawnati sul gioco

    void InitalialiseList() {
        gridPosition.Clear();
        
        for (int x = 1; x < columns - 1; x++) { // itero evitando di toccare le righe e colonne a lato per evitare situazioni in cui non si può vincere
            for (int y = 1; y < rows - 1; y++) {
                gridPosition.Add(new Vector3(x, y, 0f)); // creo una lista di possibili posizioni in cui inserire i pickups
            }
        }
    }

    void BoardSetup() {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++) { // mi muovo sui bordi per questo uso -1
            for (int y = -1; y < rows + 1; y++) {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // prendo un elemento random dal mio array di sprite per il terreno e lo instanzio

                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; // se sto considerando il bordo prendo random uno sprite per il bordo

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; // instanzio l'oggetto definito nella posizione definita dal vector3 con Quaternion.identity cioè nessuna rotazione

                instance.transform.SetParent(boardHolder); // definisco il gameObject instanziato come figlio di boardHolder
            }
        }
    }

    Vector3 RandomPosition() { // posizione random in cui mettere le cose sul terreno
        int randomIndex = Random.Range(0, gridPosition.Count); // tra 0 e il numero di posizioni presenti nella lista gridPosition
        Vector3 randomPosition = gridPosition[randomIndex];
        gridPosition.RemoveAt(randomIndex); // per non avere 2 oggetti nello stesso punto elimino quella posizione dalla lista
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) { // per far spawnare i tiles nelle posizioni random trovate
        int objectCount = Random.Range(minimum, maximum + 1); // per tenere sotto controllo il numero di oggetti spawnati

        for(int i = 0; i < objectCount; i++) {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity); // instanzio il tile scelto randomicamente dall'array alla posizione random trovata senza rotazione
        }
    }

    public void SetupScene(int level) { // richiamata dal gameManager per fare il setup del board
        BoardSetup();
        InitalialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f); // uso il numero del livello per definire il numero di nemici, ne faccio il logaritmo e lo casto ad intero
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); // faccio lo stesso anche con i nemici ma il minimo e massimo sono lo stesso perchè non sto definendo un range
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity); // instanzio exit in alto a destra 
    }
}
