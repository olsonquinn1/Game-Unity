using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Threading;

enum Image
{
    Width = 360,
    Height = 360
}

public class GUIEasyFormat
{
    private string[] delStr = { "||", ">>>" };

    public float GUIBoxMaker(float posx, float posy, string formatstring)
    {
        //GUILayout.BeginArea(new Rect(120, 10, 45, 100));
        return 0;
    }

    /*public void stringParse(string instr, Object[] vars)
    {
        string[] div = instr.Split(delStr[0], System.StringSplitOptions.RemoveEmptyEntries);
        string[][] fin = 
        foreach(string inst in div)
        {
            string[] div2 = inst.Split(delStr[1], System.StringSplitOptions.RemoveEmptyEntries);
            switch (div2[0])
            {
                case "Output":break;

                case "Input":break;
                default:break;
            }
        }
    }*/
}

public class NoiseFile
{
    public string name;
    public float a, b, c, d;
    public float amp;
    public string seed;

    public NoiseFile(string namein, float ain, float bin, float cin, float din, float ampin, string seedin)
    {
        name = string.Copy(namein);
        a = ain;
        b = bin;
        c = cin;
        d = din;
        amp = ampin;
        seed = string.Copy(seedin);
    }

    public NoiseFile()
    {
        name = "";
        a = 1;
        b = 0;
        c = 0; 
        d = 1;
        amp = 1;
        seed = "";
    }

    public float[] GetMat()
    {
        float[] matout = new float[4];
        matout[0] = a;
        matout[1] = b;
        matout[2] = c;
        matout[3] = d;
        return matout;
    }

    public NoiseFile Copy()
    {
        NoiseFile out1 = new NoiseFile(name, a, b, c, d, amp, seed);
        return out1;
    }

    override public int GetHashCode()
    {
        int out1 = name.GetHashCode();
        return out1;
    }

    public uint GetUHashCode(uint max)
    {
        uint out1 = (uint)name.GetHashCode();
        out1 = out1 % max;
        return out1;
    }

    public static uint GetUHashCode(string inname, uint max)
    {
        uint out1 = (uint)inname.GetHashCode();
        out1 = out1 % max;
        return out1;
    }

    public bool Equals(NoiseFile test)
    {
        float[] Arr = test.GetMat();
        bool t0 = string.Equals(name, test.name);
        bool t1 = (a == Arr[0]);
        bool t2 = (b == Arr[1]);
        bool t3 = (c == Arr[2]);
        bool t4 = (d == Arr[3]);
        bool t5 = string.Equals(seed, test.seed);
        return (t0 & t1 & t2 & t3 & t4 & t5);
    }

    public void Clear()
    {
        name = "";
        a = 1;
        b = 0;
        c = 0;
        d = 1;
        seed = "";
    }
}

public class PelinNoiseImageGenerator : EditorWindow
{

    float a = 1;
    float b = 0;
    float c = 0;
    float d = 1;
    float amp = 1;
    float h = 0;
    float s = 0;
    float v = 0;
    bool stop = false;

    Thread t2;

    float SBpos = 0;

    string seed = "";
    string prevseed = "";

    private NoiseFile[] fileArr = new NoiseFile[256];

    Color chooser;
    Texture2D PerlinOut;
    Color[] pix;

