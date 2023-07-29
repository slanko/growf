using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class hungerBarScript : MonoBehaviour
{
    enum healthBarPickSprite
    {
        IDLE1,
        IDLE2,
        DEAD1,
        DEAD2
    }
    [SerializeField] Image myRenderer;
    [SerializeField] healthBarPickSprite currentSprite;
    [SerializeField] List<Sprite> spriteSet;
    public Slider mySlider;
    [SerializeField] Gradient barGradient;
    [SerializeField] Image barFill;
    Animator anim;
    bool dead;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        myRenderer.sprite = spriteSet[(int)currentSprite];
        barFill.color = barGradient.Evaluate(mySlider.value/100);
        if(!dead && mySlider.value <= 0)
        {
            dead = true;
            anim.SetBool("dead", true);
        }
    }

    public void playerDied()
    {
        dead = true;
        anim.SetBool("dead", true);
    }
}
