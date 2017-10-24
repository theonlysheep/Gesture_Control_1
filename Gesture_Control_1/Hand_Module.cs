using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intel.RealSense;
using Intel.RealSense.Hand;

namespace streams.cs
{
    class HandsRecognition
    {
        private HandModule handModule = null;
        public HandConfiguration HandConfiguration { get; set; } = null;
        private HandData handData = null;

        private readonly Queue<Image> _mImages;
        private const int NumberOfFramesToDelay = 3;

        private readonly Queue<Point3DF32>[] mCursorPoints;
        private readonly int[] mCursorClick;
        private readonly BodySideType[] mCursorHandSide;

        public List<string> ActivatedGestures { get; set; }

        private Manager manager;

        private MainForm form = null;


        public HandsRecognition(Manager mngr, MainForm frm)
        {
            _mImages = new Queue<Image>();
            mCursorPoints = new Queue<Point3DF32>[2];
            mCursorPoints[0] = new Queue<Point3DF32>();
            mCursorPoints[1] = new Queue<Point3DF32>();

            mCursorHandSide = new BodySideType[2];
            mCursorClick = new int[2];

            manager = mngr;
            form = frm;

        }

        #region Events 
        /*
        public void OnFiredAlert(Object sender, HandConfiguration.AlertEventArgs args)
        {
            AlertData data = args.data;
            string sAlert = "Alert: ";
            form.UpdateGestureInfo("Frame " + data.frameNumber + ") " + sAlert + data.label.ToString() + "\n", System.Drawing.Color.RoyalBlue);
        }

        public void OnFiredGesture(Object sender, HandConfiguration.AlertEventArgs args)
        {
            GestureData data = args.data;
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
                form.UpdateGestureInfo("Frame " + data.frameNumber + ") " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
            else
                form.UpdateGestureInfo("Frame " + data.frameNumber + ") " + gestureStatusLeft + ", " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
        }
        */

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

            var depthBitmap = new System.Drawing.Bitmap(depth.Info.width, depth.Info.height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            form.DisplayBitmap(depthBitmap);
            depthBitmap.Dispose();
        }

        /* Displaying current frames hand joints */
        /*
        private void DisplayCursorJoints(long timeStamp = 0)
        {
            mCursorClick[0] = Math.Max(0, mCursorClick[0] - 1);
            mCursorClick[1] = Math.Max(0, mCursorClick[1] - 1);

            int numOfHands = cursorData.QueryNumberOfCursors();
            if (numOfHands == 1)
            {
                mCursorPoints[1].Clear();
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

                        mCursorPoints[i].Enqueue(imagePoint);
                        if (mCursorPoints[i].Count > 50)
                            mCursorPoints[i].Dequeue();

                        mCursorHandSide[i] = cursor.BodySide;
                        GestureData gestureData;
                        if (cursorData.IsGestureFiredByHand(GestureType.CURSOR_CLICK, cursor.UniqueId, out gestureData))
                        {
                            mCursorClick[i] = 7;
                        }
                    }
                }
            } // end iterating over hands

            if (numOfHands > 0)
            {
                form.DisplayCursor(numOfHands, mCursorPoints, mCursorClick, mCursorHandSide);
            }
            else
            {
                mCursorPoints[0].Clear();
                mCursorPoints[1].Clear();

                mCursorHandSide[0] = BodySideType.BODY_SIDE_UNKNOWN;
                mCursorHandSide[1] = BodySideType.BODY_SIDE_UNKNOWN;
            }
        }
        */

        /* Using SenseManager to handle data */
        public void RecogniseHands(Sample sample)
        {
            if (sample != null && sample.Depth != null)
            {
                int frameNumber = manager.FrameNumber;

                if (handData != null)
                {
                    handData.Update();

                    DisplayPicture(sample.Depth, handData);
                    DisplayGesture(handData, frameNumber);
                    DisplayJoints(handData);
                    DisplayAlerts(handData, frameNumber);
                }
                form.UpdateResultImage();
            }
        }

        
        public void RegisterHandEvents()
        {
            if (HandConfiguration != null)
            {
                HandConfiguration.AlertFired += OnFiredAlert;
                HandConfiguration.GestureFired += OnFiredGesture;                
                HandConfiguration.ApplyChanges();
            }
        }
        

