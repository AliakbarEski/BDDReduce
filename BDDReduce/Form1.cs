using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//TODO: fix remove equal nodes function, fix lines from centres (use polar coords :D), fix text (size, coords)
//make circle_radius dependent on the levels and the root coords on the expanse of the tree

//#ThugLife - bacho ki barbaadi


namespace BDDReduce
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //initStdFuncs();
        }

        //Global Variables declaration
        public class Node
        {
            public List<string> TheSet;
            public Node LeftChild;
            public Node RightChild;
            public Node Parent;
            public string IsStdFunc;
            public bool IsTerminal;
            public string expandedInput;
            public int numChildren;
            public int nodeLevel;
            public List<int> UID;

            //Drawing Variables
            public Point NodeCoordinates;

            public Node()
            {
                this.TheSet = new List<string>();
                this.LeftChild = null;
                this.RightChild = null;
                this.Parent = null;
                this.IsStdFunc = "No";
                this.IsTerminal = false;
                this.expandedInput = "";
                this.numChildren = 0;
                this.nodeLevel = 0;
                this.UID = new List<int>();

                //Drawing 
                NodeCoordinates = new Point(350, 50);
            }
        }

        string FileName = "";
        int numinputs = 0;
        List<string> ON_Set = new List<string>(); // ON set from the blif file
        List<string> DC_Set = new List<string>(); // DC set from the blif file
        List<string> OFF_Set = new List<string>(); // OFF set from the blif file
        List<int> OFF_Set_decimal = new List<int>(); // OFF set 
        List<List<string>> StdFuncs = new List<List<string>>();
        Node Root = new Node();
        Node Root1 = new Node(); //for Apply 
        Node Root0 = new Node(); //for Apply
        Node RootApplied = new Node();
        List<Node> AND = new List<Node>(), OR = new List<Node>(), NOR = new List<Node>(), XOR = new List<Node>(), NAND = new List<Node>(), XNOR = new List<Node>(), terminal0 = new List<Node>(), terminal1 = new List<Node>();
       //List<Node> ANDRC = new List<Node>(), NANDRC = new List<Node>(), ORLC = new List<Node>(), XORLC = new List<Node>(), XORRC = new List<Node>(), NORLC = new List<Node>();
        List<Node> x = new List<Node>(), xbar = new List<Node>();
        System.Drawing.Graphics Canvas;
        Pen BDDpen;
        int x_spacing = 175, y_spacing = 100;
        int circleRadius = 30;
        int maxNoLevels = 0;
        int offset = 0;
        int extendsLeft = 0, extendsRight = 0;

        void initLists()
        {
            AND.Add(new Node());
            OR.Add(new Node());
            NOR.Add(new Node());
            XOR.Add(new Node());
            XNOR.Add(new Node());
            NAND.Add(new Node());
            terminal0.Add(new Node());
            terminal1.Add(new Node());
            x.Add(new Node());
            xbar.Add(new Node());
        }

        void initStdFuncs(int whichTree)
        {
            //AND, OR, NOR XOR, NAND
            List<string> temp = new List<string>();
            temp.Add("11");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("01");
            temp.Add("11");
            temp.Add("10");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("00");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("01");
            temp.Add("10");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("10");
            temp.Add("00");
            temp.Add("01");
            StdFuncs.Add(temp);


            temp = new List<string>();
            temp.Add("11");
            temp.Add("00");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("1");
            StdFuncs.Add(temp);

            temp = new List<string>();
            temp.Add("0");
            StdFuncs.Add(temp);

            //Terminal 0
            terminal0[whichTree].IsTerminal = true;
            terminal0[whichTree].TheSet.Add("0");
            terminal0[whichTree].NodeCoordinates = new Point(Root.NodeCoordinates.X - 150, 450);

            //Terminal 1
            terminal1[whichTree].IsTerminal = true;
            terminal1[whichTree].TheSet.Add("1");
            terminal1[whichTree].NodeCoordinates = new Point(Root.NodeCoordinates.X + 150, 450);

            //x
            x[whichTree].TheSet.Add("1");
            x[whichTree].LeftChild = terminal0[whichTree];
            x[whichTree].RightChild = terminal1[whichTree];
            x[whichTree].numChildren = 2;
            //x.IsStdFunc = "x";

            //xbar
            xbar[whichTree].TheSet.Add("0");
            xbar[whichTree].LeftChild = terminal1[whichTree];
            xbar[whichTree].RightChild = terminal0[whichTree];
            xbar[whichTree].numChildren = 2;
            //xbar.IsStdFunc = "xbar";

            //AND BDD
            AND[whichTree].LeftChild = terminal0[whichTree];
            AND[whichTree].RightChild = x[whichTree];
            AND[whichTree].numChildren = 2;
            //ANDRC.LeftChild = terminal0;
            //ANDRC.RightChild = terminal1;

            //OR BDD
            OR[whichTree].LeftChild = x[whichTree];
            OR[whichTree].RightChild = terminal1[whichTree];
            OR[whichTree].numChildren = 2;
            //ORLC.LeftChild = terminal0;
            //ORLC.RightChild = terminal1;

            //XOR BDD
            XOR[whichTree].LeftChild = x[whichTree];
            XOR[whichTree].RightChild = xbar[whichTree];
            XOR[whichTree].numChildren = 2;
            //XORLC.LeftChild = terminal0;
            //XORLC.RightChild = terminal1;
            //XORRC.LeftChild = terminal1;
            //XORRC.RightChild = terminal0;

            //NAND BDD
            NAND[whichTree].LeftChild = terminal1[whichTree];
            NAND[whichTree].RightChild = xbar[whichTree];
            NAND[whichTree].numChildren = 2;
            //NANDRC.LeftChild = terminal1;
            //NANDRC.RightChild = terminal0;

            //NOR BDD
            NOR[whichTree].LeftChild = xbar[whichTree];
            NOR[whichTree].RightChild = terminal0[whichTree];
            NOR[whichTree].numChildren = 2;
            //NORLC.LeftChild = terminal1;
            //NORLC.RightChild = terminal0;

            //XNOR BDD TODO

        }

        bool compareLists(List<string> List1, List<string> List2)
        {
            bool quit = false;
            if (List1.Count == List2.Count)
            {
                bool compare;
                quit = true;
                for (int i1 = 0; i1 < List1.Count; i1++)
                {
                    compare = false;
                    for (int j1 = 0; j1 < List2.Count; j1++)
                    {
                        if (List1[i1] == List2[j1])
                        {
                            compare = true;
                        }
                    }
                    quit = quit & compare;
                }
            }
            return quit;
        }

        int IsStdFunc(List<string> List1)
        {
            bool quit;
            int r = 10;

            for (int i = 0; i < 6; i++)
            {

                if (List1.Count == StdFuncs[i].Count)
                {
                    bool compare;
                    quit = true;
                    for (int i1 = 0; i1 < List1.Count; i1++)
                    {
                        compare = false;
                        for (int j1 = 0; j1 < StdFuncs[i].Count; j1++)
                        {
                            if (List1[i1] == StdFuncs[i][j1])
                            {
                                compare = true;
                            }
                        }
                        quit = quit & compare;

                    }
                    if (quit == true)
                    {
                        r = i;
                    }
                }
            }

            return r;

        }

        void makeBDD(Node node, int level, int extends, int whichChild, int whichTree)// -1 for left and 1 for right
        {
            List<string> SetToReduce = new List<string>(node.TheSet);
            List<string> CoFactor0 = new List<string>();
            List<string> CoFactor1 = new List<string>();

            if (level > maxNoLevels)
                maxNoLevels = level;

            if (extendsLeft > extends)
                extendsLeft = extends;

            if (extendsRight < extends)
                extendsRight = extends;

            node.nodeLevel = level;

            if (SetToReduce.Count == 0) return;

            int a = IsStdFunc(SetToReduce);

            if (SetToReduce.Count == 1 && SetToReduce[0] == "0") //single element
            {
                //if (SetToReduce[0] == "0")
                /*{
                    Node LC = new Node();
                    Node RC = new Node();

                    LC.TheSet.Add("1");
                    RC.TheSet.Add("0");

                    LC.IsTerminal = true;
                    RC.IsTerminal = true;

                    node.LeftChild = LC;
                    node.RightChild = RC;
                    node.numChildren = 2;

                    LC.Parent = node;
                    RC.Parent = node;

                    LC.NodeCoordinates = new Point(node.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);
                    RC.NodeCoordinates = new Point(node.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);

                    node.expandedInput = "X" + level.ToString();
                }*/

                node.numChildren = 2;
                node.expandedInput = "X" + level.ToString();
                node.LeftChild = terminal1[whichTree];
                node.RightChild = terminal0[whichTree];
            }
            if (SetToReduce.Count == 1 && SetToReduce[0] == "1")
            {
                //else if (SetToReduce[0] == "1")
                /*{
                    Node LC = new Node();
                    Node RC = new Node();

                    LC.TheSet.Add("0");
                    RC.TheSet.Add("1");

                    LC.IsTerminal = true;
                    RC.IsTerminal = true;

                    node.LeftChild = LC;
                    node.RightChild = RC;
                    node.numChildren = 2;

                    LC.Parent = node;
                    RC.Parent = node;

                    LC.NodeCoordinates = new Point(node.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);
                    RC.NodeCoordinates = new Point(node.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);
                    
                    node.expandedInput = "X" + level.ToString();
                }*/
                /*node.numChildren = 2;
                node.expandedInput = "X" + level.ToString();
                node.RightChild = terminal1;
                node.LeftChild = terminal0;*/

                if (whichChild == -1)
                {
                    node.Parent.LeftChild = x[whichTree];
                    x[whichTree].nodeLevel = level;
                    x[whichTree].NodeCoordinates = node.NodeCoordinates;

                }

                if (whichChild == 1)
                {
                    node.Parent.RightChild = x[whichTree];
                    x[whichTree].nodeLevel = level;
                    x[whichTree].NodeCoordinates = node.NodeCoordinates;
                }
            }

            else if (SetToReduce.Count == 1 && SetToReduce[0] == "0")
            {

                if (whichChild == -1)
                {
                    node.Parent.LeftChild = xbar[whichTree];
                    xbar[whichTree].nodeLevel = level;
                    xbar[whichTree].NodeCoordinates = node.NodeCoordinates;
                }

                if (whichChild == 1)
                {
                    node.Parent.RightChild = xbar[whichTree];
                    xbar[whichTree].nodeLevel = level;
                    xbar[whichTree].NodeCoordinates = node.NodeCoordinates;
                }
            }


            else if (SetToReduce.Count == 2 && SetToReduce[0].Length == 1) //both 1 and 0
            {
                /*node.IsTerminal = true;
                List<string> Temp = new List<string>();
                Temp.Add("1");
                node.TheSet = new List<string>(Temp);*/

                if (whichChild == -1)
                {
                    node.Parent.LeftChild = terminal1[whichTree];
                }

                if (whichChild == 1)
                {
                    node.Parent.RightChild = terminal1[whichTree];
                }
            }

            else if (a == 10 && node.IsTerminal == false)
            {
                for (int i = 0; i < SetToReduce.Count; i++)
                {
                    if (SetToReduce[i][0] == '0')
                    {
                        CoFactor0.Add(SetToReduce[i].Substring(1));
                    }
                    else if (SetToReduce[i][0] == '1')
                    {
                        CoFactor1.Add(SetToReduce[i].Substring(1));
                    }
                }

                Node LC = new Node();
                Node RC = new Node();

                LC.TheSet = CoFactor0;
                RC.TheSet = CoFactor1;

                node.LeftChild = LC;
                node.RightChild = RC;
                node.numChildren = 2;

                LC.Parent = node;
                RC.Parent = node;

                LC.NodeCoordinates = new Point(node.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);
                RC.NodeCoordinates = new Point(node.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), node.NodeCoordinates.Y + y_spacing);

                node.expandedInput = "X" + level.ToString();

                makeBDD(LC, level + 1, extends - 1, -1, whichTree);
                makeBDD(RC, level + 1, extends + 1, 1, whichTree);
            }
            else
            {
                if (a == 0)
                {
                    node.IsStdFunc = "AND";
                }

                if (a == 1)
                {
                    node.IsStdFunc = "OR";
                }

                if (a == 2)
                {
                    node.IsStdFunc = "NOR";
                }

                if (a == 3)
                {
                    node.IsStdFunc = "XOR";
                }

                if (a == 4)
                {
                    node.IsStdFunc = "NAND";
                }

                if (a == 5)
                {
                    node.IsStdFunc = "XNOR";
                }

                if (a == 6)
                {
                    node.IsStdFunc = "x";
                }

                if (a == 7)
                {
                    node.IsStdFunc = "xbar";
                }
            }


        }

        bool compareNodesBasedOnChildren(Node node1, Node node2)
        {
            if (node1.numChildren == node2.numChildren && node1.numChildren != 0)
            {
                if (compareLists(node1.LeftChild.TheSet, node2.LeftChild.TheSet) && compareLists(node1.RightChild.TheSet, node2.RightChild.TheSet))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        void RemoveEqualNode(Node node, int whichChild, int whichTree) //0 - Left, 1 - Right //TODO: Rewrite this properly
        {
            if (node.LeftChild.IsStdFunc == node.RightChild.IsStdFunc && node.LeftChild.IsStdFunc != "No")
            {
                node.numChildren = 1;
                node.RightChild = node.LeftChild;
                //node.LeftChild = null;
                if (whichChild == 1)
                {
                    node.Parent.RightChild = node.RightChild;
                    node.RightChild.Parent = node.Parent;
                    node.Parent = null;
                    node.RightChild = null;
                }
                if (whichChild == 0)
                {
                    node.Parent.LeftChild = node.RightChild;
                    node.LeftChild.Parent = node.Parent;
                    node.Parent = null;
                    node.RightChild = null;
                }
                node.LeftChild = null;

            }

            else if (compareNodesBasedOnChildren(node.LeftChild, node.RightChild))
            {
                node.numChildren = 1;
                node.RightChild = node.LeftChild;
                node.LeftChild = null;
                if (whichChild == 1)
                {
                    node.Parent.RightChild = node.RightChild;
                    node.RightChild.Parent = node.Parent;
                    node.Parent = null;
                    node.RightChild = null;
                }
                if (whichChild == 0)
                {
                    node.Parent.LeftChild = node.RightChild;
                    node.RightChild.Parent = node.Parent;
                    node.Parent = null;
                    node.RightChild = null;
                }
            }

            else if ((node.LeftChild.TheSet.Count == 0 && node.LeftChild.IsStdFunc == "No") || (node.RightChild.TheSet.Count == 0 && node.RightChild.IsStdFunc == "No"))
            {
                if (node.LeftChild.TheSet.Count == 0 && node.LeftChild.IsStdFunc == "No")
                {
                    /*if (whichChild == 0)
                    {
                        node.Parent.LeftChild = node.RightChild;
                        node.RightChild.Parent = node.Parent;
                    }
                    else
                    {
                        node.Parent.RightChild = node.RightChild;
                        node.RightChild.Parent = node.Parent;
                    }*/

                    node.LeftChild = terminal0[whichTree];
                }

                if (node.RightChild.TheSet.Count == 0 && node.RightChild.IsStdFunc == "No")
                {
                    /*if (whichChild == 0)
                    {
                        node.Parent.LeftChild = node.LeftChild;
                        node.LeftChild.Parent = node.Parent;
                    }
                    else
                    {
                        node.Parent.RightChild = node.LeftChild;
                        node.LeftChild.Parent = node.Parent;
                    }*/
                    node.RightChild = terminal0[whichTree];
                }
            }

            if (node.LeftChild != null && node.LeftChild.numChildren != 0)
            {
                RemoveEqualNode(node.LeftChild, 0, whichTree);
            }

            if (node.RightChild != null && node.RightChild.numChildren != 0)
            {
                RemoveEqualNode(node.RightChild, 1, whichTree);
            }



        }

        void assignUID(Node node, int whichChild)
        {
            if (whichChild != 0) //means it is not a root
            {
                node.UID = new List<int>(node.Parent.UID);
                node.UID.Add(whichChild);
            }

            if (node.LeftChild != null)
            {
                assignUID(node.LeftChild, -1); //why wont you pass the variable properly???
            }
            if (node.RightChild != null)
            {
                assignUID(node.RightChild, 1);
            }


        }

        void Replace_x(List<string> cubes)
        {

            StringBuilder temp;
            for (int i = 0; i < cubes.Count; i++)
            {
                temp = new StringBuilder(cubes[i]);
                for (int j = 0; j < numinputs; j++)
                {
                    if (temp[j] == '-')
                    {
                        temp[j] = 'x';
                    }
                }

                cubes[i] = temp.ToString();

            }
        }

        void makeStdFuncsCoords(Node node)
        {
            if (node.LeftChild.IsTerminal == false)
            {
                node.LeftChild.NodeCoordinates.X = node.NodeCoordinates.X - 75;
                node.LeftChild.NodeCoordinates.Y = node.NodeCoordinates.Y + 100;
            }

            if (node.RightChild.IsTerminal == false)
            {
                node.RightChild.NodeCoordinates.X = node.NodeCoordinates.X + 75;
                node.RightChild.NodeCoordinates.Y = node.NodeCoordinates.Y + 100;
            }
        }

        void ReplaceStdFuncs(Node node, int whichChild, int whichTree)
        {
            if (node.IsStdFunc == "AND")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = AND[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = AND[whichTree];
                }
                if (whichChild == 0)//this case is quite redunant since there is ONLY one simple std func bdd. 
                {
                    node.LeftChild = AND[whichTree].LeftChild;
                    node.RightChild = AND[whichTree].RightChild;
                    node.numChildren = 2;
                }

                if (node.nodeLevel > AND[whichTree].nodeLevel)
                {
                    AND[whichTree].NodeCoordinates = node.NodeCoordinates;
                    AND[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(AND[whichTree]);
                    AND[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "OR")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = OR[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = OR[whichTree];
                }
                if (whichChild == 0)//this case is quite redunant since there is ONLY one simple std func bdd. 
                {
                    node.LeftChild = OR[whichTree].LeftChild;
                    node.RightChild = OR[whichTree].RightChild;
                    node.numChildren = 2;
                }

                if (node.nodeLevel > OR[whichTree].nodeLevel)
                {
                    OR[whichTree].NodeCoordinates = node.NodeCoordinates;
                    OR[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(OR[whichTree]);
                    OR[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }

            }

            if (node.IsStdFunc == "NOR")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = NOR[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = NOR[whichTree];
                }

                if (whichChild == 0)//this case is quite redunant since there is ONLY one simple std func bdd. 
                {
                    node.LeftChild = NOR[whichTree].LeftChild;
                    node.RightChild = NOR[whichTree].RightChild;
                    node.numChildren = 2;
                }

                if (node.nodeLevel > NOR[whichTree].nodeLevel)
                {
                    NOR[whichTree].NodeCoordinates = node.NodeCoordinates;
                    NOR[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(NOR[whichTree]);
                    NOR[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "XOR")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = XOR[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = XOR[whichTree];
                }

                if (whichChild == 0)//this case is quite redunant since there is ONLY one simple std func bdd. 
                {
                    node.LeftChild = XOR[whichTree].LeftChild;
                    node.RightChild = XOR[whichTree].RightChild;
                    node.numChildren = 2;
                }

                if (node.nodeLevel > XOR[whichTree].nodeLevel)
                {
                    XOR[whichTree].NodeCoordinates = node.NodeCoordinates;
                    XOR[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(XOR[whichTree]);
                    XOR[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "NAND")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = NAND[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = NAND[whichTree];
                }

                if (whichChild == 0)//this case is quite redunant since there is ONLY one simple std func bdd. 
                {
                    node.LeftChild = NAND[whichTree].LeftChild;
                    node.RightChild = NAND[whichTree].RightChild;
                    node.numChildren = 2;
                }

                if (node.nodeLevel > NAND[whichTree].nodeLevel)
                {
                    NAND[whichTree].NodeCoordinates = node.NodeCoordinates;
                    NAND[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(NAND[whichTree]);
                    NAND[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "x")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = x[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = x[whichTree];
                }

                if (node.nodeLevel > x[whichTree].nodeLevel)
                {
                    x[whichTree].NodeCoordinates = node.NodeCoordinates;
                    x[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(x[whichTree]);
                    x[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    x[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "xbar")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = xbar[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = xbar[whichTree];
                }

                if (node.nodeLevel > xbar[whichTree].nodeLevel)
                {
                    xbar[whichTree].NodeCoordinates = node.NodeCoordinates;
                    xbar[whichTree].nodeLevel = node.nodeLevel;
                    makeStdFuncsCoords(xbar[whichTree]);
                    xbar[whichTree].expandedInput = "x" + node.nodeLevel.ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                    xbar[whichTree].expandedInput = "x" + (node.nodeLevel + 1).ToString();
                }
            }

            if (node.IsStdFunc == "XNOR")
            {
                if (whichChild == -1)
                {
                    node.Parent.LeftChild = XNOR[whichTree];
                }
                if (whichChild == 1)
                {
                    node.Parent.RightChild = XNOR[whichTree];
                }
            }

            if (node.LeftChild != null)
            {
                ReplaceStdFuncs(node.LeftChild, -1, whichTree);
            }
            if (node.RightChild != null)
            {
                ReplaceStdFuncs(node.RightChild, 1, whichTree);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult DR = openFileDialog1.ShowDialog();
            if (DR == DialogResult.OK)
            {
                FileName = openFileDialog1.FileName;
            }

            StreamReader fileInput = File.OpenText(@FileName);
            string line = "";
            string[] items = new string[30];
            int numcubes = 0;
            string[] outputs = new string[30];

            // Read the input cubes/minterms from the blif file and put them in ON-set and DC-set
            while (line != ".end")
            {
                line = fileInput.ReadLine();
                items = line.Split(' ');
                if (items[0] == ".inputs")
                {
                    numinputs = (int)char.GetNumericValue(items[items.Length - 1][1]);
                }

                if (line.Length > 0)
                {
                    if (line[0] != '.')
                    {
                        outputs[numcubes] = items[1];
                        if (outputs[numcubes] == "1")
                        {
                            ON_Set.Add(items[0]);
                        }
                        else if (outputs[numcubes] == "0")
                        {
                            OFF_Set.Add(items[0]);
                        }
                        else if (outputs[numcubes] == "-")
                        {
                            DC_Set.Add(items[0]);
                        }
                        numcubes++;

                    }
                }

            }

            fileInput.Close();

            Replace_x(ON_Set);
            Replace_x(DC_Set);

            // Convert Maxterms (if any) in binary to decimal
            if (OFF_Set.Count > 0)
            {
                for (int i = 0; i < OFF_Set.Count; i++)
                {
                    OFF_Set_decimal.Add(Convert.ToInt32(OFF_Set[i], 2));
                }

                //Convert OFF set to ON set
                for (int i = 0; i < Math.Pow(2, numinputs); i++)
                {
                    if (!OFF_Set_decimal.Exists(bin => bin == i))
                    {
                        ON_Set.Add(Convert.ToString(i, 2).PadLeft(numinputs, '0'));
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ON_Set = ON_Set.Concat(DC_Set).ToList();
            //Root = new Node();
            Root.TheSet = new List<string>(ON_Set);

            //call make BDD function
            initLists();
            initStdFuncs(0);
            makeBDD(Root, 1, 0, 0, 0); //0 for which child is Root
            RemoveEqualNode(Root, 2, 0);// 2 means, no  parent
            ReplaceStdFuncs(Root, 2, 0); // 2 means Root

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        void DrawBDD(Node node, int Level) //traversal
        {
            //node.NodeCoordinates.X = node.NodeCoordinates.X + offset;

            if (node.IsTerminal == false)
                Canvas.DrawEllipse(BDDpen, node.NodeCoordinates.X - circleRadius / 2, node.NodeCoordinates.Y - circleRadius / 2, circleRadius, circleRadius);
            else
                Canvas.DrawRectangle(BDDpen, node.NodeCoordinates.X - circleRadius / 2, node.NodeCoordinates.Y - circleRadius / 2, circleRadius, circleRadius);

            if (node.IsTerminal == false && node.IsStdFunc == "No" && node.LeftChild != null)
            {
                Canvas.DrawString(node.expandedInput, new Font("Arial", 5), new SolidBrush(Color.Black), node.NodeCoordinates.X - 10, node.NodeCoordinates.Y - 10);
                BDDpen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                Canvas.DrawLine(BDDpen, node.NodeCoordinates, node.LeftChild.NodeCoordinates);
                BDDpen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                DrawBDD(node.LeftChild, Level + 1);
            }

            if (node.IsTerminal == false && node.IsStdFunc == "No" && node.RightChild != null)
            {
                Canvas.DrawString(node.expandedInput, new Font("Arial", 5), new SolidBrush(Color.Black), node.NodeCoordinates.X - 10, node.NodeCoordinates.Y - 10);
                Canvas.DrawLine(BDDpen, node.NodeCoordinates, node.RightChild.NodeCoordinates);
                DrawBDD(node.RightChild, Level + 1);
            }

            if (node.IsStdFunc != "No")
            {
                Canvas.DrawString(node.IsStdFunc, new Font("Arial", 5), new SolidBrush(Color.Black), node.NodeCoordinates.X - 10, node.NodeCoordinates.Y - 10);
            }

            if (node.IsTerminal == true)
            {
                Canvas.DrawString(node.TheSet[0], new Font("Arial", 5), new SolidBrush(Color.Black), node.NodeCoordinates);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Canvas = this.CreateGraphics();
            BDDpen = new Pen(System.Drawing.Color.Red);
            //Canvas.DrawLine(BDDpen, 130.0f, 20.0f, 130.0f, 140.0f);

            offset = ((this.Width - 100) * (-extendsLeft)) / (extendsRight - extendsLeft) - Root.NodeCoordinates.X;
            DrawBDD(Root, 0);
        }

        List<string> parseBlif(string filename)
        {
            StreamReader fileInput = File.OpenText(@filename);
            string line = "";
            string[] items = new string[30];
            int numcubes = 0;
            string[] outputs = new string[30];
            List<string> list = new List<string>();

            // Read the input cubes/minterms from the blif file and put them in ON-set and DC-set
            while (line != ".end")
            {
                line = fileInput.ReadLine();
                items = line.Split(' ');
                if (items[0] == ".inputs")
                {
                    numinputs = (int)char.GetNumericValue(items[items.Length - 1][1]);
                }

                if (line.Length > 0)
                {
                    if (line[0] != '.')
                    {
                        outputs[numcubes] = items[1];
                        if (outputs[numcubes] == "1")
                        {
                            list.Add(items[0]);
                        }
                    }
                }

            }
            
            fileInput.Close();
            return list;
        }

        bool doOperation(string string1, string string2, string Operation)
        {
            bool Opresult = false;
            
            if (string1 == "0")
            {
                string1 = "false";
            }
            else if (string1 == "1")
            {
                string1 = "true";
            }

            if (string2 == "0")
            {
                string2 = "false";
            }
            else if (string2 == "1")
            {
                string2 = "true";
            }


            if (Operation == "AND")
            {
                Opresult = Convert.ToBoolean(string1) & Convert.ToBoolean(string2); 
            }

            if (Operation == "OR")
            {
                Opresult = Convert.ToBoolean(string1) | Convert.ToBoolean(string2);
            }

            if (Operation == "NAND")
            {
                Opresult = !(Convert.ToBoolean(string1) & Convert.ToBoolean(string2));
            }

            if (Operation == "NOR")
            {
                Opresult = !(Convert.ToBoolean(string1) | Convert.ToBoolean(string2));
            }

            if (Operation == "XOR")
            {
                Opresult = Convert.ToBoolean(string1) ^ Convert.ToBoolean(string2);
            }

            if (Operation == "XNOR")
            {
                Opresult = !(Convert.ToBoolean(string1) ^ Convert.ToBoolean(string2));
            }

            return Opresult;
        }

        void ApplyBDD(Node node0, Node node1, Node nodeApplied, int level, int whichChild, string Operation)
        {
           
           // Case 1 : Both the nodes are terminal nodes (0 or 1)
           if (node0.IsTerminal == true && node1.IsTerminal == true)
           {
               bool opOut = doOperation(node0.TheSet[0], node1.TheSet[0], Operation);
               if (whichChild == -1)
               {
                   if (opOut == false)
                   {
                       nodeApplied.Parent.LeftChild = terminal0[2]; //2 is Node applied's index
                   }
                   else
                   {
                       nodeApplied.Parent.LeftChild = terminal1[2];
                   }
               }
               if (whichChild == 1)
               {
                   if (opOut == false)
                   {
                       nodeApplied.Parent.RightChild = terminal0[2]; //2 is Node applied's index
                   }
                   else
                   {
                       nodeApplied.Parent.RightChild = terminal1[2];
                   }
               }
           }

           // Case 2 : Node0 is a terminal and Node1 is a sub BDD
           if (node0.IsTerminal == true && node1.IsTerminal == false)
           {
               nodeApplied.LeftChild = new Node();
               nodeApplied.LeftChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.LeftChild.Parent = nodeApplied;
               ApplyBDD(node0, node1.LeftChild, nodeApplied.LeftChild, level + 1, -1, Operation);

               nodeApplied.RightChild = new Node();
               nodeApplied.RightChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.RightChild.Parent = nodeApplied;
               ApplyBDD(node0, node1.RightChild, nodeApplied.RightChild, level + 1, 1, Operation);
           }

           // Case 3 : Node0 is a sub BDD and Node1 is a terminal
           if (node0.IsTerminal == false && node1.IsTerminal == true)
           {
               nodeApplied.LeftChild = new Node();
               nodeApplied.LeftChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.LeftChild.Parent = nodeApplied;
               ApplyBDD(node0.LeftChild, node1, nodeApplied.LeftChild, level + 1, -1, Operation);

               nodeApplied.RightChild = new Node();
               nodeApplied.RightChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.RightChild.Parent = nodeApplied;
               ApplyBDD(node0.RightChild, node1, nodeApplied.RightChild, level + 1, 1, Operation);
           }

           // Case 4 : Both the nodes are sub BDDs
           if (node0.IsTerminal == false && node1.IsTerminal == false)
           {
               nodeApplied.LeftChild = new Node();
               nodeApplied.LeftChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X - (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.LeftChild.Parent = nodeApplied;
               ApplyBDD(node0.LeftChild, node1.LeftChild, nodeApplied.LeftChild, level + 1, -1, Operation);

               nodeApplied.RightChild = new Node();
               nodeApplied.RightChild.NodeCoordinates = new Point(nodeApplied.NodeCoordinates.X + (x_spacing - level * (circleRadius + 10)), nodeApplied.NodeCoordinates.Y + y_spacing);
               nodeApplied.expandedInput = "X" + level.ToString();
               nodeApplied.RightChild.Parent = nodeApplied;
               ApplyBDD(node0.RightChild, node1.RightChild, nodeApplied.RightChild, level + 1, 1, Operation);
           }

           
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> ONSet0 = new List<string>(parseBlif(@"C:\Users\Aliakbar\Desktop\Ranjana\test_nodes\apply1.blif"));
            List<string> ONSet1 = new List<string>(parseBlif(@"C:\Users\Aliakbar\Desktop\Ranjana\test_nodes\apply2.blif"));
            Root0.TheSet = new List<string>(ONSet0);
            Root1.TheSet = new List<string>(ONSet1);

            initLists();
            initStdFuncs(0);
            makeBDD(Root0, 1, 0, 0, 0); //0 for which child is Root
            if (Root0.numChildren > 0)
            {
                RemoveEqualNode(Root0, 2, 0);// 2 means, no  parent 
            }
            ReplaceStdFuncs(Root0, 0, 0); // 0 means Root

            initLists();
            initStdFuncs(1);
            makeBDD(Root1, 1, 0, 0, 1); //0 for which child is Root
            if (Root1.numChildren > 0)
            {
                RemoveEqualNode(Root1, 2, 1);// 2 means, no  parent 
            }
            ReplaceStdFuncs(Root1, 0, 1); // 0 means Root

            initLists();
            initStdFuncs(2);
            ApplyBDD(Root0, Root1, RootApplied, 1, 0, "AND");

            Canvas = this.CreateGraphics();
            BDDpen = new Pen(System.Drawing.Color.Red);
            //Canvas.DrawLine(BDDpen, 130.0f, 20.0f, 130.0f, 140.0f);

            //offset = ((this.Width - 100) * (-extendsLeft)) / (extendsRight - extendsLeft) - Root.NodeCoordinates.X;
            DrawBDD(RootApplied, 0);


        
        }
    }
           
}
