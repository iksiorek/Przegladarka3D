
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Przegladarka3D.Core.Scene;
using Przegladarka3D.Core.Gui;

public abstract class GElements{
	Rectangle size;
	Vector2 pos;
	Texture2D[] texturelist;
	public GElements(){
		
	}	
	public abstract void Draw();
}
