using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BodyTracker3D : MonoBehaviour
{
    private ARHumanBodyManager _arHumanBodyManager;

    // 각 joint에 띄울 프리팹
    [SerializeField] private GameObject jointPrefab;
    
    // 생성한 Gameobject를 관리할 Dictionary, index 값이랑 같이 저장함.
    private Dictionary<int, GameObject> _jointObjects;
    private void Awake()
    {
        _arHumanBodyManager = GetComponent<ARHumanBodyManager>();
        _jointObjects = new Dictionary<int, GameObject>();
    }

    private void OnEnable()
    {
        _arHumanBodyManager.humanBodiesChanged += OnHumanBodyChanged;
    }

    private void OnDisable()
    {
        _arHumanBodyManager.humanBodiesChanged -= OnHumanBodyChanged;
    }

    private void OnHumanBodyChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        foreach (ARHumanBody arHumanBody in eventArgs.updated)
        {
            NativeArray<XRHumanBodyJoint> joints = arHumanBody.joints;

            foreach (XRHumanBodyJoint joint in joints)
            {
                GameObject obj;
                // Dictionary에 특정 인덱스에 있는 Gameobject가 obj에 저장된다. 만약 없는 경우는 새롭게 생성하기 위해 Instantiate해줘야 한다.
                if (!_jointObjects.TryGetValue(joint.index, out obj))
                {
                    obj = Instantiate(jointPrefab);
                    _jointObjects.Add(joint.index, obj);
                }
                
                if (joint.tracked)
                {
                    // joint 값이 tracking 된다면 joint의 transform을 업데이트해야 한다.
                    
                    // anchorPose가 body origin을 중심으로 한 상대적인 값이기 때문에
                    obj.transform.parent = arHumanBody.transform;
                    
                    // 사용자 키를 추정한 값을 곱해 보정.
                    obj.transform.localPosition = joint.anchorPose.position * arHumanBody.estimatedHeightScaleFactor; 
                    obj.transform.localRotation = joint.anchorPose.rotation;
                    obj.SetActive(true);
                }
                else
                {
                    // tracking 되지 않으면 생성된 Gameobject를 숨긴다.
                    obj.SetActive(false);
                }
            }
        }
    }
}
