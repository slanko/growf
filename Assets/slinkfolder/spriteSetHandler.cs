using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class spriteSetHandler : MonoBehaviour
{
    enum pickSprite
    {
        IDLE1,
        IDLE2,
        RUN1,
        RUN2,
        RUN3,
        RUN4,
        FISH1,
        FISH2,
        REEL1,
        REEL2,
        SURPRISE1,
        SURPRISE2,
        DIE1,
        DIE2
    }
    [SerializeField] SpriteRenderer myRenderer;
    [SerializeField] pickSprite currentSprite;

    [Header("SAME ORDER AS ENUM!!")]
    [SerializeField] List<Sprite> spriteSet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myRenderer.sprite = spriteSet[(int)currentSprite];
    }
}
