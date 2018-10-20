using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IllusionUtility.GetUtility;
using Studio;

namespace LockOnPluginKK
{
    internal class CameraTargetManager : MonoBehaviour
    {
        public const string MOVEMENTPOINT_NAME = "MovementPoint";
        public const string CENTERPOINT_NAME = "CenterPoint";

        static float targetSize = 25f;
        public bool showLockOnTargets = false;
        public ChaInfo chara;

        public List<GameObject> GetTargets() => quickTargets;
        private List<GameObject> quickTargets = new List<GameObject>();
        private List<CustomTarget> customTargets = new List<CustomTarget>();
        private CenterPoint centerPoint;

        public static CameraTargetManager GetTargetManager(ChaInfo chara)
        {
            var targetManager = chara.gameObject.GetComponent<CameraTargetManager>();
            if(!targetManager)
            {
                targetManager = chara.gameObject.AddComponent<CameraTargetManager>();
                targetManager.UpdateAllTargets(chara);
                targetManager.chara = chara;
            }

            return targetManager;
        }

        void Update()
        {
            UpdateCustomTargetTransforms();
        }

        protected virtual void OnGUI()
        {
            if(showLockOnTargets)
            {
                var list = (from v in GuideObjectManager.Instance.selectObjectKey
                            select Studio.Studio.GetCtrlInfo(v) as OCIChar into v
                            where v != null
                            select v).ToList();

                if(list.Count > 0)
                {
                    var boneList = list[0].charInfo.objBodyBone.transform.GetComponentsInChildren<Component>().ToList();

                    for(int i = 0; i < boneList.Count; i++)
                    {
                        Vector3 pos = Camera.main.WorldToScreenPoint(boneList[i].transform.position);
                        if(pos.z > 0f && GUI.Button(new Rect(pos.x - targetSize / 2f, Screen.height - pos.y - targetSize / 2f, targetSize, targetSize), "L"))
                        {
                            Console.WriteLine(boneList[i].name);
                        }
                    }
                }

                //List<GameObject> targets = GetAllTargets();
                //for(int i = 0; i < targets.Count; i++)
                //{
                //    Vector3 pos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
                //    if(pos.z > 0f && GUI.Button(new Rect(pos.x - targetSize / 2f, Screen.height - pos.y - targetSize / 2f, targetSize, targetSize), "L"))
                //    {
                //        //CameraTargetPos += targetOffsetSize;
                //        //targetOffsetSize = new Vector3();
                //        LockOnBase.instance.LockOn(targets[i]);
                //    }
                //}
            }
        }
        
        public void UpdateCustomTargetTransforms()
        {
            for(int i = 0; i < customTargets.Count; i++) customTargets[i].UpdateTransform();
            if(centerPoint != null && centerPoint.GetPoint()) centerPoint.UpdatePosition();
        }

        public void UpdateAllTargets(ChaInfo character)
        {
            if(character)
            {
                centerPoint = new CenterPoint(character);
                customTargets = UpdateCustomTargets(character);
                quickTargets = UpdateQuickTargets(character);
            }
        }

        private List<GameObject> UpdateQuickTargets(ChaInfo character)
        {
            var quickTargets = new List<GameObject>();

            foreach(var targetName in LockOnPlugin.targetData.quickTargets)
            {
                bool customFound = false;

                if(targetName == CENTERPOINT_NAME)
                {
                    quickTargets.Add(centerPoint.GetPoint());
                    customFound = true;
                }

                foreach(var customTarget in customTargets)
                {
                    if(customTarget.GetTarget().name == targetName)
                    {
                        quickTargets.Add(customTarget.GetTarget());
                        customFound = true;
                    }
                }

                if(!customFound)
                {
                    var bone = character.objBodyBone.transform.FindLoop(targetName);
                    if(bone) quickTargets.Add(bone); 
                }
            }

            return quickTargets;
        }

        private List<CustomTarget> UpdateCustomTargets(ChaInfo character)
        {
            var customTargets = new List<CustomTarget>();

            foreach(var data in LockOnPlugin.targetData.customTargets)
            {
                var point1 = character.objBodyBone.transform.FindLoop(data.point1);
                var point2 = character.objBodyBone.transform.FindLoop(data.point2);

                foreach(var target in customTargets)
                {
                    if(target.GetTarget().name == data.point1)
                    {
                        point1 = target.GetTarget();
                    }

                    if(target.GetTarget().name == data.point2)
                    {
                        point2 = target.GetTarget();
                    }
                }

                if(point1 && point2)
                {
                    var target = new CustomTarget(data.target, point1, point2, data.midpoint);
                    target.GetTarget().transform.SetParent(character.transform);
                    customTargets.Add(target);
                }
                else
                {
                    Console.WriteLine($"CustomTarget '{data.target}' failed");
                }
            }

            return customTargets;
        }

        private class CustomTarget
        {
            private GameObject target;
            private GameObject point1;
            private GameObject point2;
            private float midpoint;

            public CustomTarget(string name, GameObject point1, GameObject point2, float midpoint = 0.5f)
            {
                target = new GameObject(name);
                this.point1 = point1;
                this.point2 = point2;
                this.midpoint = midpoint;
                UpdateTransform();
            }

            public GameObject GetTarget()
            {
                return target;
            }

            public void UpdateTransform()
            {
                UpdatePosition();
                UpdateRotation();
            }

            private void UpdatePosition()
            {
                var pos1 = point1.transform.position;
                var pos2 = point2.transform.position;
                target.transform.position = Vector3.Lerp(pos1, pos2, midpoint);
            }

            private void UpdateRotation()
            {
                var rot1 = point1.transform.rotation;
                var rot2 = point2.transform.rotation;
                target.transform.rotation = Quaternion.Slerp(rot1, rot2, 0.5f);
            }
        }

        private class CenterPoint
        {
            private List<WeightPoint> points = new List<WeightPoint>();
            private GameObject point;

            public CenterPoint(ChaInfo character)
            {
                foreach(var data in LockOnPlugin.targetData.centerWeigths)
                {
                    var point = character.objBodyBone.transform.FindLoop(data.bone);
                    points.Add(new WeightPoint(point, data.weigth));
                }

                if(points.Count > 0)
                {
                    point = new GameObject(CENTERPOINT_NAME);
                    point.transform.SetParent(character.transform);
                    UpdatePosition();
                }
                else
                {
                    point = null;
                }
            }

            public GameObject GetPoint()
            {
                return point;
            }

            public void UpdatePosition()
            {
                point.transform.position = CalculateCenterPoint(points);
            }

            private Vector3 CalculateCenterPoint(List<WeightPoint> points)
            {
                var center = new Vector3();
                float totalWeight = 0f;
                for(int i = 0; i < points.Count; i++)
                {
                    center += points[i].point.transform.position * points[i].weight;
                    totalWeight += points[i].weight;
                }

                return center / totalWeight;
            }
        }

        private struct WeightPoint
        {
            public GameObject point;
            public float weight;

            public WeightPoint(GameObject point, float weight)
            {
                this.point = point;
                this.weight = weight;
            }
        }
    }
}
