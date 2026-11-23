using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Przegladarka3D.Core;

public enum MouseButton{ Left, Middle, Right, X1, X2 }

public class InputManager{
	KeyboardState _currentKeyboard;
	KeyboardState _previousKeyboard;
	MouseState _currentmouseState;	
	MouseState _previousmouseState;	

	public InputManager(){
		_currentmouseState = Mouse.GetState();
		_previousmouseState = _currentmouseState;
	}
	public void Update(GameTime gameTime){
		_previousKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
		_previousmouseState = _currentmouseState;
		_currentmouseState = Mouse.GetState();
	}

	static ButtonState StateOf(MouseState state, MouseButton button){
		return button switch{
			MouseButton.Left   => state.LeftButton,
			MouseButton.Middle => state.MiddleButton,
			MouseButton.Right  => state.RightButton,
			MouseButton.X1     => state.XButton1,
			MouseButton.X2     => state.XButton2,
			_                  => ButtonState.Released
		};
	}

	// sprawdzenie danego przycisku
	public bool GetButtonMouseState(MouseButton buttonmouse){
		return StateOf(_currentmouseState, buttonmouse) == ButtonState.Pressed;
	}

	// czy krotka nie bylaby szybsza ? TO DO: kiedys sprawdzic
	public Vector2 GetDeltaMouseXY(){
				return new Vector2((_currentmouseState.X - _previousmouseState.X),(_currentmouseState.Y - _previousmouseState.Y));
	}
	public int GetMouseScrollDelta(){
		return _currentmouseState.ScrollWheelValue-_previousmouseState.ScrollWheelValue;
	}
	public Vector2 GetMousePos(){
		return new Vector2(_currentmouseState.X,_currentmouseState.Y);
	}

	public bool IsKeyDown(Keys key){
	        return _currentKeyboard.IsKeyDown(key);
	    }

    public bool IsKeyPressed(Keys key){
	        return _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);
	    }
}
