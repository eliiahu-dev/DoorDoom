using System.Collections;
using OLS_HyperCasual;
using UnityEngine;

public class CoroutineController : BaseController
{
    private CoroutineModel coroutineModel;

    public CoroutineController()
    {
        var coroutineView = new GameObject("Coroutine").AddComponent<CoroutineView>();
        coroutineModel = new CoroutineModel(coroutineView);
    }

    public void StartCoroutine(IEnumerator enumerator)
    {
        coroutineModel.View.AddCoroutine(enumerator);
    }
}