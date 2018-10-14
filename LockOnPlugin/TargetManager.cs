using IllusionUtility.GetUtility;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LockOnPluginKK
{
    internal class CameraTargetManager : MonoBehaviour
    {
        public const string MOVEMENTPOINT_NAME = "MovementPoint";
        public const string CENTERPOINT_NAME = "CenterPoint";

        static float targetSize = 25f;
        public bool showLockOnTargets = false;
        public ChaInfo chara;

        private List<GameObject> allTargets = new List<GameObject>();
        private List<GameObject> normalTargets = new List<GameObject>();
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
                            //where v.oiCharInfo.sex == 1
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

        public List<GameObject> GetAllTargets() => allTargets;

        public void UpdateCustomTargetTransforms()
        {
            for(int i = 0; i < customTargets.Count; i++) customTargets[i].UpdateTransform();
            if(centerPoint != null && centerPoint.GetPoint()) centerPoint.UpdatePosition();
        }

        public void UpdateAllTargets(ChaInfo character)
        {
            if(character)
            {
                allTargets = normalTargets = UpdateNormalTargets(character);
                customTargets = UpdateCustomTargets(character);
                customTargets.ForEach(target => allTargets.Add(target.GetTarget()));
                //centerPoint = new CenterPoint(character);
                //if(centerPoint != null && centerPoint.GetPoint()) allTargets.Add(centerPoint.GetPoint());
            }
            else
            {
                //if(centerPoint != null)
                //{
                //    GameObject.Destroy(centerPoint.GetPoint());
                //    centerPoint = null;
                //}

                for(int i = 0; i < customTargets.Count; i++)
                {
                    GameObject.Destroy(customTargets[i].GetTarget());
                }

                allTargets = new List<GameObject>();
                normalTargets = new List<GameObject>();
                customTargets = new List<CustomTarget>();
            }
        }

        private List<GameObject> UpdateNormalTargets(ChaInfo character)
        {
            List<GameObject> normalTargets = new List<GameObject>();
            //string prefix = character is CharFemale ? "cf_" : "cm_";
            string prefix = "cf_";

            foreach(string targetName in FileManager.GetNormalTargetNames())
            {
                GameObject bone = character.objBodyBone.transform.FindLoop(prefix + targetName);
                Console.WriteLine($"Bone '{prefix + targetName}' found: {bone != null}");
                if(bone) normalTargets.Add(bone);
            }

            return normalTargets;
        }

        private List<CustomTarget> UpdateCustomTargets(ChaInfo character)
        {
            List<CustomTarget> customTargets = new List<CustomTarget>();
            //string prefix = character is CharFemale ? "cf_" : "cm_";
            string prefix = "cf_";

            foreach(List<string> data in FileManager.GetCustomTargetNames())
            {
                GameObject point1 = character.objBodyBone.transform.FindLoop(prefix + data[1]);
                GameObject point2 = character.objBodyBone.transform.FindLoop(prefix + data[2]);

                foreach(CustomTarget target in customTargets)
                {
                    if(target.GetTarget().name == data[1])
                    {
                        point1 = target.GetTarget();
                    }

                    if(target.GetTarget().name == data[2])
                    {
                        point2 = target.GetTarget();
                    }
                }

                if(point1 && point2)
                {
                    float midpoint = 0.5f;
                    if(data.ElementAtOrDefault(3) != null)
                    {
                        if(!float.TryParse(data[3], out midpoint))
                        {
                            midpoint = 0.5f;
                        }
                    }

                    CustomTarget target = new CustomTarget(data[0], point1, point2, midpoint);
                    target.GetTarget().transform.SetParent(character.transform);
                    customTargets.Add(target);
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
                Vector3 pos1 = point1.transform.position;
                Vector3 pos2 = point2.transform.position;
                target.transform.position = Vector3.Lerp(pos1, pos2, midpoint);
            }

            private void UpdateRotation()
            {
                Quaternion rot1 = point1.transform.rotation;
                Quaternion rot2 = point2.transform.rotation;
                target.transform.rotation = Quaternion.Slerp(rot1, rot2, 0.5f);
            }
        }

        private class CenterPoint
        {
            private List<WeightPoint> points = new List<WeightPoint>();
            private GameObject point;

            public CenterPoint(ChaInfo character)
            {
                //string prefix = character is CharFemale ? "cf_" : "cm_";
                string prefix = "cf_";

                foreach(List<string> data in FileManager.GetCenterTargetWeights())
                {
                    GameObject point = character.objBodyBone.transform.FindLoop(prefix + data[0]);
                    float weight = 1f;
                    if(!float.TryParse(data[1], out weight))
                    {
                        weight = 1f;
                    }
                    points.Add(new WeightPoint(point, weight));
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
                //point.transform.position = CalculateCenterPoint(points);
            }

            private Vector3 CalculateCenterPoint(List<WeightPoint> points)
            {
                Vector3 center = new Vector3();
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
