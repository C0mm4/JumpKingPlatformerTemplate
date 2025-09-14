# 📌 점프킹 스타일 플랫포머 Rigidbody 템플릿



## 1. 모듈 개요 (Overview)  
- **설명**: 점프킹과 비슷한 물리엔진을 활용하기 위한 게임에 적용할 물리 엔진 모듈
    간단한 이동과 점프, 사선면에서 슬라이딩 하는 물리 엔진 구현
- **지원 Unity 버전**: 2022.3 LTS 이상  
- **의존성**: Unity new Input System (플레이어 이동) - 이동을 제외한 떨어짐 모듈만 실행 시 적용하지 않아도 됨.
---

## 2. 키 입력 (Key Bindings)  

| 키(Key) | 동작(Action)        | 설명(Description)             |
|---------|---------------------|--------------------------------|
| **A**   | Move Left           | 캐릭터를 왼쪽으로 이동 |
| **D**   | Move Right          | 캐릭터를 오른쪽으로 이동 |
| **Space** | Jump               | 캐릭터가 점프 |

---

## 3. 적용 방법 (Usage / Setup)  
### GameObject Inspector 연결 방식  
1. Player GameObject 생성
2. Collider2D, RigidBody2D 추가 (Rigidbody는 없으면 자동 생성, Collider2D는 필수로 추가해야함)
3. KinematicObj Component 추가
4. (키보드 입력으로 플레이어 이동을 원한다면) Player, Player Controller, Player State Machine, Status Model 추가
5. Player Input 추가 후 이벤트 연결

---

## 4. 주요 기능 (Features)  
- Kinematic Rigidbody2D를 활용한 물리 엔진
- FSM을 적용한 Player Status Machine
- IState와 IPlayerState로 객체의 상태 관리
- IPlayerState에서 InputHandle을 통해 Controller에서 키 입력을 받고 해당 상태에 키 입력에 관한 처리


**데모**  
이동과 사선에서 미끄러지는 물리처리 데모

![2025-09-14 22-37-33](https://github.com/user-attachments/assets/bfeb99a9-3b2d-44c5-abb9-e753208c7237)

---
