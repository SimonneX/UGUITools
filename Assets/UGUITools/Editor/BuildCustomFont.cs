using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class BuildCustomFontWindow: EditorWindow{
	private string m_fontName = "CustomFont";
	private const string buildPath = "Assets/Fonts/";

	private Texture2D fontImage;
	private TextAsset fontInfo;

	[MenuItem("Tools/Font/BuildCustomFont")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(BuildCustomFontWindow));
	}

	bool BuildCustomFont()
	{
		if (!Directory.Exists (buildPath)) {
			Directory.CreateDirectory (buildPath);
		}

		// create material
		Material material = new Material (Shader.Find ("UI/Default Font"));
		material.mainTexture = fontImage;
		AssetDatabase.CreateAsset (material, buildPath + m_fontName + ".mat");

		// create font
		Font font = new Font();
		AssetDatabase.CreateAsset (font, buildPath + m_fontName + ".fontsettings");

		// get fnt info
		XmlDocument xml = new XmlDocument();
		xml.LoadXml (fontInfo.text);
		List<CharacterInfo> chtInfoList = new List<CharacterInfo>();
		XmlNode node = xml.SelectSingleNode("font/chars");
		foreach (XmlNode nd in node.ChildNodes)
		{
			XmlElement xe = (XmlElement)nd;
			int x = int.Parse (xe.GetAttribute ("x"));
			int y = int.Parse (xe.GetAttribute ("y"));
			int width = int.Parse (xe.GetAttribute ("width"));
			int height = int.Parse (xe.GetAttribute ("height"));
			int advance = int.Parse (xe.GetAttribute ("xadvance"));
			int offsetX = int.Parse (xe.GetAttribute ("xoffset"));
			int offsetY = int.Parse (xe.GetAttribute ("yoffset"));

			float texWidth = fontImage.width;
			float texHeight = fontImage.width;

			CharacterInfo info = new CharacterInfo();
			info.index = int.Parse(xe.GetAttribute("id"));
			info.uv.x = (float)x / (float)texWidth;
			info.uv.y = 1 - (float)y / (float)texHeight;
			info.uv.width = (float)width / (float)texWidth;
			info.uv.height = -1f * (float)height / (float)texHeight;
			info.vert.x = offsetX;
			info.vert.y = 0;
			info.vert.width = (float)width;
			info.vert.height = (float)height;
			info.width = (float)advance;

			chtInfoList.Add(info);
		}
		// config font
		font.characterInfo = chtInfoList.ToArray();
		font.material = material;

	
//		AssetDatabase.Refresh ();
		return true;
	}

		
	void OnGUI()
	{
		fontImage = (Texture2D) EditorGUILayout.ObjectField("Font Image", fontImage, typeof(Texture2D));
		fontInfo = (TextAsset) EditorGUILayout.ObjectField("Font Info", fontInfo, typeof(TextAsset));
		m_fontName = EditorGUILayout.TextField ("Font Name", m_fontName);

		if (fontImage != null && fontInfo != null) {
			if (GUILayout.Button ("Build")) {
				if (BuildCustomFont ()) {
					Debug.Log ("Buil Successful");
					this.Close ();
				}
			}
		} else {
			if (fontImage == null) {
				GUILayout.Label ("Please Select Font Image");
			} else {
				GUILayout.Label ("Please Select Font Info");
			}
		}
	}
}