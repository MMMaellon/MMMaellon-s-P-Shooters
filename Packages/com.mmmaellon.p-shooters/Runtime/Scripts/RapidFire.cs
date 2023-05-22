﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using VRC.SDKBase.Editor.BuildPipeline;
using UnityEditor;
using UdonSharpEditor;
using System.Collections.Immutable;
#endif

namespace MMMaellon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual), RequireComponent(typeof(Animator))]
    public class RapidFire : UdonSharpBehaviour
    {
        public Animator animator;
        [UdonSynced, FieldChangeCallback(nameof(rapidFire))]
        public bool _rapidFire = true;
        [UdonSynced, FieldChangeCallback(nameof(altFire))]
        public int _altFire = 0;
        public int altFireModeCount = 1;

        public void Start()
        {
            if (!Utilities.IsValid(animator))
            {
                animator = GetComponent<Animator>();
            }
            animator.SetBool("rapidfire", rapidFire);
            animator.SetInteger("altfire", altFire);
        }
#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void Reset()
        {
            SerializedObject obj = new SerializedObject(this);
            obj.FindProperty("animator").objectReferenceValue = GetComponent<Animator>();
            obj.ApplyModifiedProperties();
        }
#endif

        public bool rapidFire
        {
            get => _rapidFire;
            set
            {
                _rapidFire = value;
                if (Utilities.IsValid(animator))
                {
                    animator.SetBool("rapidfire", value);
                }
            }
        }
        public int altFire
        {
            get => _altFire;
            set
            {
                _altFire = value;
                if (Utilities.IsValid(animator))
                {
                    animator.SetInteger("altfire", value);
                }
            }
        }

        public void ToggleAltFire()
        {
            if (altFireModeCount <= 0)
            {
                return;
            }
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            altFire = (altFire + 1) % altFireModeCount;
            RequestSerialization();
        }

        public void ToggleRapidFire()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            rapidFire = !rapidFire;
            RequestSerialization();
        }

        public void Cycle()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            //rapid -> alt -> single
            if (!rapidFire)
            {
                rapidFire = true;
                altFire = 0;
            } else if(altFire == 0)
            {
                ToggleAltFire();
            } else
            {
                ToggleAltFire();
                if (altFire == 0)
                {
                    rapidFire = false;
                }
            }
            RequestSerialization();
        }
    }
}