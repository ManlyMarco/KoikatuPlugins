using System;
using System.Collections.Generic;
using UnityEngine;

namespace StudioAddonLite
{
    public class KeyUtil
    {
        private List<KeyCode> supportKeys = new List<KeyCode>();
        private List<KeyCode> supportKeysNon = new List<KeyCode>();
        private KeyCode key = KeyCode.Help;
        private static KeyUtil NO_MATCH_KEY;

        public static KeyUtil NoMatchKey()
        {
            if(NO_MATCH_KEY == null)
            {
                NO_MATCH_KEY = new NoMatchKeyUtil();
            }

            return NO_MATCH_KEY;
        }

        public static KeyUtil Parse(string keyPattern)
        {
            if(keyPattern.ToLower() == "<none>")
            {
                return NoMatchKey();
            }

            string[] array = keyPattern.ToLower().Split('+');
            List<KeyCode> list = new List<KeyCode>();
            string text = "";
            if(array.Length == 1)
            {
                text = array[0];
            }
            else
            {
                for(int i = 0; i < array.Length - 1; i++)
                {
                    string a = array[i].ToLower();
                    if(a == "ctrl")
                    {
                        list.Add(KeyCode.LeftControl);
                        list.Add(KeyCode.RightControl);
                    }
                    else if(a == "shift")
                    {
                        list.Add(KeyCode.LeftShift);
                        list.Add(KeyCode.RightShift);
                    }
                    else if(a == "alt")
                    {
                        list.Add(KeyCode.LeftAlt);
                        list.Add(KeyCode.RightAlt);
                    }
                }
                text = array[array.Length - 1].ToLower();
            }

            if(text.Length == 1 && "0123456789".Contains(text))
            {
                text = "alpha" + text;
            }

            if(text == "esc")
            {
                text = 27.ToString();
            }

            List<KeyCode> list2 = new List<KeyCode>
            {
                KeyCode.LeftAlt,
                KeyCode.RightAlt,
                KeyCode.LeftControl,
                KeyCode.LeftControl,
                KeyCode.RightShift,
                KeyCode.LeftShift
            };

            foreach(KeyCode item in list)
            {
                list2.Remove(item);
            }
            KeyUtil keyUtil = new KeyUtil();
            keyUtil.supportKeys = list;
            keyUtil.supportKeysNon = list2;
            KeyUtil result;
            try
            {
                KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, true);
                keyUtil.key = keyCode;
                result = keyUtil;
            }
            catch(Exception)
            {
                Debug.Log(string.Format("Failed to parse {0} as KeyCode.", text));
                Console.WriteLine("Failed to parse {0} as KeyCode.", text);
                result = null;
            }

            return result;
        }

        public virtual bool TestKeyUp()
        {
            return Input.GetKeyUp(key) && TestSupports();
        }

        public virtual bool TestKeyDown()
        {
            return Input.GetKeyUp(key) && TestSupports();
        }

        public virtual bool TestKey()
        {
            return Input.GetKey(key) && TestSupports();
        }

        public virtual bool TestSupports()
        {
            for(int i = 0; i < supportKeys.Count; i += 2)
            {
                if(!Input.GetKey(supportKeys[i]) && !Input.GetKey(supportKeys[i + 1]))
                {
                    return false;
                }
            }

            for(int j = 0; j < supportKeysNon.Count; j += 2)
            {
                if(Input.GetKey(supportKeysNon[j]) || Input.GetKey(supportKeysNon[j + 1]))
                {
                    return false;
                }
            }

            return true;
        }

        private class NoMatchKeyUtil : KeyUtil
        {
            public override bool TestKey()
            {
                return false;
            }

            public override bool TestKeyDown()
            {
                return false;
            }

            public override bool TestKeyUp()
            {
                return false;
            }
        }
    }
}
