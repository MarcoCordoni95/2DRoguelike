using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite; // sprite da mostrare per far vedere che ha attaccato il muro con successo
    public int hp = 4; //vita
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;
    
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    public void DamageWall(int loss) {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        
        if(hp <= 0) 
            gameObject.SetActive(false);        
    }
}
