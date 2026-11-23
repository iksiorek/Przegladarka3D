using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
		if(iomanager.GetButtonMouseState(MouseButton.Left)){
			_camera.BeginArcball(iomanager.GetMousePos());
			_camera.UpdateArcball(iomanager.GetMousePos(),gdcard.Viewport.Width,gdcard.Viewport.Height );
		}else _camera.EndArcball();
		_camera.Zoom(iomanager.GetMouseScrollDelta());
	}
}
public class Kamera{
	
    public Vector3 camTarget{get;set;}
    public Vector3 camPosition{get;set;}
    public Matrix projectionMatrix{get;set;}
    public Matrix viewMatrix{get;set;}

    Vector3 _startDirection;
	float _radius;
	Quaternion _orientation = Quaternion.Identity;
	Quaternion _baseOrientation;
	Vector2 _dragStart;
	bool _dragging;
	float ArcballSensitivity = 3f;
	float zoomSensitivity = 2f;

	public Kamera(){
	    camTarget = new Vector3(0f,0f,0f);
        camPosition = new Vector3(0f,0f,500f);
		Vector3 startOffset = camPosition - camTarget;
		_startDirection = Vector3.Normalize(startOffset);
		_radius = startOffset.Length();
		_baseOrientation = _orientation;
		UpdateView();
	}
	public void CreatePerspectiveFOV(float angle,GraphicsDevice gd, float nearplane, float farplane){
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(angle),gd.Viewport.AspectRatio,nearplane,farplane);
		
	}

	private void UpdateView(){
		camPosition = camTarget + Vector3.Transform(_startDirection * _radius, _orientation);
		Vector3 up = Vector3.Transform(Vector3.Up, _orientation);
		viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, up);
	}
	public void BeginArcball(Vector2 mouse){
		if(_dragging) return;
		_dragging = true;
		_dragStart = mouse;
		_baseOrientation = _orientation;
	}

	public void UpdateArcball(Vector2 mouse, int width, int height){
		if (!_dragging || width <= 1 || height <= 1) return;
		Quaternion drag = GetArcballDelta(_dragStart, mouse, width, height);

		_orientation = Quaternion.Normalize(_baseOrientation * drag);
		UpdateView();
	}

	public void EndArcball(){
		_dragging = false;
	}

	//rzutowanie ruchu myszy na ruch po "kuli"
	private Vector3 ProjectToSphere(Vector2 mousePos, int width, int height){
	    float scale = (Math.Min(width, height) - 1f) / ArcballSensitivity;
		float x = (2f * mousePos.X - width + 1f) / scale;
		float y = (height - 1f - 2f * mousePos.Y) / scale;

		float radial2 = x * x + y * y;
		const float radius2 = 1f;
		float z = (2f * radial2 <= radius2) ? -MathF.Sqrt(radius2 - radial2) : -(radius2 * 0.5f) / MathF.Sqrt(radial2);

		return Vector3.Normalize(new Vector3(x, y, z));
	}

	public Quaternion GetArcballDelta(Vector2 fromMouse, Vector2 toMouse, int width, int height){
	    Vector3 v1 = ProjectToSphere(fromMouse, width, height);
	    Vector3 v2 = ProjectToSphere(toMouse, width, height);

	    Vector3 axis = Vector3.Cross(v1, v2);
		axis.Z = -axis.Z; 
	    if (axis.LengthSquared() < 1e-12f)
	        return Quaternion.Identity;

		float cosTheta = MathHelper.Clamp(
			Vector3.Dot(v1, v2) / (v1.Length() * v2.Length()), -1f, 1f);
	    float angle = MathF.Acos(cosTheta);
	    return Quaternion.CreateFromAxisAngle(Vector3.Normalize(axis), angle);
	}


	

	public void Zoom(float val){
		_radius = MathHelper.Clamp(_radius - val/zoomSensitivity, 5f, 900f);
		UpdateView();
	}
}
