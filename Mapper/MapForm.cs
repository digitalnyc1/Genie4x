using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace GenieClient.Mapper
{
    public partial class MapForm : Form
    {
        public event ListResetEventHandler ListReset;
        public delegate void ListResetEventHandler();

        public event ToggleRecordEventHandler ToggleRecord;
        public delegate void ToggleRecordEventHandler(bool toggle);

        public event EchoMapPathEventHandler EchoMapPath;
        public delegate void EchoMapPathEventHandler();

        public event MoveMapPathEventHandler MoveMapPath;
        public delegate void MoveMapPathEventHandler();

        public event ToggleAllowDuplicatesEventHandler ToggleAllowDuplicates;
        public delegate void ToggleAllowDuplicatesEventHandler(bool toggle);

        public event MapLoadedEventHandler MapLoaded;
        public delegate void MapLoadedEventHandler();

        public event ZoneIDChangeEventHandler ZoneIDChange;
        public delegate void ZoneIDChangeEventHandler(string zoneid);

        public event ZoneNameChangeEventHandler ZoneNameChange;
        public delegate void ZoneNameChangeEventHandler(string zonename);

        public event ClickNodeEventHandler ClickNode;
        public delegate void ClickNodeEventHandler(string zoneid, int nodeid);

        private Genie.Globals m_oGlobals = null;
        private NodeList m_NodeList = null;
        private Node m_CurrentNode = null;
        private Node m_PathDestination = null;
        private string m_CurrentMapFile = string.Empty;

        private static int m_Scale = 1;

        public MapForm(Genie.Globals Globals)
        {
            // This call is required by the Windows Form Designer
            m_oGlobals = Globals;
            InitializeComponent();
            UpdatePanelColor();
            // Add any initialization after the InitializeComponent() call
        }

        public string MapFile
        {
            get
            {
                return m_CurrentMapFile;
            }

            set
            {
                m_CurrentMapFile = value;
            }
        }

        private string m_CharacterName = string.Empty;

        public string CharacterName
        {
            get
            {
                return m_CharacterName;
            }

            set
            {
                m_CharacterName = value;
                SafeUpdateMainWindowTitle();
            }
        }

        public void SetNodeList(NodeList nl)
        {
            m_NodeList = nl;
        }

        public NodeList NodeList
        {
            get
            {
                return m_NodeList;
            }
        }

        public void UpdatePanelColor()
        {
            PanelBase.BackColor = m_oGlobals.PresetList["automapper.panel"].BgColor;
            PanelMap.BackColor = m_oGlobals.PresetList["automapper.panel"].BgColor;
            PanelMap.Invalidate();
        }

        public void UpdateGraph(Node n, NodeList nl, Direction dir)
        {
            m_AllowRecord = false;
            m_CurrentNode = n;
            m_NodeList = nl;

            if (!Information.IsNothing(n))
            {
                if (Information.IsNothing(n.Position))
                {
                    n.Position = new Point3D();
                }
                else
                {
                    m_CurrentLevelZ = n.Position.Z;
                    if (m_ToggleRecording == true && ToolStripButtonLockPositions.Checked == false)
                    {
                        m_NodeList.ArrangeSingle(n, dir);
                    }
                }
            }

            UpdateMapSize();
            var m_Offset = GetOffset();
            m_Offset = m_NodeList.GetOffset();
            m_Offset.X *= m_Scale;
            m_Offset.Y *= m_Scale;
            _mapOffsetX = m_Offset.X;
            _mapOffsetY = m_Offset.Y;
            UpdatePanelColor();
            PanelMap.Invalidate();
        }

        public Font m_Font = FormMain.DefaultFont; // New Font("Arial", 8, FontStyle.Regular)
        private bool m_AllowRecord = false;

        private int _mapOffsetX;
        private int _mapOffsetY;

        private Point3D GetOffset()
        {
            if (m_NodeList is not null) return new Point3D(_mapOffsetX, _mapOffsetY);
            return new Point3D(this.Width / 2 * m_Scale, this.Height / 2 * m_Scale);
        }

        public Point ConvertPoint(Point3D oPoint, int iOffset = 0)
        {
            var oResult = new Point(oPoint.X * m_Scale, oPoint.Y * m_Scale);
            var m_Offset = GetOffset();
            oResult.X += m_Offset.X - iOffset;
            oResult.Y += m_Offset.Y - iOffset;
            return oResult;
        }

        private bool m_ShiftPress = false;
        private bool m_ControlPress = false;
        public bool IsClosing = false;

        private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsClosing == false)
            {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void GraphForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                m_ShiftPress = true;
            }

            if (e.Control)
            {
                m_ControlPress = true;
            }
        }

        private void GraphForm_KeyUp(object sender, KeyEventArgs e)
        {
            m_ShiftPress = false;
            m_ControlPress = false;
            if (e.KeyCode == Keys.Delete)
            {
                if (PanelDetails.Visible == false)
                {
                    if (m_ToggleMoveNodes == true)
                    {
                        EraseSelected();
                    }
                    else
                    {
                        Interaction.Beep();
                    }
                }
            }
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            var m_Offset = GetOffset();
            m_Offset.X -= m_Offset.X % 20;
            m_Offset.Y -= m_Offset.Y % 20;
            UpdatePanelColor();
            LoadMaps();
            if (ZoneName.Length > 0)
            {
                ToolStripDropDownButtonMaps.Text = ZoneID + ". " + ZoneName;
            }
            else
            {
                ToolStripDropDownButtonMaps.Text = "Unknown";
            }
        }

        private bool LoadMaps(string sPath = "")
        {
            try
            {
                ToolStripDropDownButtonMaps.DropDownItems.Clear();
                _Zones.Clear();
                var sortedMapsList = new Genie.Collections.SortedList(new INaturalComparer());
                int iunknown = 1;
                var diDirectory = new DirectoryInfo(m_oGlobals.Config.MapDir);
                foreach (FileInfo dif in diDirectory.GetFiles())
                {
                    if ((dif.Extension.ToLower() ?? "") == ".xml")
                    {
                        string sName = LoadXMLHeader(dif.FullName);
                        if (sName.Length == 0)
                        {
                            sName = "Unknown " + iunknown.ToString();
                            iunknown += 1;
                        }

                        if (sortedMapsList.ContainsKey(sName) == false)
                        {
                            sortedMapsList.Add(sName, dif.FullName);
                        }
                    }
                }

                foreach (string s in sortedMapsList.Keys)
                {
                    var mi = new ToolStripMenuItem();
                    mi.Text = s;
                    mi.Tag = sortedMapsList[s].ToString();
                    mi.Click += ToolStripMenuItemMapButton_Click;
                    ToolStripDropDownButtonMaps.DropDownItems.Add(mi);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ToolStripMenuItemMapButton_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                if (!Information.IsNothing(mi.Tag))
                {
                    LoadXML(Conversions.ToString(mi.Tag));
                    MapLoaded?.Invoke();
                }
            }
        }

        private void GraphForm_Resize(object sender, EventArgs e)
        {
            PanelMap.Invalidate();
        }

        private string GetValue(XmlNode xn, string key, string def = "")
        {
            if (!Information.IsNothing(xn))
            {
                if (!Information.IsNothing(xn.Attributes))
                {
                    if (!Information.IsNothing(xn.Attributes.GetNamedItem(key)))
                    {
                        return xn.Attributes.GetNamedItem(key).Value;
                    }
                }
            }

            return def;
        }

        private void ToolStripButtonClear_Click(object sender, EventArgs e)
        {
            ClearMap();
        }

        public void ClearMap()
        {
            ZoneID = 0.ToString();
            ZoneName = "Untitled Map";
            SafeUpdateMainWindowTitle();
            var m_Offset = new Point3D((int)(Width / (double)2), (int)(Height / (double)2));
            m_Offset.X *= m_Scale;
            m_Offset.Y *= m_Scale;
            m_CurrentMapFile = string.Empty;
            m_CurrentNode = null;
            m_SelectedNodes.Clear();
            m_SelectedLabels.Clear();
            m_PathDestination = null;
            m_NodeList.Labels.Clear();
            m_NodeList.Clear();
            m_NodeList.NextID = 1;
            ListReset?.Invoke();
            UpdateNodeDetails();
            UpdateMapSize(true);
            PanelMap.Invalidate();
        }

        public void SelectNodes(string roomname, string roomdesc = "")
        {
            bool bCheckDescription = false;
            if (roomdesc.Length > 0)
                bCheckDescription = true;
            int iRoomID = 0;
            if (Information.IsNumeric(roomname))
            {
                int.TryParse(roomname, out iRoomID);
            }

            m_SelectedNodes.Clear();
            foreach (Node n in m_NodeList)
            {
                if (iRoomID > 0)
                {
                    if (n.ID == iRoomID)
                    {
                        m_SelectedNodes.Add(n);
                    }
                }
                else if ((n.Name ?? "") == (roomname ?? ""))
                {
                    if (bCheckDescription)
                    {
                        foreach (string s in n.Descriptions)
                        {
                            if ((s.Trim() ?? "") == (roomdesc.Trim() ?? ""))
                            {
                                m_SelectedNodes.Add(n);
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_SelectedNodes.Add(n);
                    }
                }
            }

            PanelMap.Invalidate();
        }

        public delegate void UpdateMainWindowTitleDelegate();

        private void SafeUpdateMainWindowTitle()
        {
            if (InvokeRequired == true)
            {
                Invoke(new UpdateMainWindowTitleDelegate(UpdateMainWindowTitle));
            }
            else
            {
                UpdateMainWindowTitle();
            }
        }

        public void UpdateMainWindowTitle()
        {
            Text = Conversions.ToString(Interaction.IIf(m_CharacterName.Length > 0, m_CharacterName + " ", "") + "[" + ZoneName + "] - Genie4 AutoMapper");
        }

        private void ToolStripButtonLoad_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadXML(OpenFileDialog1.FileName);
                MapLoaded?.Invoke();
            }
        }

        private class Link
        {
            public int NodeID = 0;
            public string FileName = string.Empty;

            public Link()
            {
            }

            public Link(int NodeID, string FileName)
            {
                this.NodeID = NodeID;
                this.FileName = FileName;
            }
        }

        private class LinkList : CollectionBase
        {
            public int Add(Link Item)
            {
                return List.Add(Item);
            }
        }

        private class Zone
        {
            public string Name = string.Empty;
            public string ID = string.Empty;
            public string FileName = string.Empty;
            public LinkList Links = new LinkList();

            public Zone()
            {
            }

            public Zone(int ID, string Name, string FileName)
            {
                this.ID = ID.ToString();
                this.Name = Name;
                this.FileName = FileName;
            }
        }

        private class ZoneList : CollectionBase
        {
            public int Add(Node Item)
            {
                return List.Add(Item);
            }
        }

        private ZoneList _Zones = new ZoneList();

        public string LoadXMLHeader(string sPath)
        {
            try
            {
                var xdoc = new XmlDocument();
                XmlNodeList xnlist;
                if (sPath.Contains(@"\") == false)
                {
                    sPath = m_oGlobals.Config.MapDir + @"\" + sPath;
                }

                xdoc = new XmlDocument();
                xdoc.Load(new StreamReader(sPath, true));
                var z = new Zone();
                var xZone = xdoc.SelectSingleNode("zone");
                if (!Information.IsNothing(xZone))
                {
                    string sZoneName = GetValue(xZone, "name");
                    if (sZoneName.Length > 0)
                    {
                        z.Name = sZoneName;
                    }

                    z.ID = GetValue(xZone, "id");
                    z.FileName = sPath;
                }

                xnlist = xdoc.SelectNodes("zone/node");
                foreach (XmlNode xn in xnlist)
                {
                    var n = new Node();
                    n.ID = int.Parse(GetValue(xn, "id", "0"));
                    n.Name = GetValue(xn, "name");
                    n.Note = GetValue(xn, "note");
                    n.IsLabelFile = n.Note.Contains(".xml");
                    if (n.IsLabelFile == true)
                    {
                        var argItem = new Link(n.ID, n.Note);
                        z.Links.Add(argItem);
                    }
                }

                string sReturn = string.Empty;
                if (z.Name.Length > 0)
                {
                    sReturn = z.ID.ToString() + ". " + z.Name;
                }

                return sReturn;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool LoadXML(string sPath)
        {
            try
            {
                var xdoc = new XmlDocument();
                XmlNodeList xnlist;
                if (sPath.Contains(@"\") == false)
                {
                    sPath = m_oGlobals.Config.MapDir + "\\" + sPath;
                }

                if (sPath.Length == 0)
                    return false;
                if (!File.Exists(sPath))
                    return false;
                m_CurrentMapFile = sPath;
                xdoc = new XmlDocument();
                xdoc.Load(new StreamReader(sPath, true));
                var xZone = xdoc.SelectSingleNode("zone");
                if (!Information.IsNothing(xZone))
                {
                    ZoneID = GetValue(xZone, "id");
                    string sZoneName = GetValue(xZone, "name");
                    if (sZoneName.Length > 0)
                    {
                        ZoneName = sZoneName;
                        UpdateMainWindowTitle();
                        ToolStripDropDownButtonMaps.Text = ZoneID + ". " + ZoneName;
                    }
                    else
                    {
                        ToolStripDropDownButtonMaps.Text = "Untitled Map";
                    }
                }

                xnlist = xdoc.SelectNodes("zone/node");
                m_NodeList.Clear();
                m_SelectedNodes.Clear();
                m_CurrentNode = null;
                m_PathDestination = null;
                m_NodeList.Labels.Clear();
                m_SelectedLabels.Clear();
                m_NodeList.NextID = 1;
                ListReset?.Invoke();
                ToolStripButtonRecord.Checked = false; // Turn off record. (Needs to be manually turned on when we are in a room on the map, or a clear map)
                ToggleRecord?.Invoke(ToolStripButtonRecord.Checked);
                foreach (XmlNode xn in xnlist)
                {
                    var n = new Node();
                    n.ID = int.Parse(GetValue(xn, "id", "0"));
                    if (n.ID > m_NodeList.NextID)
                        m_NodeList.NextID = n.ID;
                    n.Name = GetValue(xn, "name");
                    n.Note = GetValue(xn, "note", "");
                    n.IsLabelFile = n.Note.Contains(".xml");
                    string sColor = GetValue(xn, "color");
                    if (sColor.Length > 0)
                    {
                        n.Color = Genie.ColorCode.StringToColor(sColor);
                    }
                    else
                    {
                        n.Color = Color.Transparent;
                    }

                    var xPos = xn.SelectSingleNode("position");
                    if (!Information.IsNothing(xPos))
                    {
                        int X = int.Parse(GetValue(xPos, "x", "0"));
                        int Y = int.Parse(GetValue(xPos, "y", "0"));
                        int Z = int.Parse(GetValue(xPos, "z", "0"));
                        n.Position = new Point3D(X, Y, Z);
                    }

                    var xDescs = xn.SelectNodes("description");
                    foreach (XmlNode xdesc in xDescs)
                    {
                        if (!Information.IsNothing(xdesc))
                        {
                            n.Descriptions.Add(xdesc.InnerText);
                        }
                    }

                    var xArcs = xn.SelectNodes("arc");
                    foreach (XmlNode xarc in xArcs)
                    {
                        var a = new Arc();
                        a.DestinationID = int.Parse(GetValue(xarc, "destination", "0"));
                        bool bHidden = false;
                        string sHidden = GetValue(xarc, "hidden").ToLower();
                        if (sHidden.Length > 0 && ((sHidden ?? "") == "true" || (sHidden ?? "") == "1"))
                            bHidden = true;
                        a.HideArc = bHidden;
                        string sExit = GetValue(xarc, "exit", "").ToLower();
                        a.Move = GetValue(xarc, "move", "").ToLower();

                        // "name" is previous xml format, left for compability
                        if (sExit.Length == 0)
                        {
                            sExit = GetValue(xarc, "name").ToLower();
                            a.Move = sExit;
                        }

                        a.Direction = Direction.None;
                        if (sExit.StartsWith("go "))
                        {
                            a.Direction = Direction.Go;
                        }
                        else if (sExit.StartsWith("climb "))
                        {
                            a.Direction = Direction.Climb;
                        }
                        else
                        {
                            switch (sExit)
                            {
                                case "north":
                                case "n":
                                    {
                                        a.Direction = Direction.North;
                                        break;
                                    }

                                case "northeast":
                                case "ne":
                                    {
                                        a.Direction = Direction.NorthEast;
                                        break;
                                    }

                                case "east":
                                case "e":
                                    {
                                        a.Direction = Direction.East;
                                        break;
                                    }

                                case "southeast":
                                case "se":
                                    {
                                        a.Direction = Direction.SouthEast;
                                        break;
                                    }

                                case "south":
                                case "s":
                                    {
                                        a.Direction = Direction.South;
                                        break;
                                    }

                                case "southwest":
                                case "sw":
                                    {
                                        a.Direction = Direction.SouthWest;
                                        break;
                                    }

                                case "west":
                                case "w":
                                    {
                                        a.Direction = Direction.West;
                                        break;
                                    }

                                case "northwest":
                                case "nw":
                                    {
                                        a.Direction = Direction.NorthWest;
                                        break;
                                    }

                                case "up":
                                case "u":
                                    {
                                        a.Direction = Direction.Up;
                                        break;
                                    }

                                case "down":
                                case "d":
                                    {
                                        a.Direction = Direction.Down;
                                        break;
                                    }

                                case "out":
                                case "o":
                                    {
                                        a.Direction = Direction.Out;
                                        break;
                                    }

                                case "go":
                                    {
                                        a.Direction = Direction.Go;
                                        break;
                                    }

                                case "climb":
                                    {
                                        a.Direction = Direction.Climb;
                                        break;
                                    }
                            }
                        }

                        n.AddArc(a);
                    }

                    m_NodeList.Add(n);
                }

                m_NodeList.FixArcLinks();
                UpdateMapSize(true);
                var m_Offset = GetOffset();
                m_Offset = m_NodeList.GetOffset();
                m_Offset.X *= m_Scale;
                m_Offset.Y *= m_Scale;
                _mapOffsetX = m_Offset.X;
                _mapOffsetY = m_Offset.Y;
                PanelMap.Invalidate();
                xnlist = xdoc.SelectNodes("zone/label");
                foreach (XmlNode xn in xnlist)
                {
                    var l = new Label();
                    l.Text = GetValue(xn, "text");
                    var xPos = xn.SelectSingleNode("position");
                    if (!Information.IsNothing(xPos))
                    {
                        int X = int.Parse(GetValue(xPos, "x", "0"));
                        int Y = int.Parse(GetValue(xPos, "y", "0"));
                        int Z = int.Parse(GetValue(xPos, "z", "0"));
                        l.Position = new Point3D(X, Y, Z);
                    }

                    m_NodeList.Labels.Add(l);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string m_CurrentZoneName = string.Empty;

        public string ZoneName
        {
            get
            {
                return m_CurrentZoneName;
            }

            set
            {
                m_CurrentZoneName = value;
                ZoneNameChange?.Invoke(m_CurrentZoneName);
            }
        }

        private string m_CurrentZoneID = string.Empty;

        public string ZoneID
        {
            get
            {
                return m_CurrentZoneID;
            }

            set
            {
                m_CurrentZoneID = value;
                ZoneIDChange?.Invoke(m_CurrentZoneID);
            }
        }

        public bool SaveXML(string sPath = "")
        {
            if (sPath.Length == 0)
            {
                sPath = m_CurrentMapFile;
                if (sPath.Length == 0)
                {
                    return false;
                }
            }
            else
            {
                m_CurrentMapFile = sPath;
            }

            try
            {
                var xw = new XmlTextWriter(sPath, System.Text.Encoding.UTF8);
                xw.Formatting = Formatting.Indented;
                xw.WriteStartDocument();
                xw.WriteStartElement("zone");
                xw.WriteAttributeString("name", ZoneName);
                xw.WriteAttributeString("id", ZoneID);

                foreach (Node n in m_NodeList)
                {
                    if (!Information.IsNothing(n.Position)) // Don't save unplaced rooms
                    {
                        xw.WriteStartElement("node");
                        xw.WriteAttributeString("id", n.ID.ToString());
                        xw.WriteAttributeString("name", n.Name);
                        if (n.Note.Length > 0)
                            xw.WriteAttributeString("note", n.Note);
                        if (n.Color != Color.Transparent)
                            xw.WriteAttributeString("color", Genie.ColorCode.ColorToString(n.Color));
                        foreach (string s in n.Descriptions)
                        {
                            xw.WriteStartElement("description");
                            xw.WriteString(s);
                            xw.WriteEndElement();
                        }

                        xw.WriteStartElement("position");
                        xw.WriteAttributeString("x", n.Position.X.ToString());
                        xw.WriteAttributeString("y", n.Position.Y.ToString());
                        xw.WriteAttributeString("z", n.Position.Z.ToString());
                        xw.WriteEndElement();
                        foreach (Arc a in n.Arcs)
                        {
                            xw.WriteStartElement("arc");
                            xw.WriteAttributeString("exit", a.Direction.ToString().ToLower());
                            if (a.Move.Length > 0)
                                xw.WriteAttributeString("move", a.Move);
                            if (a.HideArc)
                                xw.WriteAttributeString("hidden", a.HideArc.ToString());
                            if (!Information.IsNothing(a.Destination))
                            {
                                if (!Information.IsNothing(a.Destination.Position)) // Don't save unplaced rooms' exits
                                {
                                    xw.WriteAttributeString("destination", a.Destination.ID.ToString());
                                }
                            }

                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement(); // End "node"
                    }
                }

                foreach (Label l in m_NodeList.Labels)
                {
                    xw.WriteStartElement("label");
                    xw.WriteAttributeString("text", l.Text);
                    xw.WriteStartElement("position");
                    xw.WriteAttributeString("x", l.Position.X.ToString());
                    xw.WriteAttributeString("y", l.Position.Y.ToString());
                    xw.WriteAttributeString("z", l.Position.Z.ToString());
                    xw.WriteEndElement();
                    xw.WriteEndElement(); // End "label"
                }

                xw.WriteEndElement(); // End "zone"
                xw.WriteEndDocument();
                xw.Flush();
                xw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ToolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (m_CurrentMapFile.Length > 0)
            {
                SaveFileDialog1.InitialDirectory = Path.GetDirectoryName(m_CurrentMapFile);
                SaveFileDialog1.FileName = Path.GetFileName(m_CurrentMapFile);
            }
            else
            {
                SaveFileDialog1.InitialDirectory = m_oGlobals.Config.MapDir;
            }

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveXML(SaveFileDialog1.FileName);
            }
        }

        private bool m_ToggleRecording = false;

        private void ToolStripButtonRecord_Click(object sender, EventArgs e)
        {
            SetRecordToggle(!ToolStripButtonRecord.Checked);
        }

        public void SetRecordToggle(bool toggle)
        {
            ToolStripButtonRecord.Checked = toggle;
            m_ToggleRecording = ToolStripButtonRecord.Checked;
            if (ToolStripButtonRecord.Checked == true && m_AllowRecord == false)
            {
                if (!Information.IsNothing(m_NodeList) && m_NodeList.Count > 0)
                {
                    ToolStripButtonRecord.Checked = false;
                    Interaction.MsgBox("Mapper needs to know your current room before you can record.");
                }
                else
                {
                    ToggleRecord?.Invoke(ToolStripButtonRecord.Checked);
                }
            }
            else
            {
                ToggleRecord?.Invoke(ToolStripButtonRecord.Checked);
            }

            ToolStripButtonLockPositions.Enabled = ToolStripButtonRecord.Checked;
            ToolStripButtonAllowDuplicates.Enabled = ToolStripButtonRecord.Checked;
            Invalidate();
        }

        private Node m_MovingNode = null;
        private Label m_MovingLabel = null;
        private Node m_ToolTipNode = null;
        private Point3D m_MoveMap = null;
        private Point3D m_LassoStart = null;
        private Rectangle m_Lasso;

        private void FlickerFreePanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Information.IsNothing(m_NodeList))
                return;
            if (e.Button == MouseButtons.Left)
            {
                m_MovingLabel = FindLabel(e.X, e.Y, m_CurrentLevelZ);
                m_MovingNode = FindNode(e.X, e.Y, m_CurrentLevelZ);
                if (!Information.IsNothing(m_MovingNode))
                {
                    ClickNode?.Invoke(m_CurrentZoneID, m_MovingNode.ID);
                }

                if (m_ToggleMoveNodes == true && !Information.IsNothing(m_MovingLabel))
                {
                    if (m_SelectedLabels.Contains(m_MovingLabel))
                    {
                        if (m_ControlPress == true)
                        {
                            m_SelectedLabels.Remove(m_MovingLabel);
                        }
                    }
                    else
                    {
                        if (m_ShiftPress == false && m_ControlPress == false)
                        {
                            m_SelectedNodes.Clear();
                            m_SelectedLabels.Clear();
                        }

                        m_SelectedLabels.Add(m_MovingLabel);
                    }
                }
                else if (!Information.IsNothing(m_MovingNode))
                {
                    // Load linked node on left click
                    if (m_ToggleMoveNodes == false && m_MovingNode.IsLabelFile == true)
                    {
                        string sFile = string.Empty;
                        foreach (string s in m_MovingNode.Note.Split('|'))
                        {
                            if (s.ToLower().EndsWith(".xml"))
                            {
                                sFile = s;
                                break;
                            }
                        }

                        if (LoadXML(sFile) == true)
                        {
                            PanelMap.Invalidate();
                            MapLoaded?.Invoke();
                            return;
                        }
                    }

                    if (Information.IsNothing(m_SelectedNodes.Find(m_MovingNode.ID)))
                    {
                        if (m_ShiftPress == false && m_ControlPress == false)
                        {
                            m_SelectedNodes.Clear();
                            m_SelectedLabels.Clear();
                        }

                        m_SelectedNodes.Add(m_MovingNode);
                    }
                    else if (m_ControlPress == true) // Node already selected. Unselect if we have control pressed
                    {
                        m_SelectedNodes.Remove(m_MovingNode);
                        m_MovingNode = null;
                    }
                }
                else if (m_ToggleMoveNodes == true) // Lasso
                {
                    m_LassoStart = new Point3D(e.X, e.Y);
                }
                else
                {
                    m_NodeList.ResetPathStates();
                    m_SelectedNodes.Clear();
                    m_SelectedLabels.Clear();
                }

                if (m_ToggleMoveNodes == false)
                {
                    m_MovingNode = null;
                    m_MovingLabel = null;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                m_MoveMap = new Point3D(e.X, e.Y);
                m_PathDestination = FindNode(e.X, e.Y, m_CurrentLevelZ);
                if (!Information.IsNothing(m_PathDestination))
                {
                    ClickNode?.Invoke(m_CurrentZoneID, m_PathDestination.ID);
                }
            }

            m_ToolTipNode = null;
            PanelMap.Invalidate();
        }

        public void SetDestinationNode(Node n)
        {
            m_PathDestination = n;
        }

        private void FlickerFreePanel1_MouseLeave(object sender, EventArgs e)
        {
            m_MovingLabel = null;
            m_MovingNode = null;
            m_MoveMap = null;
            m_ToolTipNode = null;
            m_Lasso = default;
            m_LassoStart = null;
        }

        private Node FindNode(int X, int Y, int Z = 0)
        {
            var m_Offset = GetOffset();
            X = X - m_Offset.X;
            Y = Y - m_Offset.Y;
            Node oNode = null; // Find LAST in list so the highest room get selected when you click on a location that has two
            foreach (Node n in m_NodeList)
            {
                if (!Information.IsNothing(n.Position))
                {
                    int iX = n.Position.X * m_Scale - X;
                    int iY = n.Position.Y * m_Scale - Y;
                    if (iX < 5 * m_Scale && iX > -5 * m_Scale)
                    {
                        if (iY < 5 * m_Scale && iY > -5 * m_Scale)
                        {
                            if (n.Position.Z == Z)
                            {
                                oNode = n;
                            }
                        }
                    }
                }
            }

            return oNode;
        }

        private Label FindLabel(int X, int Y, int Z = 0)
        {
            Label oLabel = null; // Find LAST in list so the highest label get selected when you click on a location that has two
            foreach (Label l in m_NodeList.Labels)
            {
                if (!Information.IsNothing(l.Position))
                {
                    if (l.Position.Z == Z)
                    {
                        if (X >= l.Rectangle.X && X < l.Rectangle.X + l.Rectangle.Width)
                        {
                            if (Y >= l.Rectangle.Y && Y < l.Rectangle.Y + l.Rectangle.Height)
                            {
                                oLabel = l;
                            }
                        }
                    }
                }
            }

            return oLabel;
        }

        // No need to invalidate whole area here later. It gets slow with a big display
        private void FlickerFreePanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Information.IsNothing(m_MovingNode) || !Information.IsNothing(m_MovingLabel))
            {
                int iMoveX = 0;
                int iMoveY = 0;
                var m_Offset = GetOffset();
                if (!Information.IsNothing(m_MovingNode))
                {
                    iMoveX = Conversions.ToInteger((double)(e.X - m_Offset.X) / m_Scale - (m_ToggleSnapToGrid ? (double)(e.X % 20) / m_Scale : 0) - m_MovingNode.Position.X);
                    iMoveY = Conversions.ToInteger((double)(e.Y - m_Offset.Y) / m_Scale - (m_ToggleSnapToGrid ? (double)(e.Y % 20) / m_Scale : 0) - m_MovingNode.Position.Y);
                }
                else
                {
                    iMoveX = Conversions.ToInteger((double)(e.X - m_Offset.X) / m_Scale - (m_ToggleSnapToGrid ? (double)(e.X % 10) / m_Scale : 0) - m_MovingLabel.Position.X);
                    iMoveY = Conversions.ToInteger((double)(e.Y - m_Offset.Y) / m_Scale - (m_ToggleSnapToGrid ? (double)(e.Y % 10) / m_Scale : 0) - m_MovingLabel.Position.Y);
                }

                if (!Information.IsNothing(m_SelectedNodes))
                {
                    foreach (Node n in m_SelectedNodes)
                        n.Position.Offset(iMoveX, iMoveY);
                }

                if (!Information.IsNothing(m_SelectedLabels))
                {
                    foreach (Label l in m_SelectedLabels)
                    {
                        l.Position.Offset(iMoveX, iMoveY);
                        l.ClearRectangle();
                    }
                }

                PanelMap.Invalidate();
            }
            else if (!Information.IsNothing(m_MoveMap))
            {
                var pt = new Point();
                pt.X = -(PanelBase.AutoScrollPosition.X + (e.X - m_MoveMap.X));
                pt.Y = -(PanelBase.AutoScrollPosition.Y + (e.Y - m_MoveMap.Y));
                PanelBase.AutoScrollPosition = pt;
                PanelMap.Invalidate();
            }
            else if (!Information.IsNothing(m_LassoStart))
            {
                SetLassoSizes(e.X, e.Y);
                PanelMap.Invalidate();
            }
            else // Hover
            {
                m_ToolTipNode = FindNode(e.X, e.Y, m_CurrentLevelZ);
                if (!Information.IsNothing(m_ToolTipNode))
                {
                    ToolStripStatusText.Text = Conversions.ToString("#" + m_ToolTipNode.ID.ToString() + " " + m_ToolTipNode.Name + Interaction.IIf(m_ToolTipNode.Note.Length > 0, " [" + m_ToolTipNode.Note + "]", ""));
                }
                else
                {
                    ToolStripStatusText.Text = "Ready.";
                }
            }
        }

        private void FlickerFreePanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Information.IsNothing(m_LassoStart)) // Select nodes
            {
                SetLassoSizes(e.X, e.Y);

                // Offset lasso so we can compare to our nodes
                var m_Offset = GetOffset();
                m_Lasso.X -= m_Offset.X;
                m_Lasso.Y -= m_Offset.Y;
                m_Lasso.X /= m_Scale;
                m_Lasso.Y /= m_Scale;
                m_Lasso.Width /= m_Scale;
                m_Lasso.Height /= m_Scale;
                if (m_ShiftPress == false && m_ControlPress == false)
                {
                    m_SelectedNodes.Clear();
                    m_SelectedLabels.Clear();
                }

                // Nodes
                foreach (Node n in m_NodeList)
                {
                    if (!Information.IsNothing(n.Position))
                    {
                        if (n.Position.X >= m_Lasso.X && n.Position.X <= m_Lasso.X + m_Lasso.Width)
                        {
                            if (n.Position.Y >= m_Lasso.Y && n.Position.Y <= m_Lasso.Y + m_Lasso.Height)
                            {
                                if (n.Position.Z == m_CurrentLevelZ)
                                {
                                    if (m_ControlPress == false)
                                    {
                                        if (m_SelectedNodes.Contains(n.ID) == false) // Don't add dupe
                                        {
                                            m_SelectedNodes.Add(n);
                                        }
                                    }
                                    else if (m_SelectedNodes.Contains(n))
                                    {
                                        m_SelectedNodes.Remove(n);
                                    }
                                }
                            }
                        }
                    }
                }

                // Labels
                foreach (Label l in m_NodeList.Labels)
                {
                    if (!Information.IsNothing(l.Position))
                    {
                        if (l.Position.X >= m_Lasso.X && l.Position.X <= m_Lasso.X + m_Lasso.Width)
                        {
                            if (l.Position.Y >= m_Lasso.Y && l.Position.Y <= m_Lasso.Y + m_Lasso.Height)
                            {
                                if (l.Position.Z == m_CurrentLevelZ)
                                {
                                    if (m_ControlPress == false)
                                    {
                                        if (m_SelectedLabels.Contains(l) == false) // Don't add dupe
                                        {
                                            m_SelectedLabels.Add(l);
                                        }
                                    }
                                    else if (m_SelectedLabels.Contains(l))
                                    {
                                        m_SelectedLabels.Remove(l);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (e.Button == MouseButtons.Right) // Right clicked on a node
            {
                if (!Information.IsNothing(m_PathDestination))
                {
                    m_NodeList.ResetPathStates();
                    if (!Information.IsNothing(m_CurrentNode) && m_SelectedNodes.Count == 0)
                    {
                        if (m_CurrentNode.ID != m_PathDestination.ID)
                        {
                            m_NodeList.FindShortestPath(m_CurrentNode, m_PathDestination);
                            if (ToolStripButtonWalk.Checked == true)
                            {
                                MoveMapPath?.Invoke();
                            }
                            else
                            {
                                EchoMapPath?.Invoke();
                            }
                        }
                    }
                    else if (!Information.IsNothing(m_SelectedNodes) && m_SelectedNodes.Count == 1)
                    {
                        if (m_SelectedNodes[0].ID != m_PathDestination.ID)
                        {
                            var argn1 = m_SelectedNodes[0];
                            m_NodeList.FindShortestPath(argn1, m_PathDestination);
                            EchoMapPath?.Invoke();
                        }
                    }
                }
            }

            if (e.Button == MouseButtons.Left) // Update properties on release left button
            {
                if (!Information.IsNothing(m_MovingLabel))
                {
                    UpdateLabelDetails();
                }
                else
                {
                    UpdateNodeDetails();
                }
            }

            m_MovingNode = null;
            m_MovingLabel = null;
            m_MoveMap = null;
            m_Lasso = default;
            m_LassoStart = null;
            PanelMap.Invalidate();
        }

        private void SetLassoSizes(int X, int Y)
        {
            if (m_LassoStart.X > X)
            {
                m_Lasso.Width = m_LassoStart.X - X;
                m_Lasso.X = m_LassoStart.X - m_Lasso.Width;
            }
            else
            {
                m_Lasso.Width = X - m_LassoStart.X;
                m_Lasso.X = m_LassoStart.X;
            }

            if (m_Lasso.Width < 1)
                m_Lasso.Width = 1;
            if (m_LassoStart.Y > Y)
            {
                m_Lasso.Height = m_LassoStart.Y - Y;
                m_Lasso.Y = m_LassoStart.Y - m_Lasso.Height;
            }
            else
            {
                m_Lasso.Height = Y - m_LassoStart.Y;
                m_Lasso.Y = m_LassoStart.Y;
            }

            if (m_Lasso.Height < 1)
                m_Lasso.Height = 1;
        }

        private void UpdateNodeDetails()
        {
            PanelLabelDetails.Visible = false;
            PanelNodeDetails.Visible = true;
            if (!Information.IsNothing(m_SelectedNodes) && m_SelectedNodes.Count == 1)
            {
                var n = m_SelectedNodes[0];
                PanelNodeDetails.Node = n;
                PanelNodeDetails.TextBoxRoomID.Text = n.ID.ToString();
                PanelNodeDetails.TextBoxRoomName.Text = n.Name;
                PanelNodeDetails.TextBoxDescription.Text = n.Descriptions.ToString();
                PanelNodeDetails.TextBoxPosition.Text = n.Position.X.ToString() + ", " + n.Position.Y.ToString() + ", " + n.Position.Z.ToString();
                PanelNodeDetails.ColorPicker1.Color = n.Color;
                PanelNodeDetails.TextBoxNote.Text = n.Note;
                PanelNodeDetails.ButtonApply.Text = "Apply";
                PanelNodeDetails.ToolStripButtonNew.Enabled = true;
                PanelNodeDetails.ToolStripButtonCopy.Enabled = true;
                PanelNodeDetails.ToolStripButtonRemove.Enabled = true;
                PanelNodeDetails.ToolStripButtonArcs.Enabled = true;
            }
            else
            {
                PanelNodeDetails.Node = null;
                PanelNodeDetails.TextBoxRoomID.Text = "0";
                PanelNodeDetails.TextBoxRoomName.Text = "";
                PanelNodeDetails.TextBoxDescription.Text = "";
                PanelNodeDetails.TextBoxPosition.Text = "0, 0, 0";
                PanelNodeDetails.ColorPicker1.Color = Color.Transparent;
                PanelNodeDetails.TextBoxNote.Text = "";
                PanelNodeDetails.ButtonApply.Text = "Create";
                PanelNodeDetails.ToolStripButtonNew.Enabled = false;
                PanelNodeDetails.ToolStripButtonCopy.Enabled = false;
                PanelNodeDetails.ToolStripButtonRemove.Enabled = false;
                PanelNodeDetails.ToolStripButtonArcs.Enabled = false;
                PanelNodeDetails.ToolStripButtonArcs.Checked = false;
                PanelNodeDetails.PanelProperties.Visible = true;
                PanelNodeDetails.ArcDetails.Visible = false;
            }
        }

        private void UpdateLabelDetails()
        {
            PanelNodeDetails.Visible = false;
            PanelLabelDetails.Visible = true;
            if (!Information.IsNothing(m_SelectedLabels) && m_SelectedLabels.Count == 1)
            {
                PanelLabelDetails.Label = m_SelectedLabels[0];
            }
        }

        private NodeList m_SelectedNodes = new NodeList();
        private LabelList m_SelectedLabels = new LabelList();
        private int m_CurrentLevelZ = 0;

        public int Level
        {
            get
            {
                return m_CurrentLevelZ;
            }

            set
            {
                m_CurrentLevelZ = value;
                ToolStripLabelZ.Text = "Level: " + m_CurrentLevelZ.ToString();
            }
        }

        private void FlickerFreePanel1_Paint(object sender, PaintEventArgs e)
        {
            if (Information.IsNothing(m_NodeList))
                return;

            // First draw all lines
            foreach (Node n in m_NodeList)
            {
                if (!Information.IsNothing(n.Position))
                {
                    if (n.Position.Z <= m_CurrentLevelZ)
                    {
                        var oColorLine = m_oGlobals.PresetList["automapper.line"].FgColor;
                        var oColorLineStump = m_oGlobals.PresetList["automapper.linestump"].FgColor;
                        var oColorLineClimb = m_oGlobals.PresetList["automapper.lineclimb"].FgColor;
                        var oColorLineGo = m_oGlobals.PresetList["automapper.linego"].FgColor;
                        if (n.Position.Z != m_CurrentLevelZ) // Mark all other levels gray
                        {
                            oColorLine = m_oGlobals.PresetList["automapper.line"].BgColor;
                            oColorLineStump = m_oGlobals.PresetList["automapper.linestump"].BgColor;
                            oColorLineClimb = m_oGlobals.PresetList["automapper.lineclimb"].BgColor;
                            oColorLineGo = m_oGlobals.PresetList["automapper.linego"].BgColor;
                        }

                        foreach (Arc a in n.Arcs)
                        {
                            var pt1 = new Point3D(n.Position);

                            // Draw stumps for cardinal directions
                            if (Arc.IsCardinalDirection(a.Direction))
                            {
                                pt1.Offset(a.Direction, 10);
                                var oLinePen = new Pen(oColorLine);
                                if (Information.IsNothing(a.Destination))
                                {
                                    oLinePen.Color = oColorLineStump;
                                }

                                if (a.HideArc == true)
                                {
                                    oLinePen.EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor;
                                }

                                e.Graphics.DrawLine(oLinePen, ConvertPoint(n.Position), ConvertPoint(pt1));
                            }

                            if (a.HideArc == false && !Information.IsNothing(a.Destination))
                            {
                                if (a.Destination.Position.Z <= m_CurrentLevelZ)
                                {
                                    if (!Information.IsNothing(a.Destination.Position))
                                    {
                                        var pt2 = new Point3D(a.Destination.Position);
                                        if (Arc.IsCardinalDirection(a.Direction))
                                        {
                                            if (a.Destination.ContainsArc(Arc.ReverseDirection(a.Direction)))
                                            {
                                                if (a.Destination.Arcs[Arc.ReverseDirection(a.Direction)].DestinationID == n.ID) // Two way
                                                {
                                                    pt2.Offset(Arc.ReverseDirection(a.Direction), 10);
                                                }
                                            }
                                        }

                                        var oLinePen = new Pen(oColorLine);
                                        if (a.Direction == Direction.Climb)
                                            oLinePen.Color = oColorLineClimb;
                                        if (a.Direction == Direction.Go || a.Direction == Direction.Up || a.Direction == Direction.Down || a.Direction == Direction.Out)
                                            oLinePen.Color = oColorLineGo;
                                        e.Graphics.DrawLine(oLinePen, ConvertPoint(pt1), ConvertPoint(pt2));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Draw unlinked exits again (In case any other lines was put on top)
            foreach (Node n in m_NodeList)
            {
                if (!Information.IsNothing(n.Position))
                {
                    if (n.Position.Z == m_CurrentLevelZ)
                    {
                        foreach (Arc a in n.Arcs)
                        {
                            if (Information.IsNothing(a.Destination))
                            {
                                var pt = new Point3D(n.Position);
                                pt.Offset(a.Direction, 10);
                                var oLinePen = new Pen(m_oGlobals.PresetList["automapper.linestump"].FgColor);
                                if (m_ToggleMoveNodes == true || m_ToggleRecording == true)
                                {
                                    oLinePen = Pens.Red;
                                }

                                e.Graphics.DrawLine(oLinePen, ConvertPoint(n.Position), ConvertPoint(pt));
                            }
                        }
                    }
                }
            }

            // Draw all lower level rooms
            foreach (Node n in m_NodeList)
            {
                if (!Information.IsNothing(n.Position))
                {
                    bool bCurrentRoom = false;
                    if (!Information.IsNothing(m_CurrentNode))
                    {
                        if (n.ID == m_CurrentNode.ID)
                        {
                            bCurrentRoom = true;
                            m_AllowRecord = true;
                        }
                    }

                    if (n.Position.Z < m_CurrentLevelZ)
                    {
                        var oColorRoom = n.Color;
                        if (n.Color == Color.Transparent) { oColorRoom = m_oGlobals.PresetList["automapper.node"].FgColor; }
                        else oColorRoom = Color.FromArgb(m_oGlobals.Config.AutoMapperAlpha, n.Color.R, n.Color.G, n.Color.B);

                        var oColorRoomBorder = m_oGlobals.PresetList["automapper.line"].FgColor;
                        if (n.Position.Z != m_CurrentLevelZ) // Mark all other levels gray
                        {
                            oColorRoom = m_oGlobals.PresetList["automapper.node"].BgColor; // Color.FromArgb(255, 255, 192) ' Base BG
                            oColorRoomBorder = m_oGlobals.PresetList["automapper.line"].BgColor;
                        }

                        if (!Information.IsNothing(m_SelectedNodes.Find(n.ID)))
                        {
                            oColorRoom = Color.Blue;
                        }
                        else if (!Information.IsNothing(m_PathDestination))
                        {
                            if (m_PathDestination.Equals(n))
                            {
                                oColorRoom = m_oGlobals.PresetList["automapper.path"].FgColor;
                            }
                            else if (n.State == Node.States.EndPath)
                            {
                                oColorRoom = m_oGlobals.PresetList["automapper.path"].BgColor;
                            }
                        }

                        if (m_ToggleRecording == true || m_ToggleMoveNodes == true)
                        {
                            // Mark rooms that conflict as red
                            if (m_NodeList.IsPointOccupied(n) == true)
                            {
                                oColorRoom = Color.Red;
                            }
                        }

                        if (n.IsLabelFile == true && n.Position.Z == m_CurrentLevelZ) // Other map
                        {
                            var oWhere = ConvertPoint(n.Position, 4 * m_Scale);
                            e.Graphics.FillRectangle(new SolidBrush(oColorRoom), oWhere.X + 1, oWhere.Y + 1, 8 * m_Scale, 8 * m_Scale);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), oWhere.X, oWhere.Y, 8 * m_Scale, 8 * m_Scale);
                        }
                        else
                        {
                            var oWhere = ConvertPoint(n.Position, 4 * m_Scale);
                            e.Graphics.FillRectangle(new SolidBrush(oColorRoom), oWhere.X + 1, oWhere.Y + 1, 8 * m_Scale, 8 * m_Scale);
                            e.Graphics.DrawRectangle(new Pen(oColorRoomBorder), oWhere.X, oWhere.Y, 8 * m_Scale, 8 * m_Scale);
                            int iSpace = 2 * m_Scale;
                            if (bCurrentRoom == true) // Draw "HERE" dot
                            {
                                e.Graphics.FillRectangle(Brushes.Magenta, oWhere.X + iSpace, oWhere.Y + iSpace, 8 * m_Scale - 2 * iSpace + 1, 8 * m_Scale - 2 * iSpace + 1);
                            }
                        }
                    }
                }
            }

            // Draw all current level rooms
            foreach (Node n in m_NodeList)
            {
                if (!Information.IsNothing(n.Position))
                {
                    bool bCurrentRoom = false;
                    if (!Information.IsNothing(m_CurrentNode))
                    {
                        if (n.ID == m_CurrentNode.ID)
                        {
                            bCurrentRoom = true;
                            m_AllowRecord = true;
                        }
                    }

                    if (n.Position.Z == m_CurrentLevelZ)
                    {
                        var oColorRoom = n.Color;
                        if (n.Color == Color.Transparent) { oColorRoom = m_oGlobals.PresetList["automapper.node"].FgColor; }
                        else oColorRoom = Color.FromArgb(m_oGlobals.Config.AutoMapperAlpha, n.Color.R, n.Color.G, n.Color.B);
                        var oColorRoomBorder = m_oGlobals.PresetList["automapper.line"].FgColor;
                        if (n.Position.Z != m_CurrentLevelZ) // Mark all other levels gray
                        {
                            oColorRoom = m_oGlobals.PresetList["automapper.node"].BgColor;
                            oColorRoomBorder = m_oGlobals.PresetList["automapper.line"].BgColor;
                        }

                        if (!Information.IsNothing(m_SelectedNodes.Find(n.ID)))
                        {
                            oColorRoom = Color.Blue;
                        }
                        else if (!Information.IsNothing(m_PathDestination))
                        {
                            if (m_PathDestination.Equals(n))
                            {
                                oColorRoom = m_oGlobals.PresetList["automapper.path"].FgColor;
                            }
                            else if (n.State == Node.States.EndPath)
                            {
                                oColorRoom = m_oGlobals.PresetList["automapper.path"].BgColor;
                            }
                        }

                        if (m_ToggleRecording == true || m_ToggleMoveNodes == true)
                        {
                            // Mark rooms that conflict as red
                            if (m_NodeList.IsPointOccupied(n) == true)
                            {
                                oColorRoom = Color.Red;
                            }
                        }

                        if (n.IsLabelFile == true && n.Position.Z == m_CurrentLevelZ) // Other map
                        {
                            var oWhere = ConvertPoint(n.Position, 4 * m_Scale);
                            e.Graphics.FillRectangle(new SolidBrush(oColorRoom), oWhere.X + 1, oWhere.Y + 1, 8 * m_Scale, 8 * m_Scale);
                            e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), oWhere.X, oWhere.Y, 8 * m_Scale, 8 * m_Scale);
                        }
                        else
                        {
                            // Debug.WriteLine("Drawing node at "&& n.Position.ToString())
                            var oWhere = ConvertPoint(n.Position, 4 * m_Scale);
                            e.Graphics.FillRectangle(new SolidBrush(oColorRoom), oWhere.X + 1, oWhere.Y + 1, 8 * m_Scale, 8 * m_Scale);
                            e.Graphics.DrawRectangle(new Pen(oColorRoomBorder), oWhere.X, oWhere.Y, 8 * m_Scale, 8 * m_Scale);
                            int iSpace = 2 * m_Scale;
                            if (bCurrentRoom == true) // Draw "HERE" dot
                            {
                                e.Graphics.FillRectangle(Brushes.Magenta, oWhere.X + iSpace, oWhere.Y + iSpace, 8 * m_Scale - 2 * iSpace + 1, 8 * m_Scale - 2 * iSpace + 1);
                            }
                        }
                    }
                }
            }

            // Draw labels
            Font LabelText = new Font(m_Font.Name, (m_Font.Size * m_Scale));

            if (!Information.IsNothing(m_NodeList))
            {
                foreach (Label l in m_NodeList.Labels)
                {
                    if (l.Position.Z == m_CurrentLevelZ)
                    {
                        var r = new RectangleF();
                        r.Height = 15;

                        var m_Offset = GetOffset();
                        r.X = (l.Position.X * m_Scale) + m_Offset.X;
                        r.Y = (l.Position.Y * m_Scale) + m_Offset.Y;
                        var size = e.Graphics.MeasureString(l.Text, LabelText);
                        r.Width = size.Width;
                        l.Rectangle = r;

                        var b = SystemBrushes.Window;
                        var bt = new SolidBrush(m_oGlobals.PresetList["automapper.panel"].FgColor);
                        if (m_SelectedLabels.Contains(l))
                        {
                            b = SystemBrushes.Highlight;
                            bt = new SolidBrush(SystemColors.HighlightText);
                            e.Graphics.FillRectangle(b, l.Rectangle.X, l.Rectangle.Y, l.Rectangle.Width, l.Rectangle.Height);
                            e.Graphics.DrawRectangle(SystemPens.WindowText, l.Rectangle.X, l.Rectangle.Y, l.Rectangle.Width, l.Rectangle.Height);
                        }

                        e.Graphics.DrawString(l.Text, LabelText, bt, l.Rectangle.X + 1, l.Rectangle.Y + 1);
                    }
                }
            }

            // Draw lasso
            if (!Information.IsNothing(m_LassoStart))
            {
                var p = new Pen(m_oGlobals.PresetList["automapper.line"].FgColor);
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawRectangle(p, m_Lasso);
            }
        }

        private void CheckScrollTo(int NodeX, int NodeY)
        {
            // Do not scroll if current possition is inside visible area
            int iScrollX = 0;
            int iScrollY = 0;
            var m_Offset = GetOffset();
            NodeX *= m_Scale;
            NodeX += m_Offset.X;
            NodeY *= m_Scale;
            NodeY += m_Offset.Y;

            var withBlock = PanelBase.AutoScrollPosition;

            if (NodeX < -withBlock.X)
            {
                iScrollX = (int)(NodeX - (PanelBase.Width - SystemInformation.VerticalScrollBarWidth - 10 * m_Scale) / (double)2);
            }
            else if (NodeX > -withBlock.X + (PanelBase.Width - SystemInformation.VerticalScrollBarWidth - 10 * m_Scale))
            {
                iScrollX = (int)(NodeX - (PanelBase.Width - SystemInformation.VerticalScrollBarWidth - 10 * m_Scale) / (double)2);
            }

            if (NodeY < -withBlock.Y)
            {
                iScrollY = (int)(NodeY - (PanelBase.Height - SystemInformation.HorizontalScrollBarHeight - 10 * m_Scale) / (double)2);
            }
            else if (NodeY > -withBlock.Y + (PanelBase.Height - SystemInformation.HorizontalScrollBarHeight - 10 * m_Scale))
            {
                iScrollY = (int)(NodeY - (PanelBase.Height - SystemInformation.HorizontalScrollBarHeight - 10 * m_Scale) / (double)2);
            }

            if (iScrollX == 0 && iScrollY == 0)
                return;
            if (iScrollX == 0)
                iScrollX = -withBlock.X;
            if (iScrollY == 0)
                iScrollY = -withBlock.Y;
            PanelBase.AutoScrollPosition = new Point(iScrollX, iScrollY);

        }

        private bool m_ToggleSnapToGrid = true;

        private void ToolStripButtonSnap_Click(object sender, EventArgs e)
        {
            SetSnapToggle(!ToolStripButtonSnap.Checked);
        }

        public void SetSnapToggle(bool toggle)
        {
            ToolStripButtonSnap.Checked = toggle;
            m_ToggleSnapToGrid = ToolStripButtonSnap.Checked;
            PanelMap.DrawLines = m_ToggleSnapToGrid;
        }

        private bool m_ToggleMoveNodes = false;

        private void ToolStripButtonMoveNodes_Click(object sender, EventArgs e)
        {
            ToolStripButtonMoveNodes.Checked = !ToolStripButtonMoveNodes.Checked;
            m_ToggleMoveNodes = ToolStripButtonMoveNodes.Checked;
            ToolStripButtonRemove.Enabled = ToolStripButtonMoveNodes.Checked;
            if (ToolStripButtonMoveNodes.Checked == true) // No destination in edit mode
            {
                m_PathDestination = null;
            }

            if (m_SelectedNodes.Count > 1) // Clear out lasso
            {
                m_SelectedNodes.Clear();
                m_SelectedLabels.Clear();
            }

            PanelMap.Invalidate();
        }

        // Remove does not work if multiple rooms match in Equals()
        private void EraseSelected()
        {
            if (!Information.IsNothing(m_SelectedNodes))
            {
                foreach (Node n in m_SelectedNodes)
                {
                    if (m_NodeList.Contains(n.ID))
                    {
                        m_NodeList.Remove(n);
                    }
                }

                // Clean up Arcs
                foreach (Node n in m_NodeList)
                {
                    foreach (Arc a in n.Arcs)
                    {
                        if (!Information.IsNothing(a.Destination) && Information.IsNothing(m_NodeList.Find(a.DestinationID)))
                        {
                            Node argdest = null;
                            a.SetDestination(argdest);
                        }
                    }
                }

                m_SelectedNodes.Clear();
            }

            if (!Information.IsNothing(m_SelectedLabels))
            {
                // Remove Labels
                foreach (Label l in m_SelectedLabels)
                {
                    if (m_NodeList.Labels.Contains(l))
                    {
                        m_NodeList.Labels.Remove(l);
                    }
                }

                m_SelectedLabels.Clear();
            }

            PanelMap.Invalidate();
        }

        private void ToolStripButtonRemove_Click(object sender, EventArgs e)
        {
            EraseSelected();
        }

        public void EraseRoom(Node n)
        {
            m_SelectedNodes.Clear();
            m_SelectedNodes.Add(n);
            EraseSelected();
        }

        private void ToolStripButtonProperties_Click(object sender, EventArgs e)
        {
            ToolStripButtonProperties.Checked = !ToolStripButtonProperties.Checked;
            PanelDetails.Visible = ToolStripButtonProperties.Checked;
            UpdateMapSize();
        }

        // Set size to minimum visible area
        private void PanelBase_Resize(object sender, EventArgs e)
        {
            UpdateMapSize();
        }

        private void UpdateMapSize(bool MapChanged = false)
        {
            // Sometimes this was changed, making the map appear outside of area
            if (!Information.IsNothing(m_NodeList) && m_NodeList.Count > 0) // No list = don't care
            {
                var r = m_NodeList.GetMapSize();
                r.Height *= m_Scale;
                r.Width *= m_Scale;
                r.Height += 5 * m_Scale;
                r.Width += 5 * m_Scale;
                if ((PanelBase.Width > r.Width) && (PanelBase.Height > r.Height))
                {
                    if (PanelMap.Dock == DockStyle.None)
                    {
                        PanelMap.Dock = DockStyle.Fill;
                        PanelMap.Top = 0;
                        PanelMap.Left = 0;
                    }
                }
                else
                {
                    if (PanelMap.Dock == DockStyle.Fill)
                    {
                        PanelMap.Dock = DockStyle.None;
                    }

                    if (MapChanged)
                    {
                        PanelBase.AutoScrollPosition = new Point(0, 0);
                        PanelMap.Top = 0;
                        PanelMap.Left = 0;
                    }
                    PanelMap.Width = r.Width;
                    PanelMap.Height = r.Height;
                }
            }
            else if (PanelMap.Dock == DockStyle.None)
            {
                PanelMap.Top = 0;
                PanelMap.Left = 0;
                PanelMap.Dock = DockStyle.Fill;
            }

            if (Visible == true)
            {
                if (!Information.IsNothing(m_CurrentNode))
                {
                    CheckScrollTo(m_CurrentNode.Position.X, m_CurrentNode.Position.Y + 20);
                }
            }
        }

        private void PanelNodeDetails_UpdateMap()
        {
            PanelMap.Invalidate();
        }

        public void UpdateMap()
        {
            PanelMap.Invalidate();
        }

        private void PanelNodeDetails_CopyNode()
        {
            if (!Information.IsNothing(m_SelectedNodes))
            {
                m_SelectedNodes.Clear();
                PanelNodeDetails.Node = null;
                PanelNodeDetails.TextBoxRoomID.Text = "0";
                PanelNodeDetails.ButtonApply.Text = "Create";
                PanelNodeDetails.ToolStripButtonNew.Enabled = false;
                PanelNodeDetails.ToolStripButtonCopy.Enabled = false;
                PanelNodeDetails.ToolStripButtonRemove.Enabled = false;
                PanelNodeDetails.ToolStripButtonArcs.Enabled = false;
                PanelMap.Invalidate();
            }
        }

        private void PanelNodeDetails_AddNode(Node oNode)
        {
            if (!Information.IsNothing(m_NodeList))
            {
                oNode.ID = m_NodeList.NextID;
                m_NodeList.Add(oNode);
                m_SelectedNodes.Clear();
                m_SelectedNodes.Add(oNode);
                UpdateMapSize();
                var m_Offset = m_NodeList.GetOffset();
                m_Offset.X *= m_Scale;
                m_Offset.Y *= m_Scale;
                _mapOffsetX = m_Offset.X;
                _mapOffsetY = m_Offset.Y;
                PanelMap.Invalidate();
                UpdateNodeDetails();
            }
        }

        private void PanelNodeDetails_RemoveNode()
        {
            EraseSelected();
        }

        private void PanelNodeDetails_NewNode()
        {
            if (!Information.IsNothing(m_SelectedNodes))
            {
                m_SelectedNodes.Clear();
                UpdateNodeDetails();
                PanelMap.Invalidate();
            }
        }

        private void PanelNodeDetails_ArcChanged()
        {
            m_NodeList.FixArcLinks();
            PanelMap.Invalidate();
        }

        private void ToolStripButtonUp_Click(object sender, EventArgs e)
        {
            Level += 1;
            PanelMap.Invalidate();
        }

        private void ToolStripButtonDown_Click(object sender, EventArgs e)
        {
            Level -= 1;
            PanelMap.Invalidate();
        }

        private void ToolStripButtonWalk_Click(object sender, EventArgs e)
        {
            if (ToolStripButtonWalk.Checked == true)
            {
                Interaction.MsgBox("Select destination node by right clicking on it.");
            }
        }

        private void ToolStripButtonAllowDuplicates_Click(object sender, EventArgs e)
        {
            SetAllowDuplicatesToggle(!ToolStripButtonAllowDuplicates.Checked);
        }

        public void SetAllowDuplicatesToggle(bool toggle)
        {
            ToolStripButtonAllowDuplicates.Checked = !ToolStripButtonAllowDuplicates.Checked;
            ToggleAllowDuplicates?.Invoke(ToolStripButtonAllowDuplicates.Checked);
        }

        private void ToolStripButtonLockPositions_Click(object sender, EventArgs e)
        {
            SetLockPositionsToggle(!ToolStripButtonLockPositions.Checked);
        }

        public void SetLockPositionsToggle(bool toggle)
        {
            ToolStripButtonLockPositions.Checked = toggle;
        }

        private void ToolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            m_Scale += 1;
            UpdateMapSize();
            var m_Offset = m_NodeList.GetOffset();
            m_Offset.X *= m_Scale;
            m_Offset.Y *= m_Scale;
            _mapOffsetX = m_Offset.X;
            _mapOffsetY = m_Offset.Y;
            PanelMap.Invalidate();
            if (m_Scale > 5)
            {
                ToolStripButtonZoomIn.Enabled = false;
            }

            if (m_Scale > 1)
            {
                ToolStripButtonZoomOut.Enabled = true;
            }
        }

        private void ToolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            m_Scale -= 1;
            UpdateMapSize();
            var m_Offset = m_NodeList.GetOffset();
            m_Offset.X *= m_Scale;
            m_Offset.Y *= m_Scale;
            _mapOffsetX = m_Offset.X;
            _mapOffsetY = m_Offset.Y;
            PanelMap.Invalidate();
            if (m_Scale < 6)
            {
                ToolStripButtonZoomIn.Enabled = true;
            }

            if (m_Scale < 2)
            {
                ToolStripButtonZoomOut.Enabled = false;
            }
        }

        private void ToolStripButtonReload_Click(object sender, EventArgs e)
        {
            if (!LoadMaps())
            {
                Interaction.MsgBox("Failed to load maps.");
            }
        }

        private void MapForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                PanelMap.Invalidate();
            }
        }

        private void EventAddLabel()
        {
            m_NodeList.Labels.Add("New Label", new Point3D(0, 0, m_CurrentLevelZ));
            m_SelectedLabels.Clear();
            var argItem = m_NodeList.Labels[m_NodeList.Labels.Count - 1];
            m_SelectedLabels.Add(argItem);
            UpdateLabelDetails();
            PanelMap.Invalidate();
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            int id = 1;
            foreach (Node n in m_NodeList)
            {
                // Fix arches linked to this one
                foreach (Node na in m_NodeList)
                {
                    foreach (Arc a in na.Arcs)
                    {
                        if (a.DestinationID == n.ID)
                        {
                            a.DestinationID = id;
                        }
                    }
                }

                n.ID = id;
                id += 1;
            }

            Interaction.MsgBox("All nodes reset!");
        }

        private Form _MdiParent;

        private void ToolStripButtonDock_Click(object sender, EventArgs e)
        {
            if (ToolStripButtonDock.Checked == true)
            {
                MdiParent = _MdiParent;
                if (!Information.IsNothing(MdiParent))
                {
                    Top = 0;
                    Height = ((FormMain)MdiParent).ClientHeight - SystemInformation.Border3DSize.Height * 2;
                    Left = (int)(MdiParent.ClientSize.Width / (double)2 - SystemInformation.Border3DSize.Width);
                    Width = MdiParent.ClientSize.Width - SystemInformation.Border3DSize.Width * 2 - Left;
                }
            }
            else
            {
                _MdiParent = MdiParent;
                MdiParent = null;
            }
        }
    }
}
