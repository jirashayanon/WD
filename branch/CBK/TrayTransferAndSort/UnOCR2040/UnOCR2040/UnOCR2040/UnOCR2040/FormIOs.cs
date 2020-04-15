using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;

namespace UnOCR2040
{
    public partial class FormIOs : Form
    {
        private static FormIOs _instance;
        public static FormIOs Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormIOs();
                }

                try
                {
                    if (_instance.updateIOsThread == null)
                    {
                        ThreadStart start = delegate
                        {
                            UpdateIOs();
                        };

                        _instance.updateIOsThread = new Thread(start);
                        _instance.updateIOsThread.Priority = ThreadPriority.Normal;
                        _instance.updateIOsThread.Start();
                    }
                    else
                    {
                        if (!_instance.updateIOsThread.IsAlive)
                        {
                            ThreadStart start = delegate
                            {
                                UpdateIOs();
                            };

                            _instance.updateIOsThread = new Thread(start);
                            _instance.updateIOsThread.Priority = ThreadPriority.Normal;
                            _instance.updateIOsThread.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                return _instance;
            }
        }


        private Thread updateIOsThread = null;
        private Point _formlocation = new Point();
        private FormIOs()
        {
            InitializeComponent();


            ThreadStart start = delegate
            {
                UpdateIOs();
            };

            updateIOsThread = new Thread(start);
            updateIOsThread.Name = "updateIOsThread";
            updateIOsThread.Priority = ThreadPriority.Normal;
            updateIOsThread.Start();

            _formlocation = this.Location;
        }

        public static void UpdateIOs()
        {
            while (true)
            {
                FormIOs.Instance.DelegateUpdateTrayImage();
                FormIOs.Instance.DelegateUpdateNozzleUpDown();
                FormIOs.Instance.DelegateUpdatePBAPosition();

                Application.DoEvents();
                Thread.Sleep(200);
            }
        }

        #region delegate updateTrayImage
        private delegate void updateTrayImage();
        int i = 0;
        private void UpdateTrayImage()
        {
            if (IOs.Instance.IsTray1In())
            {
                imgWDTray1InGreen.Visible = true;
                imgWDTray1InRed.Visible = false;
            }
            else
            {
                imgWDTray1InGreen.Visible = false;
                imgWDTray1InRed.Visible = true;
            }

            if (IOs.Instance.IsTray2In())
            {
                imgWDTray2InGreen.Visible = true;
                imgWDTray2InRed.Visible = false;
            }
            else
            {
                imgWDTray2InGreen.Visible = false;
                imgWDTray2InRed.Visible = true;
            }

            if (IOs.Instance.IsPush1Activated())
            {
                imgPush1Green.Visible = true;
                imgPush1Red.Visible = false;

                IOs.Instance.Push1SWLightOn();
            }
            else
            {
                imgPush1Green.Visible = false;
                imgPush1Red.Visible = true;

                IOs.Instance.Push1SWLightOff();
            }

            if (IOs.Instance.IsPush2Activated())
            {
                imgPush2Green.Visible = true;
                imgPush2Red.Visible = false;

                IOs.Instance.Push2SWLightOn();
            }
            else
            {
                imgPush2Green.Visible = false;
                imgPush2Red.Visible = true;

                IOs.Instance.Push2SWLightOff();
            }


            if (IOs.Instance.IsAreaSensorOn())
            {
                imgAreaSensorL1Green.Visible = true;
                imgAreaSensorL2Green.Visible = true;
                imgAreaSensorL3Green.Visible = true;
                imgAreaSensorL4Green.Visible = true;
                imgAreaSensorL5Green.Visible = true;

                imgAreaSensorR1Green.Visible = true;
                imgAreaSensorR2Green.Visible = true;
                imgAreaSensorR3Green.Visible = true;
                imgAreaSensorR4Green.Visible = true;
                imgAreaSensorR5Green.Visible = true;


                imgAreaSensorL1Red.Visible = false;
                imgAreaSensorL2Red.Visible = false;
                imgAreaSensorL3Red.Visible = false;
                imgAreaSensorL4Red.Visible = false;
                imgAreaSensorL5Red.Visible = false;

                imgAreaSensorR1Red.Visible = false;
                imgAreaSensorR2Red.Visible = false;
                imgAreaSensorR3Red.Visible = false;
                imgAreaSensorR4Red.Visible = false;
                imgAreaSensorR5Red.Visible = false;
            }
            else
            {
                imgAreaSensorL1Green.Visible = false;
                imgAreaSensorL2Green.Visible = false;
                imgAreaSensorL3Green.Visible = false;
                imgAreaSensorL4Green.Visible = false;
                imgAreaSensorL5Green.Visible = false;

                imgAreaSensorR1Green.Visible = false;
                imgAreaSensorR2Green.Visible = false;
                imgAreaSensorR3Green.Visible = false;
                imgAreaSensorR4Green.Visible = false;
                imgAreaSensorR5Green.Visible = false;


                imgAreaSensorL1Red.Visible = true;
                imgAreaSensorL2Red.Visible = true;
                imgAreaSensorL3Red.Visible = true;
                imgAreaSensorL4Red.Visible = true;
                imgAreaSensorL5Red.Visible = true;

                imgAreaSensorR1Red.Visible = true;
                imgAreaSensorR2Red.Visible = true;
                imgAreaSensorR3Red.Visible = true;
                imgAreaSensorR4Red.Visible = true;
                imgAreaSensorR5Red.Visible = true;
            }



            if (IOs.Instance.IsTray1In() && !IOs.Instance.IsTray2In())
            {
                i = 0;
            }
            if (!IOs.Instance.IsTray1In() && IOs.Instance.IsTray2In())
            {
                i = 2;
            }
            if (!IOs.Instance.IsTray1In() && !IOs.Instance.IsTray2In())
            {
                if (i == 0)
                {
                    i = 1;
                }
                else if (i == 2)
                {
                    i = 3;
                }
            }

            switch (this.i)
            {
                case 0:
                    imgWDTW1.Visible = true;
                    imgWDTW2.Visible = false;
                    imgWDTW3.Visible = false;

                    imgWDTD3.Visible = true;
                    imgWDTD4.Visible = false;
                    imgWDTD1.Visible = false;

                    break;

                case 1:
                    imgWDTW1.Visible = false;
                    imgWDTW2.Visible = true;
                    imgWDTW3.Visible = false;

                    imgWDTD3.Visible = false;
                    imgWDTD4.Visible = true;
                    imgWDTD1.Visible = false;

                    break;

                case 2:
                    imgWDTW1.Visible = false;
                    imgWDTW2.Visible = false;
                    imgWDTW3.Visible = true;

                    imgWDTD3.Visible = false;
                    imgWDTD4.Visible = false;
                    imgWDTD1.Visible = true;

                    break;

                case 3:
                    imgWDTW1.Visible = false;
                    imgWDTW2.Visible = true;
                    imgWDTW3.Visible = false;

                    imgWDTD3.Visible = false;
                    imgWDTD4.Visible = true;
                    imgWDTD1.Visible = false;

                    break;

                default:
                    break;
            }
        }
        public void DelegateUpdateTrayImage()
        {
            Invoke(new updateTrayImage(UpdateTrayImage));
        }
        #endregion

        #region delegate updateNozzleUpDown
        private delegate void updateNozzleUpDown();
        private void UpdateNozzleUpDown()
        {
            if (IOs.Instance.IsNozzleUp())
            {
                imgNozzleUpGreen.Visible = true;
                imgNozzleUpRed.Visible = false;

                imgNozzleDownGreen.Visible = false;
                imgNozzleDownRed.Visible = true;

                if (IOs.Instance.IsNozzleExtend())
                {
                    imgNozzleExtUp.Visible = true;
                    imgNozzleRetrctUp.Visible = false;

                    imgNozzleExtGreen.Visible = true;
                    imgNozzleExtRed.Visible = false;
                }
                else
                {
                    imgNozzleExtUp.Visible = false;
                    imgNozzleRetrctUp.Visible = true;

                    imgNozzleExtGreen.Visible = false;
                    imgNozzleExtRed.Visible = true;
                }
             
                imgNozzleExtDown.Visible = false;
                imgNozzleRetrctDown.Visible = false;
            }

            if (IOs.Instance.IsNozzleDown())
            {
                imgNozzleUpGreen.Visible = false;
                imgNozzleUpRed.Visible = true;

                imgNozzleDownGreen.Visible = true;
                imgNozzleDownRed.Visible = false;


                imgNozzleExtUp.Visible = false;
                imgNozzleRetrctUp.Visible = false;

                if (IOs.Instance.IsNozzleExtend())
                {
                    imgNozzleExtDown.Visible = true;
                    imgNozzleRetrctDown.Visible = false;

                    imgNozzleExtGreen.Visible = true;
                    imgNozzleExtRed.Visible = false;
                }
                else
                {
                    imgNozzleExtDown.Visible = false;
                    imgNozzleRetrctDown.Visible = true;

                    imgNozzleExtGreen.Visible = false;
                    imgNozzleExtRed.Visible = true;
                }
            }

            if (IOs.Instance.IsTurnTraySWOn())
            {
                IOs.Instance.TurnTraySWLightOn();

                imgTurnTraySWGreen.Visible = true;
                imgTurnTraySWRed.Visible = false;
            }
            else
            {
                IOs.Instance.TurnTraySWLightOff();

                imgTurnTraySWGreen.Visible = false;
                imgTurnTraySWRed.Visible = true;
            }


        }
        public void DelegateUpdateNozzleUpDown()
        {
            Invoke(new updateNozzleUpDown(UpdateNozzleUpDown));
        }
        #endregion


        #region delegate updatePBAPosition
        private delegate void updatePBAPosition();
        private void UpdatePBAPosition()
        {
            PBACoordinate currPos = FormUnOCR2040.Instance.PBAGetPosition();
            //x@900px == 0, x@300px == 500
            //x axis length = 600px
            //y@117px == 60, y@197px == 84
            //y axis length = 120px
            Point newPoint = new Point(900-(int)(currPos.X*600/500), 20+(int)currPos.Y*130/80);
            this.imgPnPhgst1.Location = newPoint;
        }
        public void DelegateUpdatePBAPosition()
        {
            Invoke(new updatePBAPosition(UpdatePBAPosition));
        }
        #endregion


        private void btnTurnTray_Click(object sender, EventArgs e)
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

        private void FormIOs_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updateIOsThread != null)
            {
                updateIOsThread.Abort();
            }

        }

        private void btnNozzleUp_Click(object sender, EventArgs e)
        {
            if (IOs.Instance.IsNozzleDown())
            {
                IOs.Instance.NozzleUp();
            }
        }

        private void btnNozzleDown_Click(object sender, EventArgs e)
        {
            if (IOs.Instance.IsNozzleUp())
            {
                IOs.Instance.NozzleDown();
            }
        }

        private void btnNozzleExtend_Click(object sender, EventArgs e)
        {
            if (!IOs.Instance.IsNozzleExtend())
            {
                IOs.Instance.NozzleExtend();
            }
        }

        private void btnNozzleRetract_Click(object sender, EventArgs e)
        {
            if (IOs.Instance.NozzleExtend())
            {
                IOs.Instance.NozzleRetract();
            }
        }

        private void imgWDTD1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!FormUnOCR2040.Instance.HomeDone)
            {
                return;
            }

            int nZone = 0;
            if (MousePosition.X < 380)
            {
                if (MousePosition.Y < 325)
                {
                    nZone = 1;
                }
                else
                {
                    nZone = 3;
                }
            }
            else if (MousePosition.X > 381)
            {
                if (MousePosition.Y < 325)
                {
                    nZone = 2;
                }
                else
                {
                    nZone = 4;
                }
            }

            IOs.Instance.PalletClampsDown();
            IOs.Instance.NozzleUp();
            Thread.Sleep(200);

            PBACoordinate coord = new PBACoordinate();
            coord = FormUnOCR2040.Instance._wdtray1._unloadPos[nZone - 1];
            FormUnOCR2040.Instance.PBAMoveAbs(coord.X, coord.Y);
        }

        private void imgWDTW1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!FormUnOCR2040.Instance.HomeDone)
            {
                return;
            }

            int nZone = 0;
            if (MousePosition.X < 380)
            {
                if (MousePosition.Y < 325)
                {
                    nZone = 1;
                }
                else
                {
                    nZone = 3;
                }
            }
            else if (MousePosition.X > 381)
            {
                if (MousePosition.Y < 325)
                {
                    nZone = 2;
                }
                else
                {
                    nZone = 4;
                }
            }

            IOs.Instance.PalletClampsDown();
            IOs.Instance.NozzleUp();
            Thread.Sleep(200);

            PBACoordinate coord = new PBACoordinate();
            coord = FormUnOCR2040.Instance._wdtray1._unloadPos[nZone - 1];
            FormUnOCR2040.Instance.PBAMoveAbs(coord.X, coord.Y);
        }

        private void FormIOs_LocationChanged(object sender, EventArgs e)
        {
            this.Location = _formlocation;
        }

        private void btnNozzleVacOn_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacAllOn();
        }

        private void btnNozzleVacOff_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacAllOff();
        }

        private void btnNozzleVacOnOff1_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacuumOnOff_1(!IOs._oBits[9]);
            //IOs.Instance.NozzleVacuumOn(0);
        }

        private void btnNozzleVacOnOff2_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacuumOnOff_2(!IOs._oBits[8]);
            //IOs.Instance.NozzleVacuumOn(1);
        }

        private void btnNozzleVacOnOff3_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacuumOnOff_3(!IOs._oBits[7]);
            //IOs.Instance.NozzleVacuumOn(2);
        }

        private void btnNozzleVacOnOff4_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacuumOnOff_4(!IOs._oBits[6]);
            //IOs.Instance.NozzleVacuumOn(3);
        }

        private void btnNozzleVacOnOff5_Click(object sender, EventArgs e)
        {
            IOs.Instance.NozzleVacuumOnOff_5(!IOs._oBits[5]);
            //IOs.Instance.NozzleVacuumOn(4);
        }

        private void btnTestScanner_Click(object sender, EventArgs e)
        {
            SICKScannerObj.Instance.COMPort = "COM4";
            txtboxScanner.Text = SICKScannerObj.Instance.ReadBarcode();
        }
 
     }
}
