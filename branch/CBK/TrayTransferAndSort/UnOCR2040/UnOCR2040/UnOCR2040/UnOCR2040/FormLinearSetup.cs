using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Xml;

using log4net;

namespace UnOCR2040
{
    public partial class FormLinearSetup : Form
    {
        private Thread motorpositionThread = null;
        private Thread updateGUIThread = null;

        public readonly log4net.ILog SetupLogger;

        private bool _bEdit = false;
        private static FormLinearSetup _instance;
        public static FormLinearSetup Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormLinearSetup();
                }

                try
                {
                    if (_instance.motorpositionThread == null)
                    {
                        //ThreadStart start = delegate
                        //{
                        //    CheckPosition();
                        //};

                        //_instance.motorpositionThread = new Thread(start);
                        //_instance.motorpositionThread.Priority = ThreadPriority.Normal;
                        //_instance.motorpositionThread.Start();
                    }
                    else
                    {
                        //if (!_instance.motorpositionThread.IsAlive)
                        //{
                        //    ThreadStart start = delegate
                        //    {
                        //        CheckPosition();
                        //    };

                        //    _instance.motorpositionThread = new Thread(start);
                        //    _instance.motorpositionThread.Priority = ThreadPriority.Normal;
                        //    _instance.motorpositionThread.Start();
                        //}
                    }

                    if (_instance.updateGUIThread == null)
                    {
                        //ThreadStart startUpdateGUI = delegate
                        //{
                        //    UpdateGUI();
                        //};

                        //_instance.updateGUIThread = new Thread(startUpdateGUI);
                        //_instance.updateGUIThread.Priority = ThreadPriority.Normal;
                        //_instance.updateGUIThread.Start();
                    }
                    else
                    {
                        //if (!_instance.updateGUIThread.IsAlive)
                        //{
                        //    ThreadStart startUpdateGUI = delegate
                        //    {
                        //        UpdateGUI();
                        //    };

                        //    _instance.updateGUIThread = new Thread(startUpdateGUI);
                        //    _instance.updateGUIThread.Priority = ThreadPriority.Normal;
                        //    _instance.updateGUIThread.Start();
                        //}
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                return _instance;
            }
        }

