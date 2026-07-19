using UnityEngine;

public interface IPlayerInput
{
    Vector2 GetMovementInput();
    Vector2 GetLookInput();
    bool GetJumpInputDown();
    bool GetShootInputHold(); //for autofire
    bool GetShootInputDown(); //for single and burst
    bool GetShootInputToggle(); //to cycle through firemodes
    bool GetADSInputDown();
    bool GetPauseInputDown();
    bool GetInteractInputDown();

}
