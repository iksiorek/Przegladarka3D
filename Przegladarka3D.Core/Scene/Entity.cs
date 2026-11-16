using Przegladarka3D.Core.Gui;
namespace Przegladarka3D.Core.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Entity: Object3D{

	struct VertexPositionColorNormal
	{
	    public Vector3 Position;
	    public Color Color;
	    public Vector3 Normal;
	    // specjalna klasa do definicji ulozenia w pamieci VRAM GPU 
	    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
	    (
	        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
	        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
	        new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
	    );
	}
	Matrix originModel;
	VertexPositionColorNormal[] mesh;
	VertexBuffer vertexBuffer;
	IndexBuffer indexbuffer;
	int[] indicies;
	public Entity(string modellocal, ConverterReader konwerter){
		
            var model = konwerter.STLReader(modellocal);
            mesh = new VertexPositionColorNormal[model.Item1.Length];
            for(int i = 0; i < model.Item1.Length; i++){
                mesh[i].Color = Color.Aqua;
                mesh[i].Position = model.Item1[i];
            }
            for(int i = 0; i <model.Item3.Length;i++){

             mesh[i].Normal = model.Item3[i];
            }


            for(int i = 0; i < mesh.Length;i++){
                mesh[i].Normal.Normalize();
            }
			indicies = model.Item2;
			pos3d = GetCenterBoundingBox() * new Vector3(-1,-1,-1);
			rotation = new Vector3(0,0,0);
			scale = new Vector3(1,1,1);
			originModel = Matrix.CreateScale(scale)*Matrix.CreateFromYawPitchRoll(rotation.X,rotation.Y,rotation.Z)*Matrix.CreateTranslation(pos3d.X,pos3d.Y,pos3d.Z);

    }
	public override void Draw(GraphicsDevice graphicsDevice, BasicEffect effect){
		
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormal.VertexDeclaration ,mesh.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(mesh);
            indexbuffer = new IndexBuffer(graphicsDevice,IndexElementSize.ThirtyTwoBits,indicies.Length, BufferUsage.WriteOnly);
            indexbuffer.SetData<int>(indicies,0,indicies.Length);            

			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.Indices = indexbuffer;
			effect.World = originModel;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
            pass.Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,(indicies.Length/3));
            }
	}
	public Vector3 GetCenterBoundingBox(){
		float xMin = float.MaxValue;
		float xMax = float.MinValue;
		float yMin = float.MaxValue;
		float yMax = float.MinValue;
		float zMin = float.MaxValue;
		float zMax = float.MinValue;
		for(int i  = 0; i< mesh.Length;i++){
				if(mesh[i].Position.X >= xMax){
					xMax = mesh[i].Position.X;					
				}else if(mesh[i].Position.X <= xMin){
					xMin = mesh[i].Position.X;
				}

				if(mesh[i].Position.Y >= yMax){
					yMax = mesh[i].Position.Y;					
				}else if(mesh[i].Position.Y <= yMin){
					yMin = mesh[i].Position.Y;
				}

				if(mesh[i].Position.Z >= zMax){
					zMax = mesh[i].Position.Z;					
				}else if(mesh[i].Position.Z <= zMin){
					zMin = mesh[i].Position.Z;
				}
		}
		return new Vector3((xMin+xMax)/2,(yMin+yMax)/2,(zMin+zMax)/2); 
	}
}
