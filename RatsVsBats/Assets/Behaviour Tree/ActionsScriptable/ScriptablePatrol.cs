using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptablePatrol", menuName = "ScriptableObjects2/ScriptableAction/ScriptablePatrol", order = 4)]
public class ScriptablePatrol : ScriptableAction
{
    public override void OnFinishedState()
    {
        GameManager.Instance.UpdateText("donde se meti�?");
    }

    public override void OnSetState(StateController2 sc)
    {
        base.OnSetState(sc);
        GameManager.Instance.UpdateText("vamo a patrulla");
    }

    public override void OnUpdate()
    {
        GameManager.Instance.UpdateText("apatrullando la ciuda");
    }
}