        public void EnableGesturesFromSelection()
        {
            ActivatedGestures = form.GetSelectedGestures();

            if (HandConfiguration != null)
            {
                if (ActivatedGestures.Count != 0)
                {
                    HandConfiguration.DisableAllGestures();
                    foreach (string gestureName in ActivatedGestures)
                    {
                        if (HandConfiguration.IsGestureEnabled(gestureName) == false)
                        {
                            HandConfiguration.EnableGesture(gestureName, true);
                            HandConfiguration.ApplyChanges(); // Ev. aus Schleife nehmen
                        }
                    }
                }

                // No gestures activated 
                else
                {
                    HandConfiguration.DisableAllGestures();
                    HandConfiguration.ApplyChanges();
                }
            }
        }

        public void SetUpHandModule()
        {
            /* Set Module */

            handModule = HandModule.Activate(manager.SenseManager);

            if (handModule == null)
            {
                manager.SetStatus("Failed Loading Module");
                manager.Stop = true;
                return;
            }

            HandConfiguration = handModule.CreateActiveConfiguration();
            if (HandConfiguration == null)
            {
                manager.SetStatus("Failed Create Configuration");
                manager.Stop = true;
                return;
            }

            handData = handModule.CreateOutput();
            if (handData == null)
            {
                manager.SetStatus("Failed Create Output");
                HandConfiguration.Dispose();
                manager.Stop = true;
                return;
            }

            HandConfiguration.TrackingMode = TrackingModeType.TRACKING_MODE_FULL_HAND;
            HandConfiguration.TrackedJointsEnabled = true;
            HandConfiguration.EnableJointSpeed(JointType.JOINT_INDEX_TIP, JointSpeedType.JOINT_SPEED_ABSOLUTE, 20);
            HandConfiguration.StabilizerEnabled = true;
            HandConfiguration.EnableAllAlerts();
            HandConfiguration.SegmentationImageEnabled = false;
            HandConfiguration.SmoothingValue = 1; // The value is from 0(not smoothed) to 1(smoothed motion).

            HandConfiguration.ApplyChanges();

        }

        public void CleanUpHands()
        {
            // Clean Up
            if (handData != null) handData.Dispose();
            if (HandConfiguration != null) HandConfiguration.Dispose();
            foreach (Image Image in _mImages)
            {
                Image.Dispose();
            }
        }

        /*_______________________________________________________________________________________________________________________________________________
        */

