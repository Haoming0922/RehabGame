using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Wheelchair
{
    public class ParameterSetting : MonoBehaviour
    {
        public GameObject wheelChair;
        public GameObject wheelChairRigidBody;
        public PhysicMaterial material;

        public Canvas canvas;
        
        [Space(5)]
        [Header("Slider")]
        public Slider sliderMass;
        public Slider sliderDrag;
        public Slider sliderAngularDrag;
        public Slider sliderForwardFactor;
        public Slider sliderTurningFactor;
        public Slider sliderDynamicFriction;
        public Slider sliderStaticFriction;

        [Space(5)]
        [Header("Text")]
        public TextMeshProUGUI textMass;
        public TextMeshProUGUI textDrag;
        public TextMeshProUGUI textAngularDrag;
        public TextMeshProUGUI textForwardFactor;
        public TextMeshProUGUI textTurningFactor;
        public TextMeshProUGUI textDynamicFriction;
        public TextMeshProUGUI textStaticFriction;

        
        
        private void OnEnable()
        {
            LoadData();
        }


        private void LoadData()
        {
            Rigidbody rb = wheelChairRigidBody.GetComponent<Rigidbody>();
            WheelchairController wc = wheelChair.GetComponent<WheelchairController>();
            
            Parameters data = Parameters.LoadData();
            if (data != null)
            {
                rb.mass = data.mass;
                rb.drag = data.drag;
                rb.angularDrag = data.angularDrag;
                wc.forwardFactor = data.forwardFactor;
                wc.turningFactor = data.turningFactor;
                material.dynamicFriction = data.dynamicFriction;
                material.staticFriction = data.staticFriction;
            }
            
            sliderMass.value = rb.mass;
            sliderDrag.value = rb.drag;
            sliderAngularDrag.value = rb.angularDrag;
            sliderForwardFactor.value = wc.forwardFactor;
            sliderTurningFactor.value = wc.turningFactor;
            sliderDynamicFriction.value = material.dynamicFriction;
            sliderStaticFriction.value = material.staticFriction;
        }

        public void OnMassChange(float value)
        {
            Rigidbody rb = wheelChairRigidBody.GetComponent<Rigidbody>();
            rb.mass = value;
            textMass.text = value.ToString();
        }
        public void OnDragChange(float value)
        {
            Rigidbody rb = wheelChairRigidBody.GetComponent<Rigidbody>();
            rb.drag = value;
            textDrag.text = value.ToString();
        }
        public void OnAngularDragChange(float value)
        {
            Rigidbody rb = wheelChairRigidBody.GetComponent<Rigidbody>();
            rb.angularDrag = value;
            textAngularDrag.text = value.ToString();
        }
        
        public void OnForwardFactorChange(float value)
        {
            WheelchairController wc = wheelChair.GetComponent<WheelchairController>();
            wc.forwardFactor = value;
            textForwardFactor.text = value.ToString();
        }
        public void OnTurningFactorChange(float value)
        {
            WheelchairController wc = wheelChair.GetComponent<WheelchairController>();
            wc.turningFactor = value;
            textTurningFactor.text = value.ToString();
        }
        
        public void OnDynamicFrictionChange(float value)
        {
            material.dynamicFriction = value;
            textDynamicFriction.text = value.ToString();
        }
        public void OnStaticFrictionChange(float value)
        {
            material.staticFriction = value;
            textStaticFriction.text = value.ToString();
        }
        

        public void OnSaveData()
        {
            Parameters data = new Parameters();
         
            Rigidbody rb = wheelChairRigidBody.GetComponent<Rigidbody>();
            WheelchairController wc = wheelChair.GetComponent<WheelchairController>();
            
            data.mass = rb.mass;
            data.drag = rb.drag;
            data.angularDrag = rb.angularDrag;
            data.forwardFactor = wc.forwardFactor;
            data.turningFactor = wc.turningFactor;
            data.dynamicFriction = material.dynamicFriction;
            data.staticFriction = material.staticFriction;
            
            data.SaveData();
        }
        
        public void OnReloadData()
        {
            LoadData();
        }

    }
}


