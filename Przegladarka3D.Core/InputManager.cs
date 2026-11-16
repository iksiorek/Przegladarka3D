using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace Przegladarka3D.Core;

public class InputManager{
	KeyboardState _currentKeyboard;
	KeyboardState _previousKeyboard;
	public InputManager(){
		
	}
	public void Update(GameTime gameTime){
		_previousKeyboard = _currentKeyboard;
        _currentKeyboard = Keyboard.GetState();
	}

	public bool IsKeyDown(Keys key){
	        return _currentKeyboard.IsKeyDown(key);
	    }

    public bool IsKeyPressed(Keys key){
	        return _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);
	    }
}
