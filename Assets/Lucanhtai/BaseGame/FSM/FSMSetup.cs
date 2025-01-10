using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMSetup : MonoBehaviour
{
    public abstract void SetUp(object data, out FSMState fSMState);
}
