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
    class NumStrCmp : IComparer<CUE.NET.Devices.Keyboard.Keys.CorsairKey> {
        public int Compare(CUE.NET.Devices.Keyboard.Keys.CorsairKey cx, CUE.NET.Devices.Keyboard.Keys.CorsairKey cy) {
            string x = cx.KeyId.ToString();
            string y = cy.KeyId.ToString();
            Regex regex = new Regex(@"(?<NumPart>\d+)(?<StrPart>\D*)", RegexOptions.Compiled);
            var mx = regex.Match(x);
            var my = regex.Match(y);
            var ret = int.Parse(mx.Groups["NumPart"].Value).CompareTo(int.Parse(my.Groups["NumPart"].Value));
            if(ret != 0) return ret;
            return mx.Groups["StrPart"].Value.CompareTo(my.Groups["StrPart"].Value);
        }
    }

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
            /*
            List<CUE.NET.Devices.Keyboard.Keys.CorsairKey> kstrs = new List<CUE.NET.Devices.Keyboard.Keys.CorsairKey>();
            foreach(var key in kb.Keys) {
                string kstr = key.KeyId.ToString();
                if(true || kstr != "F" && kstr.StartsWith("F")) {
                    kstrs.Add(key);
                }
            }*/
            //kstrs.Sort(new NumStrCmp());
            /*
            foreach(var key in kstrs) {
                this.kb[key.KeyId].Led.Color = System.Drawing.Color.Red;
                this.kb.Update();
                Thread.Sleep(50);
            }
            System.Drawing.Color c = System.Drawing.Color.Red;
            for(int i = 0; i < 360; i++) {
                if(clearFlag) break;
                if(c == System.Drawing.Color.Red)
                    c = System.Drawing.Color.Black;
                else
                    c = System.Drawing.Color.Red;
                foreach(var key in kstrs) {
                    this.kb[key.KeyId].Led.Color = c;
                }
                this.kb.Update();
                Thread.Sleep(100);
            }*/

            /*List<CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId> kids = new List<CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId>();
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.Stop);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.ScanPreviousTrack);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.PlayPause);
            kids.Add(CUE.NET.Devices.Keyboard.Enums.CorsairKeyboardKeyId.ScanNextTrack);*/
            //System.Drawing.Color c = System.Drawing.Color.Red;
            /*for(int i = 0; i < 360; i++) {
                if(clearFlag) break;
                if(c == System.Drawing.Color.Red)
                    c = System.Drawing.Color.Black;
                else
                    c = System.Drawing.Color.Red;
                foreach(var key in kstrs) {
                    this.kb[key.KeyId].Led.Color = c;
                }
                foreach(var key in kids) {
                    this.kb[key].Led.Color = c;
                }
                this.kb.Update();
                Thread.Sleep(100);
            }*/
        //}

        public void onMessage<T>(string topic, T payload) {
            Task.Run(() => executeEvent(topic));
        }

        public void raiseClearFlag() {
            this.clearFlag = true;
        }
    }
}
