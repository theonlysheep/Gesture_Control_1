using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intel.RealSense;
using Intel.RealSense.HandCursor;

namespace streams.cs
{
    class HandsRecognition
    {
        // Ereignisdaklaration
        public event EventHandler<UpdateStatusEventArgs> UpdateStatus = null;


        ImageInfo _info;
        private readonly MainForm _form;
        private bool _disconnected = false;
        //Queue containing depth image - for synchronization purposes
        private readonly Queue<Image> _mImages;

        private readonly Queue<Point3DF32>[] _mCursorPoints;
        private readonly Queue<Point3DF32>[] _mAdaptivePoints;
        private readonly int[] _mCursorClick;
        private readonly BodySideType[] _mCursorHandSide;

        private const int CURSOR_FACTOR_X = 60;
        private const int CURSOR_FACTOR_Y_UP = 120;
        private const int CURSOR_FACTOR_Y_DOWN = 40;

        public CursorData cursorData = null;

        public HandsRecognition(MainForm form)
        {
            _mImages = new Queue<Image>();
            _mCursorPoints = new Queue<Point3DF32>[2];
            _mCursorPoints[0] = new Queue<Point3DF32>();
            _mCursorPoints[1] = new Queue<Point3DF32>();

            _mAdaptivePoints = new Queue<Point3DF32>[2];
            _mAdaptivePoints[0] = new Queue<Point3DF32>();
            _mAdaptivePoints[1] = new Queue<Point3DF32>();
            _mCursorHandSide = new BodySideType[2];
            _mCursorClick = new int[2];

            this._form = form;
        }



        #region Events 
        private void SetStatus(String text)
        {
            EventHandler<UpdateStatusEventArgs> handler = UpdateStatus;
            if (handler != null)
            {
                handler(this, new UpdateStatusEventArgs(text));
            }
        }

        public void OnFiredAlert(Object sender, CursorConfiguration.AlertEventArgs args)
        {
            AlertData data = args.data;
            string sAlert = "Alert: ";
            _form.UpdateInfo("Frame " + data.frameNumber + ") " + sAlert + data.label.ToString() + "\n", System.Drawing.Color.RoyalBlue);
        }

