using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// 진입 시 처리 핸들러
    /// </summary>
    void EnterState();

    /// <summary>
    /// 재진입 시 처리 핸들러
    /// </summary>
    void ReEnterState();

    /// <summary>
    /// 종료 시 처리 핸들러
    /// </summary>
    void ExitState();

    /// <summary>
    /// Update 처리 핸들러
    /// </summary>
    void UpdateState();

}
