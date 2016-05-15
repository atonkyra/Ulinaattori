using CUE.NET;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ulinaattori.Plugins {
    class CUEPlugin {
        private CorsairKeyboard kb = null;
        private bool eventRunning = false;
        private bool clearFlag = false;
        public CUEPlugin() {
            CueSDK.Initialize();
            this.kb = CueSDK.KeyboardSDK;
            if(this.kb == null)
                throw new WrapperException("No keyboard found");
        }

        private int getCosT(double innerFactor, float t) {
            return (int)Math.Round(innerFactor * Math.Cos(t), MidpointRounding.AwayFromZero);
        }

        private void executeMKFadeAlert(string topic) {
            List<CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId> kids = new List<CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId>();
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.Escape);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F1);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F2);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F3);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F4);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F5);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F6);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F7);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F8);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F9);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F10);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F11);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.F12);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.PrintScreen);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.ScrollLock);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.PauseBreak);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.Stop);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.ScanPreviousTrack);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.PlayPause);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.ScanNextTrack);
            float t = 0;
            while(!clearFlag) {
                float tb = t;
                foreach(var key in kids) {
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(128 + getCosT(127.0, tb), 0, 16 + getCosT(15.0, tb));
                    this.kb[key].Led.Color = c;
                    tb += 0.2f;
                }
                this.kb.Update();
                t-=0.05f;
                Thread.Sleep(6);
            }
            Thread.Sleep(100);
        }

        private void executeEvent(string topic) {
            lock(this) {
                if(eventRunning) return;
                this.eventRunning = true;
            }

            this.executeMKFadeAlert(topic);

            CueSDK.Reinitialize();
            this.eventRunning = false;
            this.clearFlag = false;
        }

        public void onMessage<T>(string topic, T payload) {
            Task.Run(() => executeEvent(topic));
        }

        public void raiseClearFlag() {
            this.clearFlag = true;
        }
    }
}