        public void OnFiredGesture(Object sender, CursorConfiguration.GestureEventArgs args)
        {
            Intel.RealSense.HandCursor.GestureData data = args.data;
            string gestureStatusLeft = string.Empty;
            string gestureStatusRight = string.Empty;
            
            ICursor cursor;
            if (cursorData.QueryCursorDataById(data.handId, out cursor) != Status.STATUS_NO_ERROR)
                return;
            BodySideType bodySideType = cursor.BodySide;

            if (bodySideType == BodySideType.BODY_SIDE_LEFT)
            {
                gestureStatusLeft += "Left Hand Gesture: " + data.label.ToString();
            }
            else if (bodySideType == BodySideType.BODY_SIDE_RIGHT)
            {
                gestureStatusRight += "Right Hand Gesture: " + data.label.ToString();
            }

            if (gestureStatusLeft == String.Empty)
                _form.UpdateInfo("Frame " + data.frameNumber + ") " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
            else
                _form.UpdateInfo("Frame " + data.frameNumber + ") " + gestureStatusLeft + ", " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
        }

        public static Status OnNewFrame(Int32 mid, Base module, Sample sample)
        {
            return Status.STATUS_NO_ERROR;
        }

        #endregion 

        /* Displaying Depth/Mask Images - for depth image only we use a delay of NumberOfFramesToDelay to sync image with tracking */
        private void DisplayCursorPicture(Image depth)
        {
            if (depth == null)
                return;

            Image image = depth;
            _info = image.Info;
            var depthBitmap = new System.Drawing.Bitmap(image.Info.width, image.Info.height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            _form.DisplayBitmap(depthBitmap);
            depthBitmap.Dispose();
        }

        /* Displaying current frames hand joints */
        private void DisplayCursorJoints(long timeStamp = 0)
        {
            _mCursorClick[0] = Math.Max(0, _mCursorClick[0] - 1);
            _mCursorClick[1] = Math.Max(0, _mCursorClick[1] - 1);

            int numOfHands = cursorData.QueryNumberOfCursors();
            if (numOfHands == 1)
            {
                _mCursorPoints[1].Clear();
                _mAdaptivePoints[1].Clear();
            }

            for (int i = 0; i < numOfHands; i++)
            {
                //Get hand by time of appearance
                ICursor cursor;
                if (cursorData.QueryCursorData(AccessOrderType.ACCESS_ORDER_BY_TIME, i, out cursor) == Status.STATUS_NO_ERROR)
                {
                    if (cursor != null)
                    {
                        // collect cursor points

                        Point3DF32 imagePoint = cursor.CursorPointImage;

                        _mCursorPoints[i].Enqueue(imagePoint);
                        if (_mCursorPoints[i].Count > 50)
                            _mCursorPoints[i].Dequeue();

                        _mCursorHandSide[i] = cursor.BodySide;
                        GestureData gestureData;
                        if (cursorData.IsGestureFiredByHand(GestureType.CURSOR_CLICK, cursor.UniqueId, out gestureData))
                        {
                            _mCursorClick[i] = 7;
                        }
                    }
                }
            } // end iterating over hands

            if (numOfHands > 0)
            {
                _form.DisplayCursor(numOfHands, _mCursorPoints, _mAdaptivePoints, _mCursorClick, _mCursorHandSide);
            }
            else
            {
                _mCursorPoints[0].Clear();
                _mCursorPoints[1].Clear();
                _mAdaptivePoints[0].Clear();
                _mAdaptivePoints[1].Clear();
                _mCursorHandSide[0] = BodySideType.BODY_SIDE_UNKNOWN;
                _mCursorHandSide[1] = BodySideType.BODY_SIDE_UNKNOWN;
            }
        }

        /* Using SenseManager to handle data */
        public void SimplePipeline(Manager mngr)
        {
            Manager manager = mngr;

            _form.UpdateInfo(String.Empty, System.Drawing.Color.Black);

            //??????????
            bool flag = true;

            SenseManager senseManager = null;
            _disconnected = false;
            senseManager = manager.SenseManager;

            CaptureManager captureManager = senseManager.CaptureManager;
           
            if (captureManager != null)
            {
                if (_form == null || _form.devices.Count == 0)
                {
                    _form.UpdateStatus("No device were found");
                    return;
                }
                
                if (manager.DeviceInfo == null)
                {
                    _form.UpdateStatus("Device Failure");
                    return;
                }
            }

            if (manager.DeviceInfo == null)
            {
                _form.UpdateStatus("Device Failure");
                return;
            }

            if (manager.DeviceInfo.model != DeviceModel.DEVICE_MODEL_SR300)
            {
                _form.UpdateStatus("Cursor mode is unsupported for chosen device");
                return;
            }

            /* Set Module */
            HandCursorModule handCursorModule;
            handCursorModule = HandCursorModule.Activate(senseManager);
            if (handCursorModule == null)
            {
                _form.UpdateStatus("Failed Loading Module");
                return;
            }

            CursorConfiguration cursorConfiguration = null;
            cursorConfiguration = handCursorModule.CreateActiveConfiguration();
            if (cursorConfiguration == null)
            {
                _form.UpdateStatus("Failed Create Configuration");
                senseManager.Close();
                senseManager.Dispose();
                return;
            }

            cursorData = handCursorModule.CreateOutput();
            if (cursorData == null)
            {
                _form.UpdateStatus("Failed Create Output");
                senseManager.Close();
                senseManager.Dispose();
                return;
            }

            
            _form.UpdateStatus("Init Started");
            if (senseManager.Init() == Status.STATUS_NO_ERROR)
            {

                DeviceInfo dinfo;
                DeviceModel dModel = DeviceModel.DEVICE_MODEL_F200;
                Device device = senseManager.CaptureManager.Device;
                if (device != null)
                {
                    device.QueryDeviceInfo(out dinfo);
                    dModel = dinfo.model;
                }

                if (cursorConfiguration != null)
                {
                    cursorConfiguration.AlertFired += OnFiredAlert;
                    cursorConfiguration.GestureFired += OnFiredGesture;
                    cursorConfiguration.EnableAllAlerts();
                    cursorConfiguration.ApplyChanges();
                }

                

                _form.UpdateStatus("Streaming");         
                int frameNumber = 0;

                while (!manager.Stop)
                {
                    if (cursorConfiguration != null)
                    {
                        string gestureName = _form.GetGestureName();
                        if (string.IsNullOrEmpty(gestureName) == false)
                        {
                            cursorConfiguration.DisableAllGestures();
                            switch (gestureName)
                            {
                                case "cursor_click":
                                    if (cursorConfiguration.IsGestureEnabled(GestureType.CURSOR_CLICK) == false)
                                    {
                                        cursorConfiguration.EnableGesture(GestureType.CURSOR_CLICK);
                                    }
                                    break;
                                case "cursor_clockwise_circle":
                                    if (cursorConfiguration.IsGestureEnabled(GestureType.CURSOR_CLOCKWISE_CIRCLE) == false)
                                    {
                                        cursorConfiguration.EnableGesture(GestureType.CURSOR_CLOCKWISE_CIRCLE);
                                    }
                                    break;
                                case "cursor_counterclockwise_circle":
                                    if (cursorConfiguration.IsGestureEnabled(GestureType.CURSOR_COUNTER_CLOCKWISE_CIRCLE) == false)
                                    {
                                        cursorConfiguration.EnableGesture(GestureType.CURSOR_COUNTER_CLOCKWISE_CIRCLE);
                                    }
                                    break;
                                case "cursor_hand_opening":
                                    if (cursorConfiguration.IsGestureEnabled(GestureType.CURSOR_HAND_OPENING) == false)
                                    {
                                        cursorConfiguration.EnableGesture(GestureType.CURSOR_HAND_OPENING);
                                    }
                                    break;
                                case "cursor_hand_closing":
                                    if (cursorConfiguration.IsGestureEnabled(GestureType.CURSOR_HAND_CLOSING) == false)
                                    {
                                        cursorConfiguration.EnableGesture(GestureType.CURSOR_HAND_CLOSING);
                                    }
                                    break;

                            }
                            cursorConfiguration.ApplyChanges();

                        }
                        else
                        {
                            cursorConfiguration.DisableAllGestures();
                            cursorConfiguration.ApplyChanges();
                        }
                    }


                    if (senseManager.AcquireFrame(true) < Status.STATUS_NO_ERROR)
                    {
                        break;
                    }

                    frameNumber++;

                    if (!DisplayDeviceConnection(!senseManager.IsConnected()))
                    {
                        Sample sample = null;
                        sample = senseManager.Sample;

                        if (sample != null && sample.Depth != null)
                        {
                            

                            if (cursorData != null)
                            {
                                cursorData.Update();
                                DisplayCursorPicture(sample.Depth);
                                DisplayCursorJoints();
                            }
                            _form.UpdatePanel();
                        }
                        timer.Tick();
                    }
                    senseManager.ReleaseFrame();
                }
            }
            else
            {
                _form.UpdateStatus("Init Failed");
                flag = false;
            }
            foreach (Image Image in _mImages)
            {
                Image.Dispose();
            }

            // Clean Up
            if (cursorData != null) cursorData.Dispose();
            if (cursorConfiguration != null) cursorConfiguration.Dispose();
            
            if (flag)
            {
                UpdateStatusHandler("Stopped");
            }
        }
    }
}


