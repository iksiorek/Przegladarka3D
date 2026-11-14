using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Text;
using Przegladarka3D.Core.Gui;
namespace Przegladarka3D.Core.Scene;

public abstract class Object3D{

	protected Vector3 pos3d;
	public abstract void Draw(GraphicsDevice graphicsDevice, BasicEffect effect);

}
