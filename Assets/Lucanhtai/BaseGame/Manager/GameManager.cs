using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    protected FSMSystem fSMSystem;
    protected Dependency dependency;
    protected Adapter adapter;
    protected Navigator navigator;
    protected virtual void Awake()
    {
        fSMSystem = GetComponent<FSMSystem>();
        dependency = GetComponent<Dependency>();
        adapter = GetComponent<Adapter>();
        navigator = GetComponent<Navigator>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    public virtual void SetData<T>(T data)
    {

    }
}