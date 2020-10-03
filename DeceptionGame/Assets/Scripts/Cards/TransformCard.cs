using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TransformCard", order = 2)]
public class TransformCard : Card
{
    public List<FakeProp> pretendPropsPool;

    private FakeProp fakePropInstance;

    private List<SpriteRenderer> spriteRend;

    protected override void PlayCardStart() {
        base.PlayCardStart();

        this.spriteRend = new List<SpriteRenderer>();

        foreach (SpriteRenderer rend in this.player.GetComponentsInChildren<SpriteRenderer>()) {
            rend.enabled = false;
            this.spriteRend.Add(rend);
        }

        int randIndex = Random.Range(0, pretendPropsPool.Count);
        fakePropInstance = Instantiate<FakeProp>(pretendPropsPool[randIndex], this.player.transform);
        fakePropInstance.transform.position = this.player.transform.position;
        fakePropInstance.Initialize(this);

        Messenger.AddListener(Messages.OnMoveMainPlayer, this.UnTransform);
    }

    public override void OnCardDestroyed()
    {
        base.OnCardDestroyed();

        Messenger.RemoveListener(Messages.OnMoveMainPlayer, this.UnTransform);
        
    }

    public void UnTransform() {
        foreach (SpriteRenderer rend in this.spriteRend) {
            rend.enabled = true;
        }

        Destroy(fakePropInstance.gameObject);

        this.RemoveCardFromActiveList();
    }
}
