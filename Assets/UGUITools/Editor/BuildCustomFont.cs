using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class BuildCustomFontWindow: EditorWindow{	
	/// <summary>
	/// The build path.
	/// </summary>
	private const string buildPath = "Assets/Fonts/";

	private string m_fontName = "CustomFont";
	private Texture2D m_fontTex = null;
	private TextAsset m_fontInfo = null;
	private Font m_font = null;

	[MenuItem("Tools/Font/BuildCustomFont")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow (typeof(BuildCustomFontWindow));
	}

	protected bool BuildCustomFont()
	{
		if (!Directory.Exists (buildPath)) {
			Directory.CreateDirectory (buildPath);
		}

		// create material
		Material material = new Material (Shader.Find ("UI/Default Font"));
		material.mainTexture = m_fontTex;
		AssetDatabase.CreateAsset (material, buildPath + m_fontName + ".mat");
		AssetDatabase.Refresh ();

		// create font
		m_font = new Font();
		AssetDatabase.CreateAsset (m_font, buildPath + m_fontName + ".fontsettings");
		AssetDatabase.Refresh ();

		// get fnt info
		XmlDocument xml = new XmlDocument();
		xml.LoadXml (m_fontInfo.text);
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

			float texWidth = m_fontTex.width;
			float texHeight = m_fontTex.width;

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
		m_font.characterInfo = chtInfoList.ToArray();
		m_font.material = material;

		EditorUtility.SetDirty (m_font);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();

		AssetDatabase.LoadAssetAtPath<Font> (buildPath + m_fontName + ".fontsettings");


		return true;
	}
		
	protected void OnGUI()
	{
		m_fontTex = (Texture2D) EditorGUILayout.ObjectField("Font Image", m_fontTex, typeof(Texture2D));
		m_fontInfo = (TextAsset) EditorGUILayout.ObjectField("Font Info", m_fontInfo, typeof(TextAsset));
		m_fontName = EditorGUILayout.TextField ("Font Name", m_fontName);

		if (m_fontTex != null && m_fontInfo != null) {
			if (GUILayout.Button ("Build")) {
				if (BuildCustomFont ()) {
					Debug.Log ("BuildCustomFont >> Buil Successful");
					this.Close ();
				}
			}
		} else {
			if (m_fontTex == null) {
				GUILayout.Label ("Please Select Font Texture");
			} else {
				GUILayout.Label ("Please Select Font Info(fnt file)");
			}
		}
	}
}