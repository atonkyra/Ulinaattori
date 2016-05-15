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

        private void executeEvent() {
            lock(this) {
                if(eventRunning) return;
                eventRunning = true;
            }
            List<CUE.NET.Devices.Keyboard.Keys.CorsairKey> kstrs = new List<CUE.NET.Devices.Keyboard.Keys.CorsairKey>();
            foreach(var key in kb.Keys) {
                string kstr = key.KeyId.ToString();
                if(kstr != "F" && kstr.StartsWith("F")) {
                    kstrs.Add(key);
                    /*this.kb[key.KeyId].Led.Color = System.Drawing.Color.Red;
                    Thread.Sleep(10);
                    this.kb.Update();*/
                }
            }
            kstrs.Sort(new NumStrCmp());
            foreach(var key in kstrs) {
                this.kb[key.KeyId].Led.Color = System.Drawing.Color.Red;
                this.kb.Update();
                Thread.Sleep(100);
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
            }
            CueSDK.Reinitialize();
            this.eventRunning = false;
            this.clearFlag = false;
        }

        public void onMessage<T>(string topic, T payload) {
            Task.Run(() => executeEvent());
            Console.WriteLine("JEE");
        }

        public void raiseClearFlag() {
            this.clearFlag = true;
        }
    }
}
