﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using TrionicCANLib.CAN;
using TrionicCANLib.KWP;
using TrionicCANLib.Flasher;
using TrionicCANLib.Log;

namespace TrionicCANLib
{
    abstract public class ITrionic
    {
        protected ICANDevice canUsbDevice;

        public delegate void WriteProgress(object sender, WriteProgressEventArgs e);
        public event ITrionic.WriteProgress onWriteProgress;

        public delegate void ReadProgress(object sender, ReadProgressEventArgs e);
        public event ITrionic.ReadProgress onReadProgress;

        public delegate void CanInfo(object sender, CanInfoEventArgs e);
        public event ITrionic.CanInfo onCanInfo;

        // implements functions for canbus access for Trionic 8
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);

        protected bool m_EnableLog = false;

        public bool EnableLog
        {
            get { return m_EnableLog; }
            set
            {
                m_EnableLog = value;
                if (canUsbDevice != null)
                {
                    canUsbDevice.EnableCanLog = m_EnableLog;
                }
            }
        }

        protected int m_sleepTime = (int)SleepTime.Default;

        public SleepTime Sleeptime
        {
            get { return (SleepTime)m_sleepTime; }
            set { m_sleepTime = (int)value; }
        }

        protected int m_forcedBaudrate = 0;
        public int ForcedBaudrate
        {
            get
            {
                return m_forcedBaudrate;
            }
            set
            {
                m_forcedBaudrate = value;
            }
        }

        public int BaseBaudrate { get; set; }

        public bool isOpen()
        {
            if (canUsbDevice != null)
            {
                return canUsbDevice.isOpen();
            }
            return false;
        }

        protected string m_forcedComport = string.Empty;

        public string ForcedComport
        {
            get { return m_forcedComport; }
            set { m_forcedComport = value; }
        }

        protected bool m_OnlyPBus = false;

        public bool OnlyPBus
        {
            get { return m_OnlyPBus; }
            set { m_OnlyPBus = value; }
        }

        protected bool m_DisableCanConnectionCheck = false;

        public bool DisableCanConnectionCheck
        {
            get { return m_DisableCanConnectionCheck; }
            set { m_DisableCanConnectionCheck = value; }
        }

        abstract public void setCANDevice(CANBusAdapter adapterType);

        abstract public bool openDevice(bool requestSecurityAccess);

        abstract public void Cleanup();

        public float GetADCValue(uint channel)
        {
            return canUsbDevice.GetADCValue(channel);
        }

        public float GetThermoValue()
        {
            return canUsbDevice.GetThermoValue();
        }
        
        protected void CastProgressWriteEvent(int percentage)
        {
            if (onWriteProgress != null)
            {
                onWriteProgress(this, new WriteProgressEventArgs(percentage));
            }
        }

        protected void CastProgressReadEvent(int percentage)
        {
            if (onReadProgress != null)
            {
                onReadProgress(this, new ReadProgressEventArgs(percentage));
            }
        }

        protected void CastInfoEvent(string info, ActivityType type)
        {
            Console.WriteLine(info);
            if (onCanInfo != null)
            {
                onCanInfo(this, new CanInfoEventArgs(info, type));
            }
        }

        public class CanInfoEventArgs : System.EventArgs
        {
            private ActivityType _type;

            public ActivityType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            private string _info;

            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            public CanInfoEventArgs(string info, ActivityType type)
            {
                _info = info;
                _type = type;
            }
        }

        public class WriteProgressEventArgs : System.EventArgs
        {
            private int _percentage;

            private int _bytestowrite;

            public int Bytestowrite
            {
                get { return _bytestowrite; }
                set { _bytestowrite = value; }
            }

            private int _byteswritten;

            public int Byteswritten
            {
                get { return _byteswritten; }
                set { _byteswritten = value; }
            }

            public int Percentage
            {
                get { return _percentage; }
                set { _percentage = value; }
            }

            public WriteProgressEventArgs(int percentage)
            {
                _percentage = percentage;
            }

            public WriteProgressEventArgs(int percentage, int bytestowrite, int byteswritten)
            {
                _bytestowrite = bytestowrite;
                _byteswritten = byteswritten;
                _percentage = percentage;
            }
        }

        public class ReadProgressEventArgs : System.EventArgs
        {
            private int _percentage;

            public int Percentage
            {
                get { return _percentage; }
                set { _percentage = value; }
            }

            public ReadProgressEventArgs(int percentage)
            {
                _percentage = percentage;
            }
        }
    }
}
