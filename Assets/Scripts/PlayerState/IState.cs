using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// ���� �� ó�� �ڵ鷯
    /// </summary>
    void EnterState();

    /// <summary>
    /// ������ �� ó�� �ڵ鷯
    /// </summary>
    void ReEnterState();

    /// <summary>
    /// ���� �� ó�� �ڵ鷯
    /// </summary>
    void ExitState();

    /// <summary>
    /// Update ó�� �ڵ鷯
    /// </summary>
    void UpdateState();

}
