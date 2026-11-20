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
		basicEffect = new BasicEffect(gdcard);
		basicEffect.Alpha = 0.5f;
		basicEffect.VertexColorEnabled = true;
	}
	public void Draw(){
            basicEffect.Projection = _camera.projectionMatrix;
            basicEffect.View = _camera.viewMatrix;
            //basicEffect.World = worldMatrix;
            
            gdcard.Clear(Color.White);
            
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.Solid;
            gdcard.RasterizerState = rasterizerState1;

            Vector3 lightDirection = -(_camera.camPosition - _camera.camTarget);
            lightDirection.Normalize();
            basicEffect.LightingEnabled = true;
            basicEffect.PreferPerPixelLighting = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = lightDirection;
            basicEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            basicEffect.AmbientLightColor = new Vector3(0.2f);
			foreach(var entry in Entitytest){
				entry.Draw(gdcard,basicEffect);
			}

	}
	public void Update(InputManager iomanager){
		if(iomanager.IsKeyDown(Keys.Left)){
			_camera.Rotate(1f,0,0);
		}
		if(iomanager.IsKeyDown(Keys.Right)){
			_camera.Rotate(-1f,0,0);
		}
		if(iomanager.IsKeyDown(Keys.Up)){
			_camera.Rotate(0,1f,0);
		}
		if(iomanager.IsKeyDown(Keys.Down)){
			_camera.Rotate(0,-1f,0);
		}
		if(iomanager.IsKeyPressed(Keys.A)){
			_camera.Zoom(10);
		}
		if(iomanager.IsKeyPressed(Keys.Z)){
			_camera.Zoom(-10);
		}
		if(iomanager.IsKeyDown(Keys.Q)){
			_camera.Rotate(0,0,1);
		}
		if(iomanager.IsKeyDown(Keys.E)){
			_camera.Rotate(0,0,-1);
		}
	}
}
public class Kamera{
	
    public Vector3 camTarget{get;set;}
    public Vector3 camPosition{get;set;}
    public Matrix projectionMatrix{get;set;}
    public Matrix viewMatrix{get;set;}
	public Kamera(){
	    camTarget = new Vector3(0f,0f,0f);
        camPosition = new Vector3(0f,0f,500f);
        viewMatrix = Matrix.CreateLookAt(camPosition,camTarget,Vector3.Up);
	}
	public void CreatePerspectiveFOV(float angle,GraphicsDevice gd, float nearplane, float farplane){
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(angle),gd.DisplayMode.AspectRatio,nearplane,farplane);
		
	}
	public void Rotate(float yaw, float pitch, float roll){
	    Matrix obrot = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw),MathHelper.ToRadians(pitch),MathHelper.ToRadians(roll));
        camPosition = Vector3.Transform(camPosition, obrot);
		viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
	}
	public void Zoom(float val){

	    Vector3 direction = camTarget - camPosition; 
	    float distance = direction.Length();
	    if (distance <= 0.0001f) return;
	    direction.Normalize();
	    float newDistance = distance - val;

	    float minDistance = 5f;    
	    float maxDistance = 900f;  
	    newDistance = MathHelper.Clamp(newDistance, minDistance, maxDistance);

	    camPosition = camTarget - direction * newDistance;
	    viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
	}
}