    AnimationCurve filter = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField]
    string current = "";

    

    [MenuItem("Window/Perlin Noise")]
    public static void ShowWindow()
    {
        GetWindow<PelinNoiseImageGenerator>("Perlin Noise");
    }
    //window code:
    //handles diplaying menus and handling gui generation
    void OnEnable()
    {
        PerlinOut = new Texture2D((int)Image.Width, (int)Image.Height);
        pix = new Color[(int)Image.Width * (int)Image.Height];
        chooser = new Color();

        t2 = new Thread(new ThreadStart(ImageGenSingle));
    }

    public void import()
    {

    }

    public void add()
    {
        NoiseFile temp = new NoiseFile(current, a, b, c, d, amp, seed);
        int i = (int)temp.GetUHashCode(256);
        if (fileArr[i] == null)
            fileArr[i] = temp;
        else
        {
            bool test = false;
            while (!test)
            {
                if (string.Equals(fileArr[i].name, current))
                {
                    fileArr[i] = temp;
                    test = true;
                }
                i++;
            }
        }
    }

    public void remove()
    {

    }

    public void save()
    {

    }

    public void saveas()
    {

    }

    void PerlinEditor()
    {
        GUILayoutOption matBoxes;
        matBoxes = GUILayout.Width(45);

        GUILayout.BeginArea(new Rect(120, 10, 45, 100));
        GUILayout.Label("|x|X|a b|");
        GUILayout.Label("|y|X|c d|");
        a = EditorGUILayout.FloatField(a, matBoxes);
        c = EditorGUILayout.FloatField(c, matBoxes);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(175, 10, 100, 100));
        GUILayout.Label("|x*a+y*b|");
        GUILayout.Label("|x*a+y*b|");
        b = EditorGUILayout.FloatField(b, matBoxes);
        d = EditorGUILayout.FloatField(d, matBoxes);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(285, 10, 400, 100));
        GUILayout.Label("name");
        current = EditorGUILayout.TextField(current, GUILayout.Width(400));
        GUILayout.Label("seed");
        seed = EditorGUILayout.TextField(seed, GUILayout.Width(400));
        int hash = seed.GetHashCode();
        GUILayout.Label($"hash = {hash}");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(695, 10, 400, 100));
        amp = EditorGUILayout.Slider("amplitude:", amp, 0, 1);
        h = EditorGUILayout.Slider("h:", h, 0, 1);
        s = EditorGUILayout.Slider("s:", s, 0, 1);
        v = EditorGUILayout.Slider("v:", v, 0, 1);
        chooser = Color.HSVToRGB(h, s, v);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 10, 100, 100));
        filter = EditorGUILayout.CurveField(filter, GUILayout.Height(100));
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 120, 640, 360));
        GUILayout.Label(PerlinOut);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(660, 120, 360, 360));
        chooser = EditorGUILayout.ColorField(chooser, GUILayout.Width(360), GUILayout.Height(360));
        float s2;
        float h2;
        Color.RGBToHSV(chooser, out h2, out s2, out v);
        if (v != 0)
        {
            h = h2;
            s = s2;
        }
        GUILayout.EndArea();


        float xoff = 0;
        float yoff = 0;

        //int hash = seed.GetHashCode();

        var rnd = new System.Random(hash);

        xoff = (float)(rnd.NextDouble() * 65536);
        yoff = (float)(rnd.NextDouble() * 65536);
        if ((PerlinOut.height > 0) && (PerlinOut.width > 0))
        {
            prevseed = string.Copy(seed);
            float a2;
            float b2;
            float c2;
            float d2;

            if ((a * d - b * c) != 0)
            {
                a2 = d / (a * d - b * c);
                b2 = -b / (a * d - b * c);
                c2 = -c / (a * d - b * c);
                d2 = a / (a * d - b * c);
            }
            else
            {
                a2 = d;
                b2 = -b;
                c2 = -c;
                d2 = a;
            }

            for (int y = 0; y < PerlinOut.height; y++)
            {
                for (int x = 0; x < PerlinOut.width; x++)
                {
                    float x2 = (float)(x * a2 + y * b2) * (float)Math.PI / 100.0f + (float)xoff;
                    float y2 = (float)(x * c2 + y * d2) * (float)Math.PI / 100.0f + (float)yoff;
                    float samp = Mathf.PerlinNoise(x2, y2);

                    samp = filter.Evaluate(samp);

                    pix[y * PerlinOut.width + x] = Color.HSVToRGB(h, s, v * (1 - amp) + amp * samp);
                }
            }
            

            // matrix vector drawer
            float mag1 = (float)Math.Sqrt((double)(a * a + c * c));
            float mag2 = (float)Math.Sqrt((double)(b * b + d * d));
            Debug.Log($"mag1:{mag1}\nmag2:{mag2}");
            float maxmag = Math.Max(mag1, mag2);

            int width = PerlinOut.width;

            Vector2 tempvec = new Vector2(1.0f, 1.0f);
            //drawing x translation vector
            if (mag1 > 1.0f)
            {
                tempvec.x = a;
                tempvec.y = c;

                drawVec(tempvec, 170.0f * mag1 / maxmag, pix, new Color(0, 1, 0, 1), width);

                tempvec.x = 1;
                tempvec.y = 0;

                drawVec(tempvec, 170.0f / maxmag, pix, new Color(0, 0, 1, 1), width);
            }
            else
            {
                tempvec.x = 1;
                tempvec.y = 0;

                drawVec(tempvec, 170.0f / maxmag, pix, new Color(0, 0, 1, 1), width);

                tempvec.x = a;
                tempvec.y = c;

                drawVec(tempvec, 170.0f * mag1 / maxmag, pix, new Color(0, 1, 0, 1), width);
            }

            //drawing y translation vectors
            if (mag2 > 1.0f)
            {
                tempvec.x = b;
                tempvec.y = d;

                drawVec(tempvec, 170.0f * mag2 / maxmag, pix, new Color(1, 0, 0, 1), width);

                tempvec.x = 0;
                tempvec.y = 1;

                drawVec(tempvec, 170.0f / maxmag, pix, new Color(0, 0, 1, 1), width);
            }
            else
            {
                tempvec.x = 0;
                tempvec.y = 1;

                drawVec(tempvec, 170.0f / maxmag, pix, new Color(0, 0, 1, 1), width);

                tempvec.x = b;
                tempvec.y = d;

                drawVec(tempvec, 170.0f * mag2 / maxmag, pix, new Color(1, 0, 0, 1), width);
            }

            PerlinOut.SetPixels(pix);
            PerlinOut.Apply();
        }
    }

    private void ImageGenSingle()
    {

    }

    private static void drawVec(Vector2 vec, float mag, Color[] image, Color col, int width)
    {
        Vector2 temp = Vector2.ClampMagnitude(vec, mag);
        temp.Normalize();
        temp = Vector2.Scale(temp, new Vector2(mag, mag));

        float r = col.r;
        float g = col.g;
        float b = col.b;
        float a = col.a;

        //Bresenham Line Algorithm
        int x0 = 0;
        int y0 = 0;
        int x1 = (int)Math.Floor(temp.x);
        int y1 = (int)Math.Floor(temp.y);

        int dx = Math.Abs(x1 - x0);
        int sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0);
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;  /* error value e_xy */

        while(true)
        {
            image[(y0 + 180) * width + (x0 + 180)] = new Color(r, g, b, a);
            if (x0 == x1 && y0 == y1)
                break;
            int e2 = 2 * err;
            if (e2 >= dy) /* e_xy+e_x > 0 */
            {
                err += dy;
                x0 += sx;
            }
            if (e2 <= dx) /* e_xy+e_y < 0 */
            {
                err += dx;
                y0 += sy;
            }
        }
    }


    //runs the GUI generation software and checks for and manages inputs
    void OnGUI()
    {

        Rect r = new Rect(120, 90, 100, 20);
        if (EditorGUI.DropdownButton(r, new GUIContent("Noise List","Add, remove, and manage existing Perlin Noise sets"), FocusType.Passive))
        {

            GUI.FocusControl(null);

            GenericMenu menu = new GenericMenu();

            var f2 = new GenericMenu.MenuFunction(() =>
            {
                import();
            });
            menu.AddItem(new GUIContent("import"), false, f2);
            var f3 = new GenericMenu.MenuFunction(() =>
            {
                add();
            });
            menu.AddItem(new GUIContent("add"), false, f3);
            var f4 = new GenericMenu.MenuFunction(() =>
            {
                remove();
            });
            menu.AddItem(new GUIContent("remove"), false, f4);
            var f5 = new GenericMenu.MenuFunction(() =>
            {
                save();
            });
            menu.AddItem(new GUIContent("save"), false, f5);
            var f6 = new GenericMenu.MenuFunction(() =>
            {
                saveas();
            });
            menu.AddItem(new GUIContent("save as"), false, f6);

            for (int i = 0; i < 256; i++)
            {
                if (fileArr[i] != null)
                {
                    addNoise(menu, fileArr[i].name);
                }
            }

            menu.DropDown(r);
        }
        //window code
        //SBpos = GUILayout.VerticalScrollbar(SBpos,);
        PerlinEditor();
    }

    //adds menu items for selecting sound files
    public void addNoise(GenericMenu menu, string noisefile)
    {
        var func = new GenericMenu.MenuFunction2((name) =>
        {
            Load(name);
        });
        menu.AddItem(new GUIContent($"Load/{noisefile}"), string.Equals(current, noisefile), func, noisefile);
    }

    public void Load(object next)
    {
        string input = (string)next;
        int i = (int)NoiseFile.GetUHashCode(input, 256);
        int iST = i;
        while (!string.Equals(fileArr[i].name, input))
        {
            i = (i + 1) % 256;
            if ((fileArr[i] == null) || (iST == i))
                return;
        }
        NoiseFile temp = fileArr[i].Copy();
        i = (int)NoiseFile.GetUHashCode(current, 256);
        bool test = true;

        while(test)
        {
            if (fileArr[i] == null)
                test = false;
            else if (string.Equals(fileArr[i].name, current))
                test = false;
            else
                i = (i + 1) % 256;
        }
        if (fileArr[i] != null)
            fileArr[i] = new NoiseFile(current, a, b, c, d, amp, seed);

        current = string.Copy(input);
        float[] arr = temp.GetMat();
        a = arr[0];
        b = arr[1];
        c = arr[2];
        d = arr[3];
        amp = temp.amp;
        seed = string.Copy(temp.seed);
    }

    //returns -1 on error
    private static int scroll()
    {
        return 0;
    }


    //generates the file menu dropdown
    private void FileMenu()
    {
        var func = new GenericMenu.MenuFunction2((num) =>
        {
            FileMenuAct(num);
        });
        
    }

    //file menu action controller
    private void FileMenuAct(object input)
    {
        int choice = (int)input;

    }
}

