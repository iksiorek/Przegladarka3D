namespace Przegladarka3D.Core.Gui;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text;

public class ConverterReader{
	public ConverterReader(){
		
	}
	public (Vector3[],int[],Vector3[]) STLReader(string file){
	    Vector3[] pos;
	    UInt32 ntriangle;
	    int[] indicies;
	    if (File.Exists(file))
	    {
	        using (BinaryReader br = new BinaryReader(File.Open(file,FileMode.Open), Encoding.ASCII))
	        {
	            br.ReadBytes(80);                    // Naglowek(syf)
	            ntriangle = br.ReadUInt32();
	            pos = new Vector3[ntriangle*3];
	            indicies = new int[ntriangle*3];
	            Vector3[] normals = new Vector3[ntriangle*3];
	            Console.WriteLine("Liczba trojkatow:"+ ntriangle);
	            for(int i = 0; i < ntriangle; i++){
	                byte[] normalraw = br.ReadBytes(12); // normal do swiatla
	                float[] normal = {BitConverter.ToSingle(normalraw,0),
	                                                        BitConverter.ToSingle(normalraw,4),
	                                                        BitConverter.ToSingle(normalraw,8)
	                                                       };
	                byte[] vertex1raw = br.ReadBytes(12);
	                float[] vertex1 = {BitConverter.ToSingle(vertex1raw,0),
	                                BitConverter.ToSingle(vertex1raw,4),
	                                BitConverter.ToSingle(vertex1raw,8)
	                               };
	                pos[i*3]= new Vector3(vertex1[0],vertex1[1],vertex1[2]);
	                indicies[i*3] = i*3;
	                normals[i*3] = new Vector3(normal[0],normal[1],normal[2]);

	                byte[] vertex2raw = br.ReadBytes(12);
	                float[] vertex2 = {BitConverter.ToSingle(vertex2raw,0),
	                                BitConverter.ToSingle(vertex2raw,4),
	                                BitConverter.ToSingle(vertex2raw,8)
	                               };
	                pos[i*3+1]= new Vector3(vertex2[0],vertex2[1],vertex2[2]);
	                indicies[i*3+1] = i*3+1;
	                normals[i*3+1] = new Vector3(normal[0],normal[1],normal[2]);

	                byte[] vertex3raw = br.ReadBytes(12);
	                float[] vertex3 = {BitConverter.ToSingle(vertex3raw,0),
	                                BitConverter.ToSingle(vertex3raw,4),
	                                BitConverter.ToSingle(vertex3raw,8)
	                               };
	                pos[i*3+2]= new Vector3(vertex3[0],vertex3[1],vertex3[2]);
	                indicies[i*3+2] = i*3+2;
	                normals[i*3+2] = new Vector3(normal[0],normal[1],normal[2]);

	            br.ReadBytes(2); // Atrybuty ?
	            } 
	            return (pos, indicies, normals);
	        }

	    }
	    return (null,null,null);
	}
} 
