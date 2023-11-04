using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBodyTracker : MonoBehaviour
{
    // AR Body Tracker에서도 AR Human Body Manager와 동일하게 Prefab을 받는다.
    [SerializeField] private GameObject bodyPrefab;

    // XR Origin의 ARHumanBodyManager
    private ARHumanBodyManager _arHumanBodyManager;
    
    // 인식된 body를 인스턴스화 시킬 오브젝트
    private GameObject _bodyObject;
    private void Awake()
    {
        _arHumanBodyManager = GetComponent<ARHumanBodyManager>();
    }

    private void OnEnable()
    {
        // humanBodiesChanged Event에 OnHumanBodiesChanged 메서드 등록
        _arHumanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
    }

    private void OnDisable()
    {
        // humanBodiesChanged Event에 OnHumanBodiesChanged 메서드 등록 해제
        _arHumanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    // Event를 전달받기 위한 함수, ARHumanBodiesChangedEventArgs에서 Body가 추가, 업데이트, 제거되면 콜백 함수로 전달받을 수 있다.
    private void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        // eventArgs.added, updated, removed는 List<ARHumanBody>를 반환 
        foreach (ARHumanBody humanBody in eventArgs.added)
        {
            _bodyObject = Instantiate(bodyPrefab, humanBody.transform);
        }
        
        foreach (ARHumanBody humanBody in eventArgs.updated)
        {
            if (_bodyObject != null)
            {
                _bodyObject.transform.position = humanBody.transform.position;
                _bodyObject.transform.rotation = humanBody.transform.rotation;
                _bodyObject.transform.localScale = humanBody.transform.localScale;    
            }
        }
        foreach (ARHumanBody humanBody in eventArgs.removed)
        {
            if (_bodyObject != null)
            {
                Destroy(_bodyObject);    
            }
        }
    }
}
