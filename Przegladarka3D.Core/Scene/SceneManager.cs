
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

public class SceneManager{

	Matrix worldMatrix;
	BasicEffect basicEffect;
	GraphicsDevice gdcard;
	Kamera _camera;
 	public ConverterReader _konwerter;
	List<Object3D>objectslist = new();
	public List<Entity>Entitytest = new();
	// to do: dodac liste obiektuw
	public SceneManager(GraphicsDevice gd){
		gdcard = gd;
	}
	public void Init(){
		_camera = new Kamera();
        _camera.CreatePerspectiveFOV(45,gdcard,1f,1000f);
		_konwerter = new();
		worldMatrix = Matrix.CreateWorld(_camera.camTarget, Vector3.Forward, Vector3.Up);
		basicEffect = new BasicEffect(gdcard);
		basicEffect.Alpha = 1.0f;
		basicEffect.VertexColorEnabled = true;
	}
	public void Draw(){
            basicEffect.Projection = _camera.projectionMatrix;
            basicEffect.View = _camera.viewMatrix;
            basicEffect.World = worldMatrix;
            
            gdcard.Clear(Color.White);
            
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.Solid;
            gdcard.RasterizerState = rasterizerState1;

            Vector3 lightDirection = _camera.camPosition;
            lightDirection.Normalize();
            basicEffect.LightingEnabled = true;
            basicEffect.PreferPerPixelLighting = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = lightDirection;
            basicEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            basicEffect.AmbientLightColor = new Vector3(0.4f);
			foreach(var entry in Entitytest){
				entry.Draw(gdcard,basicEffect);
			}

	}
	public void Update(InputManager iomanager){
			_camera.RotateY(30);		
	}
}
public class Kamera{
	
    public Vector3 camTarget{get;set;}
    public Vector3 camPosition{get;set;}
    public Matrix projectionMatrix{get;set;}//Focal Plane tzw
    public Matrix viewMatrix{get;set;}// faktyczna "pozycja" kamery
	public Kamera(){
	    camTarget = new Vector3(0f,0f,0f);
        camPosition = new Vector3(0f,0f,200f);
        viewMatrix = Matrix.CreateLookAt(camPosition,camTarget,Vector3.Up);
	}
	public void CreatePerspectiveFOV(float angle,GraphicsDevice gd, float nearplane, float farplane){
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(angle),gd.DisplayMode.AspectRatio,nearplane,farplane);
		
	}
	public void RotateY(float angle){
	    Matrix obrot = Matrix.CreateRotationY(MathHelper.ToRadians(angle));
        camPosition = Vector3.Transform(camPosition, obrot);
	}
}