        private Point _formlocation = new Point();
        private FormLinearSetup()
        {
            InitializeComponent();

            _wdtraypos1[0, 0] = FormUnOCR2040.Instance._wdtrayCoord11;
            _wdtraypos1[0, 1] = FormUnOCR2040.Instance._wdtrayCoord12;
            _wdtraypos1[1, 0] = FormUnOCR2040.Instance._wdtrayCoord13;
            _wdtraypos1[1, 1] = FormUnOCR2040.Instance._wdtrayCoord14;

            _wdtraypos2[0, 0] = FormUnOCR2040.Instance._wdtrayCoord21;
            _wdtraypos2[0, 1] = FormUnOCR2040.Instance._wdtrayCoord22;
            _wdtraypos2[1, 0] = FormUnOCR2040.Instance._wdtrayCoord23;
            _wdtraypos2[1, 1] = FormUnOCR2040.Instance._wdtrayCoord24;

            _hgsttraypos[0, 0] = FormUnOCR2040.Instance._hgsttrayCoord1;
            _hgsttraypos[0, 1] = FormUnOCR2040.Instance._hgsttrayCoord2;
            _hgsttraypos[0, 2] = FormUnOCR2040.Instance._hgsttrayCoord3;
            _hgsttraypos[0, 3] = FormUnOCR2040.Instance._hgsttrayCoord4;
            _hgsttraypos[1, 0] = FormUnOCR2040.Instance._hgsttrayCoord5;
            _hgsttraypos[1, 1] = FormUnOCR2040.Instance._hgsttrayCoord6;
            _hgsttraypos[1, 2] = FormUnOCR2040.Instance._hgsttrayCoord7;
            _hgsttraypos[1, 3] = FormUnOCR2040.Instance._hgsttrayCoord8;

            SetupLogger = log4net.LogManager.GetLogger("Setup");

            string configPath = System.Windows.Forms.Application.StartupPath;
            System.IO.FileInfo configFile = new System.IO.FileInfo(configPath + @"\UnOCR2040.config");

            log4net.Config.XmlConfigurator.Configure(configFile);
            //MainLogger.Info("FormUnOCR2040 ctor");
            //SetupLogger.Info("FormLinearSetup ctor");

            txtboxVel.Text = FormUnOCR2040.Instance._pbaSpeedElem.InnerText;
            txtboxAcc.Text = FormUnOCR2040.Instance._pbaAccElem.InnerText;
            txtboxDec.Text = FormUnOCR2040.Instance._pbaDeclElem.InnerText;
            txtboxStep.Text = FormUnOCR2040.Instance._pbaStepElem.InnerText;

            //Vel
            _dblVel = Convert.ToDouble(txtboxVel.Text);
            //Acc
            _dblAcc = Convert.ToDouble(txtboxAcc.Text);
            //Decl
            _dblDecl = Convert.ToDouble(txtboxDec.Text);
            //Step
            _dblStep = Convert.ToDouble(txtboxStep.Text);

            //ThreadStart start = delegate
            //{
            //    CheckPosition();
            //};

            //motorpositionThread = new Thread(start);
            //motorpositionThread.Name = "motorpositionThread";
            //motorpositionThread.Priority = ThreadPriority.Normal;
            //motorpositionThread.Start();

            //ThreadStart startUpdateGUI = delegate
            //{
            //    UpdateGUI();
            //};

            //updateGUIThread = new Thread(startUpdateGUI);
            //updateGUIThread.Priority = ThreadPriority.Normal;
            //updateGUIThread.Start();



            for (int i = 1; i <= 4; i++)
            {
                //btnWDTrayTeach
                Control[] btnTeach = this.Controls.Find("btnWDTrayTeach" + i.ToString("0"), true);
                btnTeach[0].Enabled = _bEdit;
            }

            for (int i = 1; i <= 8; i++)
            {
                //btnHGSTTrayTeach
                Control[] btnTeach = this.Controls.Find("btnHGSTTrayTeach" + i.ToString("0"), true);
                btnTeach[0].Enabled = _bEdit;
            }

            btnSave.Enabled = _bEdit;
            _formlocation = this.Location;
        }


        #region delegate update position X
        private delegate void updateXPos();
        private void UpdateXPos()
        {
            txtboxReadonlyX.Text = FormUnOCR2040.Instance.strAxisXPos;
        }
        public void DelegateUpdateXPos()
        {
            Invoke(new updateXPos(UpdateXPos));
        }
        #endregion

        #region delegate update position Y
        private delegate void updateYPos();
        private void UpdateYPos()
        {
            txtboxReadonlyY.Text = FormUnOCR2040.Instance.strAxisYPos;
        }
        public void DelegateUpdateYPos()
        {
            Invoke(new updateYPos(UpdateYPos));
        }
        #endregion

        public static void CheckPosition()
        {
            while (true)
            {
                FormLinearSetup.Instance.DelegateUpdateXPos();
                FormLinearSetup.Instance.DelegateUpdateYPos();

                Application.DoEvents();
                Thread.Sleep(200);
            }
        }

        public static void UpdateGUI()
        {
            while (true)
            {
                FormLinearSetup.Instance.DelegateUpdateTurnTablePos();

                Application.DoEvents();
                Thread.Sleep(200);
            }
        }

        private bool _bTurnTableTray1In = false;
        #region delegate update turn table position
        private delegate void updateTurnTablePos();
        private void UpdateTurnTablePos()
        {
            if (IOs.Instance.IsTray1In())
            {
                lblTurnTable.Text = "Tray# 1 in";
                _bTurnTableTray1In = true;
            }
            if (IOs.Instance.IsTray2In())
            {
                lblTurnTable.Text = "Tray# 2 in";
                _bTurnTableTray1In = false;
            }
        }
        public void DelegateUpdateTurnTablePos()
        {
            Invoke(new updateTurnTablePos(UpdateTurnTablePos));
        }
        #endregion


        private void FormLinearSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (motorpositionThread != null)
            {
                motorpositionThread.Abort();
            }

