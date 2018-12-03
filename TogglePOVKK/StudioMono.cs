using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Studio;

namespace TogglePOVKK
{
    class StudioMono : BaseMono
    {
        private Studio.CameraControl camera => Studio.Studio.Instance.cameraCtrl;

        protected override bool CameraEnabled
        {
            get { return camera.enabled; }
            set { camera.enabled = value; }
        }

        protected override Vector3 CameraTargetPos
        {
            get { return camera.targetPos; }
        }

        protected override bool DepthOfField
        {
            get { return Manager.Config.EtcData.DepthOfField; }
            set { Manager.Config.EtcData.DepthOfField = value; }
        }

        protected override bool Shield
        {
            get { return Manager.Config.EtcData.Shield; }
            set { Manager.Config.EtcData.Shield = value; }
        }

        protected override bool CameraStopMoving()
        {
            var noCtrlCondition = camera.noCtrlCondition;
            bool result = false;
            if(noCtrlCondition != null)
            {
                result = noCtrlCondition();
            }
            return result;
        }

        protected override ChaInfo GetClosestChara(Vector3 targetPos)
        {
            var characters = GetSelectedCharacters();
            if(characters.Count > 0) return characters[0].charInfo;
            return null;
        }

        List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }
    }
}