        /* Displaying current frame gestures */
        private void DisplayGesture(HandData handAnalysis, int frameNumber)
        {
            if (handAnalysis.FiredGestureData != null)
            {
                int firedGesturesNumber = handAnalysis.FiredGestureData.Length;
                string gestureStatusLeft = string.Empty;
                string gestureStatusRight = string.Empty;

                if (firedGesturesNumber == 0)
                {
                    return;
                }

                foreach (var gestureData in handAnalysis.FiredGestureData)
                {
                    IHand handData;
                    if (handAnalysis.QueryHandDataById(gestureData.handId, out handData) != Status.STATUS_NO_ERROR)
                        return;

                    BodySideType bodySideType = handData.BodySide;
                    if (bodySideType == BodySideType.BODY_SIDE_LEFT)
                    {
                        gestureStatusLeft += "Left Hand Gesture: " + gestureData.name;
                    }
                    else if (bodySideType == BodySideType.BODY_SIDE_RIGHT)
                    {
                        gestureStatusRight += "Right Hand Gesture: " + gestureData.name;
                    }

                }

                if (gestureStatusLeft == String.Empty)
                    form.UpdateGestureInfo("Frame " + frameNumber + ") " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
                else
                    form.UpdateGestureInfo("Frame " + frameNumber + ") " + gestureStatusLeft + ", " + gestureStatusRight + "\n", System.Drawing.Color.SeaGreen);
            }
        }


        /* Displaying Depth/Mask Images - for depth image only we use a delay of NumberOfFramesToDelay to sync image with tracking */
        private unsafe void DisplayPicture(Image depth, HandData handAnalysis)
        {
            if (depth == null)
                return;

            //Make Copy of Depth Image
            Image image = depth;

            //collecting 3 images inside a queue and displaying the oldest image
            ImageInfo info;
            Image image2;
            ImageData imageData = new ImageData();
            info = image.Info;
            image2 = Image.CreateInstance(manager.Session, info, imageData);
            if (image2 == null) { return; }
            image2.CopyImage(image);
            _mImages.Enqueue(image2);
            if (_mImages.Count == NumberOfFramesToDelay)
            {
                System.Drawing.Bitmap depthBitmap;
                try
                {
                    depthBitmap = new System.Drawing.Bitmap(image.Info.width, image.Info.height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                }
                catch (Exception)
                {
                    image.Dispose();
                    Image queImage = _mImages.Dequeue();
                    queImage.Dispose();
                    return;
                }

                ImageData data3;
                Image image3 = _mImages.Dequeue();
                if (image3.AcquireAccess(ImageAccess.ACCESS_READ, PixelFormat.PIXEL_FORMAT_DEPTH, out data3) >= Status.STATUS_NO_ERROR)
                {
                    float fMaxValue = manager.GetDeviceRange();
                    byte cVal;

                    var rect = new System.Drawing.Rectangle(0, 0, image.Info.width, image.Info.height);
                    var bitmapdata = depthBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, depthBitmap.PixelFormat);

                    byte* pDst = (byte*)bitmapdata.Scan0;
                    short* pSrc = (short*)data3.planes[0];
                    int size = image.Info.width * image.Info.height;

                    for (int i = 0; i < size; i++, pSrc++, pDst += 4)
                    {
                        cVal = (byte)((*pSrc) / fMaxValue * 255);
                        if (cVal != 0)
                            cVal = (byte)(255 - cVal);

                        pDst[0] = cVal;
                        pDst[1] = cVal;
                        pDst[2] = cVal;
                        pDst[3] = 255;
                    }
                    try
                    {
                        depthBitmap.UnlockBits(bitmapdata);
                    }
                    catch (Exception)
                    {
                        image3.ReleaseAccess(data3);
                        depthBitmap.Dispose();
                        image3.Dispose();
                        return;
                    }

                    form.DisplayBitmap(depthBitmap);
                    image3.ReleaseAccess(data3);
                }
                depthBitmap.Dispose();
                image3.Dispose();
            }


        }


        /* Displaying current frames hand joints */
        private void DisplayJoints(HandData handOutput, long timeStamp = 0)
        {
            //Iterate hands
            var nodes = new JointData[][] { new JointData[0x20], new JointData[0x20] };
            var extremityNodes = new ExtremityData[][] { new ExtremityData[0x6], new ExtremityData[0x6] };

            int numOfHands = handOutput.NumberOfHands;

            if (numOfHands == 1) mCursorPoints[1].Clear();

            for (int i = 0; i < numOfHands; i++)
            {
                //Get hand by time of appearance
                IHand handData;
                if (handOutput.QueryHandData(AccessOrderType.ACCESS_ORDER_BY_TIME, i, out handData) == Status.STATUS_NO_ERROR)
                {
                    if (handData != null)
                    {
                        //Iterate Joints
                        foreach (var tJ in handData.TrackedJoints)
                        {
                            nodes[i][(int)tJ.Key] = tJ.Value;
                        }   // end iterating over joints

                        for (int j = 0; j < HandData.NUMBER_OF_EXTREMITIES; j++)
                        {
                            foreach (var eP in handData.ExtremityPoints)
                            {
                                extremityNodes[i][(int)eP.Key] = eP.Value;
                            }
                        }


                    }
                }
            } // end iterating over hands


            form.DisplayJoints(nodes, numOfHands);
            if (numOfHands > 0)
            {
                form.DisplayExtremities(numOfHands, extremityNodes);
            }
        }

        private void DisplayAlerts(HandData handAnalysis, int frameNumber)
        {
            bool isChanged = false;
            string sAlert = "Alert: ";
            if (handAnalysis.FiredAlertData != null)
                for (int i = 0; i < handAnalysis.FiredAlertData.Length; i++)
                {

                    //See HandAnalysis.AlertData.AlertType for all available alerts
                    switch (handAnalysis.FiredAlertData[i].label)
                    {
                        case AlertType.ALERT_HAND_DETECTED:
                            {

                                sAlert += "Hand Detected, ";
                                isChanged = true;
                                break;
                            }
                        case AlertType.ALERT_HAND_NOT_DETECTED:
                            {

                                sAlert += "Hand Not Detected, ";
                                isChanged = true;
                                break;
                            }
                        case AlertType.ALERT_HAND_CALIBRATED:
                            {

                                sAlert += "Hand Calibrated, ";
                                isChanged = true;
                                break;
                            }
                        case AlertType.ALERT_HAND_NOT_CALIBRATED:
                            {

                                sAlert += "Hand Not Calibrated, ";
                                isChanged = true;
                                break;
                            }
                        case AlertType.ALERT_HAND_INSIDE_BORDERS:
                            {

                                sAlert += "Hand Inside Border, ";
                                isChanged = true;
                                break;
                            }
                        case AlertType.ALERT_HAND_OUT_OF_BORDERS:
                            {

                                sAlert += "Hand Out Of Borders, ";
                                isChanged = true;
                                break;
                            }

                    }
                }
            if (isChanged)
            {
                form.UpdateGestureInfo("Frame " + frameNumber + ") " + sAlert + "\n", System.Drawing.Color.RoyalBlue);
            }
        }

    }
}


