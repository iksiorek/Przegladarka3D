using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Przegladarka3D.Core.Gui{
	public class GuiManager{
		GraphicsDevice graphicsdevice;
		SpriteBatch spriteBatch;
		public GuiManager(GraphicsDevice grapd){
			graphicsdevice = grapd;
			spriteBatch = new SpriteBatch(graphicsdevice);
		}
		public void Init(){
			
		}

		public void Update(InputManager iomanager){
			
		}
		public void Draw(){
			spriteBatch.Begin();


			spriteBatch.End();
		}
	}

	
}