            if (updateGUIThread != null)
            {
                updateGUIThread.Abort();
            }
        }

        private void btnPowerON_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAPowerOn();
        }

        private void btnPowerOFF_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAPowerOff();
        }

        private void btnClearErr_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAClearError();
        }


        #region jog
        private void btnPosY_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAAxisYStepPositive(Convert.ToDouble(txtboxStep.Text));
        }

        private void btnNegY_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAAxisYStepNegative(Convert.ToDouble(txtboxStep.Text));
        }

        private void btnPosX_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAAxisXStepPositive(Convert.ToDouble(txtboxStep.Text));
        }

        private void btnNegX_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBAAxisXStepNegative(Convert.ToDouble(txtboxStep.Text));
        }
        #endregion

        private PBACoordinate[,] _wdtraypos1 = new PBACoordinate[2, 2];
        private PBACoordinate[,] _wdtraypos2 = new PBACoordinate[2, 2];

        #region WDTrayTeach
        private void btnWDTrayTeach1_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());

            if (_bTurnTableTray1In)
            {
                _wdtraypos1[0, 0] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord11 = coord1;
            }
            else
            {
                _wdtraypos2[0, 0] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord21 = coord1;
            }
        }

        private void btnWDTrayTeach2_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());

            if (_bTurnTableTray1In)
            {
                _wdtraypos1[0, 1] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord12 = coord1;
            }
            else
            {
                _wdtraypos2[0, 1] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord22 = coord1;
            }
        }

        private void btnWDTrayTeach3_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());

            if (_bTurnTableTray1In)
            {
                _wdtraypos1[1, 0] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord13 = coord1;
            }
            else
            {
                _wdtraypos2[1, 0] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord23 = coord1;
            }
        }

        private void btnWDTrayTeach4_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());

            if (_bTurnTableTray1In)
            {
                _wdtraypos1[1, 1] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord14 = coord1;
            }
            else
            {
                _wdtraypos2[1, 1] = coord1;
                FormUnOCR2040.Instance._wdtrayCoord24 = coord1;
            }
        }
        #endregion


        #region WDTrayMove
        private void btnWDTrayMove1_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            if (_bTurnTableTray1In)
            {
                Console.WriteLine("Move: " + _wdtraypos1[0, 0].X.ToString() + "," + _wdtraypos1[0, 0].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos1[0, 0].X, _wdtraypos1[0, 0].Y);
            }
            else
            {
                Console.WriteLine("Move: " + _wdtraypos2[0, 0].X.ToString() + "," + _wdtraypos2[0, 0].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos2[0, 0].X, _wdtraypos2[0, 0].Y);
            }
        }

        private void btnWDTrayMove2_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            if (_bTurnTableTray1In)
            {
                Console.WriteLine("Move: " + _wdtraypos1[0, 1].X.ToString() + "," + _wdtraypos1[0, 1].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos1[0, 1].X, _wdtraypos1[0, 1].Y);
            }
            else
            {
                Console.WriteLine("Move: " + _wdtraypos2[0, 1].X.ToString() + "," + _wdtraypos2[0, 1].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos2[0, 1].X, _wdtraypos2[0, 1].Y);
            }
        }

        private void btnWDTrayMove3_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            if (_bTurnTableTray1In)
            {
                Console.WriteLine("Move: " + _wdtraypos1[1, 0].X.ToString() + "," + _wdtraypos1[1, 0].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos1[1, 0].X, _wdtraypos1[1, 0].Y);
            }
            else
            {
                Console.WriteLine("Move: " + _wdtraypos2[1, 0].X.ToString() + "," + _wdtraypos2[1, 0].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos2[1, 0].X, _wdtraypos2[1, 0].Y);
            }

        }

        private void btnWDTrayMove4_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            if (_bTurnTableTray1In)
            {
                Console.WriteLine("Move: " + _wdtraypos1[1, 1].X.ToString() + "," + _wdtraypos1[1, 1].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos1[1, 1].X, _wdtraypos1[1, 1].Y);
            }
            else
            {
                Console.WriteLine("Move: " + _wdtraypos2[1, 1].X.ToString() + "," + _wdtraypos2[1, 1].Y.ToString());
                FormUnOCR2040.Instance.PBAMoveAbs(_wdtraypos2[1, 1].X, _wdtraypos2[1, 1].Y);
            }
        }
        #endregion

        private PBACoordinate[,] _hgsttraypos = new PBACoordinate[2, 4];

        #region HGSTTrayTeach
        private void btnHGSTTrayTeach1_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[0, 0] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord1 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach2_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[0, 1] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord2 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach3_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[0, 2] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord3 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach4_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[0, 3] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord4 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach5_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[1, 0] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord5 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach6_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[1, 1] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord6 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach7_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[1, 2] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord7 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private void btnHGSTTrayTeach8_Click(object sender, EventArgs e)
        {
            PBACoordinate coord1 = new PBACoordinate(FormUnOCR2040.Instance.strAxisXPos, FormUnOCR2040.Instance.strAxisYPos);
            _hgsttraypos[1, 3] = coord1;
            FormUnOCR2040.Instance._hgsttrayCoord8 = coord1;
            Console.WriteLine("Teach: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }
        #endregion


        #region HGSTTrayMove
        private void btnHGSTTrayMove1_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[0, 0].X.ToString() + "," + _hgsttraypos[0, 0].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[0, 0].X, _hgsttraypos[0, 0].Y);
        }

        private void btnHGSTTrayMove2_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[0, 1].X.ToString() + "," + _hgsttraypos[0, 1].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[0, 1].X, _hgsttraypos[0, 1].Y);
        }

        private void btnHGSTTrayMove3_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[0, 2].X.ToString() + "," + _hgsttraypos[0, 2].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[0, 2].X, _hgsttraypos[0, 2].Y);
        }

        private void btnHGSTTrayMove4_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[0, 3].X.ToString() + "," + _hgsttraypos[0, 3].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[0, 3].X, _hgsttraypos[0, 3].Y);
        }

        private void btnHGSTTrayMove5_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[1, 0].X.ToString() + "," + _hgsttraypos[1, 0].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[1, 0].X, _hgsttraypos[1, 0].Y);
        }

        private void btnHGSTTrayMove6_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[1, 1].X.ToString() + "," + _hgsttraypos[1, 1].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[1, 1].X, _hgsttraypos[1, 1].Y);
        }

        private void btnHGSTTrayMove7_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[1, 2].X.ToString() + "," + _hgsttraypos[1, 2].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[1, 2].X, _hgsttraypos[1, 2].Y);
        }

        private void btnHGSTTrayMove8_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            Console.WriteLine("Move: " + _hgsttraypos[1, 3].X.ToString() + "," + _hgsttraypos[1, 3].Y.ToString());
            FormUnOCR2040.Instance.PBAMoveAbs(_hgsttraypos[1, 3].X, _hgsttraypos[1, 3].Y);
        }
        #endregion

        private void btnNozzleExtend_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.Extend();
            IOs.Instance.NozzleExtend();
        }

        private void btnNozzleRetract_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.Retract();
            IOs.Instance.NozzleRetract();
        }

        private void btnNozzleUp_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.MoveUp();
            IOs.Instance.NozzleUp();
        }

        private void btnNozzleDown_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.MoveDown();
            IOs.Instance.NozzleDown();
        }

        private void btnNozzleVacOn_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.VacAllOn();
            IOs.Instance.NozzleVacAllOn();
        }

        private void btnNozzleVacOff_Click(object sender, EventArgs e)
        {
            //FormUnOCR2040.Instance.PnP.VacAllOff();
            IOs.Instance.NozzleVacAllOff();
        }

        private double _dblVel = 0.0;
        private double _dblAcc = 0.0;
        private double _dblDecl = 0.0;
        private double _dblStep = 0.0;
        private void btnSet_Click(object sender, EventArgs e)
        {
            //Vel
            _dblVel = Convert.ToDouble(txtboxVel.Text);
            //Acc
            _dblAcc = Convert.ToDouble(txtboxAcc.Text);
            //Decl
            _dblDecl = Convert.ToDouble(txtboxDec.Text);
            //Step
            _dblStep = Convert.ToDouble(txtboxStep.Text);
            
            FormUnOCR2040.Instance._pbaSpeedElem.InnerText = _dblVel.ToString();
            FormUnOCR2040.Instance.PBASetSpeed(_dblVel);

            FormUnOCR2040.Instance._pbaAccElem.InnerText = _dblAcc.ToString();
            FormUnOCR2040.Instance.PBASetAccl(_dblAcc);

            FormUnOCR2040.Instance._pbaDeclElem.InnerText = _dblDecl.ToString();
            FormUnOCR2040.Instance.PBASetDecl(_dblDecl);

            FormUnOCR2040.Instance._pbaStepElem.InnerText = txtboxStep.Text;

            SetupLogger.Info("PBASetSpeed: " + _dblVel.ToString());
            SetupLogger.Info("PBASetAccl: " + _dblAcc.ToString());
            SetupLogger.Info("PBASetDecl: " + _dblDecl.ToString());
            SetupLogger.Info("Step: " + _dblStep.ToString());
        }

        private void btnManualGo_Click(object sender, EventArgs e)
        {
            FormUnOCR2040.Instance.PBASetSpeed(500);
            
            PBACoordinate coord1 = new PBACoordinate(txtboxManualXPos.Text, txtboxManualYPos.Text);

            FormUnOCR2040.Instance.PBAMoveAbs(coord1.X, coord1.Y);
            Console.WriteLine("Go: " + coord1.X.ToString() + "," + coord1.Y.ToString());
        }

        private string _configFilename = "";
        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSet_Click(sender, e);

            SaveFileDialog saveFileDiag = new SaveFileDialog();
            saveFileDiag.Filter = "xml files|*.xml";
            string configPath = System.Windows.Forms.Application.StartupPath;
            saveFileDiag.InitialDirectory = configPath;

            DialogResult result = saveFileDiag.ShowDialog();
            if (result == DialogResult.OK)
            {
                Console.WriteLine(saveFileDiag.FileName);
            }

            if (saveFileDiag.FileName == _configFilename)
            {
                result = MessageBox.Show("Do you want to replace existing teachning points?", "Confirm Save As", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                try
                {
                    if (result == DialogResult.Yes)
                    {
                        _savePBAConfig(_configFilename);
                    }
                }
                catch (Exception ex)
                {
                    SetupLogger.Info("FormLinearSetup: exception " + ex.Message);
                }
            }
            else
            {
                _configFilename = saveFileDiag.FileName;
                FormUnOCR2040.Instance._strPBAConfigFilename = _configFilename;
                try
                {
                    _savePBAConfig(_configFilename);
                }
                catch (Exception ex)
                {
                    SetupLogger.Info("FormLinearSetup: exception " + ex.Message);
                }
            }

            FormUnOCR2040.Instance.LoadPBAConfig(_configFilename);
        }

        private void _savePBAConfig()
        {
            try
            {
                _savePBAConfig("PBA.xml");
            }
            catch (Exception ex)
            {
                SetupLogger.Info("_savePBAConfig: exception " + ex.Message);
            }
        }

        private void _savePBAConfig(string savedFilename)
        {
            try
            {
                XmlDocument pbaXmlConfigDoc = new XmlDocument();


                XmlElement rootConfigXmlElem = pbaXmlConfigDoc.CreateElement("UnOCR2040");
                pbaXmlConfigDoc.AppendChild(rootConfigXmlElem);     ///UnOCR2040
                rootConfigXmlElem = pbaXmlConfigDoc.DocumentElement;
                rootConfigXmlElem.SetAttribute("machinenumber", FormUnOCR2040.Instance._strMachineNumber);
                rootConfigXmlElem.SetAttribute("line", FormUnOCR2040.Instance._strLine);

                // /UnOCR2040/PBA
                XmlElement pbaXmlElem = pbaXmlConfigDoc.CreateElement("PBA");
                rootConfigXmlElem.AppendChild(pbaXmlElem);          ///UnOCR2040/PBA
                pbaXmlElem.SetAttribute("product", FormUnOCR2040.Instance._strPBAConfigProduct);
                pbaXmlElem.SetAttribute("partnumber", FormUnOCR2040.Instance._strPBAConfigPartNumber);
                
                // /UnOCR2040/PBA/WDTray1
                XmlElement wdtrayXmlElem1 = pbaXmlConfigDoc.CreateElement("WDTray1");
                pbaXmlElem.AppendChild(wdtrayXmlElem1);              ///UnOCR2040/PBA/WDTray1

                // /UnOCR2040/PBA/WDTray1/pos1
                XmlElement wdtrayPos1XmlElem = pbaXmlConfigDoc.CreateElement("pos1");
                wdtrayXmlElem1.AppendChild(wdtrayPos1XmlElem);       ///UnOCR2040/PBA/WDTray1/pos1
                wdtrayPos1XmlElem.SetAttribute("x", _wdtraypos1[0, 0].X.ToString());
                wdtrayPos1XmlElem.SetAttribute("y", _wdtraypos1[0, 0].Y.ToString());
                
                // /UnOCR2040/PBA/WDTray1/pos2
                XmlElement wdtrayPos2XmlElem = pbaXmlConfigDoc.CreateElement("pos2");
                wdtrayXmlElem1.AppendChild(wdtrayPos2XmlElem);       ///UnOCR2040/PBA/WDTray1/pos2
                wdtrayPos2XmlElem.SetAttribute("x", _wdtraypos1[0, 1].X.ToString());
                wdtrayPos2XmlElem.SetAttribute("y", _wdtraypos1[0, 1].Y.ToString());
                
                // /UnOCR2040/PBA/WDTray1/pos3
                XmlElement wdtrayPos3XmlElem = pbaXmlConfigDoc.CreateElement("pos3");
                wdtrayXmlElem1.AppendChild(wdtrayPos3XmlElem);       ///UnOCR2040/PBA/WDTray1/pos3
                wdtrayPos3XmlElem.SetAttribute("x", _wdtraypos1[1, 0].X.ToString());
                wdtrayPos3XmlElem.SetAttribute("y", _wdtraypos1[1, 0].Y.ToString());
                
                // /UnOCR2040/PBA/WDTray1/pos4
                XmlElement wdtrayPos4XmlElem = pbaXmlConfigDoc.CreateElement("pos4");
                wdtrayXmlElem1.AppendChild(wdtrayPos4XmlElem);       ///UnOCR2040/PBA/WDTray1/pos4
                wdtrayPos4XmlElem.SetAttribute("x", _wdtraypos1[1, 1].X.ToString());
                wdtrayPos4XmlElem.SetAttribute("y", _wdtraypos1[1, 1].Y.ToString());




                // /UnOCR2040/PBA/WDTray2
                XmlElement wdtrayXmlElem2 = pbaXmlConfigDoc.CreateElement("WDTray2");
                pbaXmlElem.AppendChild(wdtrayXmlElem2);              ///UnOCR2040/PBA/WDTray2

                // /UnOCR2040/PBA/WDTray2/pos1
                XmlElement wdtrayPos1XmlElem2 = pbaXmlConfigDoc.CreateElement("pos1");
                wdtrayXmlElem2.AppendChild(wdtrayPos1XmlElem2);       ///UnOCR2040/PBA/WDTray2/pos1
                wdtrayPos1XmlElem2.SetAttribute("x", _wdtraypos2[0, 0].X.ToString());
                wdtrayPos1XmlElem2.SetAttribute("y", _wdtraypos2[0, 0].Y.ToString());

                // /UnOCR2040/PBA/WDTray2/pos2
                XmlElement wdtrayPos2XmlElem2 = pbaXmlConfigDoc.CreateElement("pos2");
                wdtrayXmlElem2.AppendChild(wdtrayPos2XmlElem2);       ///UnOCR2040/PBA/WDTray2/pos2
                wdtrayPos2XmlElem2.SetAttribute("x", _wdtraypos2[0, 1].X.ToString());
                wdtrayPos2XmlElem2.SetAttribute("y", _wdtraypos2[0, 1].Y.ToString());

                // /UnOCR2040/PBA/WDTray2/pos3
                XmlElement wdtrayPos3XmlElem2 = pbaXmlConfigDoc.CreateElement("pos3");
                wdtrayXmlElem2.AppendChild(wdtrayPos3XmlElem2);       ///UnOCR2040/PBA/WDTray2/pos3
                wdtrayPos3XmlElem2.SetAttribute("x", _wdtraypos2[1, 0].X.ToString());
                wdtrayPos3XmlElem2.SetAttribute("y", _wdtraypos2[1, 0].Y.ToString());

                // /UnOCR2040/PBA/WDTray2/pos4
                XmlElement wdtrayPos4XmlElem2 = pbaXmlConfigDoc.CreateElement("pos4");
                wdtrayXmlElem2.AppendChild(wdtrayPos4XmlElem2);       ///UnOCR2040/PBA/WDTray2/pos4
                wdtrayPos4XmlElem2.SetAttribute("x", _wdtraypos2[1, 1].X.ToString());
                wdtrayPos4XmlElem2.SetAttribute("y", _wdtraypos2[1, 1].Y.ToString());


                // /UnOCR2040/PBA/HGSTTray
                XmlElement hgsttrayXmlElem = pbaXmlConfigDoc.CreateElement("HGSTTray");
                pbaXmlElem.AppendChild(hgsttrayXmlElem);            ///UnOCR2040/PBA/HGSTTray

                // /UnOCR2040/PBA/HGSTTray/pos1
                XmlElement hgsttrayPos1XmlElem = pbaXmlConfigDoc.CreateElement("pos1");
                hgsttrayXmlElem.AppendChild(hgsttrayPos1XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos1
                hgsttrayPos1XmlElem.SetAttribute("x", _hgsttraypos[0, 0].X.ToString());
                hgsttrayPos1XmlElem.SetAttribute("y", _hgsttraypos[0, 0].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos2
                XmlElement hgsttrayPos2XmlElem = pbaXmlConfigDoc.CreateElement("pos2");
                hgsttrayXmlElem.AppendChild(hgsttrayPos2XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos2
                hgsttrayPos2XmlElem.SetAttribute("x", _hgsttraypos[0, 1].X.ToString());
                hgsttrayPos2XmlElem.SetAttribute("y", _hgsttraypos[0, 1].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos3
                XmlElement hgsttrayPos3XmlElem = pbaXmlConfigDoc.CreateElement("pos3");
                hgsttrayXmlElem.AppendChild(hgsttrayPos3XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos3
                hgsttrayPos3XmlElem.SetAttribute("x", _hgsttraypos[0, 2].X.ToString());
                hgsttrayPos3XmlElem.SetAttribute("y", _hgsttraypos[0, 2].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos4
                XmlElement hgsttrayPos4XmlElem = pbaXmlConfigDoc.CreateElement("pos4");
                hgsttrayXmlElem.AppendChild(hgsttrayPos4XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos4
                hgsttrayPos4XmlElem.SetAttribute("x", _hgsttraypos[0, 3].X.ToString());
                hgsttrayPos4XmlElem.SetAttribute("y", _hgsttraypos[0, 3].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos5
                XmlElement hgsttrayPos5XmlElem = pbaXmlConfigDoc.CreateElement("pos5");
                hgsttrayXmlElem.AppendChild(hgsttrayPos5XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos5
                hgsttrayPos5XmlElem.SetAttribute("x", _hgsttraypos[1, 0].X.ToString());
                hgsttrayPos5XmlElem.SetAttribute("y", _hgsttraypos[1, 0].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos6
                XmlElement hgsttrayPos6XmlElem = pbaXmlConfigDoc.CreateElement("pos6");
                hgsttrayXmlElem.AppendChild(hgsttrayPos6XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos6
                hgsttrayPos6XmlElem.SetAttribute("x", _hgsttraypos[1, 1].X.ToString());
                hgsttrayPos6XmlElem.SetAttribute("y", _hgsttraypos[1, 1].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos7
                XmlElement hgsttrayPos7XmlElem = pbaXmlConfigDoc.CreateElement("pos7");
                hgsttrayXmlElem.AppendChild(hgsttrayPos7XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos7
                hgsttrayPos7XmlElem.SetAttribute("x", _hgsttraypos[1, 2].X.ToString());
                hgsttrayPos7XmlElem.SetAttribute("y", _hgsttraypos[1, 2].Y.ToString());

                // /UnOCR2040/PBA/HGSTTray/pos8
                XmlElement hgsttrayPos8XmlElem = pbaXmlConfigDoc.CreateElement("pos8");
                hgsttrayXmlElem.AppendChild(hgsttrayPos8XmlElem);   ///UnOCR2040/PBA/HGSTTray/pos8
                hgsttrayPos8XmlElem.SetAttribute("x", _hgsttraypos[1, 3].X.ToString());
                hgsttrayPos8XmlElem.SetAttribute("y", _hgsttraypos[1, 3].Y.ToString());


                // /UnOCR2040/PBA/Speed
                XmlElement speedXmlElem = pbaXmlConfigDoc.CreateElement("Speed");
                pbaXmlElem.AppendChild(speedXmlElem);               ///UnOCR2040/PBA/Speed
                speedXmlElem.InnerText = _dblVel.ToString();

                // /UnOCR2040/PBA/Acceleration
                XmlElement accelXmlElem = pbaXmlConfigDoc.CreateElement("Acceleration");
                pbaXmlElem.AppendChild(accelXmlElem);               ///UnOCR2040/PBA/Acceleration
                accelXmlElem.InnerText = _dblAcc.ToString();

                // /UnOCR2040/PBA/Deceleration
                XmlElement decelXmlElem = pbaXmlConfigDoc.CreateElement("Deceleration");
                pbaXmlElem.AppendChild(decelXmlElem);               ///UnOCR2040/PBA/Deceleration
                decelXmlElem.InnerText = _dblDecl.ToString();

                // /UnOCR2040/PBA/Step
                XmlElement stepXmlElem = pbaXmlConfigDoc.CreateElement("Step");
                pbaXmlElem.AppendChild(stepXmlElem);               ///UnOCR2040/PBA/Step
                stepXmlElem.InnerText = _dblStep.ToString();


                pbaXmlConfigDoc.Save(savedFilename);
            }
            catch (Exception ex)
            {
                SetupLogger.Info("_savePBAConfig: exception " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            _bEdit = !_bEdit;

            for (int i = 1; i <= 4; i++)
            {
                //btnWDTrayTeach
                Control[] btnTeach = this.Controls.Find("btnWDTrayTeach" + i.ToString("0"), true);
                btnTeach[0].Enabled = _bEdit;
            }

            for (int i = 1; i <= 8; i++)
            {
                //btnHGSTTrayTeach
                Control[] btnTeach = this.Controls.Find("btnHGSTTrayTeach" + i.ToString("0"), true);
                btnTeach[0].Enabled = _bEdit;
            }

            btnSave.Enabled = _bEdit;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to reload teachning points from XML?", "Confirm yes or no", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                FormUnOCR2040.Instance.LoadPBAConfig(FormUnOCR2040.Instance._strPBAConfigFilename);
            }
        }

        private void FormLinearSetup_LocationChanged(object sender, EventArgs e)
        {
            this.Location = _formlocation;
        }

        private void btnTurnTable_Click(object sender, EventArgs e)
        {
            if (IOs.Instance.IsTray2In())
            {
                IOs.Instance.Tray1In();
            }
            else if (IOs.Instance.IsTray1In())
            {
                IOs.Instance.Tray2In();
            }
        }

        private void FormLinearSetup_Load(object sender, EventArgs e)
        {
            ThreadStart start = delegate
            {
                CheckPosition();
            };


            if (motorpositionThread == null)
            {
                motorpositionThread = new Thread(start);
                motorpositionThread.Name = "motorpositionThread";
                motorpositionThread.Priority = ThreadPriority.Normal;
                motorpositionThread.Start();
            }
            else
            {
                if (!motorpositionThread.IsAlive)
                {
                    motorpositionThread = new Thread(start);
                    motorpositionThread.Name = "motorpositionThread";
                    motorpositionThread.Priority = ThreadPriority.Normal;
                    motorpositionThread.Start();
                }
            }


            ThreadStart startUpdateGUI = delegate
            {
                UpdateGUI();
            };

            if (updateGUIThread == null)
            {
                updateGUIThread = new Thread(startUpdateGUI);
                updateGUIThread.Name = "updateGUIThread";
                updateGUIThread.Priority = ThreadPriority.Normal;
                updateGUIThread.Start();
            }
            else
            {
                if (!updateGUIThread.IsAlive)
                {
                    updateGUIThread = new Thread(startUpdateGUI);
                    updateGUIThread.Name = "updateGUIThread";
                    updateGUIThread.Priority = ThreadPriority.Normal;
                    updateGUIThread.Start();
                }
            }

        }


 
    }
}
