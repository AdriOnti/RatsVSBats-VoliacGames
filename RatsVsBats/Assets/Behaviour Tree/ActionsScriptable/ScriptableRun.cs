using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableRun", menuName = "ScriptableObjects2/ScriptableAction/ScriptableRun")]

public class ScriptableRun : ScriptableAction
{
    private ChaseBehaviour _chaseBehaviour;
    private EnemyController3 _enemyController;
    public override void OnFinishedState(StateController2 sc)
    {
        _chaseBehaviour.StopChasing();
    }

    public override void OnSetState(StateController2 sc)
    {
        base.OnSetState(sc);
        GameManager.Instance.UpdateText("estoy huyendo");
        _chaseBehaviour = sc.GetComponent<ChaseBehaviour>();
        _enemyController = (EnemyController3)sc;
    }

    public override void OnUpdate(StateController2 sc)
    {
        try
        {
            _chaseBehaviour.Run(_enemyController.target.transform, sc.transform);
        }
        catch
        {
            _chaseBehaviour.StopChasing();
        }
    }

}