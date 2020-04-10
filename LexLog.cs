// Decompiled with JetBrains decompiler
// Type: aLexicon.LexLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BC8425FC-8748-472E-A44F-7BBF6B7518D8
// Assembly location: D:\SteamLibrary\steamapps\common\Green Hell\GH_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace aLexicon
{
    public class LexLog : MonoBehaviour
    {
        private static bool ShowDebugLog = true;
        private static List<string> DebugLogs = new List<string>();
        private static List<string> APIUserIDCache = new List<string>();
        private static List<string> IPLogCache = new List<string>();
        public string targetFolder = "ExportedObj";
        private string command = string.Empty;
        private static Texture2D BoxTextures;
        private Vector2 DebugScrollPosition;
        private string avatarcache;
        private string playercache;
        private GUIStyle logsGuiStyle;
        private Texture2D boxTextures2;
        public int vertexOffset;
        public int normalOffset;
        public int uvOffset;

        public void Start()
        {
            VRFontScale = (float)Screen.height / 2160f;
        }

        public void Update()
        {
            if (Event.current.alt && Input.GetKeyDown(KeyCode.G))
                ShowDebugLog = !ShowDebugLog;
            if (!Input.GetKeyDown(KeyCode.Return))
                return;
            commandParser();
        }

        public string UintIPToString(uint ip)
        {
            return new IPAddress(new byte[4]
            {
        (byte) (ip >> 24 & (uint) byte.MaxValue),
        (byte) (ip >> 16 & (uint) byte.MaxValue),
        (byte) (ip >> 8 & (uint) byte.MaxValue),
        (byte) (ip & (uint) byte.MaxValue)
            }).ToString();
        }

        private bool IsNotPrivate(string ipAddress)
        {
            int[] array = ((IEnumerable<string>)ipAddress.Split(new string[1]
            {
        "."
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>((Func<string, int>)(s => int.Parse(s))).ToArray<int>();
            if (array[0] == 10 || array[0] == 192 && array[1] == 168)
                return false;
            if (array[0] == 172 && array[1] >= 16)
                return array[1] > 31;
            return true;
        }

        public void OnGUI()
        {
            GUIStyle style1 = new GUIStyle(GUI.skin.box);
            GUIStyle style2 = new GUIStyle(GUI.skin.box);
            logsGuiStyle = new GUIStyle(GUI.skin.box);
            logsGuiStyle.alignment = TextAnchor.MiddleLeft;
            logsGuiStyle.wordWrap = true;
            style1.alignment = TextAnchor.UpperRight;
            style2.alignment = TextAnchor.UpperRight;
            style1.wordWrap = true;
            if ((UnityEngine.Object)BoxTextures != (UnityEngine.Object)null)
            {
                style1.normal.background = BoxTextures;
                style2.normal.background = BoxTextures;
                style1.fontSize = 12;
                style2.fontSize = 12;
            }
            else
            {
                BoxTextures = new Texture2D(1, 1);
                BoxTextures.SetPixels(new Color[1]
                {
          new Color(0.45f, 0.45f, 0.45f, 0.45f)
                });
                BoxTextures.Apply();
            }
            if ((UnityEngine.Object)boxTextures2 != (UnityEngine.Object)null)
            {
                logsGuiStyle.normal.background = boxTextures2;
                logsGuiStyle.fontSize = 12;
            }
            else
            {
                boxTextures2 = new Texture2D(1, 1);
                boxTextures2.SetPixels(new Color[1]
                {
          new Color(0.35f, 0.35f, 0.45f, 0.6f)
                });
                boxTextures2.Apply();
            }
            if (!ShowDebugLog)
                return;
            Rect position = new Rect((float)Screen.width * 0.7f, (float)Screen.height * 0.63f, (float)Screen.width * 0.29f, (float)Screen.height * 0.31f);
            Rect screenRect = new Rect((float)Screen.width * 0.705f, (float)Screen.height * 0.66f, (float)Screen.width * 0.28f, (float)Screen.height * 0.27f);
            GUI.Box(position, "<b>Paradoxical*</b>", style2);
            GUILayout.BeginArea(screenRect);
            if (DebugLogs.Count > 0)
            {
                DebugScrollPosition = GUILayout.BeginScrollView(DebugScrollPosition, (GUILayoutOption[])Array.Empty<GUILayoutOption>());
                int num = 1;
                DebugScrollPosition.y = 9999f;
                if (DebugLogs.Count > 30)
                {
                    int index1 = DebugLogs.Count - 30;
                    for (int index2 = index1; index2 < DebugLogs.Count; ++index2)
                    {
                        string debugLog = DebugLogs[index1];
                        GUILayout.Box(string.Format("[{0}] {1}", (object)index1, (object)debugLog), style1, GUILayout.MaxWidth((float)Screen.width * 0.9f));
                        ++index1;
                    }
                }
                else
                {
                    foreach (string debugLog in DebugLogs)
                    {
                        GUILayout.Box(string.Format("[{0}] {1}", (object)num, (object)debugLog), style1, GUILayout.MaxWidth((float)Screen.width * 0.9f));
                        ++num;
                    }
                }
                GUILayout.EndScrollView();
            }
            command = GUILayout.TextField(command, logsGuiStyle, (GUILayoutOption[])Array.Empty<GUILayoutOption>());
            GUILayout.EndArea();
        }

        public void AddDebugLine(string line)
        {
            DebugLogs.Add("<b>" + line + "</b>");
        }

        public static LexLog Instance { get; private set; }

        private void Awake()
        {
            if ((UnityEngine.Object)Instance == (UnityEngine.Object)null)
                Instance = this;
            else if ((UnityEngine.Object)Instance != (UnityEngine.Object)this)
                Destroy((UnityEngine.Object)gameObject);
            DontDestroyOnLoad((UnityEngine.Object)gameObject);
        }

        public float VRFontScale { get; set; }

        private void commandParser()
        {
            if (!command.StartsWith("/")) return;

            void ParseCommand()
            {


                string[] args1 = command.Split(' ');
                args1[0] = args1[0].ToLower().Substring(1);


                switch (args1[0])
                {
                    default:
                        AddDebugLine("Command doesn't exist");
                        break;

                    case "invincible":
                    case "godmode":
                        Cheats.m_GodMode = !Cheats.m_GodMode;
                        AddDebugLine("God mode turned " + (Cheats.m_GodMode ? "on" : "off"));
                        break;
                    case "infdur":
                        Cheats.m_ImmortalItems = !Cheats.m_ImmortalItems;
                        AddDebugLine("God mode turned " + (Cheats.m_ImmortalItems ? "on" : "off"));
                        break;

                    case "give":
                        foreach (var item in Enum.GetNames(typeof(Enums.ItemID)))
                        {
                            if (item.IndexOf(args1[1], StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Player.Get().AddItemToInventory(item);
                                AddDebugLine("Giving Item " + item);
                                return;
                            }
                        }

                        AddDebugLine("Couldn't find item");
                        break;

                    case "spawnwave":
                        if (int.TryParse(args1[1], out int count))
                        {
                            AIs.EnemyAISpawnManager.Get().SpawnWave(count);
                            AddDebugLine($"Spawning wave of {count} enemies");
                            return;
                        }
                        AddDebugLine("Couldn't parse number");
                        break;

                }
            }

            ParseCommand();
        }

        private string MeshToString(
          MeshFilter mf,
          Dictionary<string, ObjMaterial> materialList)
        {
            Mesh sharedMesh = mf.sharedMesh;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("g ").Append(mf.name).Append("\n");
            foreach (Vector3 vertex in sharedMesh.vertices)
            {
                Vector3 vector3 = mf.transform.TransformPoint(vertex);
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", (object)-vector3.x, (object)vector3.y, (object)vector3.z));
            }
            stringBuilder.Append("\n");
            foreach (Vector3 normal in sharedMesh.normals)
            {
                Vector3 vector3 = mf.transform.TransformDirection(normal);
                stringBuilder.Append(string.Format("vn {0} {1} {2}\n", (object)-vector3.x, (object)vector3.y, (object)vector3.z));
            }
            stringBuilder.Append("\n");
            foreach (Vector2 vector2 in sharedMesh.uv)
            {
                Vector3 vector3 = (Vector3)vector2;
                stringBuilder.Append(string.Format("vt {0} {1}\n", (object)vector3.x, (object)vector3.y));
            }
            for (int submesh = 0; submesh < sharedMesh.subMeshCount; ++submesh)
            {
                stringBuilder.Append("\n");
                try
                {
                    ObjMaterial objMaterial = new ObjMaterial();
                    objMaterial.textureName = (string)null;
                    materialList.Add(objMaterial.name, objMaterial);
                }
                catch (ArgumentException ex)
                {
                }
                int[] triangles = sharedMesh.GetTriangles(submesh);
                for (int index = 0; index < triangles.Length; index += 3)
                    stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", (object)(triangles[index] + 1 + vertexOffset), (object)(triangles[index + 1] + 1 + normalOffset), (object)(triangles[index + 2] + 1 + uvOffset)));
            }
            vertexOffset += sharedMesh.vertices.Length;
            normalOffset += sharedMesh.normals.Length;
            uvOffset += sharedMesh.uv.Length;
            return stringBuilder.ToString();
        }

        private void Clear()
        {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }

        private Dictionary<string, ObjMaterial> PrepareFileWrite()
        {
            Clear();
            return new Dictionary<string, ObjMaterial>();
        }

        private void MeshToFile(MeshFilter mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
            using (StreamWriter streamWriter = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                streamWriter.Write("mtllib ./" + filename + ".mtl\n");
                streamWriter.Write(MeshToString(mf, materialList));
            }
        }

        private void MeshesToFile(MeshFilter[] mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
            using (StreamWriter streamWriter = new StreamWriter(folder + "/" + filename + ".obj"))
            {
                streamWriter.Write("mtllib ./" + filename + ".mtl\n");
                for (int index = 0; index < mf.Length; ++index)
                    streamWriter.Write(MeshToString(mf[index], materialList));
            }
        }

        private bool CreateTargetFolder()
        {
            try
            {
                Directory.CreateDirectory(targetFolder);
            }
            catch
            {
                AddDebugLine("Error! - Failed to create target folder!");
                return false;
            }
            return true;
        }

        private struct ObjMaterial
        {
            public string name;
            public string textureName;
        }
    }
}

